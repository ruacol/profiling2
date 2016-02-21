using System;
using System.Linq;
using System.Web.Mvc;
using Mvc.JQuery.Datatables;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf.Sources;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.System.Controllers
{
    public class SourceIndexLogController : SystemBaseController
    {
        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceStatisticTasks sourceStatisticTasks;

        public SourceIndexLogController(ISourceTasks sourceTasks, ISourceStatisticTasks sourceStatisticTasks)
        {
            this.sourceTasks = sourceTasks;
            this.sourceStatisticTasks = sourceStatisticTasks;
        }

        public ActionResult Index()
        {
            return View();
        }

        public class SourceIndexLogView
        {
            public int SourceID { get; set; }
            public string LogSummary { get; set; }
            public string Log { get; set; }
            public DateTime DateTime { get; set; }
            public SourceIndexLogView(SourceIndexLog log)
            {
                this.SourceID = log.SourceID;
                this.LogSummary = log.LogSummary;
                this.Log = log.Log;
                this.DateTime = log.DateTime.HasValue ? log.DateTime.Value : new DateTime();
            }
        }

        public DataTablesResult<SourceIndexLogView> DataTables(DataTablesParam p)
        {
            // TODO Mvc.Jquery.DataTables throws exception when sorting by DateTime 
            return DataTablesResult.Create(this.sourceTasks.GetSourceIndexLogsWithErrors().Select(x => new SourceIndexLogView(x)).AsQueryable(), p);
        }

        public ActionResult Details(int id)
        {
            return View(this.sourceTasks.GetSourceIndexLog(null, id));
        }

        [Transaction]
        public ActionResult DeleteAll()
        {
            this.sourceTasks.DeleteAllSourceIndexLogs();
            return null;
        }

        public ActionResult Stats()
        {
            ViewData["numIndexLogs"] = this.sourceTasks.CountSourceIndexLogs();
            ViewData["numIndexLogErrors"] = this.sourceTasks.CountSourceIndexLogErrors();

            ViewData["numNonArchivedSources"] = this.sourceStatisticTasks.GetSourceCount();
            ViewData["numUnindexableSources"] = this.sourceTasks.GetUnindexableMediaSources().Count;

            return View();
        }
    }
}