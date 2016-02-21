using System.Collections.Generic;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IResponsibilityTasks
    {
        IList<PersonResponsibility> GetAllPersonResponsibilities();

        PersonResponsibility GetPersonResponsibility(int id);

        PersonResponsibility GetPersonResponsibility(Event e, Person p);

        void DeletePersonResponsibility(PersonResponsibility pr);

        PersonResponsibility SavePersonResponsibility(PersonResponsibility pr);

        PersonResponsibilityType GetPersonResponsibilityType(int id);

        IList<PersonResponsibilityType> GetPersonResponsibilityTypes();

        PersonResponsibilityType SavePersonResponsibilityType(PersonResponsibilityType type);

        OrganizationResponsibility GetOrganizationResponsibility(int id);

        OrganizationResponsibility SaveOrganizationResponsibility(OrganizationResponsibility or);

        void DeleteOrganizationResponsibility(OrganizationResponsibility or);

        OrganizationResponsibilityType GetOrgResponsibilityType(int id);

        IList<OrganizationResponsibilityType> GetOrgResponsibilityTypes();
    }
}
