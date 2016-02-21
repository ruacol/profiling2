using System.Linq;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Web.Mvc.Controllers;

namespace Profiling2.Web.Mvc.Areas.Hrdb.Controllers
{
    [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
    public class HomeController : BaseController
    {
        protected readonly ISourceTasks sourceTasks;

        public HomeController(ISourceTasks sourceTasks)
        {
            this.sourceTasks = sourceTasks;
        }

        public ActionResult Index()
        {
            return View(this.sourceTasks.GetJhroCases().Where(x => x.IsHRDB()));
        }

        public ActionResult Old()
        {
            return View(this.sourceTasks.GetJhroCases().Where(x => !x.IsHRDB()));
        }
    }
}