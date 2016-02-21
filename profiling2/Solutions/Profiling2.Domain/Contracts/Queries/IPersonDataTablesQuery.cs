using System.Collections.Generic;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IPersonDataTablesQuery
    {
        int GetSearchTotal(SearchTerm term, string username, bool includeRestrictedProfiles);

        IList<SearchForPersonDTO> GetPaginatedResults(int iDisplayStart, int iDisplayLength, SearchTerm term,
            int iSortingCols, IList<int> iSortCol, IList<string> sSortDir, string username, bool includeRestrictedProfiles);
    }
}
