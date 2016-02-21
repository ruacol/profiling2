using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangePersonResponsibilities)]
    public class PersonResponsibilityTypesController : BaseController
    {
        protected readonly IResponsibilityTasks responsibilityTasks;

        public PersonResponsibilityTypesController(IResponsibilityTasks responsibilityTasks)
        {
            this.responsibilityTasks = responsibilityTasks;
        }

        public ActionResult Add()
        {
            PersonResponsibilityTypeViewModel vm = new PersonResponsibilityTypeViewModel();
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Add(PersonResponsibilityTypeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // TODO unique constraint on name
                PersonResponsibilityType type = new PersonResponsibilityType();
                type.PersonResponsibilityTypeName = vm.PersonRelationshipTypeName;
                type.Notes = vm.Notes;
                type = this.responsibilityTasks.SavePersonResponsibilityType(type);
                return JsonNet(string.Empty);
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }
    }
}