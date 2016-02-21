using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Hrdb
{
    public class HrdbAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Hrdb";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Hrdb_default",
                "Hrdb/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Hrdb.Controllers" }
            );
        }
    }
}
