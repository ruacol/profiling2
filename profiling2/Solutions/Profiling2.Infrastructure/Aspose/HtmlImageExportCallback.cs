using Aspose.Words.Saving;
using System.IO;

namespace Profiling2.Infrastructure.Aspose
{
    public class HtmlImageExportCallback : IImageSavingCallback
    {
        /// <summary>
        /// Creates an empty stream as a filler during image saving.
        /// </summary>
        /// <param name="args">Arguments that will be modified</param>
        public void ImageSaving(ImageSavingArgs args)
        {
            MemoryStream imageStream = new MemoryStream();
            args.ImageStream = imageStream;
            args.KeepImageStreamOpen = false;
        }
    }
}
