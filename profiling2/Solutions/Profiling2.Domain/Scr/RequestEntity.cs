using SharpArch.Domain.DomainModel;
using Profiling2.Domain.Prf;
using System.Collections.Generic;

namespace Profiling2.Domain.Scr
{
    public class RequestEntity : Entity
    {
        public const string NAME_FORCE = "Force";
        public const string NAME_PNC = "PNC";
        public const string NAME_FARDC = "FARDC";
        public const string NAME_UNPOL = "UNPOL";
        public const string NAME_UNKNOWN = "Unknown";
        public const string NAME_UNDP = "UNDP";

        public virtual string RequestEntityName { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
        public virtual IList<AdminUser> Users { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Notes))
                return this.RequestEntityName + " (" + this.Notes + ")";
            else
                return this.RequestEntityName;
        }
    }
}
