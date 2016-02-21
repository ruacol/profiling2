using System.Collections.Generic;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;

namespace Profiling2.Domain.Contracts.Queries.Search
{
    public interface IEventSearchQuery
    {
        IList<Event> GetResults(string term);

        IList<Tag> SearchTags(string term);
    }
}
