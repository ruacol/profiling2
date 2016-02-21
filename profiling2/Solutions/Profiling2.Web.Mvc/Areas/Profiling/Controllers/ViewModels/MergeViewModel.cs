using System.ComponentModel.DataAnnotations;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class MergeViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "A valid ID to keep is required.")]
        public int ToKeepId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid ID to delete is required.")]
        public int ToDeleteId { get; set; }

        public MergeViewModel() { }
    }
}