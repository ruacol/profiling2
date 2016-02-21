using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Profiling2.Domain.Contracts.Queries.Suggestions;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;
using Profiling2.Domain.Prf.Responsibility;

namespace Profiling2.Infrastructure.Queries.Suggestions
{
    public class PersonRelationshipSuggestionsQuery : NHibernateQuery, IPersonRelationshipSuggestionsQuery
    {
        public IList<PersonRelationship> GetPersonRelationships(Person p)
        {
            PersonRelationship rel = null;
            PersonRelationshipType t = null;

            var subquery = QueryOver.Of<PersonResponsibility>()
                .Where(x => x.Person != p)
                .And(x => !x.Archive)
                .Select(x => x.Person);

            return Session.QueryOver<PersonRelationship>(() => rel)
                .JoinAlias(() => rel.PersonRelationshipType, () => t, JoinType.InnerJoin)
                .Where(() => !rel.Archive)
                .And(Restrictions.Disjunction()
                        .Add(() => rel.SubjectPerson == p)
                        .Add(() => rel.ObjectPerson == p)
                    )
                .AndRestrictionOn(() => t.Code).IsIn(new string[]
                {
                    PersonRelationshipType.CODE_ACTED_TOGETHER_WITH,
                    PersonRelationshipType.CODE_BELONGED_TO_THE_SAME_GROUP_AS,
                    PersonRelationshipType.CODE_FOUGHT_IN_THE_SAME_GROUP_AS,
                    PersonRelationshipType.CODE_FOUGHT_WITH,
                    PersonRelationshipType.CODE_IS_A_SUBORDINATE_OF,
                    PersonRelationshipType.CODE_IS_A_SUPERIOR_TO,
                    PersonRelationshipType.CODE_IS_THE_BODYGUARD_OF,
                    PersonRelationshipType.CODE_IS_THE_DEPUTY_OF,
                    PersonRelationshipType.CODE_PROVIDED_WEAPONS_AND_AMMUNITION_TO
                })
                .And(Restrictions.Disjunction()
                        .Add(Subqueries.WhereProperty(() => rel.SubjectPerson).In(subquery))
                        .Add(Subqueries.WhereProperty(() => rel.ObjectPerson).In(subquery))
                    )
                .List();
        }
    }
}
