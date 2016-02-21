using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web.Mvc;
using Mvc.JQuery.Datatables;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Security;
using Profiling2.Infrastructure.Util;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using QueryInterceptor;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers
{
    public class FeedingController : BaseController
    {
        protected readonly IFeedingSourceTasks feedingSourceTasks;
        protected readonly IUserTasks userTasks;
        protected readonly IEmailTasks emailTasks;
        protected readonly ISourcePermissionTasks sourcePermissionTasks;
        protected readonly ISourceTasks sourceTasks;

        public FeedingController(IFeedingSourceTasks feedingSourceTasks, 
            IUserTasks userTasks, 
            IEmailTasks emailTasks, 
            ISourcePermissionTasks sourcePermissionTasks,
            ISourceTasks sourceTasks)
        {
            this.feedingSourceTasks = feedingSourceTasks;
            this.userTasks = userTasks;
            this.emailTasks = emailTasks;
            this.sourcePermissionTasks = sourcePermissionTasks;
            this.sourceTasks = sourceTasks;
        }

        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionAuthorize(AdminPermission.CanUploadSources, AdminPermission.CanViewAndSearchSources)]
        public DataTablesResult<FeedingSourceDTO> DataTables(DataTablesParam p)
        {
            IQueryable<FeedingSourceDTO> q = this.feedingSourceTasks.GetFeedingSourceDTOs(
                    ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchAllSources),
                    ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources),
                    User.Identity.Name
                    )
                .AsQueryable()
                .InterceptWith(new SetComparerExpressionVisitor(StringComparison.CurrentCultureIgnoreCase));

            return DataTablesResult.Create(q, p, x => x.ToJSON());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public ActionResult Details(int id)
        {
            FeedingSource fs = this.feedingSourceTasks.GetFeedingSource(id);
            if (fs != null)
            {
                if (!((PrfPrincipal)User).CanAccess(fs))
                    return new HttpUnauthorizedResult();
                return View(fs);
            }
            return new HttpNotFoundResult();
        }

        protected FeedingSource ProcessUpload(MultipleUploadViewModel vm, int index)
        {
            // we use this flag instead of ModelState.IsValid because we need to reset this value for every file; ModelState.IsValid is a single flag for all uploaded files.
            bool hasError = false;

            using (Stream fileStream = vm.FileData[index].InputStream)
            {
                // Progressively check for duplicates, most reliable check first.  Only return one duplicate validation error if any.
                // check Source hash duplicate
                string hash = BitConverter.ToString(MD5.Create().ComputeHash(fileStream)).Replace("-", "");
                IList<SourceDTO> dtos = this.sourceTasks.GetSources(hash);
                if (dtos != null && dtos.Count > 0)
                {
                    hasError = true;
                    ModelState.AddModelError(
                        "FileData",
                        "An identical source (<a href='" + Url.Action("Index", "Sources", new { area = "Profiling" }) + "#info/" + dtos.First().SourceID + "' target='_blank'>"
                            + dtos.First().SourceName + "</a>) exists already with Source ID of " + dtos.First().SourceID + "."
                    );
                }
                else
                {
                    // check Source name duplicate
                    Source s = this.sourceTasks.GetSource(vm.FileData[index].FileName);
                    if (s != null)
                    {
                        hasError = true;
                        ModelState.AddModelError(
                            "FileData",
                            "<a href='" + Url.Action("Index", "Sources", new { area = "Profiling" }) + "#info/" + s.Id + "' target='_blank'>"
                                + s.SourceName + "</a> exists already with Source ID of " + s.Id + ".  If you're sure you have a different file, rename it before uploading."
                        );
                    }
                    else
                    {
                        // check FeedingSource name duplicate
                        FeedingSource fs = this.feedingSourceTasks.GetFeedingSource(vm.FileData[index].FileName);
                        if (fs != null)
                        {
                            hasError = true;
                            ModelState.AddModelError(
                                "FileData", 
                                "<a href='" + Url.Action("Details", "Feeding", new { area = "Sources", id = fs.Id }) + "' target='_blank'>"
                                    + vm.FileData[index].FileName + "</a> was already uploaded; delete it or change your file name."
                            );
                        }
                    }
                }

                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (user == null)
                {
                    hasError = true;
                    ModelState.AddModelError("UploadedBy", "Logged-in user doesn't appear to be exist.");
                }

                if (vm.IsReadOnly && string.IsNullOrEmpty(vm.UploadNotes))
                {
                    hasError = true;
                    ModelState.AddModelError("UploadNotes", "Read-only sources should have a justification in the notes.");
                }

                if (!hasError)
                {
                    FeedingSource fs = new FeedingSource();
                    fs.Name = Path.GetFileName(vm.FileData[index].FileName);
                    fs.Restricted = vm.Restricted;
                    fs.IsReadOnly = vm.IsReadOnly;
                    fs.IsPublic = vm.IsPublic;
                    fs.FileModifiedDateTime = vm.FileModifiedDateTime != null && vm.FileModifiedDateTime[index] != null ? vm.FileModifiedDateTime[index] : DateTime.Now;
                    fs.UploadedBy = user;
                    fs.UploadDate = DateTime.Now;
                    fs.UploadNotes = vm.UploadNotes;

                    if (!string.IsNullOrEmpty(vm.AuthorIds))
                    {
                        string[] ids = vm.AuthorIds.Split(',');
                        foreach (string id in ids)
                        {
                            int result;
                            if (int.TryParse(id, out result))
                            {
                                SourceAuthor a = this.sourcePermissionTasks.GetSourceAuthor(result);
                                if (a != null)
                                    fs.SourceAuthors.Add(a);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(vm.OwnerIds))
                    {
                        string[] ids = vm.OwnerIds.Split(',');
                        foreach (string id in ids)
                        {
                            int result;
                            if (int.TryParse(id, out result))
                            {
                                SourceOwningEntity e = this.sourcePermissionTasks.GetSourceOwningEntity(result);
                                if (e != null)
                                    fs.SourceOwningEntities.Add(e);
                            }
                        }
                    }

                    fileStream.Position = 0;  // we read the stream earlier when computing hash
                    MemoryStream memoryStream = fileStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        fileStream.CopyTo(memoryStream);
                    }
                    fs.FileData = memoryStream.ToArray();

                    return fs;
                }

                return null;
            }
        }

        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public ActionResult Upload()
        {
            return View(new MultipleUploadViewModel());
        }

        /// <summary>
        /// NOTE File upload with IE8 causes this method to throw NullReferenceException.  IE8 not supported anyway.
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public ActionResult Upload(MultipleUploadViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.FileData != null && vm.FileData.Length > 0)
                {
                    IList<FeedingSource> sources = new List<FeedingSource>();

                    // construct FeedingSources and gather validation errors if any
                    for (int i = 0; i < vm.FileData.Length; i++)
                    {
                        if (vm.FileData[i] != null && vm.FileData[i].ContentLength > 0)
                        {
                            FeedingSource fs = this.ProcessUpload(vm, i);

                            if (fs != null)
                                sources.Add(fs);
                        }
                    }

                    // save those without errors
                    if (sources.Any())
                    {
                        foreach (FeedingSource fs in sources)
                            this.feedingSourceTasks.SaveFeedingSource(fs);
                        TempData["SuccessfullyUploaded"] = sources;
                        this.emailTasks.SendFeedingSourcesUploadedEmail(sources);
                    }

                    if (ModelState.IsValid)
                        return RedirectToAction("Index");
                }
            }
            return Upload();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public void Download(int id)
        {
            FeedingSource fs = this.feedingSourceTasks.GetFeedingSource(id);
            if (fs != null && fs.FileData != null)
            {

                if (((PrfPrincipal)User).CanAccess(fs))
                {
                    if ((fs.ApprovedBy != null || fs.RejectedBy != null) && fs.IsReadOnly)
                    {
                        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        Response.StatusDescription = "Source has been marked read only and has already been approved or rejected, so is not available for download.";
                    }
                    else
                    {
                        string contentType = MIMEAssistant.GetMIMEType(fs.Name);
                        Response.ContentType = contentType;
                        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fs.Name + "\"");
                        Response.OutputStream.Write(fs.FileData, 0, fs.FileData.Length);
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    Response.StatusDescription = "You are not authorized to download this source.";
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That feeding source doesn't exist.";
            }
        }

        [PermissionAuthorize(AdminPermission.CanApproveAndRejectSources)]
        public ActionResult Approve(int id)
        {
            FeedingSource fs = this.feedingSourceTasks.GetFeedingSource(id);
            if (fs != null)
            {
                if (!((PrfPrincipal)User).CanAccess(fs))
                    return new HttpUnauthorizedResult();
                return View(new FeedingSourceViewModel(fs));
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanApproveAndRejectSources)]
        public ActionResult Approve(FeedingSourceViewModel vm)
        {
            if (vm != null && vm.Id > 0)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (user != null)
                {
                    FeedingSource fs = this.feedingSourceTasks.GetFeedingSource(vm.Id);
                    if (fs != null)
                    {
                        if (!((PrfPrincipal)User).CanAccess(fs))
                            return new HttpUnauthorizedResult();

                        if (fs.UploadedBy != user)
                        {
                            fs.ApprovedBy = user;
                            fs.ApprovedDate = DateTime.Now;
                            fs.Restricted = vm.Restricted;
                            fs.IsReadOnly = vm.IsReadOnly;
                            fs.IsPublic = vm.IsPublic;
                            fs.UploadNotes = vm.UploadNotes;

                            if (!string.IsNullOrEmpty(vm.AuthorIds))
                            {
                                fs.SourceAuthors.Clear();
                                string[] ids = vm.AuthorIds.Split(',');
                                foreach (string id in ids)
                                {
                                    int result;
                                    if (int.TryParse(id, out result))
                                    {
                                        SourceAuthor a = this.sourcePermissionTasks.GetSourceAuthor(result);
                                        if (a != null)
                                            fs.SourceAuthors.Add(a);
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(vm.OwnerIds))
                            {
                                fs.SourceOwningEntities.Clear();
                                string[] ids = vm.OwnerIds.Split(',');
                                foreach (string id in ids)
                                {
                                    int result;
                                    if (int.TryParse(id, out result))
                                    {
                                        SourceOwningEntity e = this.sourcePermissionTasks.GetSourceOwningEntity(result);
                                        if (e != null)
                                            fs.SourceOwningEntities.Add(e);
                                    }
                                }
                            }

                            fs = this.feedingSourceTasks.SaveFeedingSource(fs);
                            Source source = this.feedingSourceTasks.FeedSource(fs.Id);
                            this.emailTasks.SendFeedingSourceApprovedEmail(fs);

                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("ApprovedBy", "User cannot approve a source they uploaded themselves.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Id", "Feeding source doesn't exist.");
                    }
                }

                else
                {
                    ModelState.AddModelError("ApprovedBy", "Logged-in user doesn't exist.");
                }
            }
            else
            {
                ModelState.AddModelError("Id", "No Id was sent.");
            }
            return Approve(vm.Id);
        }

        [PermissionAuthorize(AdminPermission.CanApproveAndRejectSources)]
        public ActionResult Reject(int id)
        {
            FeedingSource fs = this.feedingSourceTasks.GetFeedingSource(id);
            if (fs != null)
            {
                if (!((PrfPrincipal)User).CanAccess(fs))
                    return new HttpUnauthorizedResult();
                return View(new FeedingSourceViewModel(fs));
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanApproveAndRejectSources)]
        public ActionResult Reject(FeedingSourceViewModel vm)
        {
            if (vm != null && vm.Id > 0)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (user != null)
                {
                    FeedingSource fs = this.feedingSourceTasks.GetFeedingSource(vm.Id);
                    if (fs != null)
                    {
                        if (!((PrfPrincipal)User).CanAccess(fs))
                            return new HttpUnauthorizedResult();

                        if (fs.UploadedBy != user)
                        {
                            fs.RejectedBy = user;
                            fs.RejectedDate = DateTime.Now;
                            fs.RejectedReason = vm.RejectedReason;
                            this.feedingSourceTasks.SaveFeedingSource(fs);

                            this.emailTasks.SendFeedingSourceRejectedEmail(fs);

                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("RejectedBy", "User should delete rather than reject a source they uploaded themselves.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Id", "Feeding source doesn't exist.");
                    }
                }

                else
                {
                    ModelState.AddModelError("RejectedBy", "Logged-in user doesn't exist.");
                }
            }
            else
            {
                ModelState.AddModelError("Id", "No Id was sent.");
            }
            return Approve(vm.Id);
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public ActionResult Delete(int id)
        {
            FeedingSource fs = this.feedingSourceTasks.GetFeedingSource(id);
            if (fs != null)
            {
                if (!((PrfPrincipal)User).CanAccess(fs))
                    return new HttpUnauthorizedResult();

                this.feedingSourceTasks.DeleteFeedingSource(fs);
            }
            return RedirectToAction("Index");
        }

        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public ActionResult Rename(int id)
        {
            FeedingSource fs = this.feedingSourceTasks.GetFeedingSource(id);
            if (fs != null)
            {
                if (!((PrfPrincipal)User).CanAccess(fs))
                    return new HttpUnauthorizedResult();

                return View(new FeedingSourceViewModel(fs));
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public ActionResult Rename(FeedingSourceViewModel vm)
        {
            if (ModelState.IsValid)
            {
                FeedingSource fs = this.feedingSourceTasks.GetFeedingSource(vm.Id);
                if (fs != null)
                {
                    if (!((PrfPrincipal)User).CanAccess(fs))
                        return new HttpUnauthorizedResult();

                    if (fs.ApprovedBy == null && fs.RejectedBy == null)
                    {
                        FeedingSource existing = this.feedingSourceTasks.GetFeedingSource(vm.Name);
                        if (existing == null || (existing != null && existing.Id == fs.Id))
                        {
                            Source s = this.sourceTasks.GetSource(vm.Name);
                            if (s == null)
                            {
                                fs.Name = vm.Name;
                                fs = this.feedingSourceTasks.SaveFeedingSource(fs);

                                return RedirectToAction("Index");
                            }
                            else
                                ModelState.AddModelError("Name", "<a href='" + Url.Action("Index", "Sources", new { area = "Profiling" }) + "#info/" + s.Id + "' target='_blank'>" + s.SourceName + "</a> exists already with Source ID of " + s.Id + ".  If you're sure you have a different file, rename it before uploading.");
                        }
                        else
                            ModelState.AddModelError("Name", "A file has already been uploaded with this name.");
                    }
                    else
                        ModelState.AddModelError("Name", "File is already approved and imported into database, or is rejected.");
                }
            }

            return Rename(vm.Id);
        }

        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public ActionResult Report()
        {
            DateViewModel vm = new DateViewModel(DateTime.Now.Subtract(TimeSpan.FromDays(30)), DateTime.Now);
            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public ActionResult Report(DateViewModel vm)
        {
            ViewBag.Stats = this.feedingSourceTasks.GetFeedingSourceDTOs(null, vm.StartDateAsDate, 
                vm.EndDateAsDate,
                ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources));

            return View(vm);
        }
    }
}