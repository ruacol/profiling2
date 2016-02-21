using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangePersonResponsibilities)]
    public class PersonResponsibilitiesController : BaseController
    {
        protected readonly IEventTasks eventTasks;
        protected readonly IPersonTasks personTasks;
        protected readonly IResponsibilityTasks responsibilityTasks;
        protected readonly ISourceTasks sourceTasks;

        public PersonResponsibilitiesController(IEventTasks eventTasks, IPersonTasks personTasks, IResponsibilityTasks responsibilityTasks, ISourceTasks sourceTasks)
        {
            this.eventTasks = eventTasks;
            this.personTasks = personTasks;
            this.responsibilityTasks = responsibilityTasks;
            this.sourceTasks = sourceTasks;
        }

        public ActionResult AddForPerson(int personId)
        {
            Person p = this.personTasks.GetPerson(personId);
            if (p != null)
            {
                PersonResponsibilityViewModel vm = new PersonResponsibilityViewModel(p);
                vm.PopulateDropDowns(this.responsibilityTasks.GetPersonResponsibilityTypes());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult AddForPerson(PersonResponsibilityViewModel vm)
        {
            return Add(vm);
        }

        public ActionResult Add(int eventId)
        {
            Event e = this.eventTasks.GetEvent(eventId);
            if (e != null)
            {
                PersonResponsibilityViewModel vm = new PersonResponsibilityViewModel(e, e.EventSources.Select(x => this.sourceTasks.GetSourceDTO(x.Source.Id)).ToList());
                vm.PopulateDropDowns(this.responsibilityTasks.GetPersonResponsibilityTypes());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Add(PersonResponsibilityViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Event e = this.eventTasks.GetEvent(vm.EventId.Value);
                Person p = this.personTasks.GetPerson(vm.PersonId.Value);
                if (e != null && p != null)
                {
                    PersonResponsibility pr = new PersonResponsibility();
                    pr.Event = e;
                    if (!string.IsNullOrEmpty(vm.ViolationIds))
                    {
                        string[] ids = vm.ViolationIds.Split(',');
                        foreach (string id in ids)
                        {
                            int result;
                            if (int.TryParse(id, out result))
                            {
                                Violation v = this.eventTasks.GetViolation(result);
                                if (v != null)
                                {
                                    pr.AddViolation(v);
                                }
                            }
                        }
                    }
                    pr.Person = p;
                    pr.PersonResponsibilityType = this.responsibilityTasks.GetPersonResponsibilityType(vm.PersonResponsibilityTypeId.Value);
                    pr.Commentary = vm.Commentary;
                    pr.Notes = vm.Notes;
                    pr.Archive = false;
                    if (e.AddPersonResponsibility(pr))
                    {
                        e = this.eventTasks.SaveEvent(e);
                        return JsonNet(string.Empty);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return JsonNet("Person responsibility already exists for this event.");
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Event or person does not exist.");
                }
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        public ActionResult Edit(int id)
        {
            PersonResponsibility pr = this.responsibilityTasks.GetPersonResponsibility(id);
            if (pr != null)
            {
                PersonResponsibilityViewModel vm = new PersonResponsibilityViewModel(pr, pr.Event.EventSources.Select(x => this.sourceTasks.GetSourceDTO(x.Source.Id)).ToList());
                vm.PopulateDropDowns(this.responsibilityTasks.GetPersonResponsibilityTypes());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Edit(PersonResponsibilityViewModel vm)
        {
            if (ModelState.IsValid)
            {
                PersonResponsibility pr = this.responsibilityTasks.GetPersonResponsibility(vm.Id);
                if (pr != null)
                {
                    pr.Violations.Clear();
                    if (!string.IsNullOrEmpty(vm.ViolationIds))
                    {
                        string[] ids = vm.ViolationIds.Split(',');
                        foreach (string id in ids)
                        {
                            int result;
                            if (int.TryParse(id, out result))
                            {
                                Violation v = this.eventTasks.GetViolation(result);
                                if (v != null)
                                {
                                    pr.AddViolation(v);
                                }
                            }
                        }
                    }
                    pr.PersonResponsibilityType = this.responsibilityTasks.GetPersonResponsibilityType(vm.PersonResponsibilityTypeId.Value);
                    pr.Commentary = vm.Commentary;
                    pr.Notes = vm.Notes;
                    pr = this.responsibilityTasks.SavePersonResponsibility(pr);
                    return JsonNet(string.Empty);
                }
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Responsibility not found.");
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        public JsonNetResult Delete(int id)
        {
            PersonResponsibility pr = responsibilityTasks.GetPersonResponsibility(id);
            if (pr != null)
            {
                this.responsibilityTasks.DeletePersonResponsibility(pr);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Person's responsibility for event successfully removed.");
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Responsibility not found.");
        }

        // temporary action to help introduction of new virtual field PersonResponsibility.PersonFunctionUnitSummary
        public ActionResult List(int id)
        {
            ViewBag.Responsibilities = this.responsibilityTasks.GetAllPersonResponsibilities()
                    .Where(x => !string.IsNullOrEmpty(x.Commentary) && !string.IsNullOrEmpty(x.GetPersonFunctionUnitSummary()))
                    .Where(x => x.GetEditDistanceBetweenCommentaryAndFunction() == id)
                    .ToList();
            return View();
        }

        // temporary
        [HttpPost]
        [Transaction]
        public ActionResult List(IEnumerable<int> ids)
        {
            if (ids != null && ids.Any())
            {
                foreach (int id in ids)
                {
                    PersonResponsibility pr = this.responsibilityTasks.GetPersonResponsibility(id);
                    pr.Commentary = string.Empty;
                    this.responsibilityTasks.SavePersonResponsibility(pr);
                }
            }
            return null;
        }
    }
}