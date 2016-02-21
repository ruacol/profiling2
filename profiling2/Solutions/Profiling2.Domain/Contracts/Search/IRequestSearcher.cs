using System.Collections.Generic;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Contracts.Search
{
    public interface IRequestSearcher
    {
        /// <summary>
        /// Search screening requests.
        /// </summary>
        /// <param name="term"></param>
        /// <param name="numResults"></param>
        /// <param name="user">If not null, will filter results as if user is a screening request initiator.</param>
        /// <param name="sortField"></param>
        /// <param name="descending"></param>
        /// <returns></returns>
        IList<LuceneSearchResult> Search(string term, int numResults, AdminUser user, string sortField, bool descending);
    }
}
