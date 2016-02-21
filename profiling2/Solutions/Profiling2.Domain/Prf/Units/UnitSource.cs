using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Prf.Sources;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Units
{
    public class UnitSource : Entity
    {
        [Audited]
        public virtual Unit Unit { get; set; }
        [Audited]
        public virtual Source Source { get; set; }
        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual Reliability Reliability { get; set; }
        [Audited]
        public virtual string Commentary { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return (this.Unit != null ? "Unit(ID=" + this.Unit.Id.ToString() + ")" : string.Empty)
                + (this.Source != null ? " is linked with Source(ID=" + this.Source.Id.ToString() + ")" : string.Empty);
        }
    }
}
