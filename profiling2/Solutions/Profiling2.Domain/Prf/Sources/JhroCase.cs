using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Prf.Events;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Sources
{
    public class JhroCase : Entity
    {
        [Audited]
        public virtual string CaseNumber { get; set; }
        public virtual byte[] HrdbContentsSerialized { get; set; }

        public virtual IList<Source> Sources { get; set; }
        [Audited]
        public virtual IList<Event> Events { get; set; }

        public JhroCase()
        {
            this.Sources = new List<Source>();
            this.Events = new List<Event>();
        }

        public virtual bool IsHRDB()
        {
            return this.HrdbContentsSerialized != null;
        }

        public override string ToString()
        {
            return "HRDB Case Code=" + this.CaseNumber + (this.Events == null ? string.Empty : ", EventIDs=" + string.Join(",", this.Events.Select(x => x.Id.ToString())));
        }

        // data modification

        public virtual void AddEvent(Event e)
        {
            if (this.Events.Contains(e))
                return;

            this.Events.Add(e);
        }
    }
}
