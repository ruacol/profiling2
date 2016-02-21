using SharpArch.Domain.DomainModel;
using Profiling2.Domain.Prf.Sources;
using NHibernate.Envers.Configuration.Attributes;

namespace Profiling2.Domain.Prf.Persons
{
    public class PersonSource : Entity
    {
        [Audited]
        public virtual Person Person { get; set; }
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
            return (this.Person != null ? "Person(ID=" + this.Person.Id.ToString() + ")" : string.Empty)
                + (this.Source != null ? " is linked with Source(ID=" + this.Source.Id.ToString() + ")" : string.Empty);
        }
    }
}
