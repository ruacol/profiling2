using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Mvc.JQuery.Datatables;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Security;
using Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers
{
    [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
    public class AllController : BaseController
    {
        protected readonly ISourceTasks sourceTasks;
        protected readonly IUserTasks userTasks;
        protected readonly ILuceneTasks luceneTasks;
        protected readonly ISourcePermissionTasks sourcePermissionTasks;

        public AllController(ISourceTasks sourceTasks, IUserTasks userTasks, ILuceneTasks luceneTasks, ISourcePermissionTasks sourcePermissionTasks)
        {
            this.sourceTasks = sourceTasks;
            this.userTasks = userTasks;
            this.luceneTasks = luceneTasks;
            this.sourcePermissionTasks = sourcePermissionTasks;
        }

        public JsonNetResult Paths()
        {
            IList<SourceFolderDTO> folders = this.sourceTasks.GetFolderDTOs(null);
            folders.Add(new SourceFolderDTO("\\"));
            return JsonNet(folders.OrderBy(x => x.Folder));
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchRestrictedSources, AdminPermission.CanViewAndSearchAllSources)]
        public ActionResult Cases()
        {
            return View();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchRestrictedSources, AdminPermission.CanViewAndSearchAllSources)]
        public JsonNetResult DataTablesCases(DataTablesParam p)
        {
            if (p != null)
            {
                // calculate total results to request from lucene search
                int numResults = (p.iDisplayStart >= 0 && p.iDisplayLength > 0) ? (p.iDisplayStart + 1) * p.iDisplayLength : 10;

                // figure out sort column - tied to frontend table columns.  assuming one column for now.
                string sortField = "FileDateTimeStamp";
                if (p.iSortCol != null)
                {
                    switch (p.iSortCol.First())
                    {
                        case 0:
                            sortField = "JhroCaseNumber"; break;
                        case 1:
                            sortField = "Name"; break;
                        case 2:
                            sortField = "FileDateTimeStamp"; break;
                        case 3:
                            sortField = "FileSize"; break;
                    }
                }

                // figure out sort direction
                bool descending = true;
                if (p.sSortDir != null && string.Equals(p.sSortDir.First(), "asc"))
                    descending = false;

                // run search
                IList<LuceneSearchResult> results = this.luceneTasks.GetAllSourcesWithCaseNumbers(p.sSearch, numResults,
                    ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources),
                    sortField,
                    descending
                );

                int iTotalRecords = 0;
                if (results != null && results.Count > 0)
                    iTotalRecords = results.First().TotalHits;

                object[] aaData = results
                    .Select(x => new SourcePathDataTableLuceneView(x))
                    .Skip(p.iDisplayStart)
                    .Take(p.iDisplayLength)
                    .ToArray<SourcePathDataTableLuceneView>();

                return JsonNet(new DataTablesData
                {
                    iTotalRecords = iTotalRecords,
                    iTotalDisplayRecords = iTotalRecords,
                    sEcho = p.sEcho,
                    aaData = aaData.ToArray()
                });
            }
            return JsonNet(null);
        }

        public ActionResult Browse()
        {
            if (((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchSources)
                && ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchAllSources))
            {
                ViewBag.SourceOwningEntities = this.sourcePermissionTasks.GetAllSourceOwningEntities();
            }

            return View();
        }
    }
}