using NHibernate;

namespace Profiling2.Domain.Contracts.Queries.Stats
{
    public interface ICountsQuery
    {
        int GetCareerCount(ISession session);

        int GetOrganizationCount(ISession session);

        int GetEventCount(ISession session);

        int GetPersonResponsibilityCount(ISession session);

        int GetOrganizationResponsibilityCount(ISession session);

        int GetSourceCount(ISession session);
    }
}
