using System.Linq;
using System.Web.Mvc;
using log4net;
using Mvc.JQuery.Datatables;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Scr;
using Profiling2.Web.Mvc.Areas.System.Controllers.ViewModels;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.System.Controllers
{
    public class UsersController : SystemBaseController
    {
        private readonly ILog log = LogManager.GetLogger(typeof(UsersController));
        private readonly IUserTasks userTasks;
        private readonly IRequestTasks requestTasks;
        private readonly IScreeningTasks screeningTasks;
        private readonly ISourcePermissionTasks sourcePermissionTasks;

        public UsersController(IUserTasks userTasks, 
            IRequestTasks requestTasks, 
            IScreeningTasks screeningTasks, 
            ISourcePermissionTasks sourcePermissionTasks)
        {
            this.userTasks = userTasks;
            this.requestTasks = requestTasks;
            this.screeningTasks = screeningTasks;
            this.sourcePermissionTasks = sourcePermissionTasks;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Archived()
        {
            return View();
        }

        public JsonNetResult DataTables()
        {
            return JsonNet(new DataTablesData
            {
                aaData = this.userTasks.GetAllAdminUsers().Where(x => !x.Archive).Select(x => new 
                { 
                    Id = x.Id,
                    UserID = x.UserID,
                    UserName = x.UserName,
                    Email = x.Email,
                    Roles = x.AdminRoles.Select(y => y.Name)
                }).ToArray()
            });
        }

        public JsonNetResult DataTablesArchived()
        {
            return JsonNet(new DataTablesData
            {
                aaData = this.userTasks.GetAllAdminUsers().Where(x => x.Archive).Select(x => new
                {
                    Id = x.Id,
                    UserID = x.UserID,
                    UserName = x.UserName,
                    Email = x.Email,
                    Roles = x.AdminRoles.Select(y => y.Name)
                }).ToArray()
            });
        }

        public ActionResult Details(int id)
        {
            return View(this.userTasks.GetAdminUser(id));
        }

        public ActionResult Create()
        {
            return View(new UserViewModel());
        }

        [HttpPost]
        [Transaction]
        public ActionResult Create(UserViewModel vm)
        {
            AdminUser existing = this.userTasks.GetAdminUser(vm.UserID);
            if (existing != null)
                ModelState.AddModelError("UserID", "A user exists with that ID.");

            if (ModelState.IsValid)
            {
                AdminUser u = new AdminUser();
                u.UserID = vm.UserID;
                u.UserName = vm.UserName;
                u.Email = vm.Email;
                u = this.userTasks.SaveOrUpdateUser(u);
                return RedirectToAction("Details", new { id = u.Id });
            }
            return Create();
        }
 
        public ActionResult Edit(int id)
        {
            AdminUser u = this.userTasks.GetAdminUser(id);
            if (u != null)
            {
                UserViewModel vm = new UserViewModel();
                vm.Id = u.Id;
                vm.UserID = u.UserID;
                vm.UserName = u.UserName;
                vm.Email = u.Email;
                vm.AdminRoleIds = string.Join(",", (from r in u.AdminRoles select r.Id).ToList<int>());
                vm.SourceOwningEntityIds = string.Join(",", (from e in u.Affiliations select e.Id).ToList<int>());
                vm.RequestEntityID = (u.RequestEntities != null && u.RequestEntities.Count > 0 ? u.RequestEntities[0].Id : 0);
                vm.ScreeningEntityID = (u.ScreeningEntities != null && u.ScreeningEntities.Count > 0 ? u.ScreeningEntities[0].Id : 0);
                vm.PopulateDropDowns(this.requestTasks.GetRequestEntities(), this.screeningTasks.GetScreeningEntities());
                return View(vm);
            }
            else
                return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult Edit(UserViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminUser u = this.userTasks.GetAdminUser(vm.Id);

                u.Email = vm.Email;
                u.AdminRoles.Clear();

                if (!string.IsNullOrEmpty(vm.AdminRoleIds))
                {
                    string[] ids = vm.AdminRoleIds.Split(',');
                    foreach (string id in ids)
                    {
                        int result;
                        if (int.TryParse(id, out result))
                        {
                            AdminRole r = this.userTasks.GetAdminRole(result);
                            if (r != null)
                                u.AdminRoles.Add(r);
                        }
                    }
                }

                u.Affiliations.Clear();
                if (!string.IsNullOrEmpty(vm.SourceOwningEntityIds))
                {
                    string[] ids = vm.SourceOwningEntityIds.Split(',');
                    foreach (string id in ids)
                    {
                        int result;
                        if (int.TryParse(id, out result))
                        {
                            SourceOwningEntity e = this.sourcePermissionTasks.GetSourceOwningEntity(result);
                            if (e != null)
                                u.Affiliations.Add(e);
                        }
                    }
                }

                if (vm.RequestEntityID.HasValue)
                    this.requestTasks.SetRequestEntity(vm.RequestEntityID.Value, vm.Id);
                else
                {
                    foreach (RequestEntity re in this.requestTasks.GetRequestEntities())
                        u.RemoveRequestEntity(re);
                }

                if (vm.ScreeningEntityID.HasValue)
                    this.screeningTasks.SetScreeningEntity(vm.ScreeningEntityID.Value, vm.Id);
                else
                {
                    foreach (ScreeningEntity se in this.screeningTasks.GetScreeningEntities())
                        u.RemoveScreeningEntity(se);
                }

            }
            return RedirectToAction("Details", new { id = vm.Id });
        }

        [Transaction]
        public ActionResult Archive(int id)
        {
            AdminUser u = this.userTasks.GetAdminUser(id);
            if (u != null)
            {
                u.Archive = true;
                u = this.userTasks.SaveOrUpdateUser(u);
                return RedirectToAction("Index");
            }
            return new HttpNotFoundResult();
        }

        [Transaction]
        public ActionResult Enable(int id)
        {
            AdminUser u = this.userTasks.GetAdminUser(id);
            if (u != null)
            {
                u.Archive = false;
                u = this.userTasks.SaveOrUpdateUser(u);
                return RedirectToAction("Details", new { id = id });
            }
            return new HttpNotFoundResult();
        }
    }
}
