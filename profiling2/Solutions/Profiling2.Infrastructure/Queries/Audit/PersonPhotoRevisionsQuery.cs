using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NHibernate.Envers;
using NHibernate.Envers.Query;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public class PersonPhotoRevisionsQuery : NHibernateAuditQuery, IPersonAuditable<PersonPhoto>
    {
        public new CompareLogic CompareLogic
        {
            get
            {
                base.CompareLogic.Config.MembersToInclude.AddRange(new List<string>()
                    {
                        "PersonPhoto", "Person", "Photo", "Archive", "Notes"
                    });
                return base.CompareLogic;
            }
        }

        public IList<object[]> GetRawRevisions(Person person)
        {
            return AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(PersonPhoto), false, true)
                .Add(AuditEntity.Property("Person").Eq(person))
                .GetResultList<object[]>();
        }
    }
}
