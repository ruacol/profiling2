using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Documentation
{
    public class DocumentationFile : Entity
    {
        public virtual string FileName { get; set; }
        public virtual byte[] FileData { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime LastModifiedDate { get; set; }
        public virtual DateTime UploadedDate { get; set; }
        public virtual AdminUser UploadedBy { get; set; }
        public virtual DocumentationFileTag DocumentationFileTag { get; set; }
        public virtual bool Archive { get; set; }
    }
}
