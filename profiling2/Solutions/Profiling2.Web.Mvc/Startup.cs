using System.Web;
using Hangfire;
using Microsoft.Owin;
using Owin;
using Profiling2.Web.Mvc;

[assembly: OwinStartup(typeof(Startup))]
namespace Profiling2.Web.Mvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("Profiling2ConnectionString");
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new AuthorizationFilterForHangfire() },
                AppPath = VirtualPathUtility.ToAbsolute("~")
            });
            app.UseHangfireServer();
        }
    }
}