using System.Collections.Generic;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Tasks.Sources
{
    public interface ISourceManagementTasks
    {
        IList<object[]> DuplicatesByHash(int maxResults);

        IList<object[]> DuplicatesByName();

        IList<SourceDTO> DuplicatesByIgnored();

        /// <summary>
        /// Arrange duplicate of given source name keyed by the first source of that name, and the dictionary values containing other sources of that name.
        /// 
        /// In addition to file name, the file size must be the same for a source to be considered a duplicate.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IDictionary<SourceDTO, IList<SourceDTO>> DuplicatesByNameOf(string name);

        void MergeDuplicatesByNameOf(string name);

        /// <summary>
        /// Grab x number of hash duplicates, and clean/merge them.  Entry point for CleanDuplicatesByHashOf().
        /// </summary>
        void CleanHashDuplicates();

        void CleanDuplicatesByHashOf(string hash);

        /// <summary>
        /// Show number of PersonSource duplicates in the database.  Single-use method used before adding a unique index to PRF_PersonSource.
        /// </summary>
        /// <returns></returns>
        IList<ObjectSourceDuplicateDTO> GetPersonSourceDuplicates();

        /// <summary>
        /// Perform merge of PRF_PersonSource duplicates.  Single-use method used before adding a unique index to PRF_PersonSource.
        /// </summary>
        void MergePersonSourceDuplicates();

        /// <summary>
        /// Show number of EventSource duplicates in the database.  Single-use method used before adding a unique index to PRF_EventSource.
        /// </summary>
        /// <returns></returns>
        IList<ObjectSourceDuplicateDTO> GetEventSourceDuplicates();

        /// <summary>
        /// Perform merge of PRF_EventSource duplicates.  Single-use method used before adding a unique index to PRF_EventSource.
        /// </summary>
        void MergeEventSourceDuplicates();

        /// <summary>
        /// Show number of OrganizationSource duplicates in the database.  Single-use method used before adding a unique index to PRF_OrganizationSource.
        /// </summary>
        /// <returns></returns>
        IList<ObjectSourceDuplicateDTO> GetOrganizationSourceDuplicates();

        /// <summary>
        /// Perform merge of PRF_OrganizationSource duplicates.  Single-use method used before adding a unique index to PRF_OrganizationSource.
        /// </summary>
        void MergeOrganizationSourceDuplicates();

        IList<SourceDTO> GetExtensionlessSources();

        IList<SourceDTO> GetScannableSourceDTOs(int days);
    }
}
