using Profiling2.Domain.Prf.Sources;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Organizations
{
    public class OrganizationSource : Entity
    {
        public virtual Organization Organization { get; set; }
        public virtual Source Source { get; set; }
        public virtual Reliability Reliability { get; set; }
        public virtual string Commentary { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return (this.Organization != null ? "Organization(ID=" + this.Organization.Id.ToString() + ")" : string.Empty)
                + (this.Source != null ? " is linked with Source(ID=" + this.Source.Id.ToString() + ")" : string.Empty);
        }
    }
}
