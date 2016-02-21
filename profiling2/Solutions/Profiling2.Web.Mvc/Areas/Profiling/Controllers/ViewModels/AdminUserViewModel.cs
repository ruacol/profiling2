using Profiling2.Domain.Prf;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class AdminUserViewModel
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public AdminUserViewModel() { }

        public AdminUserViewModel(AdminUser user)
        {
            if (user != null)
            {
                this.Id = user.Id;
                this.UserID = user.UserID;
                this.UserName = user.UserName;
                this.Email = user.Email;
            }
        }
    }
}