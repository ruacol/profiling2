using System;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class HomeController : BaseController
    {
        protected readonly IPersonStatisticTasks personStatisticTasks;

        public HomeController(IPersonStatisticTasks personStatisticTasks)
        {
            this.personStatisticTasks = personStatisticTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Index()
        {
            ViewBag.Created = this.personStatisticTasks.GetCreatedProfilesCountByMonth();
            return View();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Counts()
        {
            ViewBag.CurrentProfilingCounts = this.personStatisticTasks.GetProfilingCountsView(null, null);
            return View(new DateViewModel() { EndDate = DateTime.Now.ToString() });
        }

        [HttpPost]
        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Counts(DateViewModel vm)
        {
            ViewBag.CurrentProfilingCounts = this.personStatisticTasks.GetProfilingCountsView(null, null);
            ViewBag.ProfilingCounts = this.personStatisticTasks.GetProfilingCountsView(vm.StartDateAsDate, null);
            return View(vm);
        }
    }
}
