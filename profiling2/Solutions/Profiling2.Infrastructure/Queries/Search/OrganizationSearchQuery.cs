using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Queries.Search;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Organizations;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class OrganizationSearchQuery : NHibernateQuery, IOrganizationSearchQuery
    {
        // Using COLLATE negates the effects of special characters like é and à
        public IList<Organization> GetResults(string term)
        {
            //var qo = Session.QueryOver<Organization>();
            //if (!string.IsNullOrEmpty(term))
            //    return qo.Where(Restrictions.On<Organization>(x => x.OrgShortName).IsLike("%" + term + "%")
            //        || Restrictions.On<Organization>(x => x.OrgLongName).IsLike("%" + term + "%"))
            //        .OrderBy(x => x.OrgLongName).Asc
            //        .Take(50)
            //        .List<Organization>();
            //else
            //    return new List<Organization>();

            // Sort Alphabetically with insensitive LIKE
//            if (!string.IsNullOrEmpty(term))
//                return Session.CreateCriteria<Organization>()
//                    .Add(Expression.Sql(@"
//                        OrgShortName LIKE ? COLLATE Latin1_general_CI_AI
//                        OR OrgLongName LIKE ? COLLATE Latin1_general_CI_AI
//                    ", new string[] { "%" + term + "%", "%" + term + "%" }, new IType[] { NHibernateUtil.String, NHibernateUtil.String }))
//                     .AddOrder(Order.Asc("OrgLongName"))
//                     .SetMaxResults(50)
//                    .List<Organization>();
//            else
//                return new List<Organization>();

            // Sorted by Career popularity with insensitive LIKE
            // How to measure popularity?  Careers, Units, unique Careers, unique Units?
            if (!string.IsNullOrEmpty(term))
                return Session.CreateCriteria<Career>()
                    .CreateAlias("Organization", "o", JoinType.RightOuterJoin)
                    .Add(Expression.Sql(@"
                        OrgShortName LIKE ? COLLATE Latin1_general_CI_AI
                        OR OrgLongName LIKE ? COLLATE Latin1_general_CI_AI
                    ", new string[] { "%" + term + "%", "%" + term + "%" }, new IType[] { NHibernateUtil.String, NHibernateUtil.String }))
                    .SetProjection(Projections.ProjectionList()
                        .Add(Projections.Property("o.Id"), "Id")
                        .Add(Projections.Property("o.OrgShortName"), "OrgShortName")
                        .Add(Projections.Property("o.OrgLongName"), "OrgLongName")
                        .Add(Projections.Property("o.Archive"), "Archive")
                        .Add(Projections.Property("o.Notes"), "Notes")
                        .Add(Projections.Count("o.Id"))
                        .Add(Projections.GroupProperty("o.Id"))
                        .Add(Projections.GroupProperty("o.OrgShortName"))
                        .Add(Projections.GroupProperty("o.OrgLongName"))
                        .Add(Projections.GroupProperty("o.Archive"))
                        .Add(Projections.GroupProperty("o.Notes"))
                    )
                    .AddOrder(Order.Desc(Projections.Count("o.Id")))
                    .SetMaxResults(50)
                    .SetResultTransformer(Transformers.AliasToBean<Organization>())
                    .List<Organization>();
            else
                return new List<Organization>();
        }
    }
}
