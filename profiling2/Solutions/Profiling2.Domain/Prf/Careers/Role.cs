using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Careers
{
    public class Role : Entity
    {
        [Audited]
        public virtual string RoleName { get; set; }
        [Audited]
        public virtual string RoleNameFr { get; set; }
        [Audited]
        public virtual string Description { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }
        [Audited]
        public virtual int SortOrder { get; set; }
        [Audited]
        public virtual bool IsCommander { get; set; }
        [Audited]
        public virtual bool IsDeputyCommander { get; set; }

        public virtual IList<Career> Careers { get; set; }

        public Role()
        {
            this.Careers = new List<Career>();
        }

        public virtual string FullDisplay()
        {
            if (string.IsNullOrEmpty(this.RoleName))
            {
                return this.RoleNameFr;
            }
            else
            {
                if (string.IsNullOrEmpty(this.RoleNameFr))
                    return this.RoleName;
                else
                    return string.Join("; ", new string[2] { this.RoleName, this.RoleNameFr });
            }
        }

        public override string ToString()
        {
            return this.RoleName;
        }
    }
}
