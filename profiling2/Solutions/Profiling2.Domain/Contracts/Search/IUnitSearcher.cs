using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profiling2.Domain.Contracts.Search
{
    public interface IUnitSearcher
    {
        IList<LuceneSearchResult> Search(string term, int numResults);
    }
}
