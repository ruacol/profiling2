using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Validators;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    [UniqueUnitOperation]
    public class UnitOperationViewModel : PeriodBaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "An operation has not been selected.")]
        public int? OperationId { get; set; }

        [Required(ErrorMessage = "A unit has not been selected.")]
        public int? UnitId { get; set; }

        public bool IsCommandUnit { get; set; }

        // display only
        public string UnitName { get; set; }
        public string OperationName { get; set; }

        public UnitOperationViewModel() { }

        public UnitOperationViewModel(UnitOperation uo)
        {
            if (uo != null)
            {
                Mapper.Map(uo, this);
                if (uo.Unit != null)
                {
                    this.UnitId = uo.Unit.Id;
                    this.UnitName = uo.Unit.UnitName;
                }
                if (uo.Operation != null)
                {
                    this.OperationId = uo.Operation.Id;
                    this.OperationName = uo.Operation.Name;
                }
            }
        }
    }
}