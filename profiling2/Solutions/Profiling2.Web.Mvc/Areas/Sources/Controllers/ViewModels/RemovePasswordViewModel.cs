using System.ComponentModel.DataAnnotations;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels
{
    public class RemovePasswordViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Password { get; set; }

        public RemovePasswordViewModel() { }
    }
}