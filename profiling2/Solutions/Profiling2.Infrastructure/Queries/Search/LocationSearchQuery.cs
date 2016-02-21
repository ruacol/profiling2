using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Queries.Search;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Careers;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class LocationSearchQuery : NHibernateQuery, ILocationSearchQuery
    {
        protected Location locationAlias;
        protected Region regionAlias;

        public IList<Location> GetResults(string term)
        {
            //var qo = Session.QueryOver<Location>(() => locationAlias)
            //    .JoinAlias(() => locationAlias.Region, () => regionAlias, JoinType.LeftOuterJoin);
            //if (!string.IsNullOrEmpty(term))
            //    return qo.Where(Restrictions.On(() => locationAlias.LocationName).IsLike("%" + term + "%")
            //            || Restrictions.On(() => locationAlias.Province).IsLike("%" + term + "%")
            //            || Restrictions.On(() => locationAlias.Territory).IsLike("%" + term + "%")
            //            || Restrictions.On(() => locationAlias.Town).IsLike("%" + term + "%")
            //            || Restrictions.On(() => regionAlias.RegionName).IsLike("%" + term + "%"))
            //        .OrderBy(() => locationAlias.LocationName).Asc
            //        .List<Location>();
            //else
            //    return new List<Location>();

            // Sort Alphabetically with insensitive LIKE
//            if (!string.IsNullOrEmpty(term))
//            {
//                string searchTerm = "%" + term + "%";
//                return Session.CreateCriteria<Location>()
//                    .Add(Expression.Sql(@"
//                        LocationName LIKE ? COLLATE Latin1_general_CI_AI
//                     ", new string[] { searchTerm },
//                      new IType[] { NHibernateUtil.String }))
//                     .AddOrder(Order.Asc("LocationName"))
//                     .SetMaxResults(100)
//                    .List<Location>();
//            }
//            else
//                return new List<Location>();

            // Sorted by Career popularity with insensitive LIKE
            // How to measure popularity?  Locations are joined to Careers, Organizations, Persons...
            if (!string.IsNullOrEmpty(term))
                return Session.CreateCriteria<Career>()
                    .CreateAlias("Location", "l", JoinType.RightOuterJoin)
                    .Add(Expression.Sql("LocationName LIKE ? COLLATE Latin1_general_CI_AI", "%" + term + "%", NHibernateUtil.String))
                    .SetProjection(Projections.ProjectionList()
                        .Add(Projections.Property("l.Id"), "Id")
                        .Add(Projections.Property("l.LocationName"), "LocationName")
                        .Add(Projections.Property("l.Territory"), "Territory")
                        .Add(Projections.Property("l.Town"), "Town")
                        .Add(Projections.Property("l.Region"), "Region")
                        .Add(Projections.Property("l.Province"), "Province")
                        .Add(Projections.Property("l.Latitude"), "Latitude")
                        .Add(Projections.Property("l.Longitude"), "Longitude")
                        .Add(Projections.Property("l.Archive"), "Archive")
                        .Add(Projections.Property("l.Notes"), "Notes")
                        .Add(Projections.Count("l.Id"))
                        .Add(Projections.GroupProperty("l.Id"))
                        .Add(Projections.GroupProperty("l.LocationName"))
                        .Add(Projections.GroupProperty("l.Territory"))
                        .Add(Projections.GroupProperty("l.Town"))
                        .Add(Projections.GroupProperty("l.Region"))
                        .Add(Projections.GroupProperty("l.Province"))
                        .Add(Projections.GroupProperty("l.Latitude"))
                        .Add(Projections.GroupProperty("l.Longitude"))
                        .Add(Projections.GroupProperty("l.Archive"))
                        .Add(Projections.GroupProperty("l.Notes"))
                    )
                    .AddOrder(Order.Desc(Projections.Count("l.Id")))
                    .SetMaxResults(50)
                    .SetResultTransformer(Transformers.AliasToBean<Location>())
                    .List<Location>();
            else
                return new List<Location>();
        }

        public IList<Location> GetLocationsWithCoords()
        {
            return Session.QueryOver<Location>()
                .WhereRestrictionOn(x => x.Latitude).IsNotNull
                .AndRestrictionOn(x => x.Longitude).IsNotNull
                .List();
        }
    }
}
