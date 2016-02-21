using System;
using System.Collections.Generic;
using System.IO;
using NHibernate;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Tasks.Sources
{
    public interface ISourceTasks
    {
        Source GetSource(int sourceId);

        Source GetSource(string name);

        IList<SourceDTO> GetSources(string hash);

        Source GetSource(IStatelessSession session, int id);

        IList<string> GetSourceAuthors(IStatelessSession session, int id);

        IList<string> GetSourceAuthors(int id);

        IList<string> GetSourceOwners(IStatelessSession session, int id);

        IList<string> GetSourceOwners(int id);

        SourceDTO GetSourceDTO(int id);

        Int64 GetSourceSize(int sourceId);

        IList<Source> GetSourcesByFilename(string term);

        IList<SourceDTO> GetAllSourceDTOs(IStatelessSession session, bool excludeBinaryIndexedSources, bool excludeSourceLogged);

        /// <summary>
        /// Retrieve all SourceDTOs in order to index them.  Does not include archived sources.
        /// </summary>
        /// <param name="excludeBinaryIndexedSources">Don't include sources that exist in PRF_SourceIndexLog (i.e. have been indexed for their binary content before).</param>
        /// <param name="excludeSourceLogged">Don't include sources that exist in PRF_SourceLog.</param>
        /// <returns></returns>
        IList<SourceDTO> GetAllSourceDTOs(bool excludeBinaryIndexedSources, bool excludeSourceLogged);

        Stream GetSourceData(int sourceId, bool hasOcrText);

        Source SaveSource(Source s);

        Source SaveSource(ISession session, Source s);

        /// <summary>
        /// Add or update a Source to Lucene index.  Populates several 'hors session' fields that are used only when indexing; these fields exist only
        /// to avoid loading the full entity tree (the arguments to this method are usually loaded with direct SQL or an IStatelessSession).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="uploadedByUserId"></param>
        /// <param name="authors"></param>
        /// <param name="owners"></param>
        /// <param name="jhroCaseNumber"></param>
        /// <param name="fileSize"></param>
        void IndexSource(Source s, string uploadedByUserId, IList<string> authors, IList<string> owners, string jhroCaseNumber, long fileSize);

        /// <summary>
        /// Presents a wrapper to sourceTasks.IndexSource() with simple arguments, which can be stored for background processing.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="uploadedBy"></param>
        /// <param name="authors"></param>
        /// <param name="owners"></param>
        /// <param name="caseNumber"></param>
        /// <param name="fileSize"></param>
        void IndexSourceQueueable(int sourceId, string uploadedBy, IList<string> authors, IList<string> owners, string caseNumber, long fileSize);

        void DeleteSource(int sourceId);

        int GetSearchTotal(bool canAccessRestricted, string searchName, string searchExt, string searchText, DateTime? start, DateTime? end, string authorSearchText);

        IList<SourceSearchResultDTO> GetPaginatedResults(bool canAccessRestricted, int iDisplayStart, int iDisplayLength,
            string searchName, string searchExt, string searchText, DateTime? start, DateTime? end, IList<int> adminSourceSearchIds,
            int iSortingCols, List<int> iSortCol, List<string> sSortDir,
            int userId, int? personId, int? eventId, string authorSearchText);

        IList<Reliability> GetReliabilities();

        Reliability GetReliability(int id);

        IList<Language> GetLanguages();

        Language GetLanguage(int languageId);

        /// <summary>
        /// Retrieve unique list of all folders.  SourceFolderDTO provides certain ownership details automatically depending on the folder path.
        /// </summary>
        /// <returns></returns>
        IList<SourceFolderDTO> GetFolderDTOs(string prefix);

        /// <summary>
        /// Retrieve all SourcePaths in the source table and count number of files in each directory up to given nested depth.
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        IDictionary<string, int> GetFolderCounts(int depth);

        /// <summary>
        /// Archive every source under the given folder (including subfolders).
        /// </summary>
        /// <param name="prefix"></param>
        void ArchiveSourcePath(string prefix);

        /// <summary>
        /// Get SourceDTOs of sources with media content types and ignored file extensions.
        /// </summary>
        /// <returns></returns>
        IList<SourceDTO> GetUnindexableMediaSources();

        /// <summary>
        /// <para>Get sources to index filtered by starting source ID, number of sources, and SourcePath prefix.</para>
        /// <para>All sources to index would be called by GetSourcesToIndex(0, 0, null).</para>
        /// <para>Used in conjunction with AddSourcesToIndex().</para>
        /// </summary>
        /// <param name="startSourceId">Index sources with ID greater than startSourceId.</param>
        /// <param name="num">Index this number of sources if num is greater than 0.</param>
        /// <param name="prefix">Index only sources whose SourcePath begins with prefix.</param>
        /// <returns></returns>
        IList<SourceDTO> GetSourcesToIndex(IStatelessSession session, int startSourceId, int num, string prefix);

        /// <summary>
        /// <para>Retrieves sources using a single IStatelessSession (memory optimization).</para>
        /// <para>Used in conjunction with GetSourcesToIndex().</para>
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns>Number of sources successfully added to index.</returns>
        int AddSourcesToIndex(IStatelessSession session, IList<SourceDTO> dtos);

        IList<SourceIndexLog> GetSourceIndexLogsWithErrors();

        SourceIndexLog GetSourceIndexLog(ISession session, int sourceId);

        void DeleteAllSourceIndexLogs();

        /// <summary>
        /// Get total rows in PRF_SourceIndexLog, equivalent to total number of sources processed for indexing (including those which did not index successfully).
        /// </summary>
        /// <returns></returns>
        int CountSourceIndexLogs();

        /// <summary>
        /// Get count of sources that encountered errors during indexing.
        /// </summary>
        /// <returns></returns>
        int CountSourceIndexLogErrors();

        /// <summary>
        /// Return errors encoutered when previewing sources.
        /// </summary>
        /// <returns></returns>
        IList<SourceLog> GetSourceLogsWithErrors();

        IList<SourceDTO> GetSourceDTOsWithPasswordErrors();

        /// <summary>
        /// Return number of sources that have been tested for preview errors.
        /// </summary>
        /// <returns></returns>
        int CountSourceLogs();

        int CountSourceLogsWithPasswordErrors();

        JhroCase GetJhroCase(int id);

        JhroCase GetJhroCase(string caseNumber);

        /// <summary>
        /// Same as GetJhroCase, but using given ISession.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="caseNumber"></param>
        /// <returns></returns>
        JhroCase GetJhroCase(ISession session, string caseNumber);

        IList<JhroCase> GetJhroCases();

        IList<JhroCase> SearchJhroCases(string term);

        JhroCase SaveJhroCase(JhroCase jhroCase);

        /// <summary>
        /// Same as SaveJhroCase, but using given ISession.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="jhroCase"></param>
        /// <returns></returns>
        JhroCase SaveJhroCase(ISession session, JhroCase jhroCase);

        void PopulateSourceOwners();
    }
}
