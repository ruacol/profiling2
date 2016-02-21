using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Web.Mvc.Controllers;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
    public class AdminSourceSearchesController : BaseController
    {
        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceAttachmentTasks sourceAttachmentTasks;

        public AdminSourceSearchesController(ISourceTasks sourceTasks, ISourceAttachmentTasks sourceAttachmentTasks)
        {
            this.sourceTasks = sourceTasks;
            this.sourceAttachmentTasks = sourceAttachmentTasks;
        }

        public ActionResult Details(int id)
        {
            AdminSourceSearch model = this.sourceAttachmentTasks.GetAdminSourceSearch(id);
            return View(model);
        }
    }
}
