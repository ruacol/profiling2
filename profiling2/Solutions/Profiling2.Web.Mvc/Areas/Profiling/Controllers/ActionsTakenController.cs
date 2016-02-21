using System.Net;
using System.Web.Mvc;
using AutoMapper;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Actions;
using Profiling2.Domain.Prf.Events;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangeEvents)]
    public class ActionsTakenController : BaseController
    {
        protected readonly IEventTasks eventTasks;
        protected readonly IActionTakenTasks actionTakenTasks;
        protected readonly IPersonTasks personTasks;

        public ActionsTakenController(IEventTasks eventTasks, IActionTakenTasks actionTakenTasks, IPersonTasks personTasks)
        {
            this.eventTasks = eventTasks;
            this.actionTakenTasks = actionTakenTasks;
            this.personTasks = personTasks;
        }

        public ActionResult Add(int eventId)
        {
            Event e = this.eventTasks.GetEvent(eventId);
            if (e != null)
            {
                ActionTakenViewModel vm = new ActionTakenViewModel(e);
                vm.PopulateDropDowns(this.actionTakenTasks.GetActionTakenTypes());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Add(ActionTakenViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Event e = this.eventTasks.GetEvent(vm.EventId);
                if (e != null)
                {
                    ActionTaken at = new ActionTaken();
                    at.Event = e;
                    if (vm.SubjectPersonId.HasValue)
                        at.SubjectPerson = this.personTasks.GetPerson(vm.SubjectPersonId.Value);
                    if (vm.ObjectPersonId.HasValue)
                        at.ObjectPerson = this.personTasks.GetPerson(vm.ObjectPersonId.Value);
                    at.ActionTakenType = this.actionTakenTasks.GetActionTakenType(vm.ActionTakenTypeId);
                    Mapper.Map(vm, at);

                    at = this.actionTakenTasks.SaveActionTaken(at);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Event does not exist.");
                }
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        public ActionResult Edit(int id)
        {
            ActionTaken at = this.actionTakenTasks.GetActionTaken(id);
            if (at != null)
            {
                ActionTakenViewModel vm = new ActionTakenViewModel(at);
                vm.PopulateDropDowns(this.actionTakenTasks.GetActionTakenTypes());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Edit(ActionTakenViewModel vm)
        {
            if (ModelState.IsValid)
            {
                ActionTaken at = this.actionTakenTasks.GetActionTaken(vm.Id);
                if (at != null)
                {
                    Mapper.Map(vm, at);
                    at.Event = this.eventTasks.GetEvent(vm.EventId);
                    if (vm.SubjectPersonId.HasValue)
                        at.SubjectPerson = this.personTasks.GetPerson(vm.SubjectPersonId.Value);
                    else
                        at.SubjectPerson = null;
                    if (vm.ObjectPersonId.HasValue)
                        at.ObjectPerson = this.personTasks.GetPerson(vm.ObjectPersonId.Value);
                    else
                        at.ObjectPerson = null;
                    at.ActionTakenType = this.actionTakenTasks.GetActionTakenType(vm.ActionTakenTypeId);

                    at = this.actionTakenTasks.SaveActionTaken(at);
                    return JsonNet(string.Empty);
                }
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Action taken does not exist.");
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        public JsonNetResult Delete(int id)
        {
            ActionTaken at = this.actionTakenTasks.GetActionTaken(id);
            if (at != null)
            {
                this.actionTakenTasks.DeleteActionTaken(at);
                return JsonNet("Action taken for event successfully removed.");
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Action taken not found.");
        }

        public ActionResult Wanted()
        {
            return View(this.actionTakenTasks.GetWantedActionsTaken());
        }
    }
}