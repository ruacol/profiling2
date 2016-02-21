using AutoMapper;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System.Net;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangePersons)]
    public class PersonAliasesController : BaseController
    {
        protected readonly IPersonTasks personTasks;

        public PersonAliasesController(IPersonTasks personTasks)
        {
            this.personTasks = personTasks;
        }

        public ActionResult Create(int personId)
        {
            PersonAliasViewModel vm = new PersonAliasViewModel();
            vm.PersonId = personId;
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Create(PersonAliasViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Person person = this.personTasks.GetPerson(vm.PersonId);
                if (person != null)
                {
                    PersonAlias alias = new PersonAlias();
                    Mapper.Map<PersonAliasViewModel, PersonAlias>(vm, alias);
                    alias.Person = person;
                    alias = this.personTasks.SavePersonAlias(alias);
                    return JsonNet(string.Empty);
                }
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Person not found.");
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        public ActionResult Edit(int id)
        {
            PersonAlias alias = this.personTasks.GetPersonAlias(id);
            if (alias != null)
            {
                PersonAliasViewModel vm = new PersonAliasViewModel(alias);
                return View(vm);
            }
            else
                return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Edit(PersonAliasViewModel vm)
        {
            if (ModelState.IsValid)
            {
                PersonAlias alias = this.personTasks.GetPersonAlias(vm.Id);
                if (alias != null)
                {
                    Mapper.Map<PersonAliasViewModel, PersonAlias>(vm, alias);
                    alias = this.personTasks.SavePersonAlias(alias);
                    return JsonNet(string.Empty);
                }
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Person alias not found.");
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        public JsonNetResult Delete(int id)
        {
            PersonAlias alias = this.personTasks.GetPersonAlias(id);
            if (alias != null)
            {
                this.personTasks.DeletePersonAlias(alias);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Person alias successfully removed.");
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Person alias not found.");
        }
    }
}