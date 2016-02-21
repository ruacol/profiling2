using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using Profiling2.Infrastructure.Security;

namespace Profiling2.Web.Mvc.Controllers
{
    /// <summary>
    /// All permissions listed are required to be authorised.
    /// </summary>
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(PermissionAuthorizeAttribute));

        protected virtual string[] _permissions { get; set; }

        public PermissionAuthorizeAttribute(params string[] permissions)
        {
            this._permissions = permissions;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (httpContext.User.Identity.IsAuthenticated)
            {
                PrfPrincipal user = (PrfPrincipal)httpContext.User;
                return this._permissions.Select(x => user.HasPermission(x)).Aggregate((x, y) => x && y);
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string url = filterContext.HttpContext.Request.Url.ToString();
            string username = filterContext.HttpContext.User.Identity.Name;
            log.Warn(username + " tried to access the following URL without proper permission: " + url);

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new ContentResult()
                {
                    Content = "Your account does not have the permission to view this resource."
                };
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}