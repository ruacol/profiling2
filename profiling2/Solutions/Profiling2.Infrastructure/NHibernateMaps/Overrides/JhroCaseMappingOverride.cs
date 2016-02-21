using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class JhroCaseMappingOverride : IAutoMappingOverride<JhroCase>
    {
        public void Override(AutoMapping<JhroCase> mapping)
        {
            mapping.HasManyToMany<Event>(x => x.Events)
                .Table("PRF_EventJhroCase")
                .ParentKeyColumn("JhroCaseID")
                .ChildKeyColumn("EventID")
                .AsBag();
        }
    }
}
