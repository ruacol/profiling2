using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Web.Mvc.Areas.Hrdb.Controllers.ViewModels;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Hrdb.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangeEvents)]
    public class EventsController : BaseController
    {
        protected readonly ISourceTasks sourceTasks;
        protected readonly IEventTasks eventTasks;
        protected readonly IPersonTasks personTasks;
        protected readonly IResponsibilityTasks responsibilityTasks;
        protected readonly IOrganizationTasks orgTasks;
        protected readonly ILocationTasks locationTasks;

        public EventsController(ISourceTasks sourceTasks,
            IEventTasks eventTasks,
            IPersonTasks personTasks,
            IResponsibilityTasks responsibilityTasks,
            IOrganizationTasks orgTasks,
            ILocationTasks locationTasks)
        {
            this.sourceTasks = sourceTasks;
            this.eventTasks = eventTasks;
            this.personTasks = personTasks;
            this.responsibilityTasks = responsibilityTasks;
            this.orgTasks = orgTasks;
            this.locationTasks = locationTasks;
        }

        [Transaction]  // HrdbCaseViewModel creates a location if none exists
        public ActionResult Import(JhroCaseViewModel vm)
        {
            JhroCase jc = null;
            if (vm.Id > 0)
                jc = this.sourceTasks.GetJhroCase(vm.Id);
            else if (!string.IsNullOrEmpty(vm.CaseNumber))
                jc = this.sourceTasks.GetJhroCase(vm.CaseNumber);

            if (jc != null)
            {
                ModelState.Remove("CaseNumber");  // undo unintended validation check (happens because we use JhroCaseViewModel not as intended)
                return View(new HrdbCaseViewModel(jc));
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        [HttpPost]
        [Transaction]
        public ActionResult Import(HrdbCaseViewModel vm)
        {
            JhroCase jc = this.sourceTasks.GetJhroCase(vm.Id);

            // if an existing Event is selected, ignore validation errors to do with new event
            if (vm.EventId.HasValue)
            {
                // TODO brittle
                ModelState.Remove("Event.ViolationIds");
                ModelState.Remove("Event.LocationId");
            }

            if (ModelState.IsValid)
            {
                Event e = null;
                if (vm.EventId.HasValue)
                {
                    e = this.eventTasks.GetEvent(vm.EventId.Value);
                    e.AddJhroCase(jc);
                }
                else
                {
                    // create new event - TODO duplicates code in other EventsController
                    e = new Event();
                    Mapper.Map<EventViewModel, Event>(vm.Event, e);
                    if (!string.IsNullOrEmpty(vm.Event.ViolationIds))
                    {
                        string[] ids = vm.Event.ViolationIds.Split(',');
                        foreach (string id in ids)
                        {
                            int result;
                            if (int.TryParse(id, out result))
                            {
                                Violation v = this.eventTasks.GetViolation(result);
                                if (v != null)
                                    e.Violations.Add(v);
                            }
                        }
                    }
                    e.Location = this.locationTasks.GetLocation(vm.Event.LocationId.Value);
                    e.EventVerifiedStatus = vm.Event.EventVerifiedStatusId.HasValue ? this.eventTasks.GetEventVerifiedStatus(vm.Event.EventVerifiedStatusId.Value) : null;
                    if (!string.IsNullOrEmpty(vm.Event.TagIds))
                    {
                        string[] ids = vm.Event.TagIds.Split(',');
                        foreach (string id in ids)
                        {
                            int result;
                            if (int.TryParse(id, out result))
                            {
                                Tag t = this.eventTasks.GetTag(result);
                                if (t != null)
                                    e.Tags.Add(t);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(vm.Event.JhroCaseIds))
                    {
                        string[] ids = vm.Event.JhroCaseIds.Split(',');
                        foreach (string id in ids)
                        {
                            int result;
                            if (int.TryParse(id, out result))
                            {
                                JhroCase jhroCase = this.sourceTasks.GetJhroCase(result);
                                if (jhroCase != null)
                                    e.AddJhroCase(jhroCase);
                            }
                        }
                    }
                }

                // create responsibilities
                if (e != null && vm.HrdbPerpetrators != null)
                {
                    foreach (HrdbPerpetratorViewModel pvm in vm.HrdbPerpetrators)
                    {
                        if (pvm.PersonId.HasValue)
                        {
                            Person p = this.personTasks.GetPerson(pvm.PersonId.Value);
                            if (p != null)
                            {
                                if (pvm.PersonResponsibilityTypeId.HasValue)
                                {
                                    PersonResponsibilityType prt = this.responsibilityTasks.GetPersonResponsibilityType(pvm.PersonResponsibilityTypeId.Value);
                                    if (prt != null)
                                    {
                                        PersonResponsibility pr = new PersonResponsibility()
                                        {
                                            Event = e,
                                            Person = p,
                                            PersonResponsibilityType = prt,
                                            Violations = pvm.GetViolationIds().Select(x => this.eventTasks.GetViolation(x)).ToList()
                                        };

                                        e.AddPersonResponsibility(pr);
                                    }
                                }
                            }
                        }
                        else if (pvm.OrganizationId.HasValue)
                        {
                            Organization o = this.orgTasks.GetOrganization(pvm.OrganizationId.Value);
                            if (o != null)
                            {
                                if (pvm.OrganizationResponsibilityTypeId.HasValue)
                                {
                                    OrganizationResponsibilityType ort = this.responsibilityTasks.GetOrgResponsibilityType(pvm.OrganizationResponsibilityTypeId.Value);
                                    if (ort != null)
                                    {
                                        OrganizationResponsibility or = new OrganizationResponsibility()
                                        {
                                            Event = e,
                                            Organization = o,
                                            OrganizationResponsibilityType = ort
                                        };

                                        e.AddOrganizationResponsibility(or);
                                    }
                                }
                            }
                        }
                    }
                }

                e = this.eventTasks.SaveEvent(e);

                return RedirectToAction("Details", "Cases", new { id = jc.Id });
            }

            return Import(new JhroCaseViewModel(jc));
        }
    }
}