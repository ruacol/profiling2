using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf.Organizations;
using FluentNHibernate.Automapping;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class OrganizationMappingOverride : IAutoMappingOverride<Organization>
    {
        public void Override(AutoMapping<Organization> mapping)
        {
            mapping.HasMany<OrganizationRelationship>(x => x.OrganizationRelationshipsAsSubject)
                .KeyColumn("SubjectOrganizationID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<OrganizationRelationship>(x => x.OrganizationRelationshipsAsObject)
                .KeyColumn("ObjectOrganizationID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
