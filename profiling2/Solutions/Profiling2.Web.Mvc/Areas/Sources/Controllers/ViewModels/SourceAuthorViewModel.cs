using System.ComponentModel.DataAnnotations;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels
{
    public class SourceAuthorViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "An author name is required.")]
        public string Author { get; set; }

        public SourceAuthorViewModel() { }
    }
}