using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class EthnicityViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A name is required for this ethnicity.")]
        [StringLength(255, ErrorMessage = "Name must not be longer than 255 characters.")]
        public string EthnicityName { get; set; }

        public bool Archive { get; set; }

        public string Notes { get; set; }

        public EthnicityViewModel() { }
    }
}