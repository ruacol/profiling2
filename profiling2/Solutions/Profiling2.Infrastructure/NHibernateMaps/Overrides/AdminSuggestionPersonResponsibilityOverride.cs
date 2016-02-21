using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Type;
using Profiling2.Domain.Prf.Suggestions;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class AdminSuggestionPersonResponsibilityOverride : IAutoMappingOverride<AdminSuggestionPersonResponsibility>
    {
        public void Override(AutoMapping<AdminSuggestionPersonResponsibility> mapping)
        {
            mapping.Map(x => x.SuggestionFeatures).CustomType<XmlDocType>();
        }
    }
}
