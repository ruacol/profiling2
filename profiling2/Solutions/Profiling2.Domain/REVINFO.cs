using NHibernate.Envers;
using NHibernate.Envers.Configuration.Attributes;
using SharpArch.Domain.DomainModel;
using System;

namespace Profiling2.Domain
{
    [RevisionEntity(typeof(RevinfoListener))]
    public class REVINFO : Entity
    {
        // We annotate this field as required by Envers' validation checks, but effectively this model's primary key is 
        // used as the revision number by other *_AUD tables.
        [RevisionNumber]
        public virtual int REV { get; set; }

        [RevisionTimestamp]
        public virtual DateTime? REVTSTMP { get; set; }

        public virtual string UserName { get; set; }
    }
}
