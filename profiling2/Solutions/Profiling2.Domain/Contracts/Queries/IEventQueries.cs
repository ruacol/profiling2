using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.Prf.Events;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IEventQueries
    {
        IList<Event> GetUnapprovedEvents();

        IList<Event> GetAllEvents(ISession session);
    }
}
