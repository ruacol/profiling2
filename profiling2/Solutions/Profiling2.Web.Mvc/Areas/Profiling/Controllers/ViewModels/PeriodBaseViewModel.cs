using System.ComponentModel.DataAnnotations;
using Profiling2.Domain;
using Profiling2.Web.Mvc.Validators;
using Profiling2.Web.Mvc.Helpers;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    [ValidDatePeriod]
    public class PeriodBaseViewModel : IIncompleteDate
    {
        [Range(0, 31, ErrorMessage = "Start Date Day must be between 0 and 31.")]
        public int DayOfStart { get; set; }

        [Range(0, 12, ErrorMessage = "Start Date Month must be between 0 and 12.")]
        public int MonthOfStart { get; set; }

        [Range(0, 2030, ErrorMessage = "Start Date Year must be between 0 and 2030.")]  // TODO doesn't block two digit years, which don't fit into DateTime constructorx
        public int YearOfStart { get; set; }

        [Range(0, 31, ErrorMessage = "End Date Day must be between 0 and 31.")]
        public int DayOfEnd { get; set; }

        [Range(0, 12, ErrorMessage = "End Date Month must be between 0 and 12.")]
        public int MonthOfEnd { get; set; }

        [Range(0, 2030, ErrorMessage = "End Date Year must be between 0 and 2030.")]
        public int YearOfEnd { get; set; }

        // for display purposes only
        public string StartDate
        {
            get
            {
                return new DateLabel(this.YearOfStart, this.MonthOfStart, this.DayOfStart, false).ToString();
            }
        }
        public string EndDate
        {
            get
            {
                return new DateLabel(this.YearOfEnd, this.MonthOfEnd, this.DayOfEnd, false).ToString();
            }
        }
    }
}