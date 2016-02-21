using System.Collections.Generic;
using Profiling2.Domain.Prf.Events;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IViolationDataTablesQuery
    {
        int GetSearchTotal(string searchText);

        IList<ViolationDataTableView> GetPaginatedResults(int iDisplayStart, int iDisplayLength, string searchText,
            int iSortingCols, IList<int> iSortCol, IList<string> sSortDir);
    }
}
