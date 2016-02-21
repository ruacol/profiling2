using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries.Search;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Units;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class UnitSearchQuery : NHibernateQuery, IUnitSearchQuery
    {
        public IList<Operation> GetOperationsLike(string term)
        {
            return Session.QueryOver<Operation>()
                .WhereRestrictionOn(x => x.Name).IsInsensitiveLike("%" + term + "%")
                .And(x => !x.Archive)
                .Take(50)
                .List<Operation>();
        }

        public IList<Unit> GetResults(string term)
        {
            //var qo = Session.QueryOver<Unit>();
            //if (!string.IsNullOrEmpty(term))
            //    return qo.Where(Restrictions.On<Unit>(x => x.UnitName).IsLike("%" + term + "%"))
            //        .OrderBy(x => x.UnitName).Asc
            //        .Take(50)
            //        .List<Unit>();
            //else
            //    return new List<Unit>();

            if (!string.IsNullOrEmpty(term))
                return Session.CreateCriteria<Career>()
                    .CreateAlias("Unit", "u", JoinType.RightOuterJoin)
                    .Add(Expression.Sql("UnitName LIKE ? COLLATE Latin1_general_CI_AI", "%" + term + "%", NHibernateUtil.String))
                    .SetProjection(Projections.ProjectionList()
                        .Add(Projections.Property("u.Id"), "Id")
                        .Add(Projections.Property("u.UnitName"), "UnitName")
                        .Add(Projections.Property("u.BackgroundInformation"), "BackgroundInformation")
                        .Add(Projections.Property("u.Archive"), "Archive")
                        .Add(Projections.Property("u.Notes"), "Notes")
                        .Add(Projections.Property("u.Organization"), "Organization")
                        .Add(Projections.Count("u.Id"))
                        .Add(Projections.GroupProperty("u.Id"))
                        .Add(Projections.GroupProperty("u.UnitName"))
                        .Add(Projections.GroupProperty("u.BackgroundInformation"))
                        .Add(Projections.GroupProperty("u.Archive"))
                        .Add(Projections.GroupProperty("u.Notes"))
                        .Add(Projections.GroupProperty("u.Organization"))
                    )
                    .AddOrder(Order.Desc(Projections.Count("u.Id")))
                    .SetMaxResults(50)
                    .SetResultTransformer(Transformers.AliasToBean<Unit>())
                    .List<Unit>();
            else
                return new List<Unit>();
        }
    }
}
