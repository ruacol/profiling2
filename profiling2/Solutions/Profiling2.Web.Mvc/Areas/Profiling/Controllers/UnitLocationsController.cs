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
    public class UnitLocationsController : BaseController
    {
        protected readonly IOrganizationTasks orgTasks;
        protected readonly ILocationTasks locationTasks;

        public UnitLocationsController(IOrganizationTasks orgTasks, ILocationTasks locationTasks)
        {
            this.orgTasks = orgTasks;
            this.locationTasks = locationTasks;
        }

        public ActionResult Create(int currentUnitId)
        {
            Unit u = this.orgTasks.GetUnit(currentUnitId);
            if (u != null)
            {
                ViewBag.CurrentUnit = u;
                UnitLocationViewModel vm = new UnitLocationViewModel();
                vm.UnitId = u.Id;
                vm.UnitName = u.UnitName;
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult Create(int currentUnitId, UnitLocationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                UnitLocation ul = new UnitLocation();
                Mapper.Map(vm, ul);
                ul.Unit = this.orgTasks.GetUnit(vm.UnitId);
                ul.Location = this.locationTasks.GetLocation(vm.LocationId);
                this.orgTasks.SaveUnitLocation(ul);
                return RedirectToAction("Details", "Units", new { id = currentUnitId, area = "Profiling" });
            }
            return Create(currentUnitId);
        }

        public ActionResult Edit(int currentUnitId, int id)
        {
            UnitLocation ul = this.orgTasks.GetUnitLocation(id);
            Unit u = this.orgTasks.GetUnit(currentUnitId);
            if (ul != null && u != null)
            {
                ViewBag.CurrentUnit = u;
                UnitLocationViewModel vm = new UnitLocationViewModel(ul);
                return View(vm);
            }
            else
                return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult Edit(int currentUnitId, UnitLocationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                UnitLocation ul = this.orgTasks.GetUnitLocation(vm.Id);
                if (ul != null)
                {
                    Mapper.Map(vm, ul);
                    ul.Unit = this.orgTasks.GetUnit(vm.UnitId);
                    ul.Location = this.locationTasks.GetLocation(vm.LocationId);
                    this.orgTasks.SaveUnitLocation(ul);
                    return RedirectToAction("Details", "Units", new { id = currentUnitId, area = "Profiling" });
                }
            }
            return Edit(currentUnitId, vm.Id);
        }

        [Transaction]
        public ActionResult Delete(int currentUnitId, int id)
        {
            UnitLocation ul = this.orgTasks.GetUnitLocation(id);
            Unit u = this.orgTasks.GetUnit(currentUnitId);
            if (ul != null && u != null)
            {
                this.orgTasks.DeleteUnitLocation(ul);
                return RedirectToAction("Details", "Units", new { id = currentUnitId });
            }
            else
                return new HttpNotFoundResult();
        }
    }
}