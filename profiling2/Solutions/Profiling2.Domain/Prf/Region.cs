using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Prf.Persons;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf
{
    public class Region : Entity
    {
        [Audited]
        public virtual string RegionName { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }

        [Audited]
        public virtual IList<Location> Locations { get; set; }
        [Audited]
        public virtual IList<Person> Persons { get; set; }

        public Region()
        {
            this.Locations = new List<Location>();
            this.Persons = new List<Person>();
        }

        public override string ToString()
        {
            return this.RegionName;
        }
    }
}
