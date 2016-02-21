using System.Collections.Generic;
using System.IO;
using NHibernate;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface ISourceQueries
    {
        IList<Source> GetSourcesByName(string term);

        /// <summary>
        /// Retrieve all SourceDTOs in order to index them.  Does not include archived sources.
        /// </summary>
        /// <param name="session">Use given session to run query, otherwise use built-in session linked to web request.</param>
        /// <param name="excludeBinaryIndexedSources">Don't include sources that exist in PRF_SourceIndexLog (i.e. have been indexed for their binary content before).</param>
        /// <param name="excludeSourceLogged">Don't include sources that exist in PRF_SourceLog.</param>
        /// <returns></returns>
        IList<SourceDTO> GetAllSourceDTOs(IStatelessSession session, bool excludeBinaryIndexedSources, bool excludeSourceLogged);

        /// <summary>
        /// Useful for getting a source's file size without instantiating the NHibernate object.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        SourceDTO GetSourceDTO(int sourceId);

        IList<SourceDTO> GetSourceDTOsByHash(string hash);

        IList<SourceDTO> GetScannableSourceDTOs(int days);

        IList<SourceDTO> GetSourceDTOsWithPasswordErrors();

        IList<object[]> GetDuplicatesByHash(int maxResults);

        IList<object[]> DuplicatesByName(int threshold);

        IList<SourceDTO> DuplicatesByIgnored(string[] ignoredFileExtensions);

        /// <summary>
        /// Get sources with the given name.  Straight SQL implementation to avoid loading Source.FileData into memory via NHibernate.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IList<SourceDTO> DuplicatesByNameOf(string name);

        /// <summary>
        /// SQL delete given source if it has no other attached entities.  Bypasses NHibernate, so no audit trail.
        /// </summary>
        /// <param name="sourceId"></param>
        void DeleteSource(int sourceId);

        /// <summary>
        /// Switch attached entities from one source to another using SQL (bypasses NHibernate, so no audit trail). Archives 'from' source afterwards.
        /// </summary>
        /// <param name="fromSourceId"></param>
        /// <param name="toSourceId"></param>
        void ChangeAttachmentsToAnotherSource(int fromSourceId, int toSourceId);

        IList<string> GetSourcePaths(string prefix);

        /// <summary>
        /// Archive every source whose SourcePath starts with the given prefix.
        /// </summary>
        /// <param name="prefix"></param>
        void ArchiveSourcePathPrefix(string prefix);

        /// <summary>
        /// Get source using given stateless session - saves memory during bulk processing when indexing.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Source GetSource(IStatelessSession session, int id);

        IList<SourceDTO> GetExtensionlessSources();

        /// <summary>
        /// Stream large binary data.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="hasOcrText"></param>
        /// <returns></returns>
        Stream GetSourceData(int sourceId, bool hasOcrText);

        IList<string> GetSourceAuthors(IStatelessSession session, int id);

        IList<string> GetSourceAuthors(int sourceId);

        IList<string> GetSourceOwners(IStatelessSession session, int sourceId);

        IList<string> GetSourceOwners(int sourceId);

        int InsertSourceOwner(int sourceId, int sourceOwningEntityId);

        IList<SourceOwningEntity> GetSourceOwningEntities(ISession session, string name);

        /// <summary>
        /// Allows saving a source using given ISession; useful when processing the Source outside of standard web request.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="source"></param>
        void SaveSource(ISession session, Source source);
    }
}
