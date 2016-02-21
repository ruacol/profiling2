using System;
using System.Collections.Generic;
using NHibernate;

namespace Profiling2.Domain.Contracts.Queries.Stats
{
    public interface ISourceStatisticsQueries
    {
        int GetSourceCount(bool archived);

        Int64 GetTotalSize();

        Int64 GetTotalArchivedSize();

        IList<object[]> GetSourceImportsByDay();

        DateTime GetLastAdminSourceImportDate();

        int GetSourceCountByFolder(string folder, ISession session);

        DateTime GetLastDateByFolder(string folder, ISession session);
    }
}
