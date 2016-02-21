using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Profiling2.Domain.Contracts.Queries.Suggestions;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Suggestions
{
    public class EventInSameLocationSuggestionsQuery : NHibernateQuery, IEventInSameLocationSuggestionsQuery
    {
        public IList<Event> GetEvents(Person p)
        {
            Event suggestedEvent = null;
            Location suggestedEventLocation = null;

            Event e = null;
            Location l = null;
            PersonResponsibility pr = null;

            Location l1 = null;
            Event e1 = null;
            PersonResponsibility pr1 = null;

            return Session.QueryOver<Event>(() => suggestedEvent)
                .JoinAlias(() => suggestedEvent.Location, () => suggestedEventLocation, JoinType.InnerJoin, 
                    Restrictions.Where(() => !suggestedEventLocation.Archive))
                .Where(Restrictions.Disjunction()
                    // either location of event for which person is responsible is identical
                    .Add(Subqueries.WhereExists(QueryOver.Of<PersonResponsibility>(() => pr)
                            .JoinAlias(() => pr.Event, () => e, JoinType.InnerJoin, 
                                Restrictions.Conjunction()
                                    .Add(Restrictions.Where(() => e.Id != suggestedEvent.Id))
                                    .Add(Restrictions.Where(() => !e.Archive))
                            )
                            .JoinAlias(() => e.Location, () => l, JoinType.InnerJoin,
                                Restrictions.Conjunction()
                                    .Add(Restrictions.Where(() => l.Id == suggestedEventLocation.Id))
                                    .Add(Restrictions.Where(() => !l.Archive))
                            )
                            .Where(() => pr.Person == p)
                            .And(() => !pr.Archive)
                            .Select(x => x.Event)
                    ))
                    // or it's territory is
                    // TODO slow due to string = expression
                    .Add(Subqueries.WhereExists(QueryOver.Of<PersonResponsibility>(() => pr1)
                            .JoinAlias(() => pr1.Event, () => e1, JoinType.InnerJoin,
                                Restrictions.Conjunction()
                                    .Add(Restrictions.Where(() => e1.Id != suggestedEvent.Id))
                                    .Add(Restrictions.Where(() => !e1.Archive))
                            )
                            .JoinAlias(() => e1.Location, () => l1, JoinType.InnerJoin,
                                Restrictions.Conjunction()
                                    .Add(Restrictions.Where(() => l1.Territory == suggestedEventLocation.Territory))
                                    .Add(Restrictions.On(() => l1.Territory).IsNotNull)
                                    .Add(Subqueries.WhereNotExists(QueryOver.Of<AdminUnknown>().Where(x => x.UnknownValue == l1.Territory).Select(x => x.Id)))
                                    .Add(Restrictions.Where(() => !l1.Archive))
                            )
                            .Where(() => pr1.Person == p)
                            .And(() => !pr1.Archive)
                            .Select(x => x.Event)
                    ))
                )
                .And(() => !suggestedEvent.Archive)
                .List();
        }
    }
}
