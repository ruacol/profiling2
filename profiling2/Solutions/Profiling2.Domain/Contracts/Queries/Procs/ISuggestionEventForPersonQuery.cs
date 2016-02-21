using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Queries.Procs
{
    public interface ISuggestionEventForPersonQuery
    {
        int GetSuggestionTotal(int personId);

        IList<SuggestionEventForPersonDTO> GetPaginatedResults(int iDisplayStart, int iDisplayLength, int personId);
    }
}
