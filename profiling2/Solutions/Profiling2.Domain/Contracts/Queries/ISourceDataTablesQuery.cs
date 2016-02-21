using Profiling2.Domain.Prf.Sources;
using System.Collections.Generic;
using System;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface ISourceDataTablesQuery
    {
        int GetSearchTotal(bool canAccessRestricted, string searchName, string searchExt, string searchText, DateTime? start, DateTime? end, string authorSearchText);

        IList<SourceSearchResultDTO> GetPaginatedResults(bool canAccessRestricted, int iDisplayStart, int iDisplayLength,
            string searchName, string searchExt, string searchText, DateTime? start, DateTime? end, IList<int> adminSourceSearchIds,
            int iSortingCols, List<int> iSortCol, List<string> sSortDir,
            int userId, int? personId, int? eventId, string authorSearchText);
    }
}
