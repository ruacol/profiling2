using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.System.Controllers
{
    public class PermissionsController : SystemBaseController
    {
        protected readonly IUserTasks userTasks;

        public PermissionsController(IUserTasks userTasks)
        {
            this.userTasks = userTasks;
        }

        public JsonNetResult Name(int id)
        {
            AdminPermission p = this.userTasks.GetAdminPermission(id);
            if (p != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = p.Name
                });
            }
            else
                return JsonNet(string.Empty);
        }

        public JsonNetResult All()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["term"]))
                return JsonNet(this.userTasks.GetAdminPermissionsJson(Request.QueryString["term"]));
            else
                return JsonNet(this.userTasks.GetAdminPermissionsJson());
        }
    }
}