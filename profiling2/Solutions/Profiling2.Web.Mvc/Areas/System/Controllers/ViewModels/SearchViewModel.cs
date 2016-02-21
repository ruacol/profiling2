using System.ComponentModel.DataAnnotations;

namespace Profiling2.Web.Mvc.Areas.System.Controllers.ViewModels
{
    public class SearchViewModel
    {
        [Required]
        public string Term { get; set; }

        public SearchViewModel() { }
    }
}