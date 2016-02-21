using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.API
{
    /// <summary>
    /// http://stackoverflow.com/questions/9399568/asp-mvc-3-actionfilter-for-basic-authentication
    /// </summary>
    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        private static readonly string AuthorizationHeader = "Authorization";
        private static readonly string BasicHeader = "Basic ";
        private static readonly string Username = ConfigurationManager.AppSettings["ApiUsername"].ToString();
        private static readonly string Password = ConfigurationManager.AppSettings["ApiPassword"].ToString();
        private static readonly char[] Separator = ":".ToCharArray();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

                if (!Authenticated(filterContext.HttpContext.Request))
                    filterContext.Result = new HttpUnauthorizedResult();

                base.OnActionExecuting(filterContext);
            }
            catch
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        private bool Authenticated(HttpRequestBase httpRequestBase)
        {
            bool authenticated = false;

            if (!string.IsNullOrEmpty(httpRequestBase.Headers[AuthorizationHeader]) &&
                httpRequestBase.Headers[AuthorizationHeader].StartsWith(BasicHeader, StringComparison.InvariantCultureIgnoreCase))
            {
                string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(
                    httpRequestBase.Headers[AuthorizationHeader].Substring(BasicHeader.Length))).Split(Separator);

                if (credentials.Length == 2 && credentials[0] == Username && credentials[1] == Password)
                {
                    authenticated = true;
                }
            }

            return authenticated;
        }
    }
}