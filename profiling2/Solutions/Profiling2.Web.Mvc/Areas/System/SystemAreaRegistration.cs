using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.System
{
    public class SystemAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "System";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "LuceneCreateSourceIndex",
                "System/Lucene/CreateSourceIndexes/{code}",
                new { controller = "Lucene", action = "CreateSourceIndexes", code = UrlParameter.Optional }
            );

            context.MapRoute(
                "System_default",
                "System/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.System.Controllers" }
            );
        }
    }
}
