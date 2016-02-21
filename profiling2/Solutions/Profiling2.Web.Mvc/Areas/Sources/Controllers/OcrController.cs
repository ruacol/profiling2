using System.Net;
using System.Web.Mvc;
using log4net;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers
{
    [PermissionAuthorize(AdminPermission.CanAdministrate)]
    public class OcrController : BaseController
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(OcrController));
        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceContentTasks sourceContentTasks;
        protected readonly ISourceManagementTasks sourceManagementTasks;

        public OcrController(ISourceTasks sourceTasks, 
            ISourceContentTasks sourceContentTasks, 
            ISourceManagementTasks sourceManagementTasks)
        {
            this.sourceTasks = sourceTasks;
            this.sourceContentTasks = sourceContentTasks;
            this.sourceManagementTasks = sourceManagementTasks;
        }

        public ActionResult Index()
        {
            int days = 0;
            int.TryParse(Request.QueryString["days"], out days);
            return View(this.sourceManagementTasks.GetScannableSourceDTOs(days));
        }

        [Transaction]
        public JsonNetResult Scan(int id)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null)
            {
                source = this.sourceContentTasks.OcrScanAndSetSource(source);

                if (source.OriginalFileData != null)
                {
                    this.sourceTasks.SaveSource(source);
                }

                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Source was scanned and " + (source.OriginalFileData == null ? " NO " : "") + " text was found.");
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(null);
        }

        /// <summary>
        /// TODO take x days input and reverse SourceID
        /// </summary>
        /// <returns></returns>
        [Transaction]
        public ActionResult ScanScannableSources()
        {
            int days = 0;
            int.TryParse(Request.QueryString["days"], out days);

            log.Info("Starting OCR scan of sources newer than " + days + " days.");

            int numUpdated = 0;
            int numNotUpdated = 0;
            foreach (SourceDTO dto in this.sourceManagementTasks.GetScannableSourceDTOs(days))
            {
                Source source = this.sourceTasks.GetSource(dto.SourceID);
                source = this.sourceContentTasks.OcrScanAndSetSource(source);
                if (source.OriginalFileData != null)
                    numUpdated++;
                else
                    numNotUpdated++;
            }

            log.Info(numUpdated + " sources updated with OCR, " + numNotUpdated + " not updated.");

            return RedirectToAction("Index", new { days = days });
        }
    }
}