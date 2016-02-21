using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using NHibernate;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Services;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Aspose;
using Profiling2.Infrastructure.Util;
using Profiling2.Ocr;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Contracts.Repositories;
using TikaOnDotNet;

namespace Profiling2.Tasks.Sources
{
    /// <summary>
    /// Source related tasks that require access to the source's binary data.  This includes Aspose/Tika/Tesseract
    /// functions that parse file contents.
    /// </summary>
    public class SourceContentTasks : ISourceContentTasks
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(SourceContentTasks));
        protected readonly ISourceTasks sourceTasks;
        protected readonly IAsposeService asposeService;
        protected readonly INHibernateRepository<SourceLog> sourceLogRepo;
        protected readonly ISourceLogQueries sourceLogQueries;

        public SourceContentTasks(ISourceTasks sourceTasks, IAsposeService asposeService, INHibernateRepository<SourceLog> sourceLogRepo, ISourceLogQueries sourceLogQueries)
        {
            this.sourceTasks = sourceTasks;
            this.asposeService = asposeService;
            this.sourceLogRepo = sourceLogRepo;
            this.sourceLogQueries = sourceLogQueries;
        }

        public bool CheckPreviewProblems(int sourceId)
        {
            return this.sourceLogQueries.CheckPreviewProblem(sourceId);
        }

        protected SourceLog GetSourceLog(int sourceId)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria["SourceID"] = sourceId;
            IList<SourceLog> list = this.sourceLogRepo.FindAll(criteria);
            if (list != null && list.Any())
                return list.First();
            return null;
        }

        public void ProcessPreviewProblemsQueueable()
        {
            using (IStatelessSession session = NHibernateSession.GetDefaultSessionFactory().OpenStatelessSession())
            {
                IList<SourceDTO> dtos = this.sourceTasks.GetAllSourceDTOs(session, false, true).Take(100).ToList();
                this.FindPreviewProblems(dtos);
            }
        }

        public void FindPreviewProblems(IList<SourceDTO> dtos)
        {
            using (IStatelessSession session = NHibernateSession.GetDefaultSessionFactory().OpenStatelessSession())
            {
                foreach (SourceDTO dto in dtos)
                {
                    log.Debug("Finding preview problems for SourceID " + dto.SourceID + "...");

                    // find existing SourceLog if exists
                    SourceLog sourceLog = this.sourceLogQueries.GetSourceLog(session, dto.SourceID);
                    if (sourceLog == null)
                    {
                        sourceLog = new SourceLog() { SourceID = dto.SourceID };
                    }

                    try
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            this.asposeService.ConvertToHtml(this.sourceTasks.GetSource(session, dto.SourceID), ms);
                        }
                    }
                    catch (Exception e)
                    {
                        sourceLog.LogSummary = e.Message;
                        sourceLog.Log = e.ToString();
                    }

                    sourceLog.DateTime = DateTime.Now;
                    this.sourceLogQueries.InsertSourceLog(session, sourceLog);
                }
            }
        }

        public byte[] GetHtmlPreview(Source source, Stream destination)
        {
            if (source != null)
            {
                try
                {
                    MemoryStream html = (MemoryStream)this.asposeService.ConvertToHtml(source, destination);
                    return html.ToArray();
                }
                catch (Exception e)
                {
                    log.Error("Error generating preview for SourceID=" + source.Id + ".", e);
                    UTF8Encoding encoding = new UTF8Encoding();
                    return encoding.GetBytes("Error generating preview. " + e.Message + (e.Message.EndsWith(".") ? string.Empty : ".") + " Try downloading the file instead.");
                }
            }
            return null;
        }

        public IDictionary<string, object> GetWordDocumentProperties(int sourceId)
        {
            Source source = this.sourceTasks.GetSource(sourceId);
            if (source != null && source.FileData != null && AsposeService.WordExtensions.Contains(source.GetFileExtension()))
                return this.asposeService.GetWordDocumentProperties(source);
            return null;
        }

        public Source OcrScanAndSetSource(Source source)
        {
            if (source != null && source.FileData != null)
            {
                string text = string.Empty;
                bool scanStarted = false;

                string ext = source.GetFileExtension();
                MemoryStream fileDataStream = new MemoryStream(source.FileData);

                if (string.Equals(ext, "pdf"))
                {
                    // PDFs are sometimes simply scanned pages of a document.  Check this
                    using (PdfToImages pti = new PdfToImages(fileDataStream))
                    {
                        if (pti.IsScanWorthy)
                        {
                            log.Info("Found images in file " + source.SourceName + ", starting OCR scan.");
                            scanStarted = true;
                            foreach (MemoryStream ms in pti.ImageStreams)
                            {
                                TesseractImageToText ocrPdfImage = new TesseractImageToText(ms, null, null);
                                if (ocrPdfImage.PassedThreshold)
                                {
                                    text += ocrPdfImage.Text;
                                }
                            }
                        }
                    }
                }
                else if (Source.IMAGE_FILE_EXTENSIONS.Contains(ext))
                {
                    log.Info("Starting OCR scan of image file " + source.SourceName + ".");
                    scanStarted = true;
                    TesseractImageToText ocr = new TesseractImageToText(fileDataStream, null, null);
                    if (ocr.PassedThreshold)
                        text = ocr.Text;
                }

                if (!string.IsNullOrEmpty(text))
                {
                    log.Info("Saving OCR text for " + source.SourceName + " to Source.FileData and backing up original to OriginalFileData.");

                    UTF8Encoding encoding = new UTF8Encoding();
                    source.OriginalFileData = source.FileData;
                    source.FileData = encoding.GetBytes(text);

                    // PRF_Source.FileExtension is used by full-text index to determine document type
                    source.FileExtension = "txt";

                    // we could set source.FileLanguage here, but PRF_FileLanguage is not really implemented
                    //source.FileLanguage =
                }
                else if (scanStarted)
                {
                    log.Info("No output from OCR scan of " + source.SourceName + ".");
                }
            }
            return source;
        }

        public void OcrScanAndSetSourceQueueable(int sourceId)
        {
            Source source = null;
            using (IStatelessSession session = NHibernateSession.GetDefaultSessionFactory().OpenStatelessSession())
            {
                source = this.sourceTasks.GetSource(session, sourceId);
                source = this.OcrScanAndSetSource(source);
            }

            // our SaveSource method for the moment only accepts an ISession, not an IStatelessSession, so we create a new session;
            // not great, but only happens when OCR has found text, which should be rarer than not.
            if (source != null && source.OriginalFileData != null)
            {
                using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
                {
                    using (ITransaction tx = session.BeginTransaction())
                    {
                        this.sourceTasks.SaveSource(session, source);
                        tx.Commit();
                    }
                }
            }
        }

        public string GuessContentType(int sourceId)
        {
            Source source = this.sourceTasks.GetSource(sourceId);
            if (source != null)
            {
                TextExtractionResult result = new TextExtractor().Extract(source.FileData);
                return result.ContentType;
            }
            return null;
        }
    }
}
