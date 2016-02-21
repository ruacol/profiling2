using System.Web.Mvc;
using log4net;

namespace Profiling2.Web.Mvc.Controllers
{
    public class MultiRoleAuthorizeAttribute : AuthorizeAttribute
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(MultiRoleAuthorizeAttribute));

        public MultiRoleAuthorizeAttribute(params string[] roles)
        {
            this.Roles = string.Join(", ", roles);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string url = filterContext.HttpContext.Request.Url.ToString();
            string username = filterContext.HttpContext.User.Identity.Name;
            log.Warn(username + " tried to access the following URL without authorisation: " + url);

            // redirects to root URL, i.e. /Profiling2/
            base.HandleUnauthorizedRequest(filterContext);

            // display plain text message if user hasn't an appropriate role
            //ContentResult result = new ContentResult();
            //result.Content = "Not authorized.";
            //filterContext.Result = result;
        }
    }
}