using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class OperationMappingOverride : IAutoMappingOverride<Operation>
    {
        public void Override(AutoMapping<Operation> mapping)
        {
            mapping.HasMany<Operation>(x => x.FormerOperations)
                .KeyColumn("NextOperationID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
