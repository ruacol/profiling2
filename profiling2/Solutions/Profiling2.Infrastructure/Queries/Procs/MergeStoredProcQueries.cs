using NHibernate;
using Profiling2.Domain.Contracts.Queries.Procs;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Procs
{
    public class MergeStoredProcQueries : NHibernateQuery, IMergeStoredProcQueries
    {
        public int MergePersons(int toKeepPersonId, int toDeletePersonId, string userId, bool isProfilingChange)
        {
            return Session.GetNamedQuery("PRF_SP_PersonMerge_NHibernate")
                .SetParameter("ToKeepPersonID", toKeepPersonId, NHibernateUtil.Int64)
                .SetParameter("ToDeletePersonID", toDeletePersonId, NHibernateUtil.Int64)
                .SetParameter("UserID", userId, NHibernateUtil.String)
                .SetParameter("IsProfilingChange", isProfilingChange, NHibernateUtil.Boolean)
                .UniqueResult<int>();
        }

        public int MergeUnits(int toKeepUnitId, int toDeleteUnitId, string userId, bool isProfilingChange)
        {
            return Session.GetNamedQuery("PRF_SP_UnitMerge_NHibernate")
                .SetParameter("ToKeepUnitID", toKeepUnitId, NHibernateUtil.Int64)
                .SetParameter("ToDeleteUnitID", toDeleteUnitId, NHibernateUtil.Int64)
                .SetParameter("UserID", userId, NHibernateUtil.String)
                .SetParameter("IsProfilingChange", isProfilingChange, NHibernateUtil.Boolean)
                .UniqueResult<int>();
        }

        public int MergeEvents(int toKeepEventId, int toDeleteEventId, string userId, bool isProfilingChange)
        {
            return Session.GetNamedQuery("PRF_SP_EventMerge_NHibernate")
                .SetParameter("ToKeepEventID", toKeepEventId, NHibernateUtil.Int64)
                .SetParameter("ToDeleteEventID", toDeleteEventId, NHibernateUtil.Int64)
                .SetParameter("UserID", userId, NHibernateUtil.String)
                .SetParameter("IsProfilingChange", isProfilingChange, NHibernateUtil.Boolean)
                .UniqueResult<int>();
        }
    }
}
