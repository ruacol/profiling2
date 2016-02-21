using Profiling2.Domain.Prf;

namespace Profiling2.Infrastructure.Security
{
    /// <summary>
    /// Creates an admin user on login.  
    /// 
    /// Intended for use only during initial setup and configuration.
    /// </summary>
    public class SetupMembershipProvider : LocalMembershipProvider
    {
        public override bool ValidateUser(string username, string password)
        {
            AdminUser user = this.GetUserTasks().CreateOrUpdateAdminUser(username, username);
            if (user != null && !user.HasRole(AdminRole.ProfilingAdmin))
            {
                AdminRole role = this.GetUserTasks().GetAdminRole(AdminRole.ProfilingAdmin);
                if (role != null)
                    user.AdminRoles.Add(role);
            }
            return true;
        }
    }
}
