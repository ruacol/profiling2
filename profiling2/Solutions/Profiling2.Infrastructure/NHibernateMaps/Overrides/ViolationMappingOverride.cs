using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Responsibility;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class ViolationMappingOverride : IAutoMappingOverride<Violation>
    {
        public void Override(AutoMapping<Violation> mapping)
        {
            mapping.HasManyToMany<Event>(x => x.Events)
                .Table("PRF_EventViolation")
                .ParentKeyColumn("ViolationID")
                .ChildKeyColumn("EventID")
                .Inverse()
                .Cascade.None()
                .AsBag();
            mapping.HasManyToMany<PersonResponsibility>(x => x.PersonResponsibilities)
                .Table("PRF_PersonResponsibilityViolation")
                .ParentKeyColumn("ViolationID")
                .ChildKeyColumn("PersonResponsibilityID")
                .Inverse()
                .Cascade.None()
                .AsBag();
            mapping.HasMany<Violation>(x => x.ChildrenViolations)
                .KeyColumn("ParentViolationID")
                .Cascade.None()
                .Inverse();
        }
    }
}
