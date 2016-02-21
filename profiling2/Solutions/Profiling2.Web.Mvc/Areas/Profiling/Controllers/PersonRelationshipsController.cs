using AutoMapper;
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
    public class PersonRelationshipsController : BaseController
    {
        protected readonly IPersonTasks personTasks;

        public PersonRelationshipsController(IPersonTasks personTasks)
        {
            this.personTasks = personTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Get()
        {
            string term = Request.QueryString["term"];
            if (string.IsNullOrEmpty(term))
            {
                IList<PersonRelationshipType> types = this.personTasks.GetAllPersonRelationshipTypes();
                object[] objects = (from t in types
                                    select new { id = t.Id, text = t.PersonRelationshipTypeName }).ToArray();
                return JsonNet(objects);
            }
            else
            {
                term = term.Trim();
                IList<PersonRelationshipType> types = this.personTasks.GetPersonRelationshipTypesByName(term);
                object[] objects = (from t in types
                                    select new { id = t.Id, text = t.PersonRelationshipTypeName }).ToArray();
                return JsonNet(objects);
            }
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Name(int id)
        {
            PersonRelationshipType type = this.personTasks.GetPersonRelationshipType(id);
            if (type != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = type.PersonRelationshipTypeName
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(int personId, int relationshipId)
        {
            PersonRelationship relationship = this.personTasks.GetPersonRelationship(relationshipId);
            Person person = this.personTasks.GetPerson(personId);
            if (relationship != null && person != null)
            {
                PersonRelationshipViewModel vm = new PersonRelationshipViewModel(relationship);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult Edit(PersonRelationshipViewModel vm)
        {
            if (ModelState.IsValid)
            {
                PersonRelationship relationship = this.personTasks.GetPersonRelationship(vm.Id);
                if (relationship != null)
                {
                    Mapper.Map<PersonRelationshipViewModel, PersonRelationship>(vm, relationship);
                    relationship.SubjectPerson = this.personTasks.GetPerson(vm.SubjectPersonId.Value);
                    relationship.ObjectPerson = this.personTasks.GetPerson(vm.ObjectPersonId.Value);
                    relationship.PersonRelationshipType = this.personTasks.GetPersonRelationshipType(vm.PersonRelationshipTypeId.Value);
                    relationship = this.personTasks.SavePersonRelationship(relationship);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Person relationship not found.");
                }
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult Delete(int personId, int relationshipId)
        {
            PersonRelationship relationship = this.personTasks.GetPersonRelationship(relationshipId);
            Person person = this.personTasks.GetPerson(personId);
            if (relationship != null && person != null)
            {
                this.personTasks.DeletePersonRelationship(relationship);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Person relationship successfully deleted.");
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Person or person relationship not found.");
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Add(int personId)
        {
            Person p = this.personTasks.GetPerson(personId);
            if (p != null)
            {
                PersonRelationshipViewModel vm = new PersonRelationshipViewModel();
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult Add(PersonRelationshipViewModel vm)
        {
            if (ModelState.IsValid)
            {
                PersonRelationship relationship = new PersonRelationship();
                Mapper.Map<PersonRelationshipViewModel, PersonRelationship>(vm, relationship);
                relationship.SubjectPerson = this.personTasks.GetPerson(vm.SubjectPersonId.Value);
                relationship.ObjectPerson = this.personTasks.GetPerson(vm.ObjectPersonId.Value);
                relationship.PersonRelationshipType = this.personTasks.GetPersonRelationshipType(vm.PersonRelationshipTypeId.Value);
                relationship = this.personTasks.SavePersonRelationship(relationship);
                return JsonNet(string.Empty);
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }
    }
}