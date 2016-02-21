using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface ISourceLogQueries
    {
        IList<SourceIndexLog> GetSourceIndexLogsWithErrors();

        int CountSourceIndexLogs();

        int CountSourceIndexLogErrors();

        SourceIndexLog GetSourceIndexLog(ISession session, int sourceId);

        void SaveSourceIndexLog(ISession session, SourceIndexLog sil);

        IList<SourceLog> GetSourceLogsWithErrors();

        int CountSourceLogs();

        int CountSourceLogsWithPasswordErrors();

        /// <summary>
        /// Return true if given sourceId has been processed AND has an error stored in PRF_SourceLog.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        bool CheckPreviewProblem(int sourceId);

        SourceLog GetSourceLog(IStatelessSession session, int sourceId);

        void InsertSourceLog(IStatelessSession session, SourceLog sourceLog);
    }
}
