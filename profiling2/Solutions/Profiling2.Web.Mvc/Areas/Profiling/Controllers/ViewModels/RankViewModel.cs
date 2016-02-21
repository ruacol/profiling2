using Profiling2.Domain.Prf.Careers;
using System.ComponentModel.DataAnnotations;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class RankViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A name is required for this rank.")]
        [StringLength(255, ErrorMessage = "Name must not be longer than 255 characters.")]
        public string RankName { get; set; }

        [StringLength(255, ErrorMessage = "Name (French) must not be longer than 255 characters.")]
        public string RankNameFr { get; set; }

        public string Description { get; set; }
        public bool Archive { get; set; }
        public string Notes { get; set; }
        public int SortOrder { get; set; }

        public RankViewModel() { }
    }
}