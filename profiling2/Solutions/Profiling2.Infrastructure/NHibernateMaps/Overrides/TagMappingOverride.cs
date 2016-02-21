using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class TagMappingOverride : IAutoMappingOverride<Tag>
    {
        public void Override(AutoMapping<Tag> mapping)
        {
            mapping.HasManyToMany<Event>(x => x.Events)
                .Table("PRF_EventTag")
                .ParentKeyColumn("TagID")
                .ChildKeyColumn("EventID")
                .AsBag();
        }
    }
}
