using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Scr;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf
{
    public class AdminUser : Entity
    {
        /// <summary>
        /// Typically MONUSCO ID like I-1000 (if MonuscoMembershipProvider used) or
        /// Windows login (if ActiveDirectoryMembershipProvider used).
        /// </summary>
        public virtual string UserID { get; set; }

        /// <summary>
        /// Full human-readable name of the user.
        /// </summary>
        public virtual string UserName { get; set; }

        public virtual string Password { get; protected set; }
        public virtual string Email { get; set; }
        public virtual Boolean Archive { get; set; }
        public virtual IList<AdminRole> AdminRoles { get; set; }
        public virtual IList<RequestEntity> RequestEntities { get; set; }
        public virtual IList<ScreeningEntity> ScreeningEntities { get; set; }
        public virtual IList<AdminExportedPersonProfile> AdminExportedPersonProfiles { get; set; }
        public virtual IList<SourceOwningEntity> Affiliations { get; set; }

        public AdminUser()
        {
            this.AdminRoles = new List<AdminRole>();
            this.RequestEntities = new List<RequestEntity>();
            this.ScreeningEntities = new List<ScreeningEntity>();
            this.AdminExportedPersonProfiles = new List<AdminExportedPersonProfile>();
            this.Affiliations = new List<SourceOwningEntity>();
        }

        public virtual string Headline
        {
            get
            {
                string s = this.UserID;
                if (!string.IsNullOrEmpty(this.UserName))
                {
                    s += "  (" + this.UserName + ")";
                }
                return s;
            }
        }

        private HashAlgorithm _hashAlgo { get; set; }
        protected virtual HashAlgorithm HashAlgorithm
        {
            get
            {
                if (this._hashAlgo == null)
                    this._hashAlgo = new SHA256Managed();
                return this._hashAlgo;
            }
        }

        public virtual bool ValidateUser(string password)
        {
            this.HashAlgorithm.ComputeHash(UnicodeEncoding.Unicode.GetBytes(password));

            string providedPassword = UnicodeEncoding.Unicode.GetString(this.HashAlgorithm.Hash);

            return string.Equals(this.Password, providedPassword);
        }

        /// <summary>
        /// Store encoded password.
        /// </summary>
        /// <param name="newPassword">New password in plaintext.</param>
        public virtual void UpdatePassword(string newPassword)
        {
            this.HashAlgorithm.ComputeHash(UnicodeEncoding.Unicode.GetBytes(newPassword));
            this.Password = UnicodeEncoding.Unicode.GetString(this.HashAlgorithm.Hash);
        }

        /// <summary>
        /// Get screening entity (database mapping allows for more than one, but logically user should only have one).
        /// </summary>
        /// <returns></returns>
        public virtual ScreeningEntity GetScreeningEntity()
        {
            return (this.ScreeningEntities.Any() ? this.ScreeningEntities.First() : null);
        }

        /// <summary>
        /// Get request entity (database mapping allows for more than one, but logically user should only have one).
        /// </summary>
        /// <returns></returns>
        public virtual RequestEntity GetRequestEntity()
        {
            return (this.RequestEntities.Any() ? this.RequestEntities.First() : null);
        }

        public override string ToString()
        {
            return this.Headline;
        }

        public virtual void AddRequestEntity(RequestEntity re)
        {
            if (!this.RequestEntities.Contains(re))
            {
                this.RequestEntities.Add(re);
                re.Users.Add(this);
            }
        }

        public virtual void RemoveRequestEntity(RequestEntity re)
        {
            if (this.RequestEntities.Contains(re))
            {
                this.RequestEntities.Remove(re);
                re.Users.Remove(this);
            }
        }

        public virtual void AddScreeningEntity(ScreeningEntity se)
        {
            if (!this.ScreeningEntities.Contains(se))
            {
                this.ScreeningEntities.Add(se);
                se.Users.Add(this);
            }
        }

        public virtual void RemoveScreeningEntity(ScreeningEntity se)
        {
            if (this.ScreeningEntities.Contains(se))
            {
                this.ScreeningEntities.Remove(se);
                se.Users.Remove(this);
            }
        }

        public virtual bool HasSameRequestEntityAs(AdminUser user)
        {
            if (user != null && user.RequestEntities != null && this.RequestEntities != null)
                foreach (RequestEntity re in this.RequestEntities)
                    if (user.RequestEntities.Contains(re))
                        return true;
            return false;
        }

        public virtual bool IsUser(string s)
        {
            return string.Equals(this.UserID, s, StringComparison.OrdinalIgnoreCase)
                || string.Equals(this.UserName, s, StringComparison.OrdinalIgnoreCase);
        }

        public virtual bool HasRole(string role)
        {
            return this.AdminRoles.Where(x => string.Equals(x.Name, role)).Any();
        }

        public virtual bool HasPermission(string permission)
        {
            if (this.AdminRoles.Any())
            {
                return this.AdminRoles.Select(x => x.AdminPermissions)
                    .Aggregate((x, y) => x.Concat(y).ToList())
                    .Where(x => string.Equals(x.Name, permission))
                    .Any();
            }
            return false;
        }
    }
}
