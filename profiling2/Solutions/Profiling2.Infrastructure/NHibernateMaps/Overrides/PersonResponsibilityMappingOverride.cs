using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Responsibility;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class PersonResponsibilityMappingOverride : IAutoMappingOverride<PersonResponsibility>
    {
        public void Override(AutoMapping<PersonResponsibility> mapping)
        {
            mapping.HasManyToMany<Violation>(x => x.Violations)
                .Table("PRF_PersonResponsibilityViolation")
                .ParentKeyColumn("PersonResponsibilityID")
                .ChildKeyColumn("ViolationID")
                .AsBag();
        }
    }
}
