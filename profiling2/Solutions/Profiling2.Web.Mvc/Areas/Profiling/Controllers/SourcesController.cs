using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Hangfire;
using log4net;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Aspose;
using Profiling2.Infrastructure.Security;
using Profiling2.Infrastructure.Util;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class SourcesController : BaseController
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(SourcesController));
        protected readonly ISourceTasks sourceTasks;
        protected readonly IUserTasks userTasks;
        protected readonly ISourceContentTasks sourceContentTasks;
        protected readonly ISourceAttachmentTasks sourceAttachmentTasks;
        protected readonly ISourcePermissionTasks sourcePermissionTasks;

        public SourcesController(ISourceTasks sourceTasks, 
            IUserTasks userTasks, 
            ISourceContentTasks sourceContentTasks, 
            ISourceAttachmentTasks sourceAttachmentTasks,
            ISourcePermissionTasks sourcePermissionTasks)
        {
            this.sourceTasks = sourceTasks;
            this.userTasks = userTasks;
            this.sourceContentTasks = sourceContentTasks;
            this.sourceAttachmentTasks = sourceAttachmentTasks;
            this.sourcePermissionTasks = sourcePermissionTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public ActionResult Search()
        {
            return View();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public JsonNetResult Get()
        {
            string term = Request.QueryString["term"];
            if (!string.IsNullOrEmpty(term))
            {
                term = term.Trim();
                Regex regex = new Regex("^[0-9]+$");
                if (regex.IsMatch(term))
                {
                    int sourceId;
                    if (int.TryParse(term, out sourceId))
                    {
                        Source s = this.sourceTasks.GetSource(sourceId);
                        if (((PrfPrincipal)User).CanAccess(s))
                        {
                            return JsonNet(new object[] 
                                {
                                    new
                                    {
                                        id = s.Id,
                                        text = s.SourceName
                                    }
                                });
                        }
                        else
                        {
                            return JsonNet(string.Empty);
                        }
                    }
                }
                else
                {
                    IList<Source> sources = null;
                    if (((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchAllSources))
                    {
                        sources = this.sourceTasks.GetSourcesByFilename(term);
                        if (!((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources))
                        {
                            sources = sources.Where(x => !x.IsRestricted).ToList();
                        }
                    }
                    else
                    {
                        sources = this.sourceTasks.GetSourcesByFilename(term)
                            // user can access sources that they uploaded
                            .Where(x => (x.HasUploadedBy() && string.Equals(x.GetUploadedBy().UserID, User.Identity.Name))
                                // or is public
                                || x.IsPublic)
                            .ToList();
                    }
                    object[] objects = (from s in sources
                                        select new { id = s.Id, text = s.SourceName }).ToArray();
                    return JsonNet(objects);
                }
            }
            return JsonNet(string.Empty);
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources, AdminPermission.CanViewAndSearchAllSources)]
        public JsonNetResult DataTables(SourceDataTablesParam p)
        {
            int iTotalRecords = 0;
            IList<SourceDataTableView> aaData = new List<SourceDataTableView>();

            // get or create new AdminSourceSearch for this search
            AdminUser user = this.userTasks.GetAdminUser(this.User.Identity.Name);
            AdminSourceSearch adminSourceSearch;

            if (p.searchAdminSourceSearchId.HasValue && p.searchAdminSourceSearchId.Value > 0)
            {
                adminSourceSearch = this.sourceAttachmentTasks.GetAdminSourceSearch(p.searchAdminSourceSearchId.Value);
            }
            else
            {
                adminSourceSearch = new AdminSourceSearch(p.searchText);
                adminSourceSearch.SearchedByAdminUser = user;
                adminSourceSearch = this.sourceAttachmentTasks.SaveOrUpdateAdminSourceSearch(adminSourceSearch);
            }

            if (p.searchId.HasValue)
            {
                // bypasses NHibernate OutOfMemoryError, but doesn't populate IsAttached
                SourceDTO dto = this.sourceTasks.GetSourceDTO(p.searchId.Value);

                if (dto != null)
                {
                    aaData.Add(new SourceDataTableView(dto));
                    iTotalRecords = 1;
                    adminSourceSearch.NumOfMatchingSources = 1;
                }
            }
            else
            {
                // get past searches in order to use their relevance ratings
                IList<int> adminSourceSearchIds = this.sourceAttachmentTasks.GetAdminSourceSearchIds(adminSourceSearch);

                IList<SourceSearchResultDTO> sources = null;
                if (((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchAllSources))
                {
                    iTotalRecords = this.sourceTasks.GetSearchTotal(
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources),
                        p.searchName,
                        p.searchExtension,
                        adminSourceSearch.FullTextSearchTerm,
                        p.GetStartDate(),
                        p.GetEndDate(),
                        p.authorSearchText);
                    sources = this.sourceTasks.GetPaginatedResults(
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources),
                        p.iDisplayStart, p.iDisplayLength,
                        p.searchName,
                        p.searchExtension,
                        adminSourceSearch.FullTextSearchTerm,
                        p.GetStartDate(),
                        p.GetEndDate(),
                        adminSourceSearchIds,
                        p.iSortingCols, p.iSortCol, p.sSortDir,
                        user.Id, p.personId, p.eventId,
                        p.authorSearchText);
                }
                else
                {
                    throw new NotImplementedException();
                }

                foreach (SourceSearchResultDTO x in sources)
                    aaData.Add(new SourceDataTableView(x));

                adminSourceSearch.NumOfMatchingSources = iTotalRecords;
                this.sourceAttachmentTasks.SaveOrUpdateAdminSourceSearch(adminSourceSearch);
            }

            return JsonNet(new DataTablesSourceData
            {
                iTotalRecords = iTotalRecords,
                iTotalDisplayRecords = iTotalRecords,
                sEcho = p.sEcho,
                aaData = aaData.ToArray(),
                adminSourceSearchId = adminSourceSearch.Id
            });
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public JsonNetResult Details(int id, int? adminSourceSearchId)
        {
            SourceDTO dto = this.sourceTasks.GetSourceDTO(id);
            if (dto != null)
            {
                if (((PrfPrincipal)User).CanAccess(dto, this.sourceTasks.GetSourceAuthors(id).ToArray(), this.sourceTasks.GetSourceOwners(id).ToArray()))
                {
                    // create AdminReviewedSource if not already exists - necessary for frontend to receive current search terms
                    if (adminSourceSearchId.HasValue)
                    {
                        AdminReviewedSource ars = this.sourceAttachmentTasks.GetOrCreateAdminReviewedSource(id, adminSourceSearchId.Value);
                    }

                    SourceInfoView view = new SourceInfoView(dto);

                    if (AsposeService.WordExtensions.Contains(dto.FileExtension))
                    {
                        try
                        {
                            view.DocumentProperties = this.sourceContentTasks.GetWordDocumentProperties(id);
                        }
                        catch (Exception e)
                        {
                            log.Error("Source ID=" + id + " encountered error retrieving word document properties...", e);
                        }
                    }

                    view.SetAdminReviewedSources(this.sourceAttachmentTasks.GetReviewsForSource(id));
                    view.SetAdminSourceImports(this.sourceAttachmentTasks.GetAdminImportsForSource(id));
                    view.SetPersonSources(this.sourceAttachmentTasks.GetPersonSources(id));
                    view.SetEventSources(this.sourceAttachmentTasks.GetEventSources(id));
                    view.SetUnitSources(this.sourceAttachmentTasks.GetUnitSources(id));
                    view.SetOperationSources(this.sourceAttachmentTasks.GetOperationSources(id));
                    view.SetParentSource(this.sourceAttachmentTasks.GetParentSourceOf(id));
                    view.SetChildSources(this.sourceAttachmentTasks.GetChildSourcesOf(id));
                    view.SetSourceAuthors(this.sourceTasks.GetSourceAuthors(id));
                    view.SetSourceOwners(this.sourceTasks.GetSourceOwners(id));

                    return JsonNet(view);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    Response.StatusDescription = "You don't have permission to view this source.";
                    return JsonNet(Response.StatusDescription);
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That source doesn't exist.";
                return JsonNet(Response.StatusDescription);
            }
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public ActionResult Download(int id, int? adminSourceSearchId)
        {
            SourceDTO dto = this.sourceTasks.GetSourceDTO(id);
            if (dto != null && dto.FileSize > 0)
            {
                if (((PrfPrincipal)User).CanAccess(dto, this.sourceTasks.GetSourceAuthors(id).ToArray(), this.sourceTasks.GetSourceOwners(id).ToArray()))
                {
                    if (!dto.IsReadOnly)
                    {
                        // record WasDownloaded for this search
                        if (adminSourceSearchId.HasValue)
                        {
                            AdminReviewedSource ars = this.sourceAttachmentTasks.GetOrCreateAdminReviewedSource(id, adminSourceSearchId.Value);
                            ars.WasDownloaded = true;
                        }

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
                    return new HttpUnauthorizedResult();
                }
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public void Preview(int id, int? adminSourceSearchId)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null && source.FileData != null)
            {
                if (((PrfPrincipal)User).CanAccess(source))
                {
                    // record WasPreviewed for this search
                    if (adminSourceSearchId.HasValue)
                    {
                        AdminReviewedSource ars = this.sourceAttachmentTasks.GetOrCreateAdminReviewedSource(id, adminSourceSearchId.Value);
                        ars.WasPreviewed = true;
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
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    Response.StatusDescription = "You don't have permission to view this source.";
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That source doesn't exist.";
            }
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public ActionResult PreviewImages(int id, string filename)
        {
            string contentType = MIMEAssistant.GetMIMEType(filename);
            string folderName = ConfigurationManager.AppSettings["PreviewTempFolder"];
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            return File(folderName + "\\" + filename, contentType);
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public void IsIrrelevant(int id, int? adminSourceSearchId)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null)
            {
                // unset IsRelevant for this source
                if (adminSourceSearchId.HasValue)
                {
                    AdminReviewedSource ars = this.sourceAttachmentTasks.GetOrCreateAdminReviewedSource(id, adminSourceSearchId.Value);
                    ars.IsRelevant = false;

                    Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    Response.StatusDescription = "No adminSourceSearchId was specified.";
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That source doesn't exist.";
            }
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public void IsRelevant(int id, int? adminSourceSearchId)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null)
            {
                // set IsRelevant for this source
                if (adminSourceSearchId.HasValue)
                {
                    AdminReviewedSource ars = this.sourceAttachmentTasks.GetOrCreateAdminReviewedSource(id, adminSourceSearchId.Value);
                    ars.IsRelevant = true;

                    Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    Response.StatusDescription = "No adminSourceSearchId was specified.";
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That source doesn't exist.";
            }
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public void IsUnknown(int id, int? adminSourceSearchId)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null)
            {
                // set IsRelevant for this source
                if (adminSourceSearchId.HasValue)
                {
                    AdminReviewedSource ars = this.sourceAttachmentTasks.GetOrCreateAdminReviewedSource(id, adminSourceSearchId.Value);
                    ars.IsRelevant = null;

                    Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    Response.StatusDescription = "No adminSourceSearchId was specified.";
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That source doesn't exist.";
            }
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeSources)]
        public void Archive(int id)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null)
            {
                source.Archive = true;
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That source doesn't exist.";
            }
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeSources)]
        public void Unarchive(int id)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null)
            {
                source.Archive = false;
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That source doesn't exist.";
            }
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeSources)]
        public void SetReadOnly(int id)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null)
            {
                source.IsReadOnly = true;
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That source doesn't exist.";
            }
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeSources)]
        public void UnsetReadOnly(int id)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null)
            {
                source.IsReadOnly = false;
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That source doesn't exist.";
            }
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeSources)]
        public void SetPublic(int id)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null)
            {
                source.IsPublic = true;
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That source doesn't exist.";
            }
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeSources)]
        public void UnsetPublic(int id)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null)
            {
                source.IsPublic = false;
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That source doesn't exist.";
            }
        }

        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public ActionResult Create()
        {
            SourceViewModel vm = new SourceViewModel();
            vm.PopulateDropDowns(this.sourceTasks.GetLanguages());
            return View(vm);
        }

        /// <summary>
        /// Deprecated - previously enabled users to upload new source while attaching to Person/Event.
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public JsonNetResult Create(SourceViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Source s = this.sourceTasks.GetSource(vm.Id);
                if (s != null)
                {
                    s.FullReference = vm.FullReference;
                    s.IsRestricted = vm.IsRestricted;
                    if (vm.FileLanguageId.HasValue)
                        s.FileLanguage = this.sourceTasks.GetLanguage(vm.FileLanguageId.Value);
                    s.Notes = vm.Notes;
                    this.sourceTasks.SaveSource(s);
                    return JsonNet(string.Empty);
                }
                else
                    ModelState.AddModelError("FileData", "Source doesn't exist or file didn't finish uploading.");
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public JsonNetResult Upload(HttpPostedFileBase FileData)
        {
            if (FileData != null && FileData.ContentLength > 0)
            {
                Source s = new Source();
                s.SourceName = FileData.FileName;
                s.SourcePath = FileData.FileName;
                s.SourceDate = DateTime.Now;
                s.FileExtension = FileUtil.GetExtension(FileData.FileName);
                
                using (Stream inputStream = FileData.InputStream)
                {
                    s.FileData = StreamUtil.StreamToBytes(inputStream);
                }

                s = this.sourceContentTasks.OcrScanAndSetSource(s);

                s = this.sourceTasks.SaveSource(s);
                return JsonNet(new
                {
                    Id = s.Id,
                    SourceName = s.SourceName
                });
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return JsonNet("Didn't receive any file.");
        }

        [PermissionAuthorize(AdminPermission.CanChangeSources)]
        public ActionResult Edit(int id)
        {
            Source s = this.sourceTasks.GetSource(id);
            if (s != null)
            {
                SourceViewModel vm = new SourceViewModel(s);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeSources)]
        public JsonNetResult Edit(SourceViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Source s = this.sourceTasks.GetSource(vm.Id);
                s.FullReference = vm.FullReference;
                s.Notes = vm.Notes;
                s.IsRestricted = vm.IsRestricted;
                s.IsReadOnly = vm.IsReadOnly;
                s.IsPublic = vm.IsPublic;
                s.Archive = vm.Archive;

                s.SourceAuthors.Clear();
                if (!string.IsNullOrEmpty(vm.SourceAuthorIds))
                {
                    string[] ids = vm.SourceAuthorIds.Split(',');
                    foreach (string id in ids)
                    {
                        int result;
                        if (int.TryParse(id, out result))
                        {
                            SourceAuthor a = this.sourcePermissionTasks.GetSourceAuthor(result);
                            if (a != null)
                                s.SourceAuthors.Add(a);
                        }
                    }
                }

                s.SourceOwningEntities.Clear();
                if (!string.IsNullOrEmpty(vm.SourceOwningEntityIds))
                {
                    string[] ids = vm.SourceOwningEntityIds.Split(',');
                    foreach (string id in ids)
                    {
                        int result;
                        if (int.TryParse(id, out result))
                        {
                            SourceOwningEntity e = this.sourcePermissionTasks.GetSourceOwningEntity(result);
                            if (e != null)
                                s.SourceOwningEntities.Add(e);
                        }
                    }
                }

                s = this.sourceTasks.SaveSource(s);

                // queue indexing
                BackgroundJob.Enqueue<ISourceTasks>(x =>
                    x.IndexSourceQueueable(s.Id,
                        s.HasUploadedBy() ? s.GetUploadedBy().UserID : string.Empty,
                        s.SourceAuthors.Select(y => y.Author).ToList(),
                        s.SourceOwningEntities.Select(y => y.Name).ToList(),
                        s.JhroCase != null ? s.JhroCase.CaseNumber : string.Empty,
                        this.sourceTasks.GetSourceDTO(s.Id).FileSize)
                    );

                return JsonNet(string.Empty);
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [PermissionAuthorize(AdminPermission.CanChangeSources)]
        public ActionResult Replace(int id)
        {
            Source s = this.sourceTasks.GetSource(id);
            if (s != null)
            {
                SourceFileDataViewModel vm = new SourceFileDataViewModel(s);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeSources)]
        public ActionResult Replace(SourceFileDataViewModel vm)
        {
            if (ModelState.IsValid && vm.FileData != null)
            {
                Source s = this.sourceTasks.GetSource(vm.Id);
                if (s != null)
                {
                    using (Stream inputStream = vm.FileData.InputStream)
                    {
                        s.FileData = StreamUtil.StreamToBytes(inputStream);
                    }

                    s = this.sourceTasks.SaveSource(s);

                    // queue indexing
                    BackgroundJob.Enqueue<ISourceTasks>(x =>
                        x.IndexSourceQueueable(s.Id,
                            s.HasUploadedBy() ? s.GetUploadedBy().UserID : string.Empty,
                            s.SourceAuthors.Select(y => y.Author).ToList(),
                            s.SourceOwningEntities.Select(y => y.Name).ToList(),
                            s.JhroCase != null ? s.JhroCase.CaseNumber : string.Empty,
                            this.sourceTasks.GetSourceDTO(s.Id).FileSize)
                        );
                }
                return RedirectToAction("Index", "Sources", new { area = "Profiling" });
            }
            return Replace(vm.Id);
        }
    }
}
