using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr.Proposed
{
    public class RequestProposedPersonStatus : Entity
    {
        public const string NAME_PROPOSED = "Proposed";
        public const string NAME_WITHDRAWN = "Withdrawn";

        public virtual string RequestProposedPersonStatusName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.RequestProposedPersonStatusName;
        }
    }
}
