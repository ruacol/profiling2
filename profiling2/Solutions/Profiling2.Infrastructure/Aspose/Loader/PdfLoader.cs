using System;
using System.Configuration;
using System.IO;
using Aspose.Pdf;

namespace Profiling2.Infrastructure.Aspose.Loader
{
    public class PdfLoader : BaseAsposeLoader
    {
        protected Document Document { get; set; }

        public PdfLoader(Stream stream, string password)
            : base(stream)
        {
            try
            {
                this.Document = new Document(stream, password);
                this.Document.Decrypt();

                this.IsPasswordCorrect = true;
            }
            catch (Exception e)
            {
                this.Exception = e;
            }
        }

        public override Stream GetUnprotectedStream(Stream destination)
        {
            if (this.IsPasswordCorrect && this.Document != null)
            {
                this.Document.Save(destination);
                return destination;
            }
            return null;
        }

        public override Stream GetHtml(Stream destination)
        {
            if (this.Document != null)
            {
                HtmlSaveOptions opts = new HtmlSaveOptions();
                // these options go together, but are incompatible with saving to a stream (custom strategies cannot be null).
                opts.PartsEmbeddingMode = HtmlSaveOptions.PartsEmbeddingModes.EmbedAllIntoHtml;
                opts.RasterImagesSavingMode = HtmlSaveOptions.RasterImagesSavingModes.AsEmbeddedPartsOfPngPageBackground;
                opts.LettersPositioningMethod = HtmlSaveOptions.LettersPositioningMethods.UseEmUnitsAndCompensationOfRoundingErrorsInCss;
                opts.FontSavingMode = HtmlSaveOptions.FontSavingModes.SaveInAllFormats;
                opts.CustomResourceSavingStrategy = null;
                opts.CustomCssSavingStrategy = null;
                opts.CustomStrategyOfCssUrlCreation = null;

                // save HTML to file - ideally we'd save and return a Stream, but this seems to be currently (Aspose.Pdf 9.6.0) incompatible with the above options.
                string fileName = string.IsNullOrEmpty(this.Document.FileName) ? "pdfPreview" + DateTime.Now.Ticks.ToString() + ".pdf" : this.Document.FileName;
                string path = Path.Combine(ConfigurationManager.AppSettings["PreviewTempFolder"], fileName);
                this.Document.Save(path, opts);

                FileStream fs = null;
                try
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    fs.CopyTo(destination);
                    return destination;
                }
                finally
                {
                    if (fs != null)
                        fs.Dispose();
                    if (File.Exists(path))
                        File.Delete(path);
                }
            }
            throw new LoadSourceException(this.Exception.Message, this.Exception.InnerException);
        }
    }
}
