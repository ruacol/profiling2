using System;

namespace Profiling2.Domain.Prf.Sources
{
    public class SourceDTO : BaseSourceDTO
    {
        // convenience columns
        public Int64 FileSize { get; set; }
        public string JhroCaseNumber { get; set; }
        public int JhroCaseID { get; set; }
        public bool HasOcrText { get; set; }
        public string UploadedByUserID { get; set; }

        public string GetFileDownloadName()
        {
            if (string.IsNullOrEmpty(this.JhroCaseNumber))
            {
                return this.SourceName;
            }
            else
            {
                return string.Join(".", new string[] { this.JhroCaseNumber.Replace('/', '-'), this.FileExtension });
            }
        }
    }
}
