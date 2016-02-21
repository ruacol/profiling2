using System.Collections.Generic;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Contracts.Queries.Search
{
    public interface ILocationSearchQuery
    {
        IList<Location> GetResults(string term);

        IList<Location> GetLocationsWithCoords();
    }
}
