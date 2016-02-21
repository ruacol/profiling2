using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Web.Mvc.Validators;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    [UnitIsPartOfOrganization]
    public class OrgResponsibilityViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "An organization is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid organization is required.")]
        public int? OrganizationId { get; set; }
        public int? UnitId { get; set; }
        [Required(ErrorMessage = "An event is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid event is required.")]
        public int EventId { get; set; }
        [Required(ErrorMessage = "A responsibility type.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid responsibility type is required.")]
        public int OrganizationResponsibilityTypeId { get; set; }
        public string Commentary { get; set; }
        public bool Archive { get; set; }
        public string Notes { get; set; }

        // for display purposes only
        public string OrganizationName { get; set; }
        public string EventHeadline { get; set; }
        public string UnitName { get; set; }

        public SelectList OrgResponsibilityTypes { get; set; }

        public OrgResponsibilityViewModel() { }

        public OrgResponsibilityViewModel(Event e)
        {
            this.EventId = e.Id;
            this.EventHeadline = e.Headline;
        }

        public OrgResponsibilityViewModel(OrganizationResponsibility or)
        {
            this.Id = or.Id;
            this.OrganizationId = or.Organization.Id;
            this.OrganizationName = or.Organization.ToString();
            if (or.Unit != null)
            {
                this.UnitId = or.Unit.Id;
                this.UnitName = or.Unit.UnitName;
            }
            this.EventId = or.Event.Id;
            this.EventHeadline = or.Event.Headline;
            this.OrganizationResponsibilityTypeId = or.OrganizationResponsibilityType.Id;
            this.Commentary = or.Commentary;
            this.Archive = or.Archive;
            this.Notes = or.Notes;
        }

        public void PopulateDropDowns(IEnumerable<OrganizationResponsibilityType> orgResponsibilityTypes)
        {
            this.OrgResponsibilityTypes = new SelectList(orgResponsibilityTypes, "Id", "OrganizationResponsibilityTypeName", this.OrganizationResponsibilityTypeId);
        }
    }
}