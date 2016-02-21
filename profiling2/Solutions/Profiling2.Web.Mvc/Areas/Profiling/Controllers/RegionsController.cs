using System.Collections.Generic;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class RegionsController : BaseController
    {
        protected readonly ILocationTasks locationTasks;

        public RegionsController(ILocationTasks locationTasks)
        {
            this.locationTasks = locationTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Name(int id)
        {
            Region region = this.locationTasks.GetRegion(id);
            if (region != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = region.ToString()
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Json()
        {
            string term = Request.QueryString["term"];
            if (!string.IsNullOrEmpty(term))
                term = term.Trim();
            return JsonNet(this.locationTasks.GetRegionsJson(term));
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Index()
        {
            return View(this.locationTasks.GetAllRegions());
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Create()
        {
            RegionViewModel vm = new RegionViewModel();
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Create(RegionViewModel vm)
        {
            Region existing = this.locationTasks.GetRegion(vm.RegionName);
            if (existing != null)
                ModelState.AddModelError("RegionName", "Region name already exists.");

            if (ModelState.IsValid)
            {
                Region reg = new Region();
                reg.RegionName = vm.RegionName;
                reg.Notes = vm.Notes;
                reg.Archive = vm.Archive;
                reg = this.locationTasks.SaveRegion(reg);
                return RedirectToAction("Index");
            }
            else
            {
                return Create();
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(int id)
        {
            Region reg = this.locationTasks.GetRegion(id);
            if (reg != null)
            {
                RegionViewModel vm = new RegionViewModel(reg);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(RegionViewModel vm)
        {
            Region reg = this.locationTasks.GetRegion(vm.Id);
            Region existing = this.locationTasks.GetRegion(vm.RegionName);
            if (reg != null && existing != null && reg.Id != existing.Id)
                ModelState.AddModelError("RegionName", "Region name already exists.");

            if (ModelState.IsValid)
            {
                reg.RegionName = vm.RegionName;
                reg.Notes = vm.Notes;
                reg.Archive = vm.Archive;
                reg = this.locationTasks.SaveRegion(reg);
                return RedirectToAction("Index");
            }
            else
            {
                return Edit(vm.Id);
            }
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Delete(int id)
        {
            Region reg = this.locationTasks.GetRegion(id);
            if (reg != null)
            {
                this.locationTasks.DeleteRegion(reg);
                return RedirectToAction("Index");
            }
            return new HttpNotFoundResult();
        }
    }
}