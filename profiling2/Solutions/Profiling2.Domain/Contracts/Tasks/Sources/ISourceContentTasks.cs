using System.Collections.Generic;
using System.IO;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Tasks.Sources
{
    public interface ISourceContentTasks
    {
        bool CheckPreviewProblems(int sourceId);

        void ProcessPreviewProblemsQueueable();

        /// <summary>
        /// Record any exceptions when attempting to generate HTML preview of given sources.  Exception text is recorded in PRF_SourceLog.
        /// </summary>
        /// <param name="dtos"></param>
        void FindPreviewProblems(IList<SourceDTO> dtos);

        /// <summary>
        /// Convert source to HTML, ready to be output to HTTP.  Responsibility for closing destination stream rests with caller.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        byte[] GetHtmlPreview(Source source, Stream destination);

        IDictionary<string, object> GetWordDocumentProperties(int sourceId);

        /// <summary>
        /// Check the file type and perform an OCR scan if eligible; if legible text detected, place OCR text into given source.FileData
        /// and backup original to source.OriginalFileData.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        Source OcrScanAndSetSource(Source source);

        void OcrScanAndSetSourceQueueable(int sourceId);

        /// <summary>
        /// Use Tika to return a content type for the given source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        string GuessContentType(int sourceId);
    }
}
