using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;

namespace Profiling2.Domain.Contracts.Queries.Suggestions
{
    public interface IOrganizationResponsibilitySuggestionsQuery
    {
        IList<OrganizationResponsibility> GetOrganizationResponsibilitiesLinkedByOrganization(Person p);

        IList<OrganizationResponsibility> GetOrganizationResponsibilitiesLinkedByUnit(Person p);
    }
}
