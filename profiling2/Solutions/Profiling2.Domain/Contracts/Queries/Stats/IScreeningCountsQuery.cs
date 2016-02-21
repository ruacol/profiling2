using System;
using System.Collections.Generic;

namespace Profiling2.Domain.Contracts.Queries.Stats
{
    public interface IScreeningCountsQuery
    {
        IList<object[]> GetFinalDecisionCountByMonth();

        IList<object[]> GetFinalDecisionCountByRequestEntity(DateTime start, DateTime end);
    }
}
