using System.Linq;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers
{
    public class OwnersController : BaseController
    {
        protected readonly ISourcePermissionTasks sourcePermissionTasks;
        protected readonly ISourceTasks sourceTasks;

        public OwnersController(ISourcePermissionTasks sourcePermissionTasks, ISourceTasks sourceTasks)
        {
            this.sourcePermissionTasks = sourcePermissionTasks;
            this.sourceTasks = sourceTasks;
        }

        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public JsonNetResult Name(int id)
        {
            SourceOwningEntity e = this.sourcePermissionTasks.GetSourceOwningEntity(id);
            if (e != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = e.Name
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public JsonNetResult All()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["term"]))
                return JsonNet(this.sourcePermissionTasks.GetSourceOwningEntitiesJson(Request.QueryString["term"]));
            else
                return JsonNet(this.sourcePermissionTasks.GetSourceOwningEntitiesJson());
        }

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Index()
        {
            return View(this.sourcePermissionTasks.GetAllSourceOwningEntities());
        }

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Create()
        {
            SourceOwningEntityViewModel vm = new SourceOwningEntityViewModel();
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Create(SourceOwningEntityViewModel vm)
        {
            if (this.sourcePermissionTasks.GetSourceOwningEntities(vm.Name).Any())
                ModelState.AddModelError("Name", "Name must be unique.");

            if (ModelState.IsValid)
            {
                SourceOwningEntity soe = new SourceOwningEntity();
                soe.Name = vm.Name;
                soe.SourcePathPrefix = vm.SourcePathPrefix;
                soe = this.sourcePermissionTasks.SaveSourceOwningEntity(soe);
                return RedirectToAction("Index");
            }
            return Create();
        }

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Edit(int id)
        {
            SourceOwningEntity soe = this.sourcePermissionTasks.GetSourceOwningEntity(id);
            if (soe != null)
            {
                SourceOwningEntityViewModel vm = new SourceOwningEntityViewModel(soe);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Edit(SourceOwningEntityViewModel vm)
        {
            SourceOwningEntity soe = this.sourcePermissionTasks.GetSourceOwningEntity(vm.Id);
            if (soe != null)
            {
                if (this.sourcePermissionTasks.GetSourceOwningEntities(vm.Name).Where(x => x.Id != soe.Id).Any())
                    ModelState.AddModelError("Name", "Name must be unique.");

                if (ModelState.IsValid)
                {
                    soe.Name = vm.Name;
                    soe.SourcePathPrefix = vm.SourcePathPrefix;
                    soe = this.sourcePermissionTasks.SaveSourceOwningEntity(soe);
                    return RedirectToAction("Index");
                }
                return Edit(vm.Id);
            }
            return new HttpNotFoundResult();
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Delete(int id)
        {
            SourceOwningEntity soe = this.sourcePermissionTasks.GetSourceOwningEntity(id);
            if (soe != null)
            {
                this.sourcePermissionTasks.DeleteSourceOwningEntity(soe);
                return RedirectToAction("Index");
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Populate()
        {
            this.sourceTasks.PopulateSourceOwners();
            return null;
        }
    }
}