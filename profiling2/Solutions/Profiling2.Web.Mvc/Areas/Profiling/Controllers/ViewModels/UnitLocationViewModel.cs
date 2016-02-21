using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Helpers;
using AutoMapper;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class UnitLocationViewModel : PeriodBaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A unit is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid unit is required.")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "A location is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid location is required.")]
        public int LocationId { get; set; }

        [Range(0, 31, ErrorMessage = "As Of Day must be between 0 and 31.")]
        public int DayAsOf { get; set; }

        [Range(0, 12, ErrorMessage = "As Of Month must be between 0 and 12.")]
        public int MonthAsOf { get; set; }

        [Range(0, 2030, ErrorMessage = "As Of Year must be between 0 and 2030.")]
        public int YearAsOf { get; set; }

        public string Commentary { get; set; }
        public string Notes { get; set; }

        // for display purposes only
        public string UnitName { get; set; }
        public string LocationName { get; set; }
        public string AsOfDate { get; set; }

        public UnitLocationViewModel() { }

        public UnitLocationViewModel(UnitLocation ul)
        {
            if (ul != null)
            {
                if (ul.Unit != null)
                {
                    this.UnitId = ul.Unit.Id;
                    this.UnitName = ul.Unit.UnitName;
                }
                if (ul.Location != null)
                {
                    this.LocationId = ul.Location.Id;
                    this.LocationName = ul.Location.LocationName;
                }
            }

            Mapper.Map<UnitLocation, UnitLocationViewModel>(ul, this);

            this.AsOfDate = new DateLabel(this.YearAsOf, this.MonthAsOf, this.DayAsOf, false).ToString();
        }
    }
}