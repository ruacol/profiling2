using System;
using System.ComponentModel.DataAnnotations;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class AdminSuggestionPersonResponsibilityViewModel
    {
        [Required(ErrorMessage = "A person is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid person is required.")]
        public int? PersonId { get; set; }

        [Required(ErrorMessage = "An event is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid event is required.")]
        public int? EventId { get; set; }

        public bool IsAccepted { get; set; }
        public string SuggestionFeatures { get; set; }
        public DateTime DecisionDateTime { get; set; }
        public int DecisionAdminUserId { get; set; }
        public bool Archive { get; set; }
        public string Notes { get; set; }
    }
}