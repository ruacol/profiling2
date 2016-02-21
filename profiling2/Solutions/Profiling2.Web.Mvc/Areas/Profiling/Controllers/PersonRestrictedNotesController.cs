using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
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
    [PermissionAuthorize(AdminPermission.CanViewAndChangePersonRestrictedNotes)]
    public class PersonRestrictedNotesController : BaseController
    {
        protected readonly IPersonTasks personTasks;

        public PersonRestrictedNotesController(IPersonTasks personTasks) 
        {
            this.personTasks = personTasks;
        }

        public JsonNetResult Json(int personId)
        {
            Person person = this.personTasks.GetPerson(personId);
            if (person != null)
                return JsonNet(new Dictionary<string, object>() { { "Notes", person.PersonRestrictedNotes.Select(x => new PersonRestrictedNoteViewModel(x)) } });

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(string.Empty);
        }

        public ActionResult CreateModal(int personId)
        {
            Person p = this.personTasks.GetPerson(personId);
            if (p != null)
            {
                PersonRestrictedNoteViewModel vm = new PersonRestrictedNoteViewModel(p);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult CreateModal(PersonRestrictedNoteViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Person p = this.personTasks.GetPerson(vm.PersonId);
                if (p != null)
                {
                    PersonRestrictedNote n = new PersonRestrictedNote();
                    n.Person = p;
                    n.Note = vm.Note;
                    this.personTasks.SavePersonRestrictedNote(n);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Person does not exist.");
                }
            }
            else
                return JsonNet(this.GetErrorsForJson());
        }

        public ActionResult EditModal(int personId, int id)
        {
            Person p = this.personTasks.GetPerson(personId);
            PersonRestrictedNote n = this.personTasks.GetPersonRestrictedNote(id);
            if (p != null && n != null)
            {
                PersonRestrictedNoteViewModel vm = new PersonRestrictedNoteViewModel(n);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult EditModal(PersonRestrictedNoteViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Person p = this.personTasks.GetPerson(vm.PersonId);
                PersonRestrictedNote n = this.personTasks.GetPersonRestrictedNote(vm.Id);
                if (p != null && n != null)
                {
                    n.Person = p;
                    n.Note = vm.Note;
                    this.personTasks.SavePersonRestrictedNote(n);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Person or note does not exist.");
                }
            }
            else
                return JsonNet(this.GetErrorsForJson());
        }

        [Transaction]
        public JsonNetResult Delete(int id)
        {
            PersonRestrictedNote n = this.personTasks.GetPersonRestrictedNote(id);
            if (n != null)
            {
                this.personTasks.DeletePersonRestrictedNote(n);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Person note successfully deleted.");
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Person note not found.");
            }
        }
    }
}