using System.Collections.Generic;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IUserTasks
    {
        AdminUser GetAdminUser(int userId);

        AdminUser GetAdminUser(string userId);

        IList<AdminUser> GetAllAdminUsers();

        AdminUser CreateOrUpdateAdminUser(string id, string name);

        /// <summary>
        /// Updates a user's ID and Name.
        /// </summary>
        /// <param name="userId">ID as exists in database.</param>
        /// <param name="newUserId">New ID to replace old.</param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        void UpdateUser(string userId, string newUserId, string firstName, string lastName, string email);

        void ArchiveUser(string username);

        AdminUser SaveOrUpdateUser(AdminUser user);

        AdminRole GetAdminRole(int id);

        AdminRole GetAdminRole(string name);

        IList<AdminRole> GetAllAdminRoles();

        IList<object> GetAdminRolesJson(string term);

        IList<object> GetAdminRolesJson();

        AdminRole SaveOrUpdateAdminRole(AdminRole role);

        AdminPermission GetAdminPermission(int id);

        AdminPermission GetAdminPermission(string name);

        IList<AdminPermission> GetAllAdminPermissions();

        IList<object> GetAdminPermissionsJson(string term);

        IList<object> GetAdminPermissionsJson();

        bool ValidateUser(string username, string password);

        IList<AdminUser> GetUsersWithRole(string role);

        IList<AdminUser> GetUsersWithPermission(string permission);

        /// <summary>
        /// Get user emails by querying database using a new NHibernate Session (useful in background jobs).
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        IList<string> GetUserEmailsWithPermissionUsingNewSession(string permission);
    }
}
