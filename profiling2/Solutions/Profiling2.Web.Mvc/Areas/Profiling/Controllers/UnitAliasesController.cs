using System.Net;
using System.Web.Mvc;
using AutoMapper;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangeUnits)]
    public class UnitAliasesController : BaseController
    {
        protected readonly IOrganizationTasks orgTasks;

        public UnitAliasesController(IOrganizationTasks orgTasks)
        {
            this.orgTasks = orgTasks;
        }

        public ActionResult Create(int unitId)
        {
            UnitAliasViewModel vm = new UnitAliasViewModel();
            vm.UnitId = unitId;
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Create(UnitAliasViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Unit unit = this.orgTasks.GetUnit(vm.UnitId);
                if (unit != null)
                {
                    UnitAlias alias = new UnitAlias();
                    Mapper.Map<UnitAliasViewModel, UnitAlias>(vm, alias);
                    alias.Unit = unit;
                    alias = this.orgTasks.SaveUnitAlias(alias);
                    return JsonNet(string.Empty);
                }
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Unit not found.");
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        public ActionResult Edit(int id)
        {
            UnitAlias alias = this.orgTasks.GetUnitAlias(id);
            if (alias != null)
            {
                UnitAliasViewModel vm = new UnitAliasViewModel(alias);
                return View(vm);
            }
            else
                return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Edit(UnitAliasViewModel vm)
        {
            if (ModelState.IsValid)
            {
                UnitAlias alias = this.orgTasks.GetUnitAlias(vm.Id);
                if (alias != null)
                {
                    Mapper.Map<UnitAliasViewModel, UnitAlias>(vm, alias);
                    alias = this.orgTasks.SaveUnitAlias(alias);
                    return JsonNet(string.Empty);
                }
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Unit alias not found.");
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        public JsonNetResult Delete(int id)
        {
            UnitAlias alias = this.orgTasks.GetUnitAlias(id);
            if (alias != null)
            {
                this.orgTasks.DeleteUnitAlias(alias);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Unit alias successfully removed.");
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Unit alias not found.");
        }
    }
}