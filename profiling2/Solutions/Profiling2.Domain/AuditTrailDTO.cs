using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NHibernate.Envers;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain
{
    public class AuditTrailDTO
    {
        public Entity Entity { get; set; }
        public REVINFO REVINFO { get; set; }
        public RevisionType RevisionType { get; set; }
        public string DifferencesString { get; set; }
        public IList<Difference> Differences { get; set; }

        public AuditTrailDTO()
        {
            this.Differences = new List<Difference>();
        }
    }
}
