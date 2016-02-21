using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Organizations
{
    public class OrganizationAlias : Entity
    {
        public virtual Organization Organization { get; set; }
        public virtual string OrgShortName { get; set; }
        public virtual string OrgLongName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }
    }
}
