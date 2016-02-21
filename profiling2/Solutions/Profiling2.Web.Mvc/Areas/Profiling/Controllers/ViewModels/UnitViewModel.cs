using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Validators;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    [UniqueUnit]
    public class UnitViewModel : PeriodBaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A name is required for this unit.")]
        [StringLength(255, ErrorMessage = "Name must not be longer than 255 characters.")]
        public string UnitName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid organization is required.")]
        public int? OrganizationId { get; set; }

        public string BackgroundInformation { get; set; }
        public bool Archive { get; set; }
        public string Notes { get; set; }

        // display only
        public string OrganizationName { get; set; }

        public UnitViewModel() { }

        public UnitViewModel(Unit u)
        {
            if (u != null)
            {
                this.Id = u.Id;
                this.UnitName = u.UnitName;
                if (u.Organization != null)
                {
                    this.OrganizationId = u.Organization.Id;
                    this.OrganizationName = u.Organization.OrgShortName;
                }
                this.DayOfStart = u.DayOfStart;
                this.MonthOfStart = u.MonthOfStart;
                this.YearOfStart = u.YearOfStart;
                this.DayOfEnd = u.DayOfEnd;
                this.MonthOfEnd = u.MonthOfEnd;
                this.YearOfEnd = u.YearOfEnd;

                this.BackgroundInformation = u.BackgroundInformation;
                this.Archive = u.Archive;
                this.Notes = u.Notes;
            }
        }
    }
}