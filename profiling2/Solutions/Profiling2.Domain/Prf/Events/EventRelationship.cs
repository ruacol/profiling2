using NHibernate.Envers.Configuration.Attributes;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Events
{
    public class EventRelationship : Entity, IIncompleteDate
    {
        [Audited]
        public virtual Event SubjectEvent { get; set; }
        [Audited]
        public virtual Event ObjectEvent { get; set; }
        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual EventRelationshipType EventRelationshipType { get; set; }
        [Audited]
        public virtual int DayOfStart { get; set; }
        [Audited]
        public virtual int MonthOfStart { get; set; }
        [Audited]
        public virtual int YearOfStart { get; set; }
        [Audited]
        public virtual int DayOfEnd { get; set; }
        [Audited]
        public virtual int MonthOfEnd { get; set; }
        [Audited]
        public virtual int YearOfEnd { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return "EventRelationship(ID=" + this.Id + ")";
        }

        public virtual object ToJSON()
        {
            return new
                {
                    Id = this.Id,
                    ObjectEvent = this.ObjectEvent != null ? this.ObjectEvent.ToShortJSON() : null,
                    SubjectEvent = this.SubjectEvent != null ? this.SubjectEvent.ToShortJSON() : null,
                    RelationshipType = this.EventRelationshipType.ToString(),
                    Notes = this.Notes
                };
        }
    }
}
