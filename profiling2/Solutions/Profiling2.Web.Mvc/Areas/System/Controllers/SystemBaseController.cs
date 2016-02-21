using Profiling2.Web.Mvc.Controllers;
using Profiling2.Domain;
using Profiling2.Domain.Prf;

namespace Profiling2.Web.Mvc.Areas.System.Controllers
{
    [PermissionAuthorize(AdminPermission.CanAdministrate)]
    public class SystemBaseController : BaseController
    {

    }
}
