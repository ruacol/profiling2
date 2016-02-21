using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System.Net;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangePersons, AdminPermission.CanViewAndSearchSources)]
    public class SourceAttachController : BaseController
    {
        protected readonly ISourceTasks sourceTasks;
        protected readonly IEventTasks eventTasks;
        protected readonly IPersonTasks personTasks;
        protected readonly ISourceAttachmentTasks sourceAttachmentTasks;

        public SourceAttachController(ISourceTasks sourceTasks, IEventTasks eventTasks, IPersonTasks personTasks, ISourceAttachmentTasks sourceAttachmentTasks)
        {
            this.sourceTasks = sourceTasks;
            this.eventTasks = eventTasks;
            this.personTasks = personTasks;
            this.sourceAttachmentTasks = sourceAttachmentTasks;
        }

        public void Index() { }

        public ActionResult Person(int sourceId, int targetId)
        {
            Source source = this.sourceTasks.GetSource(sourceId);
            if (source != null)
            {
                Profiling2.Domain.Prf.Persons.Person person = this.personTasks.GetPerson(targetId);
                if (person != null)
                {
                    PersonSourceViewModel vm = new PersonSourceViewModel();
                    vm.SourceId = source.Id;
                    vm.PersonId = person.Id;
                    vm.PopulateDropDowns(this.sourceTasks.GetReliabilities());

                    ViewBag.Person = person;
                    ViewBag.Source = source;

                    PersonSource ps = source.GetPersonSource(person);
                    if (ps != null)
                    {
                        // Person is already attached to Source
                        vm.Id = ps.Id;
                        vm.Commentary = ps.Commentary;
                        vm.Notes = ps.Notes;
                    }
                    return View(vm);
                }
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Person(PersonSourceViewModel vm)
        {
            if (vm.Id.HasValue)
            {
                this.sourceAttachmentTasks.DeletePersonSource(vm.Id.Value);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Person successfully detached from source.");
            }
            else if (ModelState.IsValid)
            {
                Source source = this.sourceTasks.GetSource(vm.SourceId.Value);
                Profiling2.Domain.Prf.Persons.Person person = this.personTasks.GetPerson(vm.PersonId.Value);
                if (source != null && person != null)
                {
                    if (source.GetPersonSource(person) == null)
                    {
                        PersonSource ps = new PersonSource();
                        ps.Source = source;
                        ps.Person = person;
                        ps.Reliability = this.sourceTasks.GetReliability(vm.ReliabilityId.Value);
                        ps.Commentary = vm.Commentary;
                        ps.Notes = vm.Notes;
                        this.sourceAttachmentTasks.SavePersonSource(ps);
                    }
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return JsonNet("Person successfully attached.");
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Person or source not found.");
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonNet("Form data failed validation.");
            }
        }

        public ActionResult Event(int sourceId, int targetId)
        {
            Source source = this.sourceTasks.GetSource(sourceId);
            if (source != null)
            {
                Profiling2.Domain.Prf.Events.Event ev = this.eventTasks.GetEvent(targetId);
                if (ev != null)
                {
                    EventSourceViewModel vm = new EventSourceViewModel();
                    vm.SourceId = source.Id;
                    vm.EventId = ev.Id;
                    vm.PopulateDropDowns(this.sourceTasks.GetReliabilities());

                    ViewBag.Event = ev;
                    ViewBag.Source = source;

                    EventSource es = source.GetEventSource(ev);
                    if (es != null)
                    {
                        // Event is already attached to Source
                        vm.Id = es.Id;
                        vm.Commentary = es.Commentary;
                        vm.Notes = es.Notes;
                    }

                    return View(vm);
                }
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        // TODO this detaches the event if POSTed an EventSource.Id.  Function replicated in EventSourcesController.
        public JsonNetResult Event(EventSourceViewModel vm)
        {
            if (vm.Id.HasValue)
            {
                this.sourceAttachmentTasks.DeleteEventSource(vm.Id.Value);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Event successfully detached from source.");
            }
            else if (ModelState.IsValid)
            {
                Source source = this.sourceTasks.GetSource(vm.SourceId.Value);
                Profiling2.Domain.Prf.Events.Event ev = this.eventTasks.GetEvent(vm.EventId.Value);
                if (source != null && ev != null)
                {
                    if (source.GetEventSource(ev) == null)
                    {
                        EventSource es = new EventSource();
                        es.Source = source;
                        es.Event = ev;
                        es.Reliability = this.sourceTasks.GetReliability(vm.ReliabilityId.Value);
                        es.Commentary = vm.Commentary;
                        es.Notes = vm.Notes;
                        this.sourceAttachmentTasks.SaveEventSource(es);
                    }
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return JsonNet("Event successfully attached.");
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Event or source not found.");
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonNet("Form failed validation.");
            }
        }
    }
}