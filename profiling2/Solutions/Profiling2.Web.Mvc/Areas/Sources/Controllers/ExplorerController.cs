using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using log4net;
using Mvc.JQuery.Datatables;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Scr;
using Profiling2.Infrastructure.Security;
using Profiling2.Infrastructure.Util;
using Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers
{
    [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
    public class ExplorerController : BaseController
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(ExplorerController));

        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceContentTasks sourceContentTasks;
        protected readonly IUserTasks userTasks;
        protected readonly ILuceneTasks luceneTasks;
        protected readonly ISourcePermissionTasks sourcePermissionTasks;

        public ExplorerController(ISourceTasks sourceTasks, 
            ISourceContentTasks sourceContentTasks, 
            IUserTasks userTasks, 
            ILuceneTasks luceneTasks, 
            ISourcePermissionTasks sourcePermissionTasks)
        {
            this.sourceTasks = sourceTasks;
            this.sourceContentTasks = sourceContentTasks;
            this.userTasks = userTasks;
            this.luceneTasks = luceneTasks;
            this.sourcePermissionTasks = sourcePermissionTasks;
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonNetResult Paths(string code)
        {
            IList<SourceFolderDTO> list = null;
            string prefix = this.sourcePermissionTasks.GetSourceOwningEntityPrefixPath(code);
            if (!string.IsNullOrEmpty(prefix))
            {
                list = this.sourceTasks.GetFolderDTOs(prefix)
                    .Where(x => string.Equals(x.OwnerCode, code))
                    .ToList();
            }
            return JsonNet(list);
        }

        public ActionResult Browse(string code)
        {
            AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
            ScreeningEntity screeningEntity = user.GetScreeningEntity();

            if (!string.IsNullOrEmpty(code))
            {
                ViewBag.Code = code;
                return View();
            }
            return new HttpNotFoundResult();
        }

        public JsonNetResult DataTables(DataTablesParam p)
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
                            sortField = "Name"; break;
                        case 1:
                            sortField = "FileDateTimeStamp"; break;
                        case 2:
                            sortField = "FileSize"; break;
                    }
                }

                // figure out sort direction
                bool descending = true;
                if (p.sSortDir != null && string.Equals(p.sSortDir.First(), "asc"))
                    descending = false;

                // get user affiliations
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);

                // run search
                bool showAllSubdirectories = Convert.ToBoolean(Request.QueryString["showAllSubDirectories"]);
                IList<LuceneSearchResult> results;
                if (showAllSubdirectories)
                {
                    results = this.luceneTasks.SourcePathPrefixSearch(p.sSearch,
                        numResults,
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchAllSources),
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources),
                        User.Identity.Name,
                        user.Affiliations.Select(x => x.Name).ToList(),
                        sortField,
                        descending
                    );
                }
                else
                {
                    results = this.luceneTasks.SourcePathExactSearch(p.sSearch,
                        numResults,
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchAllSources),
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources),
                        User.Identity.Name,
                        user.Affiliations.Select(x => x.Name).ToList(),
                        sortField,
                        descending
                    );
                }

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

        /// <summary>
        /// Just preview - doesn't record the fact like SourcesController.Preview() does. Also accessible by conditionality participants.
        /// </summary>
        /// <param name="id"></param>
        public void Preview(int id)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source == null || source.FileData == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That source doesn't exist.";
                return;
            }

            if (!((PrfPrincipal)User).CanAccess(source))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Response.StatusDescription = "You don't have permission to view this source.";
                return;
            }

            string contentType = MIMEAssistant.GetMIMEType(source.SourceName);
            if (!string.IsNullOrEmpty(contentType) && source.HasOcrText())
            {
                Response.ContentType = "text/plain";
                Response.OutputStream.Write(source.FileData, 0, source.FileData.Length);
            }
            else if (!string.IsNullOrEmpty(contentType) && (contentType.StartsWith("image") || contentType.StartsWith("text/plain")))
            {
                Response.ContentType = contentType;
                Response.OutputStream.Write(source.FileData, 0, source.FileData.Length);
            }
            else if (!string.IsNullOrEmpty(contentType) && contentType.StartsWith("text/html"))
            {
                Response.ContentType = contentType;
                Response.OutputStream.Write(source.FileData, 0, source.FileData.Length);
            }
            else
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] previewBytes = this.sourceContentTasks.GetHtmlPreview(source, ms);
                        if (previewBytes != null)
                        {
                            Response.ContentType = "text/html";
                            Response.OutputStream.Write(previewBytes, 0, previewBytes.Length);
                        }
                        else
                        {
                            Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                            byte[] error = Encoding.UTF8.GetBytes("Preview for this file not supported.");
                            Response.OutputStream.Write(error, 0, error.Length);
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("Problem generating HTML preview.", e);
                    byte[] error = Encoding.UTF8.GetBytes("Problem generating HTML preview: " + e.Message);
                    Response.ContentType = "text/html";
                    Response.OutputStream.Write(error, 0, error.Length);
                }
            }
        }

        /// <summary>
        /// Just download - doesn't record the fact like SourcesController.Download() does.
        /// </summary>
        /// <param name="id"></param>
        public ActionResult Download(int id)
        {
            SourceDTO dto = this.sourceTasks.GetSourceDTO(id);
            if (dto != null && dto.FileSize > 0)
            {
                if (((PrfPrincipal)User).CanAccess(dto, this.sourceTasks.GetSourceAuthors(id).ToArray(), this.sourceTasks.GetSourceOwners(id).ToArray()) && !dto.IsReadOnly)
                {
                    string contentType = dto.GetDTOFileExtension();
                    Stream stream = this.sourceTasks.GetSourceData(id, dto.HasOcrText);
                    return new FileStreamResult(stream, contentType) { FileDownloadName = dto.GetFileDownloadName() };
                }
                else
                {
                    return new HttpUnauthorizedResult();
                }
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        // source preview references
        public ActionResult Images(string filename)
        {
            string contentType = MIMEAssistant.GetMIMEType(filename);
            string folderName = ConfigurationManager.AppSettings["PreviewTempFolder"];
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            return File(folderName + "\\" + filename, contentType);
        }

        public JsonNetResult MoreLikeThis(int id)
        {
            // TODO check for restricted sources
            IList<LuceneSearchResult> results = this.luceneTasks.GetSourcesLikeThis(id, 10);
            if (results != null && results.Any())
                return JsonNet((from result in results select new SourceResultDataTableLuceneView(result)));

            Response.StatusCode = (int)HttpStatusCode.NoContent;
            return JsonNet("No sources like this.");
        }

        public JsonNetResult SearchGetFacets()
        {
            // parse search term
            string term = Request.Params["term"];
            if (!string.IsNullOrEmpty(term))
            {
                // parse date filter inputs
                DateTime s, e;
                DateTime? start = null, end = null;
                if (DateTime.TryParse(Request.Params["start-date"], out s))
                    start = s;
                if (DateTime.TryParse(Request.Params["end-date"], out e))
                    end = e;

                // get path prefix
                string prefix = this.sourcePermissionTasks.GetSourceOwningEntityPrefixPath(Request.Params["code"]);

                // need user's affiliations
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                IList<string> affiliations = user.Affiliations.Select(x => x.Name).ToList();

                // run search
                IDictionary<IDictionary<string, string>, long> facets = this.luceneTasks.SourceSearchFacets(term, prefix, start, end, 50,
                    ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchAllSources),
                    ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources),
                    User.Identity.Name, affiliations);

                return JsonNet(facets.Select(x => new
                    {
                        Facets = x.Key.Select(y => new
                            {
                                Name = y.Key,
                                Value = y.Value
                            }),
                        Count = x.Value
                    }));
            }
            return JsonNet(null);
        }

        public JsonNetResult DataTablesSearch(DataTablesParam p)
        {
            if (p != null)
            {
                // calculate total results to request from lucene search
                int numResults = (p.iDisplayStart >= 0 && p.iDisplayLength > 0) ? (p.iDisplayStart + 1) * p.iDisplayLength : 10;

                // figure out sort column - tied to frontend table columns.  assuming one column for now.
                string sortField = null;
                if (p.iSortCol != null)
                {
                    switch (p.iSortCol.First())
                    {
                        case 0:
                            sortField = null; break;
                        case 1:
                            sortField = "SourceName"; break;
                        case 2:
                            sortField = "FileDateTimeStamp"; break;
                    }
                }

                // figure out sort direction
                bool descending = true;
                if (p.sSortDir != null && string.Equals(p.sSortDir.First(), "asc"))
                    descending = false;

                // parse date filter inputs
                DateTime s, e;
                DateTime? start = null, end = null;
                if (DateTime.TryParse(Request.Params["start-date"], out s))
                    start = s;
                if (DateTime.TryParse(Request.Params["end-date"], out e))
                    end = e;

                // get path prefix
                string prefix = this.sourcePermissionTasks.GetSourceOwningEntityPrefixPath(Request.Params["code"]);

                // need user's affiliations
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                IList<string> affiliations = user.Affiliations.Select(x => x.Name).ToList();

                // run search
                IList<LuceneSearchResult> results = null;
                if (start.HasValue || end.HasValue)
                {
                    results = this.luceneTasks.SourceSearch(p.sSearch, prefix, start, end, numResults,
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchAllSources),
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources),
                        User.Identity.Name, affiliations, sortField, descending);
                }
                else
                {
                    results = this.luceneTasks.SourceSearch(p.sSearch, prefix, numResults,
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchAllSources),
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources),
                        User.Identity.Name, affiliations, sortField, descending);
                }

                int iTotalRecords = 0;
                if (results != null && results.Count > 0)
                    iTotalRecords = results.First().TotalHits;

                // NOTE HasOcrText not included here (but those and other attrs available via extra call to Sources/Details/sourceId from frontend...)
                object[] aaData = results
                    .Select(x => new SourceResultDataTableLuceneView(x))
                    .Skip(p.iDisplayStart)
                    .Take(p.iDisplayLength)
                    .ToArray<SourceResultDataTableLuceneView>();

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
    }
}