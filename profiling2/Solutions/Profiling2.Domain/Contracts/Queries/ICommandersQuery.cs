using System.Collections.Generic;
using Profiling2.Domain.Prf.Careers;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface ICommandersQuery
    {
        IList<Career> GetCommanders();
    }
}
