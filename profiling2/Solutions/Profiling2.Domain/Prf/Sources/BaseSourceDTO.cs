using System;

namespace Profiling2.Domain.Prf.Sources
{
    /// <summary>
    /// Contains immediate members of Source entity, barring potentially OutOfMemoryError-inducing binary columns.
    /// </summary>
    public abstract class BaseSourceDTO
    {
        public int SourceID { get; set; }
        public string SourceName { get; set; }
        public string FullReference { get; set; }
        public string SourcePath { get; set; }
        public DateTime? SourceDate { get; set; }
        public string FileExtension { get; set; }
        public bool IsRestricted { get; set; }
        public DateTime? FileDateTimeStamp { get; set; }
        public bool Archive { get; set; }
        public bool IsReadOnly { get; set; }
        public string Notes { get; set; }
        public bool IsPublic { get; set; }
    }
}
