using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using log4net;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Web.Mvc.Areas.System.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers
{
    [PermissionAuthorize(AdminPermission.CanAdministrate)]
    public class SourceAdminController : BaseController
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(SourceAdminController));
        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceManagementTasks sourceManagementTasks;
        protected readonly ISourceStatisticTasks sourceStatisticTasks;

        public SourceAdminController(ISourceTasks sourceTasks, 
            ISourceManagementTasks sourceManagementTasks, 
            ISourceStatisticTasks sourceStatisticTasks)
        {
            this.sourceTasks = sourceTasks;
            this.sourceManagementTasks = sourceManagementTasks;
            this.sourceStatisticTasks = sourceStatisticTasks;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DuplicatesByHash()
        {
            IList<object[]> duplicates = this.sourceManagementTasks.DuplicatesByHash(0);
            return View(duplicates);
        }

        public ActionResult DuplicatesByName()
        {
            IList<object[]> duplicates = this.sourceManagementTasks.DuplicatesByName();
            return View(duplicates);
        }

        public ActionResult DuplicatesByIgnored()
        {
            ViewBag.IgnoredFileExtensions = Source.IGNORED_FILE_EXTENSIONS;
            IList<SourceDTO> duplicates = this.sourceManagementTasks.DuplicatesByIgnored();
            return View(duplicates);
        }

        public ActionResult DeleteIgnored()
        {
            IList<SourceDTO> duplicates = this.sourceManagementTasks.DuplicatesByIgnored();
            foreach (SourceDTO dto in duplicates)
                this.sourceTasks.DeleteSource(dto.SourceID);
            return RedirectToAction("DuplicatesByIgnored");
        }

        public ActionResult Delete(int id)
        {
            this.sourceTasks.DeleteSource(id);
            return RedirectToAction("DuplicatesByIgnored");
        }

        public ActionResult DuplicatesByNameOf()
        {
            string sourceName = Request.Params["SourceName"];
            if (!string.IsNullOrEmpty(sourceName))
                return View(this.sourceManagementTasks.DuplicatesByNameOf(sourceName));
            return new HttpNotFoundResult();
        }

        public ActionResult MergeDuplicatesByNameOf()
        {
            string sourceName = Request.Params["SourceName"];
            if (!string.IsNullOrEmpty(sourceName))
            {
                this.sourceManagementTasks.MergeDuplicatesByNameOf(sourceName);
                return RedirectToAction("DuplicatesByNameOf", new { SourceName = sourceName });
            }
            return new HttpNotFoundResult();
        }

        public ActionResult MergeAllDuplicatesByName()
        {
            IList<object[]> duplicates = this.sourceManagementTasks.DuplicatesByName();
            log.Info(duplicates.Count() + " file names with duplicates before merge...");
            foreach (object[] row in duplicates)
            {
                if (row[0] != null)
                {
                    string fileName = (string)row[0];
                    if (!string.IsNullOrEmpty(fileName))
                        this.sourceManagementTasks.MergeDuplicatesByNameOf(fileName);
                }
            }
            log.Info(this.sourceManagementTasks.DuplicatesByName().Count() + " file names with duplicates after merge.");
            return RedirectToAction("DuplicatesByName");
        }

        public ActionResult DuplicatesByHashOf()
        {
            string hash = Request.Params["Hash"];
            if (!string.IsNullOrEmpty(hash))
                return View(this.sourceTasks.GetSources(hash));
            return new HttpNotFoundResult();
        }

        [Transaction]
        public ActionResult CleanDuplicatesByHashOf()
        {
            string hash = Request.Params["Hash"];
            if (!string.IsNullOrEmpty(hash))
            {
                this.sourceManagementTasks.CleanDuplicatesByHashOf(hash);
                return RedirectToAction("DuplicatesByHashOf", new { Hash = hash });
            }
            return new HttpNotFoundResult();
        }

        /// <summary>
        /// Warning: long running transaction.
        /// </summary>
        /// <returns></returns>
        [Transaction]
        public ActionResult CleanDuplicatesByHash()
        {
            this.sourceManagementTasks.CleanHashDuplicates();

            return RedirectToAction("DuplicatesByHash");
        }

        public ActionResult Folders()
        {
            return View(new SearchViewModel());
        }

        [HttpPost]
        public ActionResult Folders(SearchViewModel vm)
        {
            if (ModelState.IsValid)
            {
                string[] paths = vm.Term.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                ViewData["pathCounts"] = this.sourceStatisticTasks.GetSourceFolderCounts(paths, null);

                return View(vm);
            }
            return Folders();
        }

        [Transaction]
        public ActionResult ArchiveFolder(SearchViewModel vm)
        {
            if (ModelState.IsValid)
            {
                this.sourceTasks.ArchiveSourcePath(vm.Term);
            }
            return RedirectToAction("Folders");
        }
    }
}