using NHibernate;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using System.Collections.Generic;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IPersonTasks
    {
        Person GetPerson(int personId);

        IList<Person> GetAllPersons();

        IList<Person> GetPersons(ISession session, int pageSize, int page);

        int GetPersonsCount(ISession session);

        IList<Person> GetPersonsWithSameEthnicity(Person person, bool canAccessRestrictedProfiles);

        /// <summary>
        /// Save or create Person.  Updates Lucene Person index.
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        Person SavePerson(Person person);

        /// <summary>
        /// Background task entry point to luceneTasks.UpdatePerson(person).
        /// </summary>
        /// <param name="personId"></param>
        void LuceneUpdatePersonQueueable(int personId);

        IList<SearchForPersonDTO> GetPersonsByName(string term, string username);

        IList<Person> GetPersonsWithUnmatchedMilitaryID();

        /// <summary>
        /// Delete Person.  Removes entry from Lucene Person index.
        /// 
        /// Historical revision of the person remains in the audit tables, accessible via Envers.
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        bool DeletePerson(Person person);

        int GetPersonDataTablesCount(string term, string username, bool includeRestrictedProfiles);

        IList<SearchForPersonDTO> GetPersonDataTablesPaginated(int iDisplayStart, int iDisplayLength, string searchText,
            int iSortingCols, IList<int> iSortCol, IList<string> sSortDir, string username, bool includeRestrictedProfiles);

        IList<ProfileStatus> GetAllProfileStatuses();

        IList<ProfileStatus> GetAllProfileStatuses(ISession session);

        ProfileStatus GetProfileStatus(int id);

        ProfileStatus GetProfileStatus(string name);

        ProfileStatus GetProfileStatus(string name, ISession session);

        IList<Person> GetPersonsIncomplete();

        IList<Ethnicity> GetEthnicities();

        Ethnicity GetEthnicity(int id);

        Ethnicity GetEthnicity(string name);

        Ethnicity SaveEthnicity(Ethnicity e);

        bool DeleteEthnicity(Ethnicity e);

        IList<object> GetEthnicitiesJson(string term);

        PersonAlias GetPersonAlias(int id);

        IList<PersonAlias> GetPersonAliases(Person person);

        /// <summary>
        /// Save or create new PersonAlias.  Updates Lucene Person index.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        PersonAlias SavePersonAlias(PersonAlias alias);

        /// <summary>
        /// Delete PersonAlias.  Updates Lucene Person index.
        /// 
        /// Historical revision of the alias remains in the Envers audit table.
        /// </summary>
        /// <param name="alias"></param>
        void DeletePersonAlias(PersonAlias alias);

        PersonRelationship GetPersonRelationship(int id);

        PersonRelationship SavePersonRelationship(PersonRelationship relationship);

        void DeletePersonRelationship(PersonRelationship relationship);

        PersonRelationshipType GetPersonRelationshipType(int id);

        IList<PersonRelationshipType> GetAllPersonRelationshipTypes();

        IList<PersonRelationshipType> GetPersonRelationshipTypesByName(string term);

        PersonRelationshipType SavePersonRelationshipType(PersonRelationshipType type);

        bool DeletePersonRelationshipType(PersonRelationshipType type);

        IList<Person> GetPersonsWanted();

        ActiveScreening GetActiveScreening(int id);

        ActiveScreening SaveActiveScreening(ActiveScreening a);

        void DeleteActiveScreening(ActiveScreening a);

        /// <summary>
        /// Perform person merge.  Updates Lucene Person index after merge.
        /// 
        /// Deleted person is audited via Profiling1 auditing mechanism via the stored proc (and NOT Envers).
        /// </summary>
        /// <param name="toKeepPersonId"></param>
        /// <param name="toDeletePersonId"></param>
        /// <param name="userId">Logged-in user's UN ID, or string username.</param>
        /// <param name="isProfilingChange">Whether logged-in user is member of Profiling team or not.</param>
        /// <returns></returns>
        int MergePersons(int toKeepPersonId, int toDeletePersonId, string userId, bool isProfilingChange);

        /// <summary>
        /// Direct call to avoid loading attached Source entities into memory by NHibernate.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        IList<PersonSourceDTO> GetPersonSourceDTOs(int personId);

        PersonRestrictedNote GetPersonRestrictedNote(int id);

        PersonRestrictedNote SavePersonRestrictedNote(PersonRestrictedNote note);

        void DeletePersonRestrictedNote(PersonRestrictedNote note);

        IList<Person> MatchPerson(SearchForPersonDTO dto);

        IDictionary<SearchForPersonDTO, IList<Person>> MatchPersons(IList<SearchForPersonDTO> dtos);

        /// <summary>
        /// Generate a Word document of the profile for export.  Logs to PRF_AdminExportedPersonProfile.
        /// </summary>
        /// <param name="person"></param>
        /// <param name="includeBackground"></param>
        /// <param name="user"></param>
        /// <param name="clientDnsName"></param>
        /// <param name="clientIpAddress"></param>
        /// <param name="clientUserAgent"></param>
        /// <returns></returns>
        byte[] ExportDocument(Person person, bool includeBackground, AdminUser user, string clientDnsName, string clientIpAddress, string clientUserAgent);
    }
}
