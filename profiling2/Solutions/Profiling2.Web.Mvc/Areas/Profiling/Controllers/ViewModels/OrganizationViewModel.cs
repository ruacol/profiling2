using AutoMapper;
using Profiling2.Domain.Prf.Organizations;
using System.ComponentModel.DataAnnotations;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class OrganizationViewModel : PeriodBaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "An abbreviated name is required for this organization.")]
        [StringLength(255, ErrorMessage = "Name must not be longer than 255 characters.")]
        public string OrgShortName { get; set; }

        [Required(ErrorMessage = "A name is required for this organization.")]
        [StringLength(500, ErrorMessage = "Name must not be longer than 500 characters.")]
        public string OrgLongName { get; set; }
        
        public bool Archive { get; set; }
        public string Notes { get; set; }

        public OrganizationViewModel() { }

        public OrganizationViewModel(Organization o)
        {
            Mapper.Map<Organization, OrganizationViewModel>(o, this);
        }
    }
}