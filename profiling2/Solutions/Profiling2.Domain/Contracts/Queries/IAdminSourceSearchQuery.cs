using System.Collections.Generic;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IAdminSourceSearchQuery
    {
        IList<int> GetAdminSourceSearchIds(AdminSourceSearch adminSourceSearch);
    }
}
