using System.Collections.Generic;
using Profiling2.Domain.Prf.Organizations;

namespace Profiling2.Domain.Contracts.Queries.Search
{
    public interface IOrganizationSearchQuery
    {
        IList<Organization> GetResults(string term);
    }
}
