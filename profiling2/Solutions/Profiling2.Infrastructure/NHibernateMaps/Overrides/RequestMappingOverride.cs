using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Scr;
using FluentNHibernate.Automapping;
using Profiling2.Domain.Scr.Proposed;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonEntity;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class RequestMappingOverride : IAutoMappingOverride<Request>
    {
        public void Override(AutoMapping<Request> mapping)
        {
            mapping.HasMany<RequestHistory>(x => x.RequestHistories)
                .KeyColumn("RequestID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<RequestProposedPerson>(x => x.ProposedPersons)
                .KeyColumn("RequestID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<RequestPerson>(x => x.Persons)
                .KeyColumn("RequestID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<ScreeningRequestEntityResponse>(x => x.ScreeningRequestEntityResponses)
                .KeyColumn("RequestID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
