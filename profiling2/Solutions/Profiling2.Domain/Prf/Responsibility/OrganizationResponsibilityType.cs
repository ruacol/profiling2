using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Responsibility
{
    public class OrganizationResponsibilityType : Entity
    {
        public virtual string OrganizationResponsibilityTypeName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.OrganizationResponsibilityTypeName;
        }
    }
}
