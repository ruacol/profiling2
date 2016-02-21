using Profiling2.Web.Mvc.Validators;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    [SensibleOperationNameChange]
    public class OperationNameChangeViewModel
    {
        public int CurrentOperationId { get; set; }
        public int? OldOperationId { get; set; }
        public int? NewOperationId { get; set; }

        public OperationNameChangeViewModel() { }
    }
}