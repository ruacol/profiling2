using System;
using System.IO;

namespace Profiling2.Infrastructure.Aspose.Loader
{
    public abstract class BaseAsposeLoader
    {
        protected string LOADING_ERROR_MESSAGE = "Problem initialising the file using Aspose.";
        protected Stream Stream { get; set; }
        protected Exception Exception { get; set; }
        public bool IsPasswordCorrect { get; set; }

        public BaseAsposeLoader(Stream stream)
        {
            this.IsPasswordCorrect = false;
            this.Stream = stream;
        }

        public abstract Stream GetUnprotectedStream(Stream destination);

        public abstract Stream GetHtml(Stream destination);
    }
}
