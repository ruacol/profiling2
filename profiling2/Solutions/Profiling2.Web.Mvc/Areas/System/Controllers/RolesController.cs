using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Web.Mvc.Areas.System.Controllers.ViewModels;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.System.Controllers
{
    public class RolesController : SystemBaseController
    {
        protected readonly IUserTasks userTasks;

        public RolesController(IUserTasks userTasks)
        {
            this.userTasks = userTasks;
        }

        public JsonNetResult Name(int id)
        {
            AdminRole r = this.userTasks.GetAdminRole(id);
            if (r != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = r.Name
                });
            }
            else
                return JsonNet(string.Empty);
        }

        public JsonNetResult All()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["term"]))
                return JsonNet(this.userTasks.GetAdminRolesJson(Request.QueryString["term"]));
            else
                return JsonNet(this.userTasks.GetAdminRolesJson());
        }

        public ActionResult Index()
        {
            return View(this.userTasks.GetAllAdminRoles());
        }

        public ActionResult Create()
        {
            return View(new RoleViewModel());
        }

        [HttpPost]
        [Transaction]
        public ActionResult Create(RoleViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminRole role = this.userTasks.GetAdminRole(vm.Name);
                if (role != null)
                {
                    ModelState.AddModelError("Name", "Role name already exists.");
                    return View();
                }

                role = new AdminRole();
                role.Name = vm.Name;
                if (!string.IsNullOrEmpty(vm.AdminPermissionIds))
                {
                    string[] ids = vm.AdminPermissionIds.Split(',');
                    foreach (string id in ids)
                    {
                        int result;
                        if (int.TryParse(id, out result))
                        {
                            AdminPermission p = this.userTasks.GetAdminPermission(result);
                            if (p != null)
                                role.AdminPermissions.Add(p);
                        }
                    }
                }
                role = this.userTasks.SaveOrUpdateAdminRole(role);

                return RedirectToAction("Index");
            }
            return Create();
        }

        public ActionResult Edit(int id)
        {
            AdminRole role = this.userTasks.GetAdminRole(id);
            if (role != null)
            {
                RoleViewModel vm = new RoleViewModel(role);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult Edit(RoleViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminRole role = this.userTasks.GetAdminRole(vm.Id);
                if (role != null)
                {
                    role.Name = vm.Name;

                    role.AdminPermissions.Clear();
                    if (!string.IsNullOrEmpty(vm.AdminPermissionIds))
                    {
                        string[] ids = vm.AdminPermissionIds.Split(',');
                        foreach (string id in ids)
                        {
                            int result;
                            if (int.TryParse(id, out result))
                            {
                                AdminPermission p = this.userTasks.GetAdminPermission(result);
                                if (p != null)
                                    role.AdminPermissions.Add(p);
                            }
                        }
                    }

                    return RedirectToAction("Index");
                }
                return new HttpNotFoundResult();
            }
            return Edit(vm.Id);
        }
    }
}