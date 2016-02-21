using AutoMapper;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System.Net;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class CareersController : BaseController
    {
        protected readonly IPersonTasks personTasks;
        protected readonly ICareerTasks careerTasks;
        protected readonly IOrganizationTasks organizationTasks;
        protected readonly ILocationTasks locationTasks;

        public CareersController(IPersonTasks personTasks, 
            ICareerTasks careerTasks,
            IOrganizationTasks organizationTasks,
            ILocationTasks locationTasks)
        {
            this.personTasks = personTasks;
            this.careerTasks = careerTasks;
            this.organizationTasks = organizationTasks;
            this.locationTasks = locationTasks;
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(int personId, int careerId)
        {
            Career career = this.careerTasks.GetCareer(careerId);
            Person person = this.personTasks.GetPerson(personId);
            if (career != null && person != null)
            {
                CareerViewModel vm = new CareerViewModel(career);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult Edit(CareerViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Career career = this.careerTasks.GetCareer(vm.Id);
                if (career != null)
                {
                    Mapper.Map<CareerViewModel, Career>(vm, career);
                    career.Person = this.personTasks.GetPerson(vm.PersonId.Value);
                    career.Organization = vm.OrganizationId.HasValue ? this.organizationTasks.GetOrganization(vm.OrganizationId.Value) : null;
                    career.Location = vm.LocationId.HasValue ? this.locationTasks.GetLocation(vm.LocationId.Value) : null;
                    career.Rank = vm.RankId.HasValue ? this.organizationTasks.GetRank(vm.RankId.Value) : null;
                    career.Role = vm.RoleId.HasValue ? this.organizationTasks.GetRole(vm.RoleId.Value) : null;
                    career.Unit = vm.UnitId.HasValue ? this.organizationTasks.GetUnit(vm.UnitId.Value) : null;
                    career = this.careerTasks.SaveCareer(career);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Career not found.");
                }
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult Delete(int personId, int careerId)
        {
            Career career = this.careerTasks.GetCareer(careerId);
            Person person = this.personTasks.GetPerson(personId);
            if (career != null && person != null)
            {
                this.careerTasks.DeleteCareer(career);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Career successfully deleted.");
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Person or career not found.");
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Add(int personId)
        {
            Person p = this.personTasks.GetPerson(personId);
            if (p != null)
            {
                CareerViewModel vm = new CareerViewModel(p);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult Add(CareerViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Career career = new Career();
                Mapper.Map<CareerViewModel, Career>(vm, career);
                career.Person = this.personTasks.GetPerson(vm.PersonId.Value);
                if (vm.OrganizationId.HasValue)
                    career.Organization = this.organizationTasks.GetOrganization(vm.OrganizationId.Value);
                if (vm.LocationId.HasValue)
                    career.Location = this.locationTasks.GetLocation(vm.LocationId.Value);
                if (vm.RankId.HasValue)
                    career.Rank = this.organizationTasks.GetRank(vm.RankId.Value);
                if (vm.RoleId.HasValue)
                    career.Role = this.organizationTasks.GetRole(vm.RoleId.Value);
                if (vm.UnitId.HasValue)
                    career.Unit = this.organizationTasks.GetUnit(vm.UnitId.Value);
                career = this.careerTasks.SaveCareer(career);
                return JsonNet(string.Empty);
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Commanders()
        {
            return View(this.careerTasks.GetCommanders());
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanViewAndChangePersonRestrictedNotes)]
        public ActionResult MigrateSomeFunctionsToRanks()
        {
            this.organizationTasks.MigrateSomeFunctionsToRanks();
            return RedirectToAction("Index", "Roles");
        }
    }
}