using System.Collections.Generic;
using NHibernate.Criterion;
using Profiling2.Domain.Contracts.Queries.Suggestions;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Suggestions
{
    public class EventRelationshipSuggestionsQuery : NHibernateQuery, IEventRelationshipSuggestionsQuery
    {
        public IList<EventRelationship> GetEventRelationships(Person p)
        {
            EventRelationship er = null;

            var subquery = QueryOver.Of<PersonResponsibility>()
                .Where(x => x.Person == p)
                .And(x => !x.Archive)
                .Select(x => x.Event);

            return Session.QueryOver<EventRelationship>(() => er)
                .Where(Restrictions.Disjunction()
                        .Add(Subqueries.WhereProperty(() => er.SubjectEvent).In(subquery))
                        .Add(Subqueries.WhereProperty(() => er.ObjectEvent).In(subquery))
                      )
                .And(x => !x.Archive)
                .List();
        }
    }
}
