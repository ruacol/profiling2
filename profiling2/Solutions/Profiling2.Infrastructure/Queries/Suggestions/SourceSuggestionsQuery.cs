using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Profiling2.Domain.Contracts.Queries.Suggestions;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Suggestions
{
    public class SourceSuggestionsQuery : NHibernateQuery, ISourceSuggestionsQuery
    {
        public IList<EventSource> GetEventSources(Person p)
        {
            Event e = null;
            EventSource es = null;

            var subquery = QueryOver.Of<PersonSource>()
                .Where(x => x.Person == p)
                .And(x => !x.Archive)
                .Select(x => x.Source);

            return Session.QueryOver<EventSource>(() => es)
                .JoinAlias(() => es.Event, () => e, JoinType.InnerJoin)
                .Where(() => !e.Archive)
                .And(() => !es.Archive)
                .WithSubquery.WhereProperty(() => es.Source).In(subquery)
                .List<EventSource>();
        }
    }
}
