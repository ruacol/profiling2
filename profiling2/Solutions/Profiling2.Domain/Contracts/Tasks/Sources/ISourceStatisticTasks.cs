using System;
using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.DTO;

namespace Profiling2.Domain.Contracts.Tasks.Sources
{
    public interface ISourceStatisticTasks
    {
        /// <summary>
        /// Retrieve the most recent import date as stored in the PRF_AdminSourceImport table.  This table is populated by the
        /// stored procedures used to import into PRF_Source.
        /// </summary>
        /// <returns></returns>
        DateTime GetLastAdminSourceImportDate();

        /// <summary>
        /// Get number of archived sources.
        /// </summary>
        /// <returns></returns>
        int GetNumArchived();

        /// <summary>
        /// Get total number of (non-archived) sources.
        /// </summary>
        /// <returns></returns>
        int GetSourceCount();

        /// <summary>
        /// Get size of PRF_Source.FileData as returned by SQL DATALENGTH function.  Includes archived sources.
        /// </summary>
        /// <returns></returns>
        Int64 GetTotalSourceSize();

        /// <summary>
        /// Get size of PRF_Source.FileData, for archived sources only, as returned by SQL DATALENGTH function.
        /// </summary>
        /// <returns></returns>
        Int64 GetTotalArchivedSourceSize();

        IList<object[]> GetSourceImportsByDay();

        /// <summary>
        /// Count non-archived sources and find the most recent FileDateTimeStamp in a list of folders.
        /// </summary>
        /// <param name="paths">List of prefixes to Source.SourcePath.</param>
        /// <returns></returns>
        IDictionary<string, SourceFolderDTO> GetSourceFolderCounts(IList<string> paths, ISession session);
    }
}
