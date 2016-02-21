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
    public class OperationAliasesController : BaseController
    {
        protected readonly IOrganizationTasks orgTasks;

        public OperationAliasesController(IOrganizationTasks orgTasks)
        {
            this.orgTasks = orgTasks;
        }

        public ActionResult Create(int operationId)
        {
            Operation op = this.orgTasks.GetOperation(operationId);
            if (op != null)
            {
                OperationAliasViewModel vm = new OperationAliasViewModel();
                vm.OperationId = operationId;
                vm.OperationName = op.Name;
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult Create(OperationAliasViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Operation op = this.orgTasks.GetOperation(vm.OperationId);
                if (op != null)
                {
                    OperationAlias alias = new OperationAlias();
                    Mapper.Map<OperationAliasViewModel, OperationAlias>(vm, alias);
                    alias.Operation = op;
                    alias = this.orgTasks.SaveOperationAlias(alias);
                    return RedirectToAction("Details", "Operations", new { area = "Profiling", id = op.Id });
                }
                ModelState.AddModelError("OperationId", "That operation doesn't exist.");
            }
            return Create(vm.OperationId);
        }

        public ActionResult Edit(int id)
        {
            OperationAlias alias = this.orgTasks.GetOperationAlias(id);
            if (alias != null)
            {
                OperationAliasViewModel vm = new OperationAliasViewModel(alias);
                return View(vm);
            }
            else
                return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult Edit(OperationAliasViewModel vm)
        {
            if (ModelState.IsValid)
            {
                OperationAlias alias = this.orgTasks.GetOperationAlias(vm.Id);
                if (alias != null)
                {
                    Mapper.Map<OperationAliasViewModel, OperationAlias>(vm, alias);
                    alias = this.orgTasks.SaveOperationAlias(alias);
                    return JsonNet(string.Empty);
                }
                return RedirectToAction("Details", "Operations", new { area = "Profiling", id = vm.OperationId });
            }
            return Edit(vm.Id);
        }

        [Transaction]
        public JsonNetResult Delete(int id)
        {
            OperationAlias alias = this.orgTasks.GetOperationAlias(id);
            if (alias != null)
            {
                this.orgTasks.DeleteOperationAlias(alias);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Operation alias successfully removed.");
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Operation alias not found.");
        }
    }
}