using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class EventMappingOverride : IAutoMappingOverride<Event>
    {
        public void Override(AutoMapping<Event> mapping)
        {
            mapping.HasMany<EventRelationship>(x => x.EventRelationshipsAsSubject)
                .KeyColumn("SubjectEventID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<EventRelationship>(x => x.EventRelationshipsAsObject)
                .KeyColumn("ObjectEventID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<AdminReviewedSource>(x => x.AdminReviewedSources)
                .KeyColumn("AttachedToProfileEventID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasManyToMany<Violation>(x => x.Violations)
                .Table("PRF_EventViolation")
                .ParentKeyColumn("EventID")
                .ChildKeyColumn("ViolationID")
                .AsBag();
            mapping.HasManyToMany<Tag>(x => x.Tags)
                .Table("PRF_EventTag")
                .ParentKeyColumn("EventID")
                .ChildKeyColumn("TagID")
                .AsBag();
            mapping.HasManyToMany<JhroCase>(x => x.JhroCases)
                .Table("PRF_EventJhroCase")
                .ParentKeyColumn("EventID")
                .ChildKeyColumn("JhroCaseID")
                .AsBag();
        }
    }
}
