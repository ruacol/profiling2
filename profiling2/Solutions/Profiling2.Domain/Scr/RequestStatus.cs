using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr
{
    public class RequestStatus : Entity
    {
        public const string NAME_CREATED = "Created";
        public const string NAME_SENT_FOR_VALIDATION = "Sent for validation";
        public const string NAME_DELETED = "Deleted";
        public const string NAME_EDITED = "Edited";
        public const string NAME_REJECTED = "Rejected";
        public const string NAME_SENT_FOR_SCREENING = "Sent for screening";
        public const string NAME_SCREENING_IN_PROGRESS = "Screening in progress";
        public const string NAME_SENT_FOR_CONSOLIDATION = "Sent for consolidation";
        public const string NAME_SENT_FOR_FINAL_DECISION = "Sent for final decision";
        public const string NAME_COMPLETED = "Completed";

        public virtual string RequestStatusName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.RequestStatusName;
        }
    }
}
