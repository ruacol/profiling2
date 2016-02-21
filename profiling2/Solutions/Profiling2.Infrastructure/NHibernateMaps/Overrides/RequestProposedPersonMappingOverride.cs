using Profiling2.Domain.Scr.Proposed;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class RequestProposedPersonMappingOverride : IAutoMappingOverride<RequestProposedPerson>
    {
        public void Override(AutoMapping<RequestProposedPerson> mapping)
        {
            mapping.HasMany<RequestProposedPersonHistory>(x => x.RequestProposedPersonHistories)
                .KeyColumn("RequestProposedPersonID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
