using System;
using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Tasks.Sources
{
    public interface IFeedingSourceTasks
    {
        FeedingSource SaveFeedingSource(FeedingSource fs);

        IList<FeedingSourceDTO> GetFeedingSourceDTOs(bool canViewAndSearchAll, bool includeRestricted, string uploadedByUserId);

        FeedingSource GetFeedingSource(int id);

        FeedingSource GetFeedingSource(string name);

        /// <summary>
        /// Get statstics of feeding source activity indexed by username.
        /// </summary>
        /// <param name="session">Optional, useful when calling from outside a HTTP request.  Set to null otherwise.</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="includeRestricted"></param>
        /// <returns></returns>
        IDictionary<string, FeedingSourceStat> GetFeedingSourceDTOs(ISession session, DateTime start, DateTime end, bool includeRestricted);

        /// <summary>
        /// Entry point to GetFeedingSourceDTOs to be called by background job.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, FeedingSourceStat> GetFeedingSourceDTOsRecurring();

        void DeleteFeedingSource(FeedingSource fs);

        /// <summary>
        /// Create a new Source given a FeedingSource. Adds Source to Lucene index.
        /// </summary>
        /// <param name="feedingSourceId"></param>
        /// <returns></returns>
        Source FeedSource(int feedingSourceId);
    }

    public class FeedingSourceStat
    {
        public string User { get; set; }
        public int Uploaded { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }

        public FeedingSourceStat(string user) { this.User = user; this.Uploaded = 0; this.Approved = 0; this.Rejected = 0; }
    }
}
