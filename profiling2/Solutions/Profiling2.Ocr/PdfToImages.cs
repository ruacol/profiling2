using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using Aspose.Pdf;
using log4net;

namespace Profiling2.Ocr
{
    /// <summary>
    /// When given PDF containing any images, populate this.ImageStreams with images in BMP format.
    /// </summary>
    public class PdfToImages : IDisposable
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(PdfToImages));
        public IList<MemoryStream> ImageStreams { get; set; }
        public int NumPages { get; set; }

        /// <summary>
        /// Only scan pdf images if it's the type of document that consists of one scanned photo per page
        /// </summary>
        public bool IsScanWorthy
        {
            get
            {
                return this.NumPages == this.ImageStreams.Count;
            }
        }

        public PdfToImages(Stream pdfStream)
        {
            this.ImageStreams = new List<MemoryStream>();

            if (pdfStream != null)
            {
                try
                {
                    Document pdf = new Document(pdfStream);
                    this.NumPages = pdf.Pages.Count;
                    foreach (Page page in pdf.Pages)
                    {
                        foreach (XImage xImage in page.Resources.Images)
                        {
                            try
                            {
                                MemoryStream outputImageStream = new MemoryStream();
                                xImage.Save(outputImageStream, ImageFormat.Bmp);

                                // this stream is not closed in order for processing to occur on it later
                                this.ImageStreams.Add(outputImageStream);
                            }
                            catch (Exception e)
                            {
                                Log.Error("Problem extracting image from PDF.", e);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Problem instantiating PDF file.", e);
                }
            }
        }

        public void Dispose()
        {
            foreach (MemoryStream ms in this.ImageStreams)
                if (ms != null)
                    ms.Dispose();
        }
    }
}
