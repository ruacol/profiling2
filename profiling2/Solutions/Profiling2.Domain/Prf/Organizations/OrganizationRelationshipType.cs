using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Organizations
{
    public class OrganizationRelationshipType : Entity
    {
        public virtual string OrganizationRelationshipTypeName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.OrganizationRelationshipTypeName;
        }
    }
}
