using System;
using System.Collections.Generic;
using SharpArch.NHibernate;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Contracts.Queries;

namespace Profiling2.Infrastructure.Queries
{
    public class AdminSourceSearchQuery : NHibernateQuery, IAdminSourceSearchQuery
    {
        // TODO the criteria for finding previous searches is a little inflexible here.
        public IList<int> GetAdminSourceSearchIds(AdminSourceSearch adminSourceSearch)
        {
            var qo = Session.QueryOver<AdminSourceSearch>();
            if (!string.IsNullOrEmpty(adminSourceSearch.AndSearchTerms))
            {
                qo.Where(x => x.AndSearchTerms == adminSourceSearch.AndSearchTerms
                    || x.AndSearchTerms == string.Join(" ", adminSourceSearch.AndSearchTermsQuoted));
                if (!string.IsNullOrEmpty(adminSourceSearch.ExcludeSearchTerms))
                    qo.And(x => x.ExcludeSearchTerms == adminSourceSearch.ExcludeSearchTerms
                        || x.ExcludeSearchTerms == string.Join(" ", adminSourceSearch.ExcludeSearchTermsQuoted));

                return qo.Select(x => x.Id).List<int>();
            }
            else
            {
                // OrSearchTerms still ignored.
                return new List<int>();
            }
        }
    }
}
