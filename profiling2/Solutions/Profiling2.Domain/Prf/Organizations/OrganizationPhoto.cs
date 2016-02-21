using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Organizations
{
    public class OrganizationPhoto : Entity
    {
        public virtual Organization Organization { get; set; }
        public virtual Photo Photo { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }
    }
}
