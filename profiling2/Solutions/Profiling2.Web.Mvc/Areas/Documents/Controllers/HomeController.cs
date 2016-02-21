using System;
using System.IO;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Documentation;
using Profiling2.Infrastructure.Util;
using Profiling2.Web.Mvc.Areas.Documents.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Documents.Controllers
{
    public class HomeController : BaseController
    {
        protected readonly IDocumentationFileTasks docTasks;
        protected readonly IUserTasks userTasks;

        public HomeController(IDocumentationFileTasks docTasks,
            IUserTasks userTasks)
        {
            this.docTasks = docTasks;
            this.userTasks = userTasks;
        }

        public ActionResult Index()
        {
            return View(this.docTasks.GetDocumentationFileTags());
        }

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Manage()
        {
            ViewBag.Files = this.docTasks.GetDocumentationFiles();
            ViewBag.Tags = this.docTasks.GetDocumentationFileTags();
            return View();
        }

        // tag related actions

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult CreateTag()
        {
            DocumentationFileTagViewModel vm = new DocumentationFileTagViewModel();
            vm.PopulateDropDowns(this.userTasks.GetAllAdminPermissions());
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult CreateTag(DocumentationFileTagViewModel vm)
        {
            if (ModelState.IsValid)
            {
                DocumentationFileTag tag = new DocumentationFileTag();
                tag.Name = vm.Name;
                tag.AdminPermission = this.userTasks.GetAdminPermission(vm.AdminPermissionId);
                tag = this.docTasks.SaveDocumentationFileTag(tag);
                return RedirectToAction("Manage");
            }
            return CreateTag();
        }

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult EditTag(int id)
        {
            DocumentationFileTag tag = this.docTasks.GetDocumentationFileTag(id);
            if (tag != null)
            {
                DocumentationFileTagViewModel vm = new DocumentationFileTagViewModel(tag);
                vm.PopulateDropDowns(this.userTasks.GetAllAdminPermissions());
                return View(vm);
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult EditTag(DocumentationFileTagViewModel vm)
        {
            if (ModelState.IsValid)
            {
                DocumentationFileTag tag = this.docTasks.GetDocumentationFileTag(vm.Id);
                if (tag != null)
                {
                    tag.Name = vm.Name;
                    tag.AdminPermission = this.userTasks.GetAdminPermission(vm.AdminPermissionId);
                    tag = this.docTasks.SaveDocumentationFileTag(tag);
                    return RedirectToAction("Manage");
                }
                else
                {
                    return new HttpNotFoundResult();
                }
            }
            return EditTag(vm.Id);
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult DeleteTag(int id)
        {
            DocumentationFileTag tag = this.docTasks.GetDocumentationFileTag(id);
            if (tag != null)
            {
                this.docTasks.DeleteDocumentationFileTag(tag);
                return RedirectToAction("Manage");
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        // file related actions

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult CreateFile()
        {
            DocumentationFileViewModel vm = new DocumentationFileViewModel();
            vm.PopulateDropDowns(this.docTasks.GetDocumentationFileTags());
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult CreateFile(DocumentationFileViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.FileData != null && vm.FileData.ContentLength > 0)
                {
                    using (Stream fileStream = vm.FileData.InputStream)
                    {
                        DocumentationFile file = new DocumentationFile();
                        file.FileName = vm.FileData.FileName;
                        file.FileData = StreamUtil.StreamToBytes(fileStream);
                        file.Title = vm.Title;
                        file.Description = vm.Description;
                        file.LastModifiedDate = vm.LastModifiedDate.HasValue ? vm.LastModifiedDate.Value : DateTime.Now;
                        file.UploadedBy = this.userTasks.GetAdminUser(User.Identity.Name);
                        file.UploadedDate = DateTime.Now;
                        file.DocumentationFileTag = this.docTasks.GetDocumentationFileTag(vm.TagId);
                        file = this.docTasks.SaveDocumentationFile(file);
                        return RedirectToAction("Manage");
                    }
                }
            }
            return CreateFile();
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult DeleteFile(int id)
        {
            DocumentationFile file = this.docTasks.GetDocumentationFile(id);
            if (file != null)
            {
                this.docTasks.DeleteDocumentationFile(file);
                return RedirectToAction("Manage");
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        public ActionResult Download(int id)
        {
            DocumentationFile file = this.docTasks.GetDocumentationFile(id);
            if (file != null)
            {
                return new FileStreamResult(
                    new MemoryStream(file.FileData), 
                    MIMEAssistant.GetMIMEType(file.FileName)) 
                    { 
                        FileDownloadName = file.FileName 
                    };
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }
    }
}