using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Scr.PersonFinalDecision;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class ScreeningRequestPersonFinalDecisionMappingOverride : IAutoMappingOverride<ScreeningRequestPersonFinalDecision>
    {
        public void Override(AutoMapping<ScreeningRequestPersonFinalDecision> mapping)
        {
            mapping.HasMany<ScreeningRequestPersonFinalDecisionHistory>(x => x.Histories)
                .KeyColumn("ScreeningRequestPersonFinalDecisionID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.Version(x => x.Version);
        }
    }
}
