using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf.Sources;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class SourceLogQueries : NHibernateQuery, ISourceLogQueries
    {
        public IList<SourceIndexLog> GetSourceIndexLogsWithErrors()
        {
            return Session.QueryOver<SourceIndexLog>()
                .WhereRestrictionOn(x => x.LogSummary).IsNotNull
                .List();
        }

        public int CountSourceIndexLogs()
        {
            return Session.QueryOver<SourceIndexLog>().RowCount();
        }

        public int CountSourceIndexLogErrors()
        {
            return Session.QueryOver<SourceIndexLog>()
                .WhereRestrictionOn(x => x.LogSummary).IsNotNull
                .AndRestrictionOn(x => x.Log).IsNotNull
                .RowCount();
        }

        public SourceIndexLog GetSourceIndexLog(ISession session, int sourceId)
        {
            ISession thisSession = session == null ? Session : session;
            IList<SourceIndexLog> list = thisSession.QueryOver<SourceIndexLog>()
                .Where(x => x.SourceID == sourceId)
                .OrderBy(x => x.DateTime).Desc
                .List<SourceIndexLog>();

            if (list != null && list.Count > 0)
                return list[0];
            return null;
        }

        public void SaveSourceIndexLog(ISession session, SourceIndexLog sil)
        {
            ISession thisSession = session == null ? Session : session;
            thisSession.SaveOrUpdate(sil);
        }

        public IList<SourceLog> GetSourceLogsWithErrors()
        {
            return Session.QueryOver<SourceLog>()
                .WhereRestrictionOn(x => x.LogSummary).IsNotNull
                .List();
        }

        public int CountSourceLogs()
        {
            //return Session.QueryOver<SourceLog>()
            //    .RowCount();

            return Session.CreateSQLQuery(@"
                SELECT COUNT(DISTINCT l.SourceID) 
                FROM PRF_SourceLog l, PRF_Source s 
                WHERE l.SourceID = s.SourceID 
                AND s.Archive = 0
            ").UniqueResult<int>();
        }

        public int CountSourceLogsWithPasswordErrors()
        {
            return Session.CreateSQLQuery(@"
                SELECT COUNT(DISTINCT l.SourceID) 
                FROM PRF_SourceLog l, PRF_Source s 
                WHERE l.SourceID = s.SourceID 
                AND s.Archive = 0
                AND l.LogSummary LIKE '%assword%'
            ").UniqueResult<int>();
        }

        public bool CheckPreviewProblem(int sourceId)
        {
            IList<SourceLog> list = Session.QueryOver<SourceLog>()
                .WhereRestrictionOn(x => x.LogSummary).IsNotNull
                .And(x => x.SourceID == sourceId)
                .List();
            return list != null && list.Count > 0;
        }

        public SourceLog GetSourceLog(IStatelessSession session, int sourceId)
        {
            IList<SourceLog> list = session.QueryOver<SourceLog>()
                .Where(x => x.SourceID == sourceId)
                .List<SourceLog>();

            if (list != null && list.Count > 0)
                return list[0];
            return null;
        }

        public void InsertSourceLog(IStatelessSession session, SourceLog sourceLog)
        {
            session.Insert(sourceLog);
        }
    }
}
