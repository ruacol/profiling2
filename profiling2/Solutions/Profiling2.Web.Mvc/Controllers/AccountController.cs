using System;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;
using System.Web.Security;
using log4net;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Infrastructure.Security;
using Profiling2.Infrastructure.Security.Identity;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using SharpArch.NHibernate.Web.Mvc;
using StackExchange.Profiling;

namespace Profiling2.Web.Mvc.Controllers
{
    public class AccountController : Controller
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(AccountController));
        protected IUserTasks userTasks;

        public AccountController(IUserTasks userTasks)
        {
            this.userTasks = userTasks;
        }

        //
        // GET: /Login/

        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return this.RedirectToAction("Index", "Home", null);
            else
                return View();
        }

        /// <summary>
        /// Traditional authentication check using MembershipProvider.
        /// 
        /// Membership.ValidateUser() may update the local AdminUser attributes like UserName.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [Transaction]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            var profiler = MiniProfiler.Current;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                this.TempData["Message"] = "Username or password must not be blank.";
                return this.RedirectToAction("Login");
            }
            try
            {
                bool validated = false;
                using (profiler.Step("Validating user"))
                    validated = Membership.ValidateUser(username, password);

                if (validated)
                {
                    return this.OnSuccessAuthentication(username, returnUrl);
                }
                else
                {
                    this.TempData["Message"] = "Username or password incorrect.";
                    return this.RedirectToAction("Login");
                }
            }
            catch (SqlException ex)
            {
                log.Error("Failed to connect to database for authentication", ex);
                this.TempData["Message"] = "Connection to authentication server failed, contact system administrator.";
                return this.RedirectToAction("Login");
            }
            catch (PrincipalServerDownException ex)
            {
                log.Error("Failed to connect to Active Directory server for authentication", ex);
                this.TempData["Message"] = "Connection to Active Directory server failed, contact system administrator.";
                return this.RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                log.Error("Unknown error", ex);
                this.TempData["Message"] = "Unknown error occurred, contact system administrator.";
                return this.RedirectToAction("Login");
            }
        }

        // Authentication check using HMAC-SHA1 single-sign on
        [Transaction]
        public ActionResult SSO()
        {
            return this.RedirectToAction("Login");
            //string username = Request.Params["IdTelNo"];
            //string h = Request.Params["h"];
            //string t = Request.Params["t"];

            //if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(h) || string.IsNullOrEmpty(t))
            //{
            //    // TODO should we do something else here?
            //    return this.RedirectToAction("Login");
            //}
            //else
            //{
            //    if (this.securityTasks.hmacAuthenticate(username, h, t))
            //    {
            //        return this.OnSuccessAuthentication(username, null);
            //    }
            //    else
            //    {
            //        this.TempData.Add("Message", "SSO authentication parameters incorrect.");
            //        return this.RedirectToAction("Login");
            //    }
            //}
        }

        /// <summary>
        /// Checks user authorization before granting authentication cookie.  Authorization check used to be
        /// required because MONUSCO's personnel database contained roles for multiple applications.
        /// 
        /// This action will update the Archive attribute of the user if necessary.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        protected ActionResult OnSuccessAuthentication(string username, string returnUrl)
        {
            string[] roles = Roles.GetRolesForUser(username);
            if (roles != null && roles.Length > 0)
            {
                FormsAuthentication.SetAuthCookie(username, false);

                var identity = new ExpandedIdentity(username);
                this.log.Info(identity.DisplayName + " has logged in.");

                if (!string.IsNullOrEmpty(returnUrl))
                    return this.Redirect(returnUrl);
                else
                    return this.RedirectToAction("Index", "Home", null);
            }

            // set the archive flag on this user
            this.userTasks.ArchiveUser(username);

            this.log.Warn(username + " authenticated, but not authorised to use this application.");
            this.TempData.Add("Message", "User authenticated, but not authorised to use this application.");
            return this.RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return this.RedirectToAction("Login");
        }

        [Authorize]
        public ActionResult Details()
        {
            return View(this.userTasks.GetAdminUser(User.Identity.Name));
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            AdminUser user = this.userTasks.GetAdminUser(id);
            if (user == null)
                return new HttpNotFoundResult();
            if (!User.Identity.Name.Equals(user.UserName) && !((PrfPrincipal)User).HasPermission(AdminPermission.CanAdministrate))
                return new HttpUnauthorizedResult();

            return View(new AdminUserViewModel(user));
        }

        [HttpPost]
        [Authorize]
        [Transaction]
        public ActionResult Edit(AdminUserViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminUser user = this.userTasks.GetAdminUser(vm.Id);
                if (user == null)
                    return new HttpNotFoundResult();
                if (!User.Identity.Name.Equals(user.UserName) && !((PrfPrincipal)User).HasPermission(AdminPermission.CanAdministrate))
                    return new HttpUnauthorizedResult();

                user.Email = vm.Email;
                this.userTasks.SaveOrUpdateUser(user);

                return RedirectToAction("Details", new { id = vm.Id });
            }
            return Edit(vm.Id);
        }

        [Authorize]
        public ActionResult Password(int id)
        {
            AdminUser user = this.userTasks.GetAdminUser(id);
            if (user == null)
                return new HttpNotFoundResult();
            if (!User.Identity.Name.Equals(user.UserName) && !((PrfPrincipal)User).HasPermission(AdminPermission.CanAdministrate))
                return new HttpUnauthorizedResult();

            return View(new AdminUserViewModel(user));
        }

        [HttpPost]
        [Authorize]
        [Transaction]
        public ActionResult Password(AdminUserViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminUser user = this.userTasks.GetAdminUser(vm.Id);
                if (user == null)
                    return new HttpNotFoundResult();
                if (!User.Identity.Name.Equals(user.UserName) && !((PrfPrincipal)User).HasPermission(AdminPermission.CanAdministrate))
                    return new HttpUnauthorizedResult();

                Membership.Provider.ChangePassword(user.UserID, null, vm.Password);

                if (((PrfPrincipal)User).HasPermission(AdminPermission.CanAdministrate))
                {
                    return RedirectToAction("Details", "Users", new { area = "System", id = vm.Id });
                }
                else
                {
                    return RedirectToAction("Details");
                }
            }
            return Password(vm.Id);
        }

    }
}
