using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Validators;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class OperationViewModel : PeriodBaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A name is required for this operation.")]
        [StringLength(255, ErrorMessage = "Name must not be longer than 500 characters.")]
        [UniqueOperationName]
        public string Name { get; set; }

        public string Objective { get; set; }

        public bool Archive { get; set; }

        public string Notes { get; set; }

        public OperationViewModel() { }

        public OperationViewModel(Operation o)
        {
            if (o != null)
            {
                Mapper.Map(o, this);
            }
        }
    }
}