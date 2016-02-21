using System.Collections.Generic;
using Profiling2.Domain.Prf.Careers;

namespace Profiling2.Domain.Contracts.Queries.Search
{
    public interface IRankSearchQuery
    {
        IList<Rank> GetResults(string term);
    }
}
