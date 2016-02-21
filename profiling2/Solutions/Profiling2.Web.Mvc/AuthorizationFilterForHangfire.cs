using System.Collections.Generic;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Profiling2.Domain.Prf;
using Profiling2.Infrastructure.Security;

namespace Profiling2.Web.Mvc
{
    public class AuthorizationFilterForHangfire : IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            // In case you need an OWIN context, use the next line,
            // `OwinContext` class is the part of the `Microsoft.Owin` package.
            var context = new OwinContext(owinEnvironment);

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            if (context.Authentication.User.Identity.IsAuthenticated)
            {
                return ((PrfPrincipal)context.Authentication.User).HasPermission(AdminPermission.CanAdministrate);
            }
            return false;
        }
    }
}