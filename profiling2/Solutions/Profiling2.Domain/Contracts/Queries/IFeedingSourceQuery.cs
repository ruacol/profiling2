using System;
using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IFeedingSourceQuery
    {
        /// <summary>
        /// Get all FeedingSources that have not been fed into PRF_Source.
        /// </summary>
        /// <param name="includeRestricted"></param>
        /// <returns></returns>
        IList<FeedingSourceDTO> GetFeedingSourceDTOs(bool canViewAndSearchAll, bool includeRestricted, string uploadedByUserId);

        /// <summary>
        /// Get all FeedingSources bound by given dates for reporting purposes.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="includeRestricted"></param>
        /// <returns></returns>
        IList<FeedingSourceDTO> GetFeedingSourceDTOs(ISession session, DateTime start, DateTime end, bool includeRestricted);
    }
}
