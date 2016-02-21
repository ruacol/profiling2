using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf.Events;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class EventQueries : NHibernateQuery, IEventQueries
    {
        public IList<Event> GetUnapprovedEvents()
        {
            return this.Session.QueryOver<Event>()
                .WhereRestrictionOn(x => x.EventApprovals).IsEmpty
                .List<Event>();
        }

        public IList<Event> GetAllEvents(ISession session)
        {
            ISession thisSession = session == null ? Session : session;
            return thisSession.QueryOver<Event>().List();
        }
    }
}
