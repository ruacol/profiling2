using SharpArch.Domain.DomainModel;
using System.Collections.Generic;

namespace Profiling2.Domain.Scr.Proposed
{
    public class RequestProposedPerson : Entity
    {
        public virtual Request Request { get; set; }
        public virtual string MilitaryIDNumber { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string PersonName { get; set; }
        public virtual IList<RequestProposedPersonHistory> RequestProposedPersonHistories { get; set; }

        public RequestProposedPerson()
        {
            this.RequestProposedPersonHistories = new List<RequestProposedPersonHistory>();
        }

        public virtual void AddRequestProposedPersonHistory(RequestProposedPersonHistory h)
        {
            this.RequestProposedPersonHistories.Add(h);
        }
    }
}
