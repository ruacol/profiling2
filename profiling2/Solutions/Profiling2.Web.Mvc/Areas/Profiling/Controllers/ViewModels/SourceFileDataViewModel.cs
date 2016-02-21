using System.Web;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class SourceFileDataViewModel
    {
        public int Id { get; set; }
        public string SourceName { get; set; }
        public HttpPostedFileBase FileData { get; set; }

        public SourceFileDataViewModel() { }

        public SourceFileDataViewModel(Source s)
        {
            if (s != null)
            {
                this.Id = s.Id;
                this.SourceName = s.SourceName;
            }
        }
    }
}