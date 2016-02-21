using System.Collections.Generic;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Queries.Search
{
    public interface ISourceAuthorSearchQuery
    {
        IList<SourceAuthor> GetSourceAuthorsLike(string term);
    }
}
