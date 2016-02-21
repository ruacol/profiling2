using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Documents
{
    public class DocumentsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Documents";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Documents_default",
                "Documents/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Documents.Controllers" }
            );
        }
    }
}
