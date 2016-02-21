using Profiling2.Domain.Prf;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Profiling2.Web.Mvc.Areas.System.Controllers.ViewModels
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "A name is required.")]
        public string Name { get; set; }
        public string AdminPermissionIds { get; set; }

        public RoleViewModel() { }

        public RoleViewModel(AdminRole r)
        {
            if (r != null)
            {
                this.Id = r.Id;
                this.Name = r.Name;
                this.AdminPermissionIds = string.Join(",", r.AdminPermissions.Select(x => x.Id.ToString()));
            }
        }
    }
}