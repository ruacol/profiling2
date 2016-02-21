using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Organizations
{
    public class OrganizationRelationship : Entity
    {
        public virtual Organization SubjectOrganization { get; set; }
        public virtual Organization ObjectOrganization { get; set; }
        public virtual OrganizationRelationshipType OrganizationRelationshipType { get; set; }
        public virtual int DayOfStart { get; set; }
        public virtual int MonthOfStart { get; set; }
        public virtual int YearOfStart { get; set; }
        public virtual int DayOfEnd { get; set; }
        public virtual int MonthOfEnd { get; set; }
        public virtual int YearOfEnd { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }
    }
}
