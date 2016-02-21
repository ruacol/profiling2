using System;
using System.Linq;
using System.Web.Security;
using Microsoft.Practices.ServiceLocation;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;

namespace Profiling2.Infrastructure.Security
{
    /// <summary>
    /// Roles check using local tables.
    /// </summary>
    public class LocalRoleProvider : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            AdminUser u = this.GetUserTasks().GetAdminUser(username);
            if (u != null)
                return u.AdminRoles.Select(x => x.Name).ToArray();
            return new string[0];
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        private IUserTasks GetUserTasks()
        {
            return ServiceLocator.Current.GetInstance<IUserTasks>();
        }
    }
}
