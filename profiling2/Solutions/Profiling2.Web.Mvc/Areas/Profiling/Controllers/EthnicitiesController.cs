using AutoMapper;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
	public class EthnicitiesController : BaseController
	{
        protected readonly IPersonTasks personTasks;

        public EthnicitiesController(IPersonTasks personTasks)
        {
            this.personTasks = personTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Name(int id)
        {
            Ethnicity ethnicity = this.personTasks.GetEthnicity(id);
            if (ethnicity != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = ethnicity.ToString()
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Json()
        {
            string term = Request.QueryString["term"];
            if (!string.IsNullOrEmpty(term))
                term = term.Trim();
            return JsonNet(this.personTasks.GetEthnicitiesJson(term));
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Index()
        {
            return View(this.personTasks.GetEthnicities());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Details(int id)
        {
            Ethnicity e = this.personTasks.GetEthnicity(id);
            if (e != null)
                return View(e);
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Create()
        {
            EthnicityViewModel vm = new EthnicityViewModel();
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Create(EthnicityViewModel vm)
        {
            if (this.personTasks.GetEthnicity(vm.EthnicityName) != null)
                ModelState.AddModelError("EthnicityName", "Ethnicity name already exists.");
            if (ModelState.IsValid)
            {
                Ethnicity ethnicity = Mapper.Map(vm, new Ethnicity());
                ethnicity = this.personTasks.SaveEthnicity(ethnicity);
                return RedirectToAction("Details", new { id = ethnicity.Id });
            }
            return Create();
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(int id)
        {
            Ethnicity e = this.personTasks.GetEthnicity(id);
            if (e != null)
                return View(Mapper.Map(e, new EthnicityViewModel()));
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(EthnicityViewModel vm)
        {
            Ethnicity ethnicity = this.personTasks.GetEthnicity(vm.Id);
            Ethnicity newEthnicity = this.personTasks.GetEthnicity(vm.EthnicityName);
            if (ethnicity != null && newEthnicity != null && newEthnicity.Id != ethnicity.Id)
                ModelState.AddModelError("EthnicityName", "Ethnicity name already exists.");

            Ethnicity e = this.personTasks.GetEthnicity(vm.Id);
            if (ModelState.IsValid)
            {
                e = Mapper.Map(vm, e);
                return RedirectToAction("Details", new { id = vm.Id });
            }
            return Edit(vm.Id);
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Delete(int id)
        {
            Ethnicity e = this.personTasks.GetEthnicity(id);
            if (e != null)
            {
                if (this.personTasks.DeleteEthnicity(e))
                    return RedirectToAction("Index");
                return RedirectToAction("Details", "Organizations", new { id = id, area = "Profiling" });
            }
            return new HttpNotFoundResult();
        }
	}
}