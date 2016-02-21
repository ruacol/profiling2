
namespace Profiling2.Domain.Prf.Events
{
    public class ViolationDataTableView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ConditionalityInterest { get; set; }
        public int NumberOfEvents { get; set; }
        //public int NumberOfPersonResponsibilities { get; set; }
        //public int NumberOfOrganizationResponsibilities { get; set; }
        public string ParentViolation { get; set; }

        public ViolationDataTableView() { }

        public ViolationDataTableView(Violation v)
        {
            this.Id = v.Id;
            this.Name = v.Name;
            this.Description = v.Description;
            this.ConditionalityInterest = v.ConditionalityInterest;
            //this.NumberOfEvents = v.Events.Count;
            //this.NumberOfPersonResponsibilities = v.PersonResponsibilityCount;
            //this.NumberOfOrganizationResponsibilities = v.OrganizationResponsibilityCount;
            if (v.ParentViolation != null)
                this.ParentViolation = v.ParentViolation.Name;
        }
    }
}