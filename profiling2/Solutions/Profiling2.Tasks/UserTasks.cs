using System.Collections.Generic;
using System.Linq;
using NHibernate;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks
{
    public class UserTasks : IUserTasks
    {
        protected readonly INHibernateRepository<AdminUser> userRepository;
        protected readonly INHibernateRepository<AdminRole> roleRepository;
        protected readonly INHibernateRepository<AdminPermission> permissionRepository;
        protected readonly IAdminUserQueries adminUserQueries;

        public UserTasks(INHibernateRepository<AdminUser> userRepository, 
            INHibernateRepository<AdminRole> roleRepository, 
            INHibernateRepository<AdminPermission> permissionRepository,
            IAdminUserQueries adminUserQueries)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.permissionRepository = permissionRepository;
            this.adminUserQueries = adminUserQueries;
        }

        public AdminUser GetAdminUser(int userId)
        {
            return this.userRepository.Get(userId);
        }

        public IList<AdminUser> GetAllAdminUsers()
        {
            return this.userRepository.GetAll();
        }

        public AdminUser GetAdminUser(string userId)
        {
            return this.adminUserQueries.GetAdminUser(userId);
        }

        public AdminUser CreateOrUpdateAdminUser(string userId, string userName)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("UserID", userId);
            AdminUser user = this.userRepository.FindOne(criteria);
            if (user != null)
            {
                user.UserName = userName;
            }
            else
            {
                user = new AdminUser();
                user.UserID = userId;
                user.UserName = userName;
            }
            // when this method is called, assumption is that it shouldn't be archived
            user.Archive = false;
            return this.userRepository.SaveOrUpdate(user);
        }

        public void UpdateUser(string userId, string newUserId, string firstName, string lastName, string email)
        {
            AdminUser user = this.GetAdminUser(userId);

            if (user != null && !user.Archive)
            {
                if (!string.IsNullOrEmpty(newUserId))
                    user.UserID = newUserId;

                if (!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName))
                    user.UserName = string.Join(" ", new string[] { firstName, lastName }).Trim();

                user.Email = email;

                this.SaveOrUpdateUser(user);
            }
        }

        public void ArchiveUser(string userId)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("UserID", userId);
            AdminUser user = this.userRepository.FindOne(criteria);
            if (user != null)
            {
                user.Archive = true;
                this.userRepository.SaveOrUpdate(user);
            }
        }

        public AdminUser SaveOrUpdateUser(AdminUser user)
        {
            return this.userRepository.SaveOrUpdate(user);
        }

        public AdminRole GetAdminRole(int id)
        {
            return this.roleRepository.Get(id);
        }

        public IList<AdminRole> GetAllAdminRoles()
        {
            return this.roleRepository.GetAll();
        }

        public AdminRole GetAdminRole(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Name", name);
            IList<AdminRole> roles = this.roleRepository.FindAll(criteria);
            if (roles != null && roles.Any())
                return roles.First();
            return null;
        }

        public IList<object> GetAdminRolesJson(string term)
        {
            IList<AdminRole> roles;
            if (string.IsNullOrEmpty(term))
                roles = (from r in this.roleRepository.GetAll() orderby r.Name select r).ToList<AdminRole>();
            else
                roles = (from r in this.roleRepository.GetAll() orderby r.Name where r.Name.ToLower().Contains(term.ToLower()) select r).ToList<AdminRole>();

            IList<object> list = new List<object>();
            foreach (AdminRole r in roles)
                list.Add(new { id = r.Id, text = r.Name });
            return list;
        }

        public IList<object> GetAdminRolesJson()
        {
            return this.GetAdminRolesJson(string.Empty);
        }

        public AdminRole SaveOrUpdateAdminRole(AdminRole role)
        {
            return this.roleRepository.SaveOrUpdate(role);
        }

        public AdminPermission GetAdminPermission(int id)
        {
            return this.permissionRepository.Get(id);
        }

        public IList<AdminPermission> GetAllAdminPermissions()
        {
            return this.permissionRepository.GetAll();
        }

        public AdminPermission GetAdminPermission(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Name", name);
            IList<AdminPermission> perms = this.permissionRepository.FindAll(criteria);
            if (perms != null && perms.Any())
                return perms.First();
            return null;
        }

        public IList<object> GetAdminPermissionsJson(string term)
        {
            IList<AdminPermission> perms;
            if (string.IsNullOrEmpty(term))
                perms = (from p in this.permissionRepository.GetAll() orderby p.Name select p).ToList<AdminPermission>();
            else
                perms = (from p in this.permissionRepository.GetAll() orderby p.Name where p.Name.ToLower().Contains(term.ToLower()) select p).ToList<AdminPermission>();

            IList<object> list = new List<object>();
            foreach (AdminPermission p in perms)
                list.Add(new { id = p.Id, text = p.Name });
            return list;
        }

        public IList<object> GetAdminPermissionsJson()
        {
            return this.GetAdminPermissionsJson(string.Empty);
        }

        public bool ValidateUser(string username, string password)
        {
            AdminUser user = this.GetAdminUser(username);
            if (user != null && !user.Archive)
                return user.ValidateUser(password);
            return false;
        }

        public IList<AdminUser> GetUsersWithRole(string role)
        {
            return this.GetAllAdminUsers().Where(x => x.HasRole(role) && !x.Archive).ToList();
        }

        public IList<AdminUser> GetUsersWithPermission(string permission)
        {
            return this.GetAllAdminUsers().Where(x => x.HasPermission(permission) && !x.Archive).ToList();
        }

        public IList<string> GetUserEmailsWithPermissionUsingNewSession(string permission)
        {
            using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
            {
                IList<AdminUser> users = session.QueryOver<AdminUser>()
                    .Where(x => !x.Archive)
                    .AndRestrictionOn(x => x.Email).IsNotNull
                    .List();
                return users.Where(x => x.HasPermission(permission) && !string.IsNullOrEmpty(x.Email))
                    .Select(x => x.Email)
                    .Distinct()
                    .ToList();
            }
        }
    }
}
