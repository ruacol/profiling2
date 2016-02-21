using System.Net;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanLinkEventsAndSources, AdminPermission.CanViewAndSearchSources)]
    public class EventSourcesController : BaseController
    {
        protected readonly IEventTasks eventTasks;
        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceAttachmentTasks sourceAttachmentTasks;

        public EventSourcesController(IEventTasks eventTasks, ISourceTasks sourceTasks, ISourceAttachmentTasks sourceAttachmentTasks)
        {
            this.eventTasks = eventTasks;
            this.sourceTasks = sourceTasks;
            this.sourceAttachmentTasks = sourceAttachmentTasks;
        }

        public ActionResult Edit(int id)
        {
            EventSource es = this.sourceAttachmentTasks.GetEventSource(id);
            if (es != null)
            {
                EventSourceViewModel vm = new EventSourceViewModel(es);
                vm.PopulateDropDowns(this.sourceTasks.GetReliabilities());

                // get SourceDTO in order to not load Source entity
                vm.PopulateSource(this.sourceTasks.GetSourceDTO(es.Source.Id));

                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Edit(EventSourceViewModel vm)
        {
            if (ModelState.IsValid && vm.Id.HasValue)
            {
                EventSource es = this.sourceAttachmentTasks.GetEventSource(vm.Id.Value);
                if (es != null)
                {
                    if (vm.ReliabilityId.HasValue)
                        es.Reliability = this.sourceTasks.GetReliability(vm.ReliabilityId.Value);
                    es.Commentary = vm.Commentary;
                    es.Notes = vm.Notes;
                    return JsonNet(string.Empty);
                }
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Event source does not exist.");
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        public JsonNetResult Delete(int id)
        {
            EventSource es = this.sourceAttachmentTasks.GetEventSource(id);
            if (es != null)
            {
                this.sourceAttachmentTasks.DeleteEventSource(es.Id);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Source successfully detached from event.");
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Event source not found.");
            }
        }

        public ActionResult Add(int eventId)
        {
            Event e = this.eventTasks.GetEvent(eventId);
            if (e != null)
            {
                EventSourceViewModel esvm = new EventSourceViewModel(e);
                esvm.PopulateDropDowns(this.sourceTasks.GetReliabilities());
                return View(esvm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Add(EventSourceViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Source source = this.sourceTasks.GetSource(vm.SourceId.Value);
                Event ev = this.eventTasks.GetEvent(vm.EventId.Value);
                if (source != null && ev != null)
                {
                    if (source.GetEventSource(ev) == null)
                    {
                        EventSource es = new EventSource();
                        es.Source = source;
                        es.Event = ev;
                        if (vm.ReliabilityId.HasValue)
                            es.Reliability = this.sourceTasks.GetReliability(vm.ReliabilityId.Value);
                        es.Commentary = vm.Commentary;
                        es.Notes = vm.Notes;
                        this.sourceAttachmentTasks.SaveEventSource(es);
                        return JsonNet(string.Empty);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return JsonNet("Source already attached to this event.");
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Event or source not found.");
                }
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }
    }
}