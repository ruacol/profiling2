using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr.Attach
{
    public class RequestAttachmentStatus : Entity
    {
        public const int ADDED = 1;
        public const int REMOVED = 2;

        public virtual string RequestAttachmentStatusName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.RequestAttachmentStatusName;
        }
    }
}
