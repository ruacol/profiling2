using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
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
    public class PersonSourcesController : BaseController
    {
        protected readonly IPersonTasks personTasks;
        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceAttachmentTasks sourceAttachmentTasks;

        public PersonSourcesController(IPersonTasks personTasks, ISourceTasks sourceTasks, ISourceAttachmentTasks sourceAttachmentTasks)
        {
            this.personTasks = personTasks;
            this.sourceTasks = sourceTasks;
            this.sourceAttachmentTasks = sourceAttachmentTasks;
        }

        public ActionResult Edit(int id)
        {
            PersonSource ps = this.sourceAttachmentTasks.GetPersonSource(id);
            if (ps != null)
            {
                PersonSourceViewModel vm = new PersonSourceViewModel(ps);
                vm.PopulateDropDowns(this.sourceTasks.GetReliabilities());
                vm.PopulateSource(this.sourceTasks.GetSourceDTO(ps.Source.Id));
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Edit(PersonSourceViewModel vm)
        {
            if (ModelState.IsValid)
            {
                PersonSource ps = this.sourceAttachmentTasks.GetPersonSource(vm.Id.Value);
                if (ps != null)
                {
                    if (vm.ReliabilityId.HasValue)
                        ps.Reliability = this.sourceTasks.GetReliability(vm.ReliabilityId.Value);
                    ps.Commentary = vm.Commentary;
                    ps.Notes = vm.Notes;
                    ps = this.sourceAttachmentTasks.SavePersonSource(ps);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Person source not found.");
                }
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        public JsonNetResult Delete(int id)
        {
            PersonSource ps = this.sourceAttachmentTasks.GetPersonSource(id);
            if (ps != null)
            {
                this.sourceAttachmentTasks.DeletePersonSource(ps.Id);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Source successfully detached from person.");
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Person source not found.");
            }
        }

        public ActionResult Add(int personId)
        {
            Person p = this.personTasks.GetPerson(personId);
            if (p != null)
            {
                PersonSourceViewModel vm = new PersonSourceViewModel(p);
                vm.PopulateDropDowns(this.sourceTasks.GetReliabilities());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Add(PersonSourceViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Source s = this.sourceTasks.GetSource(vm.SourceId.Value);
                Person p = this.personTasks.GetPerson(vm.PersonId.Value);
                if (s != null && p != null)
                {
                    if (s.GetPersonSource(p) == null)
                    {
                        PersonSource ps = new PersonSource()
                            {
                                Source = s,
                                Person = p,
                                Commentary = vm.Commentary,
                                Notes = vm.Notes
                            };
                        if (vm.ReliabilityId.HasValue)
                            ps.Reliability = this.sourceTasks.GetReliability(vm.ReliabilityId.Value);
                        this.sourceAttachmentTasks.SavePersonSource(ps);
                        return JsonNet(string.Empty);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return JsonNet("Source already attached to this person.");
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Person or source not found.");
                }
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }
    }
}