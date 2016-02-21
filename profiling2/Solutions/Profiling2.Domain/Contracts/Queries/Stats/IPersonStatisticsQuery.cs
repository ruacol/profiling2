using System.Collections.Generic;
using NHibernate;

namespace Profiling2.Domain.Contracts.Queries.Stats
{
    public interface IPersonStatisticsQuery
    {
        IList<object[]> GetCreatedProfilesCountByMonth();

        IList<object[]> GetLiveCreatedProfilesCount(ISession session);

        /// <summary>
        /// Get count of persons grouped by organization short name and profile status name.  Organization
        /// comes from most recent career.  WARNING: doesn't include person profiles which have NO careers.
        /// </summary>
        /// <returns></returns>
        IList<object[]> GetProfileStatusCountsByOrganization();

        /// <summary>
        /// Get count of persons with no career grouped by profile status name.  Combined with person status counts
        /// by organization, this gives a full picture of persons by status and organization.
        /// </summary>
        /// <returns></returns>
        IList<object[]> GetProfileStatusCountsNoOrganization();
    }
}
