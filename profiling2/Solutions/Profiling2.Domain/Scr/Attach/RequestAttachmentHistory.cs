using System;
using SharpArch.Domain.DomainModel;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Scr.Attach
{
    public class RequestAttachmentHistory : Entity
    {
        public virtual RequestAttachment RequestAttachment { get; set; }
        public virtual RequestAttachmentStatus RequestAttachmentStatus { get; set; }
        public virtual DateTime DateStatusReached { get; set; }
        public virtual AdminUser AdminUser { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }
    }
}
