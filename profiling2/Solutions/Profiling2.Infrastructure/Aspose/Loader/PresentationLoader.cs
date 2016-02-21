using System;
using System.IO;
using Aspose.Slides;
using Aspose.Slides.Export;

namespace Profiling2.Infrastructure.Aspose.Loader
{
    public class PresentationLoader : BaseAsposeLoader
    {
        protected Presentation Presentation { get; set; }

        public PresentationLoader(Stream stream, string password)
            : base(stream)
        {
            try
            {
                LoadOptions loadOptions = new LoadOptions(LoadFormat.Auto);
                loadOptions.Password = password;
                this.Presentation = new Presentation(stream, loadOptions);
                this.Presentation.ProtectionManager.RemoveEncryption();

                try
                {
                    if (this.Presentation.ProtectionManager.IsWriteProtected)
                        this.Presentation.ProtectionManager.RemoveWriteProtection();
                }
                catch (Exception e)
                {
                    this.Exception = e;
                }
                this.IsPasswordCorrect = true;
            }
            catch (Exception e)
            {
                this.Exception = e;
            }
        }

        public override Stream GetUnprotectedStream(Stream destination)
        {
            if (this.IsPasswordCorrect && this.Presentation != null)
            {
                this.Presentation.Save(destination, SaveFormat.Pptx);
                return destination;
            }
            return null;
        }

        public override Stream GetHtml(Stream destination)
        {
            if (this.Presentation != null)
            {
                this.Presentation.Save(destination, SaveFormat.Html);
                return destination;
            }
            throw new LoadSourceException(this.Exception.Message, this.Exception.InnerException);
        }
    }
}
