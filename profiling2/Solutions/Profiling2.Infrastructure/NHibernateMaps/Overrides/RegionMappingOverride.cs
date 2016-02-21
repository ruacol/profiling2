using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class RegionMappingOverride : IAutoMappingOverride<Region>
    {
        public void Override(AutoMapping<Region> mapping)
        {
            mapping.HasMany<Person>(x => x.Persons)
                .KeyColumn("BirthRegionID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
