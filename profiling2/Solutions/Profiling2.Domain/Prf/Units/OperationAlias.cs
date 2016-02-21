using NHibernate.Envers.Configuration.Attributes;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Units
{
    public class OperationAlias : Entity
    {
        [Audited]
        public virtual Operation Operation { get; set; }
        [Audited]
        public virtual string Name { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
