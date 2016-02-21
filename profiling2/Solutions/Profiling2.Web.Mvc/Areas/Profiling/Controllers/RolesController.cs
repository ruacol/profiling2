using AutoMapper;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class RolesController : BaseController
    {
        protected readonly IOrganizationTasks orgTasks;

        public RolesController(IOrganizationTasks orgTasks)
        {
            this.orgTasks = orgTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Get()
        {
            string term = Request.QueryString["term"];
            if (string.IsNullOrEmpty(term))
            {
                IList<Role> roles = this.orgTasks.GetAllRoles();
                object[] objects = (from t in roles
                                    select new { id = t.Id, text = t.ToString() }).ToArray();
                return JsonNet(objects);
            }
            else
            {
                term = term.Trim();
                IList<Role> roles = this.orgTasks.GetRolesByName(term.Trim());
                object[] objects = (from t in roles
                                    select new { id = t.Id, text = t.ToString() }).ToArray();
                return JsonNet(objects);
            }
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Name(int id)
        {
            Role role = this.orgTasks.GetRole(id);
            if (role != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = role.ToString()
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Index()
        {
            return View(this.orgTasks.GetAllRoles());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Details(int id)
        {
            Role role = this.orgTasks.GetRole(id);
            if (role != null)
                return View(role);
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Create()
        {
            RoleViewModel vm = new RoleViewModel();
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Create(RoleViewModel vm)
        {
            if (this.orgTasks.GetRole(vm.RoleName) != null)
                ModelState.AddModelError("RoleName", "Function name already exists.");
            if (ModelState.IsValid)
            {
                Role role = Mapper.Map(vm, new Role());
                role = this.orgTasks.SaveRole(role);
                return RedirectToAction("Details", "Roles", new { id = role.Id });
            }
            return Create();
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult CreateModal()
        {
            return Create();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult CreateModal(RoleViewModel vm)
        {
            if (this.orgTasks.GetRole(vm.RoleName) != null)
                ModelState.AddModelError("RoleName", "Function name already exists.");
            if (ModelState.IsValid)
            {
                Role role = Mapper.Map(vm, new Role());
                role = this.orgTasks.SaveRole(role);
                return JsonNet(new
                {
                    Id = role.Id,
                    Name = role.RoleName,
                    WasSuccessful = true
                });
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(int id)
        {
            Role role = this.orgTasks.GetRole(id);
            if (role != null)
                return View(Mapper.Map(role, new RoleViewModel()));
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(RoleViewModel vm)
        {
            Role role = this.orgTasks.GetRole(vm.Id);
            Role newRole = this.orgTasks.GetRole(vm.RoleName);
            if (role != null && newRole != null && newRole.Id != role.Id)
                ModelState.AddModelError("RoleName", "Function name already exists.");
            if (ModelState.IsValid)
            {
                role = Mapper.Map(vm, role);
                return RedirectToAction("Details", new { id = vm.Id });
            }
            return Edit(vm.Id);
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult EditModal(int id)
        {
            return Edit(id);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult EditModal(RoleViewModel vm)
        {
            Role role = this.orgTasks.GetRole(vm.Id);
            Role newRole = this.orgTasks.GetRole(vm.RoleName);
            if (role != null && newRole != null && newRole.Id != role.Id)
                ModelState.AddModelError("RoleName", "Function name already exists.");
            if (ModelState.IsValid)
            {
                role = Mapper.Map(vm, role);
                return JsonNet(string.Empty);
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Delete(int id)
        {
            Role role = this.orgTasks.GetRole(id);
            if (role != null)
            {
                if (this.orgTasks.DeleteRole(role))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Details", new { id = id });
            }
            return new HttpNotFoundResult();
        }
    }
}