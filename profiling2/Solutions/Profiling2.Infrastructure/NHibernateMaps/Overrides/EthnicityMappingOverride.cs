using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class EthnicityMappingOverride : IAutoMappingOverride<Ethnicity>
    {
        public void Override(AutoMapping<Ethnicity> mapping)
        {
            mapping.HasManyToMany<Ethnicity>(x => x.SameEthnicitiesFrom1)
                .Table("PRF_SameEthnicity")
                .ParentKeyColumn("Ethnicity1ID")
                .ChildKeyColumn("Ethnicity2ID")
                .AsBag();

            mapping.HasManyToMany<Ethnicity>(x => x.SameEthnicitiesFrom2)
                .Table("PRF_SameEthnicity")
                .ParentKeyColumn("Ethnicity2ID")
                .ChildKeyColumn("Ethnicity1ID")
                .AsBag();
        }
    }
}
