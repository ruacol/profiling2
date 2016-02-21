using System.Web.Mvc;
using Hangfire;
using Profiling2.Domain.Contracts;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Infrastructure.Util;

namespace Profiling2.Web.Mvc.Areas.System.Controllers
{
    public class BackgroundController : SystemBaseController
    {
        protected readonly IFeedingSourceTasks feedingSourceTasks;
        protected readonly IPostalEmailTasks postalEmailTasks;

        public BackgroundController(IFeedingSourceTasks feedingSourceTasks,
            IPostalEmailTasks postalEmailTasks)
        {
            this.feedingSourceTasks = feedingSourceTasks;
            this.postalEmailTasks = postalEmailTasks;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegisterSourceFolderCountsReportJob()
        {
            RecurringJob.AddOrUpdate<IPostalEmailTasks>("source-folder-counts-report", x => x.SendSourceFolderCountsReport(), Cron.Monthly());
            return null;
        }

        public ActionResult RegisterProfilingCountsReportJob()
        {
            RecurringJob.AddOrUpdate<IPostalEmailTasks>("profiling-counts-report", x => x.SendProfilingCountsReportEmail(), Cron.Weekly());
            return null;
        }

        public ActionResult RegisterFeedingSourceReportJob()
        {
            RecurringJob.AddOrUpdate<IPostalEmailTasks>("feeding-source-report", x => x.SendFeedingSourceReport(), Cron.Weekly());
            return null;
        }

        public ActionResult RegisterTempCleanupJob()
        {
            RecurringJob.AddOrUpdate("temp-cleanup", () => FileUtil.DeleteTempDirFiles(), Cron.Daily(1));
            return null;
        }

        public ActionResult RegisterProcessSourcePreviewsJob()
        {
            RecurringJob.AddOrUpdate<ISourceContentTasks>("process-source-previews", x => x.ProcessPreviewProblemsQueueable(), Cron.Daily(4));
            return null;
        }

        public ActionResult RegisterDailyOhchrImportJob()
        {
            RecurringJob.AddOrUpdate<IOhchrWebServiceTasks>("daily-ohchr-import", x => x.GetAndPersistHrdbCasesQueueable(), Cron.Daily(3));
            return null;
        }

        public ActionResult RegisterResetLuceneIndexesJob()
        {
            RecurringJob.AddOrUpdate<IBackgroundTasks>("reset-lucene-indexes", x => x.ResetLuceneIndexes(), Cron.Daily(5));
            return null;
        }

        public ActionResult RegisterUpdateSourceIndexJob()
        {
            RecurringJob.AddOrUpdate<IBackgroundTasks>("update-source-index", x => x.UpdateSourceIndex(), Cron.Daily(2));
            return null;
        }
    }
}