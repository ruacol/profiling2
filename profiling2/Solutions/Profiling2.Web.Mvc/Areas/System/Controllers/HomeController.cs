using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks.Sources;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.System.Controllers
{
    public class HomeController : SystemBaseController
    {
        protected readonly ISourceManagementTasks sourceManagementTasks;

        public HomeController(ISourceManagementTasks sourceManagementTasks)
        {
            this.sourceManagementTasks = sourceManagementTasks;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowPersonSourceDuplicates()
        {
            return View(this.sourceManagementTasks.GetPersonSourceDuplicates());
        }

        [Transaction]
        public ActionResult MergePersonSourceDuplicates()
        {
            this.sourceManagementTasks.MergePersonSourceDuplicates();
            return RedirectToAction("ShowPersonSourceDuplicates");
        }

        public ActionResult ShowEventSourceDuplicates()
        {
            return View(this.sourceManagementTasks.GetEventSourceDuplicates());
        }

        [Transaction]
        public ActionResult MergeEventSourceDuplicates()
        {
            this.sourceManagementTasks.MergeEventSourceDuplicates();
            return RedirectToAction("ShowEventSourceDuplicates");
        }

        public ActionResult ShowOrganizationSourceDuplicates()
        {
            return View(this.sourceManagementTasks.GetOrganizationSourceDuplicates());
        }

        [Transaction]
        public ActionResult MergeOrganizationSourceDuplicates()
        {
            this.sourceManagementTasks.MergeOrganizationSourceDuplicates();
            return RedirectToAction("ShowOrganizationSourceDuplicates");
        }
    }
}
