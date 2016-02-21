using System.Net;
using System.Web.Mvc;
using AutoMapper;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class EventRelationshipsController : BaseController
    {
        protected readonly IEventTasks eventTasks;

        public EventRelationshipsController(IEventTasks eventTasks)
        {
            this.eventTasks = eventTasks;
        }

        [PermissionAuthorize(AdminPermission.CanLinkEvents)]
        public ActionResult Add()
        {
            EventRelationshipViewModel vm = new EventRelationshipViewModel();
            vm.PopulateDropDowns(this.eventTasks.GetEventRelationshipTypes());
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanLinkEvents)]
        public JsonNetResult Add(EventRelationshipViewModel vm)
        {
            if (ModelState.IsValid)
            {
                EventRelationship relationship = new EventRelationship();
                Mapper.Map<EventRelationshipViewModel, EventRelationship>(vm, relationship);
                relationship.SubjectEvent = this.eventTasks.GetEvent(vm.SubjectEventId.Value);
                relationship.ObjectEvent = this.eventTasks.GetEvent(vm.ObjectEventId.Value);
                relationship.EventRelationshipType = this.eventTasks.GetEventRelationshipType(vm.EventRelationshipTypeId);
                relationship = this.eventTasks.SaveEventRelationship(relationship);
                return JsonNet(string.Empty);
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [PermissionAuthorize(AdminPermission.CanLinkEvents)]
        public ActionResult Edit(int id)
        {
            EventRelationship relationship = this.eventTasks.GetEventRelationship(id);
            if (relationship != null)
            {
                EventRelationshipViewModel vm = new EventRelationshipViewModel(relationship);
                vm.PopulateDropDowns(this.eventTasks.GetEventRelationshipTypes());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanLinkEvents)]
        public JsonNetResult Edit(EventRelationshipViewModel vm)
        {
            if (ModelState.IsValid)
            {
                EventRelationship relationship = this.eventTasks.GetEventRelationship(vm.Id);
                if (relationship != null)
                {
                    Mapper.Map<EventRelationshipViewModel, EventRelationship>(vm, relationship);
                    relationship.SubjectEvent = this.eventTasks.GetEvent(vm.SubjectEventId.Value);
                    relationship.ObjectEvent = this.eventTasks.GetEvent(vm.ObjectEventId.Value);
                    relationship.EventRelationshipType = this.eventTasks.GetEventRelationshipType(vm.EventRelationshipTypeId);
                    relationship = this.eventTasks.SaveEventRelationship(relationship);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Event relationship not found.");
                }
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanLinkEvents)]
        public JsonNetResult Delete(int id)
        {
            EventRelationship relationship = this.eventTasks.GetEventRelationship(id);
            if (relationship != null)
            {
                this.eventTasks.DeleteEventRelationship(relationship);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Event relationship successfully deleted.");
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Event relationship not found.");
            }
        }
    }
}
