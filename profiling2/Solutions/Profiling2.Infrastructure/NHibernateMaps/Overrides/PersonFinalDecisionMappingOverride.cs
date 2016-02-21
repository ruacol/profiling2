using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Scr.PersonFinalDecision.Old;
using FluentNHibernate.Automapping;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class PersonFinalDecisionMappingOverride : IAutoMappingOverride<PersonFinalDecision>
    {
        public void Override(AutoMapping<PersonFinalDecision> mapping)
        {
            mapping.HasMany<PersonFinalDecisionHistory>(x => x.Histories)
                .KeyColumn("PersonFinalDecisionID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
