using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using log4net;
using Profiling2.Domain.Contracts.Services;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Util;
using Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers
{
    public class PreviewsController : BaseController
    {
        protected static ILog log = LogManager.GetLogger(typeof(PreviewsController));
        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceContentTasks sourceContentTasks;
        protected readonly ISourceStatisticTasks sourceStatisticTasks;
        protected readonly IAsposeService asposeService;

        public PreviewsController(ISourceTasks sourceTasks, 
            ISourceContentTasks sourceContentTasks, 
            ISourceStatisticTasks sourceStatisticTasks,
            IAsposeService asposeService)
        {
            this.sourceTasks = sourceTasks;
            this.sourceContentTasks = sourceContentTasks;
            this.sourceStatisticTasks = sourceStatisticTasks;
            this.asposeService = asposeService;
        }

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Index()
        {
            ViewBag.PasswordErrorCount = this.sourceTasks.CountSourceLogsWithPasswordErrors();
            ViewBag.SourceLogCount = this.sourceTasks.CountSourceLogs();
            ViewBag.SourceCount = this.sourceStatisticTasks.GetSourceCount();
            return View(this.sourceTasks.GetSourceLogsWithErrors());
        }

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Passwords()
        {
            return View(this.sourceTasks.GetSourceDTOsWithPasswordErrors());
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Process(int id)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            log.Info("Starting processing of " + id + " source previews...");

            IList<SourceDTO> dtos = this.sourceTasks.GetAllSourceDTOs(false, true).Take(id).ToList();
            log.Info("Found " + dtos.Count + " sources to process...");

            this.sourceContentTasks.FindPreviewProblems(dtos);

            sw.Stop();
            log.Info("Finished processing " + dtos.Count + " source previews in: " + sw.Elapsed);

            return RedirectToAction("Index");
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Rescan(int id)
        {
            SourceDTO dto = this.sourceTasks.GetSourceDTO(id);
            if (dto != null)
            {
                this.sourceContentTasks.FindPreviewProblems(new List<SourceDTO>() { dto });
            }
            return RedirectToAction("Index");
        }

        [PermissionAuthorize(AdminPermission.CanChangeSources)]
        public ActionResult RemovePassword(int id)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null && source.FileData != null)
            {
                if (!this.sourceContentTasks.CheckPreviewProblems(id))
                {
                    TempData["Result"] = "This source does not seem to have any problems being previewed (or it doesn't have any entry in PRF_SourceLog).";
                }
                return View(new RemovePasswordViewModel());
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeSources)]
        public ActionResult RemovePassword(RemovePasswordViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Source source = this.sourceTasks.GetSource(vm.Id);
                if (source != null && source.FileData != null)
                {
                    using (Stream destination = new MemoryStream())
                    {
                        if (this.asposeService.StripPassword(source, vm.Password, destination) != null)
                        {
                            source.FileData = StreamUtil.StreamToBytes(destination);
                            this.sourceTasks.SaveSource(source);

                            TempData["Result"] = "Source loaded successfully, and saved without password.";
                        }
                        else
                        {
                            TempData["Result"] = "Source did not load successfully.";
                        }
                        return View();
                    }
                }
                else
                {
                    return new HttpNotFoundResult();
                }
            }
            return View();
        }

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult BulkRemovePassword()
        {
            throw new NotImplementedException();
        }
    }
}