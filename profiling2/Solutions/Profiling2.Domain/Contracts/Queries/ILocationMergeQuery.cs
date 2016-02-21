using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface ILocationMergeQuery
    {
        void MergeLocations(int toKeepId, int toDeleteId);
    }
}
