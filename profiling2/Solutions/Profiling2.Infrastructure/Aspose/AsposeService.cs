using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Profiling2.Domain.Contracts.Services;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Aspose.Loader;
using Profiling2.Infrastructure.Util;

namespace Profiling2.Infrastructure.Aspose
{
    public class AsposeService : IAsposeService
    {
        public static readonly IList<string> WordExtensions = new ReadOnlyCollection<string>
            (new List<string> { "docm", "dotm", "dotx", "docx", "rtf", "doc", "mht", "html", "htm" });
        public static readonly IList<string> CellExtensions = new ReadOnlyCollection<string>
            (new List<string> { "xlam", "xlsb", "xlsm", "xlsx", "xltm", "xltx", "csv", "xls" });
        public static readonly IList<string> SlideExtensions = new ReadOnlyCollection<string>
            (new List<string> { "potm", "potx", "ppam", "ppsm", "ppsx", "pptm", "pptx", "ppt" });
        public static readonly IList<string> PdfExtensions = new ReadOnlyCollection<string>
            (new List<string> { "pdf" });

        public Stream ConvertToHtml(Source source, Stream destination)
        {
            if (source != null && source.FileData != null)
            {
                string fileExt = source.GetFileExtension();
                if (!string.IsNullOrEmpty(fileExt))
                {
                    using (MemoryStream ms = new MemoryStream(source.FileData))
                    {
                        BaseAsposeLoader loader = null;
                        if (WordExtensions.Contains(fileExt))
                        {
                            loader = new WordDocumentLoader(ms, null);
                        }
                        else if (CellExtensions.Contains(fileExt))
                        {
                            loader = new WorkbookLoader(ms, null);
                        }
                        else if (SlideExtensions.Contains(fileExt))
                        {
                            loader = new PresentationLoader(ms, null);
                        }
                        else if (PdfExtensions.Contains(fileExt))
                        {
                            loader = new PdfLoader(ms, null);
                        }
                        return loader != null ? loader.GetHtml(destination) : null;
                    }
                }
            }
            return null;
        }

        public IDictionary<string, object> GetWordDocumentProperties(Source source)
        {
            if (source != null && source.FileData != null && WordExtensions.Contains(source.GetFileExtension()))
            {
                using (MemoryStream ms = new MemoryStream(source.FileData))
                {
                    WordDocumentLoader loader = new WordDocumentLoader(ms, null);
                    return loader.GetDocumentProperties();
                }
            }
            return null;
        }

        public Stream StripPassword(Source source, string password, Stream destination)
        {
            if (source != null && source.FileData != null)
            {
                string fileExt = source.GetFileExtension();
                if (!string.IsNullOrEmpty(fileExt))
                {
                    using (MemoryStream ms = new MemoryStream(source.FileData))
                    {
                        BaseAsposeLoader loader = null;
                        if (WordExtensions.Contains(fileExt))
                        {
                            loader = new WordDocumentLoader(ms, password);
                        }
                        else if (CellExtensions.Contains(fileExt))
                        {
                            loader = new WorkbookLoader(ms, password);
                        }
                        else if (SlideExtensions.Contains(fileExt))
                        {
                            loader = new PresentationLoader(ms, password);
                        }
                        else if (PdfExtensions.Contains(fileExt))
                        {
                            loader = new PdfLoader(ms, password);
                        }

                        if (loader != null && loader.IsPasswordCorrect)
                        {
                            return loader.GetUnprotectedStream(destination);
                        }
                    }
                }
            }
            return null;
        }
    }
}
