using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profiling2.Domain.Contracts.Search
{
    public interface IEventSearcher
    {
        IList<LuceneSearchResult> Search(string term, int numResults, string sortField, bool descending);

        IList<LuceneSearchResult> Search(string term, DateTime? start, DateTime? end, int numResults, string sortField, bool descending);
    }
}
