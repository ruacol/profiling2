using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using AutoMapper;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class OrganizationsController : BaseController
    {
        protected readonly IOrganizationTasks orgTasks;

        public OrganizationsController(IOrganizationTasks orgTasks)
        {
            this.orgTasks = orgTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchOrganizations)]
        public JsonNetResult Get()
        {
            string term = Request.QueryString["term"];
            if (!string.IsNullOrEmpty(term))
            {
                term = term.Trim();
                Regex regex = new Regex("^[0-9]+$");
                if (regex.IsMatch(term))
                {
                    int orgId;
                    if (int.TryParse(term, out orgId))
                    {
                        Organization o = this.orgTasks.GetOrganization(orgId);
                        if (o != null)
                        {
                            return JsonNet(new object[] {
                        new
                        {
                            id = o.Id,
                            text = o.ToString()
                        }
                    });
                        }
                    }
                }
                else
                {
                    IList<Organization> orgs = this.orgTasks.GetOrganizationsByName(term);
                    object[] objects = (from i in orgs
                                        select new { id = i.Id, text = i.ToString() }).ToArray();
                    return JsonNet(objects);
                }
            }
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchOrganizations)]
        public JsonNetResult Name(int id)
        {
            Organization org = this.orgTasks.GetOrganization(id);
            if (org != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = org.ToString()
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchOrganizations)]
        public ActionResult Index()
        {
            return View(this.orgTasks.GetAllOrganizations());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchOrganizations)]
        public ActionResult Details(int id)
        {
            Organization o = this.orgTasks.GetOrganization(id);
            if (o != null)
                return View(o);
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanChangeOrganizations)]
        public ActionResult Create()
        {
            OrganizationViewModel vm = new OrganizationViewModel();
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeOrganizations)]
        public ActionResult Create(OrganizationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Organization org = Mapper.Map(vm, new Organization());
                org = this.orgTasks.SaveOrganization(org);
                return RedirectToAction("Details", new { id = org.Id });
            }
            return Create();
        }

        [PermissionAuthorize(AdminPermission.CanChangeOrganizations)]
        public ActionResult Edit(int id)
        {
            Organization o = this.orgTasks.GetOrganization(id);
            if (o != null)
                return View(new OrganizationViewModel(o));
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeOrganizations)]
        public ActionResult Edit(OrganizationViewModel vm)
        {
            Organization o = this.orgTasks.GetOrganization(vm.Id);
            if (ModelState.IsValid)
            {
                o = Mapper.Map(vm, o);
                return RedirectToAction("Details", new { id = vm.Id });
            }
            return Edit(vm.Id);
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeOrganizations)]
        public ActionResult Delete(int id)
        {
            Organization o = this.orgTasks.GetOrganization(id);
            if (o != null)
            {
                if (this.orgTasks.DeleteOrganization(o))
                    return RedirectToAction("Index");
                return RedirectToAction("Details", "Organizations", new { id = id, area = "Profiling" });
            }
            return new HttpNotFoundResult();
        }
    }
}