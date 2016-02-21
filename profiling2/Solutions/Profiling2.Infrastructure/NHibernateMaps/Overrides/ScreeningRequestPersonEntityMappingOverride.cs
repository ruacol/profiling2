using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Scr.PersonEntity;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class ScreeningRequestPersonEntityMappingOverride : IAutoMappingOverride<ScreeningRequestPersonEntity>
    {
        public void Override(AutoMapping<ScreeningRequestPersonEntity> mapping)
        {
            mapping.HasMany<ScreeningRequestPersonEntityHistory>(x => x.Histories)
                .KeyColumn("ScreeningRequestPersonEntityID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<AdminScreeningRequestPersonEntityImport>(x => x.Imports)
                .KeyColumn("ScreeningRequestPersonEntityID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.Version(x => x.Version);
        }
    }
}
