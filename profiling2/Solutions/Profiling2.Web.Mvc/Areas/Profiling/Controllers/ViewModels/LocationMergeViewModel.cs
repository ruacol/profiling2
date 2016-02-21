using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class LocationMergeViewModel : LocationViewModel
    {
        public int ToDeleteLocationId { get; set; }
        public int NumEvents { get; set; }
        public int NumCareers { get; set; }
        public int NumUnitLocations { get; set; }

        /// <summary>
        /// This is the Location to keep when merging.
        /// </summary>
        public LocationMergeViewModel() { }
    }
}