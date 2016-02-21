using NHibernate.Envers.Configuration.Attributes;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Units
{
    public class UnitAlias : Entity
    {
        [Audited]
        public virtual Unit Unit { get; set; }
        [Audited]
        public virtual string UnitName { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }
    }
}
