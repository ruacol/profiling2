using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Careers
{
    public class Rank : Entity
    {
        [Audited]
        public virtual string RankName { get; set; }
        [Audited]
        public virtual string RankNameFr { get; set; }
        [Audited]
        public virtual string Description { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }
        [Audited]
        public virtual int SortOrder { get; set; }

        public virtual IList<Career> Careers { get; set; }

        public Rank()
        {
            this.Careers = new List<Career>();
        }

        public virtual string FullDisplay()
        {
            if (string.IsNullOrEmpty(this.RankName))
            {
                return this.RankNameFr;
            }
            else
            {
                if (string.IsNullOrEmpty(this.RankNameFr))
                    return this.RankName;
                else
                    return string.Join("; ", new string[2] { this.RankName, this.RankNameFr });
            }
        }

        public override string ToString()
        {
            return this.RankName;
        }
    }
}
