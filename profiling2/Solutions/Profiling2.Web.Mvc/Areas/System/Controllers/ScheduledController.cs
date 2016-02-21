using System.Web.Mvc;
using log4net;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Scr;
using Profiling2.Web.Mvc.Controllers;

namespace Profiling2.Web.Mvc.Areas.System.Controllers
{
    [Compress]
    public class ScheduledController : Controller
    {
        protected static ILog log = LogManager.GetLogger(typeof(ScheduledController));
        protected readonly ISourceManagementTasks sourceManagementTasks;

        /// <summary>
        /// No authorize attribute means these actions can be called as scheduled tasks.  Actions should check for Request.IsLocal.
        /// </summary>
        /// <param name="sourceTasks"></param>
        public ScheduledController(ISourceManagementTasks sourceManagementTasks)
        {
            this.sourceManagementTasks = sourceManagementTasks;
        }

        /// <summary>
        /// Merges source hash duplicates.  Slow running.  Runs in a transaction.  Holds lock on source table.
        /// 
        /// Run in off-hours so as not to interfere with daily work.
        /// </summary>
        /// <returns></returns>
        public ActionResult CleanDuplicatesByHash()
        {
            if (Request.IsLocal)
            {
                this.sourceManagementTasks.CleanHashDuplicates();
            }
            return null;
        }
    }
}