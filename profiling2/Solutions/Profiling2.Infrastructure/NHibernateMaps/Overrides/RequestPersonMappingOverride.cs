using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Scr.Person;
using FluentNHibernate.Automapping;
using Profiling2.Domain.Scr.PersonEntity;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class RequestPersonMappingOverride : IAutoMappingOverride<RequestPerson>
    {
        public void Override(AutoMapping<RequestPerson> mapping)
        {
            mapping.HasMany<RequestPersonHistory>(x => x.RequestPersonHistories)
                .KeyColumn("RequestPersonID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<ScreeningRequestPersonEntity>(x => x.ScreeningRequestPersonEntities)
                .KeyColumn("RequestPersonID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
