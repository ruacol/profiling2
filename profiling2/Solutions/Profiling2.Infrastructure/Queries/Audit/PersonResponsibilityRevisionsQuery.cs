using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NHibernate.Envers;
using NHibernate.Envers.Query;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public class PersonResponsibilityRevisionsQuery : NHibernateAuditQuery, IPersonAuditable<PersonResponsibility>
    {
        public new CompareLogic CompareLogic
        {
            get
            {
                base.CompareLogic.Config.MembersToInclude.AddRange(new List<string>()
                    {
                        "PersonResponsibility", "Person", "Event", "PersonResponsibilityType",
                        "Commentary", "Archive", "Notes"
                    });
                return base.CompareLogic;
            }
        }

        public IList<object[]> GetRawRevisions(Person person)
        {
            return AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(PersonResponsibility), false, true)
                .Add(AuditEntity.Property("Person").Eq(person))
                .GetResultList<object[]>();
        }
    }
}
