using Profiling2.Domain;
using Profiling2.Domain.Prf;
using Profiling2.Web.Mvc.Controllers;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers
{
    [MultiRoleAuthorize(
        AdminRole.ScreeningRequestInitiator, 
        AdminRole.ScreeningRequestValidator,
        AdminRole.ScreeningRequestConditionalityParticipant,
        AdminRole.ScreeningRequestConsolidator,
        AdminRole.ScreeningRequestFinalDecider
    )]
    public class ScreeningBaseController : BaseController
    {

    }
}
