using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Profiling2.Domain.Prf;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class LocationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A name is required for this location.")]
        [StringLength(255, ErrorMessage = "Name must not be longer than 255 characters.")]
        public string LocationName { get; set; }

        [StringLength(500, ErrorMessage = "Territory name must not be longer than 500 characters.")]
        public string Territory { get; set; }

        [StringLength(500, ErrorMessage = "Town name must not be longer than 500 characters.")]
        public string Town { get; set; }

        public int? RegionId { get; set; }

        public int? ProvinceId { get; set; }

        public float? Longitude { get; set; }

        public float? Latitude { get; set; }

        public bool Archive { get; set; }

        public string Notes { get; set; }

        // for display purposes only
        public string RegionName { get; set; }

        // for form population purposes only
        public SelectList RegionList { get; set; }
        public SelectList ProvinceList { get; set; }

        public LocationViewModel() { }

        public void PopulateDropDowns(IEnumerable<Region> regions, IEnumerable<Province> provinces)
        {
            this.RegionList = this.RegionId.HasValue ? new SelectList(regions, "Id", "RegionName", this.RegionId) : new SelectList(regions, "Id", "RegionName");
            this.ProvinceList = this.ProvinceId.HasValue ? new SelectList(provinces, "Id", "ProvinceName", this.ProvinceId) : new SelectList(provinces, "Id", "ProvinceName");
        }
    }
}