namespace Profiling2.Web.Mvc.Controllers
{
    using System.Web.Mvc;
    using Profiling2.Domain.Prf;
    using Profiling2.Infrastructure.Security;

    public class HomeController : BaseController
    {
        public HomeController() { }

        public ActionResult Index()
        {
            if (((PrfPrincipal)User).HasPermission(AdminPermission.CanViewPersonResponsibilities))
            {
                return RedirectToAction("Index", "Home", new { area = "Profiling" });
            }
            else if (User.IsInRole(AdminRole.ScreeningRequestInitiator)
                || User.IsInRole(AdminRole.ScreeningRequestValidator)
                || ((PrfPrincipal)User).HasPermission(AdminPermission.CanPerformScreeningInput)
                || User.IsInRole(AdminRole.ScreeningRequestConsolidator)
                || User.IsInRole(AdminRole.ScreeningRequestFinalDecider))
            {
                return RedirectToAction("Index", "Home", new { area = "Screening" });
            }
            else if (((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchSources))
            {
                return RedirectToAction("Index", "Home", new { area = "Sources" });
            }
            else
            {
                return View();
            }
        }
    }
}
