using System.Collections.Generic;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Domain.Contracts.Queries.Search
{
    public interface IUnitSearchQuery
    {
        IList<Operation> GetOperationsLike(string term);

        IList<Unit> GetResults(string term);
    }
}
