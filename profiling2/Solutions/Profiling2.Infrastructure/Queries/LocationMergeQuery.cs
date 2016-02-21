using Profiling2.Domain.Contracts.Queries;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class LocationMergeQuery : NHibernateQuery, ILocationMergeQuery
    {
        public void MergeLocations(int toKeepId, int toDeleteId)
        {
            string sql = @"
                UPDATE PRF_Event 
                SET LocationID = :toKeepId
                WHERE LocationID = :toDeleteId
            ";
            Session.CreateSQLQuery(sql).SetInt32("toKeepId", toKeepId).SetInt32("toDeleteId", toDeleteId).ExecuteUpdate();

            sql = @"
                UPDATE PRF_Career 
                SET LocationID = :toKeepId
                WHERE LocationID = :toDeleteId
            ";
            Session.CreateSQLQuery(sql).SetInt32("toKeepId", toKeepId).SetInt32("toDeleteId", toDeleteId).ExecuteUpdate();

            sql = @"
                UPDATE PRF_UnitLocation 
                SET LocationID = :toKeepId
                WHERE LocationID = :toDeleteId
            ";
            Session.CreateSQLQuery(sql).SetInt32("toKeepId", toKeepId).SetInt32("toDeleteId", toDeleteId).ExecuteUpdate();
        }
    }
}
