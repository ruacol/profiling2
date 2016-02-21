using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Units;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Responsibility
{
    public class OrganizationResponsibility : Entity
    {
        [Audited]
        public virtual Organization Organization { get; set; }
        [Audited]
        public virtual Event Event { get; set; }
        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual OrganizationResponsibilityType OrganizationResponsibilityType { get; set; }
        [Audited]
        public virtual string Commentary { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }
        [Audited]
        public virtual Unit Unit { get; set; }

        public override string ToString()
        {
            return "OrganizationResponsibility(ID=" + this.Id + ")"
                + ", Organization(ID=" + this.Organization.Id + ")"
                + (this.Unit == null ? string.Empty : ", Unit(ID=" + this.Unit.Id + ")")
                + ", Event(ID=" + this.Event.Id + ")"
                + ", OrganizationResponsibilityType=(" + this.OrganizationResponsibilityType.ToString() + ")";
        }
    }
}
