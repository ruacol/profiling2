using System.Collections.Generic;
using Profiling2.Domain.Contracts.Queries.Search;
using Profiling2.Domain.Prf.Sources;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Search
{
    public class SourceAuthorSearchQuery : NHibernateQuery, ISourceAuthorSearchQuery
    {
        public IList<SourceAuthor> GetSourceAuthorsLike(string term)
        {
            return Session.QueryOver<SourceAuthor>()
                .WhereRestrictionOn(x => x.Author).IsInsensitiveLike("%" + term + "%")
                .Take(50)
                .List<SourceAuthor>();
        }
    }
}
