using System.Collections.Generic;

namespace Profiling2.Domain.Contracts.Search
{
    public interface IPersonSearcher
    {
        IList<LuceneSearchResult> Search(string term, int numResults, bool includeRestrictedProfiles);
    }
}
