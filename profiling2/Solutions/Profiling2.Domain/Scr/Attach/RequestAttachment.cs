using SharpArch.Domain.DomainModel;
using System.Collections.Generic;
using System.Linq;

namespace Profiling2.Domain.Scr.Attach
{
    public class RequestAttachment : Entity
    {
        public virtual Request Request { get; set; }
        public virtual Attachment Attachment { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }
        public virtual IList<RequestAttachmentHistory> Histories { get; set; }

        public RequestAttachment()
        {
            this.Histories = new List<RequestAttachmentHistory>();
        }

        public virtual RequestAttachmentHistory MostRecentHistory
        {
            get
            {
                if (this.Histories != null)
                {
                    return (from h in this.Histories
                            orderby h.DateStatusReached
                            select h).Last<RequestAttachmentHistory>();
                }
                else
                {
                    return null;
                }
            }
        }

        public virtual RequestAttachmentStatus CurrentStatus
        {
            get
            {
                if (this.MostRecentHistory != null)
                {
                    return this.MostRecentHistory.RequestAttachmentStatus;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
