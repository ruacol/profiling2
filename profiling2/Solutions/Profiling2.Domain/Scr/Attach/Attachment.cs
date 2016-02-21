using System;
using SharpArch.Domain.DomainModel;
using Profiling2.Domain.Prf;
using System.Collections.Generic;

namespace Profiling2.Domain.Scr.Attach
{
    public class Attachment : Entity
    {
        public virtual string FileName { get; set; }
        public virtual byte[] FileData { get; set; }
        public virtual DateTime UploadedDateTime { get; set; }
        public virtual AdminUser UploadedByAdminUser { get; set; }
        public virtual string FileExtension { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }
        public virtual IList<RequestAttachment> RequestAttachments { get; set; }

        public Attachment()
        {
            this.RequestAttachments = new List<RequestAttachment>();
        }
    }
}
