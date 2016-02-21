using System;

namespace Profiling2.Infrastructure.Aspose.Loader
{
    public class LoadSourceException : Exception
    {
        public LoadSourceException(string message, Exception e) 
            : base(message, e) 
        { 
        
        }
    }
}
