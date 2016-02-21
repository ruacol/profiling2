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
    public class UnitOperationsController : BaseController
    {
        protected readonly IOrganizationTasks orgTasks;

        public UnitOperationsController(IOrganizationTasks orgTasks)
        {
            this.orgTasks = orgTasks;
        }

        public ActionResult EditModal(int id)
        {
            UnitOperation uo = this.orgTasks.GetUnitOperation(id);
            if (uo != null)
                return View(new UnitOperationViewModel(uo));
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult EditModal(UnitOperationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                UnitOperation uo = this.orgTasks.GetUnitOperation(vm.Id);
                uo = Mapper.Map(vm, uo);

                uo = this.orgTasks.SaveUnitOperation(uo);
                return JsonNet(string.Empty);
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [Transaction]
        public JsonNetResult Remove(int id)
        {
            UnitOperation uo = this.orgTasks.GetUnitOperation(id);
            if (uo != null)
            {
                this.orgTasks.DeleteUnitOperation(uo);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Unit involvement with operation successfully removed.");
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Unit involvement with operation not found.");
            }
        }
    }
}