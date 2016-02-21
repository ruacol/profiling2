using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Profiling2.Domain.Contracts.Queries.Suggestions;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Suggestions
{
    public class CareerLocationSuggestionsQuery : NHibernateQuery, ICareerLocationSuggestionsQuery
    {
        public IList<Event> GetEvents(Person p)
        {
            var unknownSubquery = QueryOver.Of<AdminUnknown>()
                .Where(x => !x.Archive)
                .Select(x => x.UnknownValue);

            var locationSubquery = QueryOver.Of<Career>()
                .Where(x => x.Person == p)
                .And(x => !x.Archive)
                .Select(x => x.Location);

            Career c = null;
            Location cl = null;

            var locationTerritorySubquery = QueryOver.Of<Location>(() => cl)
                .JoinAlias(() => cl.Careers, () => c, JoinType.InnerJoin)
                .Where(() => c.Person == p)
                .And(() => !c.Archive)
                .WithSubquery.WhereProperty(() => cl.LocationName).NotIn(unknownSubquery)
                .Select(x => x.Territory);

            Event e = null;
            Location l = null;

            return Session.QueryOver<Event>(() => e)
                .JoinAlias(() => e.Location, () => l, JoinType.InnerJoin)
                .Where(() => !e.Archive)
                .And(() => !l.Archive)
                .WithSubquery.WhereProperty(() => l.LocationName).NotIn(unknownSubquery)
                .And(Restrictions.Disjunction()
                        .Add(Subqueries.WhereProperty(() => e.Location).In(locationSubquery))
                        .Add(Subqueries.WhereProperty(() => l.Territory).In(locationTerritorySubquery))
                    )
                .List<Event>();
        }
    }
}
