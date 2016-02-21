using System.Collections.Generic;
using System.Linq;
using KellermanSoftware.CompareNetObjects;
using NHibernate.Envers;
using NHibernate.Envers.Query;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public class PersonRelationshipRevisionsQuery : NHibernateAuditQuery, IPersonAuditable<PersonRelationship>
    {
        public new CompareLogic CompareLogic
        {
            get
            {
                base.CompareLogic.Config.MembersToInclude.AddRange(new List<string>()
                    {
                        "PersonRelationship", "SubjectPerson", "ObjectPerson", "PersonRelationshipType",
                        "DayOfStart", "MonthOfStart", "YearOfStart", "DayOfEnd", "MonthOfEnd", "YearOfEnd",
                        "Archive", "Notes"
                    });
                return base.CompareLogic;
            }
        }

        public IList<object[]> GetRawRevisions(Person person)
        {
            IList<object[]> asSubject = AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(PersonRelationship), false, true)
                .Add(AuditEntity.Property("SubjectPerson").Eq(person))
                .GetResultList<object[]>();

            IList<object[]> asObject = AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(PersonRelationship), false, true)
                .Add(AuditEntity.Property("ObjectPerson").Eq(person))
                .GetResultList<object[]>();

            return asSubject.Concat(asObject).ToList<object[]>();
        }
    }
}
