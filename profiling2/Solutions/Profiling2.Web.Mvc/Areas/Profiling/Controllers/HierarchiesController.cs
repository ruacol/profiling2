using System.Web.Mvc;
using AutoMapper;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangeUnits)]
    public class HierarchiesController : BaseController
    {
        protected readonly IOrganizationTasks orgTasks;

        public HierarchiesController(IOrganizationTasks orgTasks)
        {
            this.orgTasks = orgTasks;
        }

        public ActionResult CreateParent(int currentUnitId)
        {
            Unit u = this.orgTasks.GetUnit(currentUnitId);
            if (u != null)
            {
                ViewBag.CurrentUnit = u;
                UnitHierarchyViewModel vm = new UnitHierarchyViewModel();
                vm.UnitId = u.Id;
                vm.UnitName = u.UnitName;
                vm.UnitHierarchyTypeId = this.orgTasks.GetUnitHierarchyType(UnitHierarchyType.NAME_HIERARCHY).Id;
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult CreateParent(int currentUnitId, UnitHierarchyViewModel vm)
        {
            if (ModelState.IsValid)
            {
                UnitHierarchy uh = new UnitHierarchy();
                Mapper.Map(vm, uh);
                uh.ParentUnit = this.orgTasks.GetUnit(vm.ParentUnitId);
                uh.Unit = this.orgTasks.GetUnit(vm.UnitId);
                uh.UnitHierarchyType = this.orgTasks.GetUnitHierarchyType(vm.UnitHierarchyTypeId);
                this.orgTasks.SaveUnitHierarchy(uh);
                return RedirectToAction("Details", "Units", new { id = currentUnitId, area = "Profiling" });
            }
            return CreateParent(currentUnitId);
        }

        public ActionResult CreateChild(int currentUnitId)
        {
            Unit u = this.orgTasks.GetUnit(currentUnitId);
            if (u != null)
            {
                ViewBag.CurrentUnit = u;
                UnitHierarchyViewModel vm = new UnitHierarchyViewModel();
                vm.ParentUnitId = u.Id;
                vm.ParentUnitName = u.UnitName;
                vm.UnitHierarchyTypeId = this.orgTasks.GetUnitHierarchyType(UnitHierarchyType.NAME_HIERARCHY).Id;
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult CreateChild(int currentUnitId, UnitHierarchyViewModel vm)
        {
            if (ModelState.IsValid)
            {
                UnitHierarchy uh = new UnitHierarchy();
                Mapper.Map(vm, uh);
                uh.ParentUnit = this.orgTasks.GetUnit(vm.ParentUnitId);
                uh.Unit = this.orgTasks.GetUnit(vm.UnitId);
                uh.UnitHierarchyType = this.orgTasks.GetUnitHierarchyType(vm.UnitHierarchyTypeId);
                this.orgTasks.SaveUnitHierarchy(uh);
                return RedirectToAction("Details", "Units", new { id = currentUnitId, area = "Profiling" });
            }
            return CreateChild(currentUnitId);
        }

        public ActionResult Edit(int currentUnitId, int id)
        {
            UnitHierarchy uh = this.orgTasks.GetUnitHierarchy(id);
            Unit u = this.orgTasks.GetUnit(currentUnitId);
            if (uh != null && u != null)
            {
                ViewBag.CurrentUnit = u;
                UnitHierarchyViewModel vm = new UnitHierarchyViewModel(uh);
                vm.PopulateDropDowns(this.orgTasks.GetUnitHierarchyTypes());
                return View(vm);
            }
            else
                return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult Edit(int currentUnitId, UnitHierarchyViewModel vm)
        {
            if (ModelState.IsValid)
            {
                UnitHierarchy uh = this.orgTasks.GetUnitHierarchy(vm.Id);
                if (uh != null)
                {
                    Mapper.Map(vm, uh);
                    uh.ParentUnit = this.orgTasks.GetUnit(vm.ParentUnitId);
                    uh.Unit = this.orgTasks.GetUnit(vm.UnitId);
                    uh.UnitHierarchyType = this.orgTasks.GetUnitHierarchyType(vm.UnitHierarchyTypeId);
                    this.orgTasks.SaveUnitHierarchy(uh);
                    return RedirectToAction("Details", "Units", new { id = currentUnitId, area = "Profiling" });
                }
            }
            return Edit(currentUnitId, vm.Id);
        }

        [Transaction]
        public ActionResult Delete(int currentUnitId, int id)
        {
            UnitHierarchy uh = this.orgTasks.GetUnitHierarchy(id);
            Unit u = this.orgTasks.GetUnit(currentUnitId);
            if (uh != null && u != null)
            {
                this.orgTasks.DeleteUnitHierarchy(uh);
                return RedirectToAction("Details", "Units", new { id = currentUnitId });
            }
            else
                return new HttpNotFoundResult();
        }

        public ActionResult CreateNameChange(int currentUnitId)
        {
            Unit u = this.orgTasks.GetUnit(currentUnitId);
            if (u != null)
            {
                ViewBag.CurrentUnit = u;
                UnitHierarchyViewModel vm = new UnitHierarchyViewModel();
                vm.ParentUnitId = u.Id;
                vm.ParentUnitName = u.UnitName;
                vm.UnitHierarchyTypeId = this.orgTasks.GetUnitHierarchyType(UnitHierarchyType.NAME_CHANGED_NAME_TO).Id;
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult CreateNameChange(int currentUnitId, UnitHierarchyViewModel vm)
        {
            if (ModelState.IsValid)
            {
                UnitHierarchy uh = new UnitHierarchy();
                Mapper.Map(vm, uh);
                uh.ParentUnit = this.orgTasks.GetUnit(vm.ParentUnitId);
                uh.Unit = this.orgTasks.GetUnit(vm.UnitId);
                uh.UnitHierarchyType = this.orgTasks.GetUnitHierarchyType(vm.UnitHierarchyTypeId);
                this.orgTasks.SaveUnitHierarchy(uh);
                return RedirectToAction("Details", "Units", new { id = currentUnitId, area = "Profiling" });
            }
            return CreateNameChange(currentUnitId);
        }

        public ActionResult EditNameChange(int currentUnitId, int id)
        {
            UnitHierarchy uh = this.orgTasks.GetUnitHierarchy(id);
            Unit u = this.orgTasks.GetUnit(currentUnitId);
            if (uh != null && u != null)
            {
                ViewBag.CurrentUnit = u;
                UnitHierarchyViewModel vm = new UnitHierarchyViewModel(uh);
                vm.UnitHierarchyTypeId = this.orgTasks.GetUnitHierarchyType(UnitHierarchyType.NAME_CHANGED_NAME_TO).Id;
                return View(vm);
            }
            else
                return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult EditNameChange(int currentUnitId, UnitHierarchyViewModel vm)
        {
            if (ModelState.IsValid)
            {
                UnitHierarchy uh = this.orgTasks.GetUnitHierarchy(vm.Id);
                if (uh != null)
                {
                    Mapper.Map(vm, uh);
                    uh.ParentUnit = this.orgTasks.GetUnit(vm.ParentUnitId);
                    uh.Unit = this.orgTasks.GetUnit(vm.UnitId);
                    uh.UnitHierarchyType = this.orgTasks.GetUnitHierarchyType(vm.UnitHierarchyTypeId);
                    this.orgTasks.SaveUnitHierarchy(uh);
                    return RedirectToAction("Details", "Units", new { id = currentUnitId, area = "Profiling" });
                }
            }
            return EditNameChange(currentUnitId, vm.Id);
        }
    }
}