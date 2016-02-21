using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class UnitAliasViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A unit is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid unit is required.")]
        public int UnitId { get; set; }

        [StringLength(500, ErrorMessage = "Unit name must not be longer than 500 characters.")]
        public string UnitName { get; set; }

        public bool Archive { get; set; }

        public string Notes { get; set; }

        public UnitAliasViewModel() { }

        public UnitAliasViewModel(UnitAlias ua)
        {
            this.Id = ua.Id;
            this.UnitId = ua.Unit.Id;
            this.UnitName = ua.UnitName;
            this.Archive = ua.Archive;
            this.Notes = ua.Notes;
        }
    }
}