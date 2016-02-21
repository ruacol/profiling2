using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Actions;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangePersons)]
    public class ActionTakenTypesController : BaseController
    {
        protected readonly IActionTakenTasks actionTakenTasks;

        public ActionTakenTypesController(IActionTakenTasks actionTakenTasks)
        {
            this.actionTakenTasks = actionTakenTasks;
        }

        public ActionResult Index()
        {
            return View(this.actionTakenTasks.GetActionTakenTypes());
        }

        public ActionResult Details(int id)
        {
            ActionTakenType att = this.actionTakenTasks.GetActionTakenType(id);
            if (att != null)
                return View(att);
            return new HttpNotFoundResult();
        }

        public ActionResult Create()
        {
            return View(new ActionTakenTypeViewModel());
        }

        [HttpPost]
        [Transaction]
        public ActionResult Create(ActionTakenTypeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                ActionTakenType type = new ActionTakenType();
                type.ActionTakenTypeName = vm.ActionTakenTypeName;
                type.Notes = vm.Notes;
                type.IsRemedial = vm.IsRemedial;
                type.IsDisciplinary = vm.IsDisciplinary;
                type = this.actionTakenTasks.SaveActionTakenType(type);
                return RedirectToAction("Details", new { id = type.Id });
            }
            return Create();
        }

        public ActionResult Edit(int id)
        {
            ActionTakenType type = this.actionTakenTasks.GetActionTakenType(id);
            if (type != null)
                return View(new ActionTakenTypeViewModel(type));
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult Edit(ActionTakenTypeViewModel vm)
        {
            ActionTakenType type = this.actionTakenTasks.GetActionTakenType(vm.Id);
            IList<ActionTakenType> types = this.actionTakenTasks.GetActionTakenTypesByName(vm.ActionTakenTypeName);
            if (type != null && types != null && types.Any() && types.Where(x => x.Id != type.Id).Any())
                ModelState.AddModelError("ActionTakenTypeName", "Action taken type already exists.");
            if (ModelState.IsValid)
            {
                type.ActionTakenTypeName = vm.ActionTakenTypeName;
                type.Notes = vm.Notes;
                type.IsRemedial = vm.IsRemedial;
                type.IsDisciplinary = vm.IsDisciplinary;
                type = this.actionTakenTasks.SaveActionTakenType(type);
                return RedirectToAction("Details", new { id = vm.Id });
            }
            return Edit(vm.Id);
        }

        [Transaction]
        public ActionResult Delete(int id)
        {
            ActionTakenType type = this.actionTakenTasks.GetActionTakenType(id);
            if (type != null)
            {
                if (this.actionTakenTasks.DeleteActionTakenType(type))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Details", new { id = id });
            }
            return new HttpNotFoundResult();
        }
    }
}