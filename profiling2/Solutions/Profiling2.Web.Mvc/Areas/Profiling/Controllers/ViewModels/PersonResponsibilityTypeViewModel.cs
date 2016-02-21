using System.ComponentModel.DataAnnotations;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class PersonResponsibilityTypeViewModel
    {
        [Required(ErrorMessage = "A name is required for this responsibility type.")]
        [StringLength(255, ErrorMessage = "Name must not be longer than 255 characters.")]
        public string PersonRelationshipTypeName { get; set; }
        public bool Archive { get; set; }
        public string Notes { get; set; }
        public bool IsCommutative { get; set; }
    }
}