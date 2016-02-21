using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IPersonQueries
    {
        Person GetPerson(ISession session, int personId);

        IList<Person> GetPersonsByEthnicity(Ethnicity ethnicity, bool canAccessRestrictedProfiles);

        IList<Person> GetIncompletePersons();

        IList<Person> GetPagedPersons(ISession session, int pageSize, int page);

        int GetPersonsCount(ISession session);

        IList<Person> GetPersonsWanted();

        IList<Person> GetPersonsByName(string term);

        IList<ProfileStatus> GetAllProfileStatuses(ISession session);

        ProfileStatus GetProfileStatus(string code, ISession session);
    }
}
