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
    public class RoleSearchQuery : NHibernateQuery, IRoleSearchQuery
    {
        public IList<Role> GetResults(string term)
        {
            //var qo = Session.QueryOver<Role>();
            //if (!string.IsNullOrEmpty(term))
            //    return qo.Where(Restrictions.On<Role>(x => x.RoleName).IsLike("%" + term + "%"))
            //        .OrderBy(x => x.RoleName).Asc
            //        .Take(50)
            //        .List<Role>();
            //else
            //    return new List<Role>();

            // Alphabetically ordered with insensitive LIKE
            //if (!string.IsNullOrEmpty(term))
            //    return Session.CreateCriteria<Role>()
            //        .Add(Expression.Sql("RoleName LIKE ? COLLATE Latin1_general_CI_AI", "%" + term + "%", NHibernateUtil.String))
            //        .AddOrder(Order.Asc("RoleName"))
            //        .SetMaxResults(50)
            //        .List<Role>();
            //else
            //    return new List<Role>();

            // Sorted by Career popularity with insensitive LIKE
            if (!string.IsNullOrEmpty(term))
                return Session.CreateCriteria<Career>()
                    .CreateAlias("Role", "r", JoinType.RightOuterJoin)
                    .Add(Expression.Sql(@"
                        RoleName LIKE ? COLLATE Latin1_general_CI_AI 
                        OR RoleNameFr LIKE ? COLLATE Latin1_general_CI_AI",
                        new string[] { "%" + term + "%", "%" + term + "%" }, new IType[] { NHibernateUtil.String, NHibernateUtil.String }))
                    .SetProjection(Projections.ProjectionList()
                        .Add(Projections.Property("r.Id"), "Id")
                        .Add(Projections.Property("r.RoleName"), "RoleName")
                        .Add(Projections.Property("r.RoleNameFr"), "RoleNameFr")
                        .Add(Projections.Property("r.Archive"), "Archive")
                        .Add(Projections.Property("r.Notes"), "Notes")
                        .Add(Projections.Count("r.Id"))
                        .Add(Projections.GroupProperty("r.Id"))
                        .Add(Projections.GroupProperty("r.RoleName"))
                        .Add(Projections.GroupProperty("r.RoleNameFr"))
                        .Add(Projections.GroupProperty("r.Archive"))
                        .Add(Projections.GroupProperty("r.Notes"))
                    )
                    .AddOrder(Order.Desc(Projections.Count("r.Id")))
                    .SetMaxResults(50)
                    .SetResultTransformer(Transformers.AliasToBean<Role>())
                    .List<Role>();
            else
                return new List<Role>();
        }
    }
}
