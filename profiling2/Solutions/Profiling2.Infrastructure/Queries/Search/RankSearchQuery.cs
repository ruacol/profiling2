using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using Profiling2.Domain.Contracts.Queries.Search;
using Profiling2.Domain.Prf.Careers;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class RankSearchQuery : NHibernateQuery, IRankSearchQuery
    {
        public IList<Rank> GetResults(string term)
        {
            //var qo = Session.QueryOver<Rank>();
            //if (!string.IsNullOrEmpty(term))
            //    return qo.Where(Restrictions.On<Rank>(x => x.RankName).IsLike("%" + term + "%"))
            //        .OrderBy(x => x.RankName).Asc
            //        .Take(50)
            //        .List<Rank>();
            //else
            //    return new List<Rank>();

            // Alphabetically ordered with insensitive LIKE
            //if (!string.IsNullOrEmpty(term))
            //    return Session.CreateCriteria<Rank>()
            //        .Add(Expression.Sql("RankName LIKE ? COLLATE Latin1_general_CI_AI", "%" + term + "%", NHibernateUtil.String))
            //        .AddOrder(Order.Asc("RankName"))
            //        .SetMaxResults(50)
            //        .List<Rank>();
            //else
            //    return new List<Rank>();

            // Sorted by Career popularity with insensitive LIKE
            if (!string.IsNullOrEmpty(term))
                return Session.CreateCriteria<Career>()
                    .CreateAlias("Rank", "r", JoinType.RightOuterJoin)
                    .Add(Expression.Sql(@"
                        RankName LIKE ? COLLATE Latin1_general_CI_AI 
                        OR RankNameFr LIKE ? COLLATE Latin1_general_CI_AI",
                        new string[] { "%" + term + "%", "%" + term + "%" }, new IType[] { NHibernateUtil.String, NHibernateUtil.String }))
                    .SetProjection(Projections.ProjectionList()
                        .Add(Projections.Property("r.Id"), "Id")
                        .Add(Projections.Property("r.RankName"), "RankName")
                        .Add(Projections.Property("r.RankNameFr"), "RankNameFr")
                        .Add(Projections.Property("r.Archive"), "Archive")
                        .Add(Projections.Property("r.Notes"), "Notes")
                        .Add(Projections.Count("r.Id"))
                        .Add(Projections.GroupProperty("r.Id"))
                        .Add(Projections.GroupProperty("r.RankName"))
                        .Add(Projections.GroupProperty("r.RankNameFr"))
                        .Add(Projections.GroupProperty("r.Archive"))
                        .Add(Projections.GroupProperty("r.Notes"))
                    )
                    .AddOrder(Order.Desc(Projections.Count("r.Id")))
                    .SetMaxResults(50)
                    .SetResultTransformer(Transformers.AliasToBean<Rank>())
                    .List<Rank>();
            else
                return new List<Rank>();
        }
    }
}
