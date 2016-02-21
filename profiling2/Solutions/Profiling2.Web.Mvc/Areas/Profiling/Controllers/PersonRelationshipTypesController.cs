using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangePersons)]
    public class PersonRelationshipTypesController : BaseController
    {
        protected readonly IPersonTasks personTasks;

        public PersonRelationshipTypesController(IPersonTasks personTasks)
        {
            this.personTasks = personTasks;
        }

        public ActionResult Index()
        {
            return View(this.personTasks.GetAllPersonRelationshipTypes()
                .Select(x => new PersonRelationshipTypeViewModel(x))
                .ToList());
        }

        public ActionResult Details(int id)
        {
            PersonRelationshipType type = this.personTasks.GetPersonRelationshipType(id);
            if (type != null)
                return View(type);
            return new HttpNotFoundResult();
        }

        public ActionResult Create()
        {
            return View(new PersonRelationshipTypeViewModel());
        }

        [HttpPost]
        [Transaction]
        public ActionResult Create(PersonRelationshipTypeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                PersonRelationshipType type = new PersonRelationshipType();
                type.PersonRelationshipTypeName = vm.PersonRelationshipTypeName;
                type.IsCommutative = vm.IsCommutative;
                type.Archive = vm.Archive;
                type.Notes = vm.Notes;
                type.Code = vm.PersonRelationshipTypeName.ToUpper().Replace(' ', '_');
                type = this.personTasks.SavePersonRelationshipType(type);
                return RedirectToAction("Details", new { id = type.Id });
            }
            return Create();
        }

        public ActionResult Edit(int id)
        {
            PersonRelationshipType type = this.personTasks.GetPersonRelationshipType(id);
            if (type != null)
                return View(new PersonRelationshipTypeViewModel(type));
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult Edit(PersonRelationshipTypeViewModel vm)
        {
            PersonRelationshipType type = this.personTasks.GetPersonRelationshipType(vm.Id);
            IList<PersonRelationshipType> types = this.personTasks.GetPersonRelationshipTypesByName(vm.PersonRelationshipTypeName);
            if (type != null && types != null && types.Any() && types.Where(x => x.Id != type.Id).Any())
                ModelState.AddModelError("PersonRelationshipTypeName", "Relationship type already exists.");
            if (ModelState.IsValid)
            {
                type.PersonRelationshipTypeName = vm.PersonRelationshipTypeName;
                type.IsCommutative = vm.IsCommutative;
                type.Archive = vm.Archive;
                type.Notes = vm.Notes;
                type = this.personTasks.SavePersonRelationshipType(type);
                return RedirectToAction("Details", new { id = vm.Id });
            }
            return Edit(vm.Id);
        }

        [Transaction]
        public ActionResult Delete(int id)
        {
            PersonRelationshipType type = this.personTasks.GetPersonRelationshipType(id);
            if (type != null)
            {
                if (this.personTasks.DeletePersonRelationshipType(type))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Details", new { id = id });
            }
            return new HttpNotFoundResult();
        }
    }
}