using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Actions;
using Profiling2.Domain.Prf.Responsibility;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class PersonMappingOverride : IAutoMappingOverride<Person>
    {
        public void Override(AutoMapping<Person> mapping)
        {
            mapping.HasMany<ActionTaken>(x => x.ActionsTakenAsSubject)
                .KeyColumn("SubjectPersonID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<ActionTaken>(x => x.ActionsTakenAsObject)
                .KeyColumn("ObjectPersonID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<AdminReviewedSource>(x => x.AdminReviewedSources)
                .KeyColumn("AttachedToProfilePersonID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<PersonRelationship>(x => x.PersonRelationshipAsSubject)
                .KeyColumn("SubjectPersonID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<PersonRelationship>(x => x.PersonRelationshipAsObject)
                .KeyColumn("ObjectPersonID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<PersonPhoto>(x => x.PersonPhotos)
                .KeyColumn("PersonID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
