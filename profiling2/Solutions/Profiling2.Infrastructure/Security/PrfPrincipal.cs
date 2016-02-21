using System.Linq;
using System.Security.Principal;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Infrastructure.Security
{
    public class PrfPrincipal : GenericPrincipal, IPrincipal
    {
        protected virtual string[] Permissions { get; set; }
        protected virtual string[] Affiliations { get; set; }

        public PrfPrincipal(IIdentity identity, string[] roles, string[] permissions, string[] affiliations)
            : base(identity, roles)
        {
            this.Permissions = permissions;
            this.Affiliations = affiliations;
        }

        public virtual bool HasPermission(string permission)
        {
            return this.Permissions.Contains(permission);
        }

        public virtual bool CanAccess(Source s)
        {
            if (s != null)
            {
                return this.CanAccess(s.IsRestricted, 
                    s.HasUploadedBy() ? s.GetUploadedBy().UserID : null, 
                    s.SourceAuthors.Select(x => x.Author).ToArray(), 
                    s.SourceOwningEntities.Select(x => x.Name).ToArray(),
                    s.IsPublic);
            }
            return false;
        }

        public virtual bool CanAccess(SourceDTO dto, string[] authors, string[] owners)
        {
            if (dto != null)
            {
                return this.CanAccess(dto.IsRestricted, dto.UploadedByUserID, authors, owners, dto.IsPublic);
            }
            return false;
        }

        public virtual bool CanAccess(FeedingSource fs)
        {
            if (fs != null)
            {
                return this.CanAccess(fs.Restricted, 
                    fs.UploadedBy != null ? fs.UploadedBy.UserID : null, 
                    fs.SourceAuthors.Select(x => x.Author).ToArray(), 
                    fs.SourceOwningEntities.Select(x => x.Name).ToArray(), 
                    fs.IsPublic);
            }
            return false;
        }

        protected virtual bool CanAccess(bool isRestricted, string uploadedByUserId, string[] authors, string[] owners, bool isPublic)
        {
            // protect restricted sources
            if (isRestricted && !this.HasPermission(AdminPermission.CanViewAndSearchRestrictedSources))
            {
                return false;
            }
            else if (this.HasPermission(AdminPermission.CanViewAndSearchAllSources))
            {
                return true;
            }
            else
            {
                // user can access sources they uploaded.
                if (!string.IsNullOrEmpty(uploadedByUserId) && string.Equals(uploadedByUserId, this.Identity.Name))
                {
                    return true;
                }

                // user can access sources they authored.
                if (authors != null && authors.Length > 0 && authors.Where(x => string.Equals(x, this.Identity.Name, System.StringComparison.OrdinalIgnoreCase)).Any())
                {
                    return true;
                }

                // user can access sources owned by entity they are affiliated with.
                if (owners != null && owners.Length > 0)
                {
                    foreach (string owner in owners)
                        if (this.Affiliations.Where(x => string.Equals(x, owner, System.StringComparison.OrdinalIgnoreCase)).Any())
                            return true;
                }

                // user can access sources marked public.
                if (isPublic)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
