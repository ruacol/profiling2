using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Web.Mvc.Controllers;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers
{
    public class HomeController : BaseController
    {
        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceStatisticTasks sourceStatisticTasks;

        public HomeController(ISourceTasks sourceTasks, ISourceStatisticTasks sourceStatisticTasks)
        {
            this.sourceTasks = sourceTasks;
            this.sourceStatisticTasks = sourceStatisticTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public ActionResult Index()
        {
            ViewBag.LastImportDate = this.sourceStatisticTasks.GetLastAdminSourceImportDate();
            ViewBag.TotalSize = this.sourceStatisticTasks.GetTotalSourceSize();
            ViewBag.TotalArchivedSize = this.sourceStatisticTasks.GetTotalArchivedSourceSize();
            ViewBag.Count = this.sourceTasks.GetSearchTotal(true, null, null, null, null, null, null);
            ViewBag.CountArchived = this.sourceStatisticTasks.GetNumArchived();
            ViewBag.ImportCounts = this.sourceStatisticTasks.GetSourceImportsByDay();
            return View();
        }
    }
}