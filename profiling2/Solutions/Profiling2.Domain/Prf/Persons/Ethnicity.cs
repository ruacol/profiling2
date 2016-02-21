using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Persons
{
    public class Ethnicity : Entity
    {
        [Audited]
        public virtual string EthnicityName { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }

        public virtual IList<Person> Persons { get; set; }
        public virtual IList<Ethnicity> SameEthnicitiesFrom1 { get; set; }
        public virtual IList<Ethnicity> SameEthnicitiesFrom2 { get; set; }

        public Ethnicity()
        {
            this.Persons = new List<Person>();
            this.SameEthnicitiesFrom1 = new List<Ethnicity>();
            this.SameEthnicitiesFrom2 = new List<Ethnicity>();
        }

        public virtual IList<Ethnicity> GetSameEthnicities()
        {
            return this.SameEthnicitiesFrom1.Concat(this.SameEthnicitiesFrom2).Distinct().ToList();
        }

        public override string ToString()
        {
            return this.EthnicityName;
        }
    }
}
