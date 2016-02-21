using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profiling2.Domain.Contracts.Search
{
    public interface IScreeningResponseSearcher
    {
        IList<LuceneSearchResult> Search(string term, string screeningEntityName, int numResults);
    }
}
