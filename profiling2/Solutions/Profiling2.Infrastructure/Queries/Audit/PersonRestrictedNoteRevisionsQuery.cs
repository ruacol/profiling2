using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NHibernate.Envers;
using NHibernate.Envers.Query;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public class PersonRestrictedNoteRevisionsQuery : NHibernateAuditQuery, IPersonAuditable<PersonRestrictedNote>
    {
        public new CompareLogic CompareLogic
        {
            get
            {
                base.CompareLogic.Config.MembersToInclude.AddRange(new List<string>()
                    {
                        "Note"
                    });
                return base.CompareLogic;
            }
        }

        public IList<object[]> GetRawRevisions(Person person)
        {
            return AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(PersonRestrictedNote), false, true)
                .Add(AuditEntity.Property("Person").Eq(person))
                .GetResultList<object[]>();
        }
    }
}
