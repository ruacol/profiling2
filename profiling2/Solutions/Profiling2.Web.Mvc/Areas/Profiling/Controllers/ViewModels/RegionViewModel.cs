using Profiling2.Domain.Prf;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class RegionViewModel
    {
        public int Id { get; set; }
        public string RegionName { get; set; }
        public string Notes { get; set; }
        public bool Archive { get; set; }

        public RegionViewModel() { }

        public RegionViewModel(Region reg)
        {
            if (reg != null)
            {
                this.Id = reg.Id;
                this.RegionName = reg.RegionName;
                this.Notes = reg.Notes;
                this.Archive = reg.Archive;
            }
        }
    }
}