using System;
using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.DTO;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IPersonStatisticTasks
    {
        IList<object[]> GetCreatedProfilesCountByMonth();

        ProfilingCountsView GetProfilingCountsView(DateTime? date, ISession session);

        /// <summary>
        /// Get person profile count first by short organization name then profile status.  Doesn't include 'FARDC 2007 List' profile status.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, IDictionary<string, int>> GetPersonCountByStatusAndOrganization();
    }
}
