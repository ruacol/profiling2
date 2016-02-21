using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profiling2.Domain.Contracts.Search
{
    public interface ISourceSearcher
    {
        IList<LuceneSearchResult> GetSourcesLikeThis(int sourceId, int numResults);

        int GetMaxSourceID();

        IList<LuceneSearchResult> Search(string term, string prefix, bool usePrefixQuery, DateTime? start, DateTime? end,
            int numResults, bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners, string sortField, bool descending);

        /// <summary>
        /// Run search, but get facet counts only.
        /// </summary>
        /// <param name="term"></param>
        /// <param name="prefix"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="numResults"></param>
        /// <param name="includeRestrictedSources"></param>
        /// <returns></returns>
        IDictionary<IDictionary<string, string>, long> SearchGetFacets(string term, string prefix, bool usePrefixQuery, DateTime? start, DateTime? end,
            int numResults, bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners);

        IList<LuceneSearchResult> AllSourcesWithCaseNumbers(string term, int numResults, bool includeRestrictedSources, string sortField, bool descending);
    }
}
