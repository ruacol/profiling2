using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers
{
    public class RequestEntityController : ScreeningBaseController
    {
        protected readonly IRequestTasks requestTasks;
        protected readonly IUserTasks userTasks;

        public RequestEntityController(IRequestTasks requestTasks, IUserTasks userTasks)
        {
            this.requestTasks = requestTasks;
            this.userTasks = userTasks;
        }

        public ActionResult Details(int id)
        {
            return View(this.requestTasks.GetRequestEntity(id));
        }
    }
}