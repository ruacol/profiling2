using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class UnitMappingOverride : IAutoMappingOverride<Unit>
    {
        public void Override(AutoMapping<Unit> mapping)
        {
            mapping.HasMany<UnitHierarchy>(x => x.UnitHierarchies)
                .KeyColumn("UnitID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<UnitHierarchy>(x => x.UnitHierarchyChildren)
                .KeyColumn("ParentUnitID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
