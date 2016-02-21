using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Scr.PersonRecommendation;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class ScreeningRequestPersonRecommendationMappingOverride : IAutoMappingOverride<ScreeningRequestPersonRecommendation>
    {
        public void Override(AutoMapping<ScreeningRequestPersonRecommendation> mapping)
        {
            mapping.HasMany<ScreeningRequestPersonRecommendationHistory>(x => x.Histories)
                .KeyColumn("ScreeningRequestPersonRecommendationID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<AdminScreeningRequestPersonRecommendationImport>(x => x.Imports)
                .KeyColumn("ScreeningRequestPersonRecommendationID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.Version(x => x.Version);
        }
    }
}
