using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IAdminUserQueries
    {
        AdminUser GetAdminUser(string userId);
    }
}
