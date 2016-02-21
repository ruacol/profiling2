using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using log4net;
using NHibernate;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Export.Profiling;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Queries.Procs;
using Profiling2.Domain.Contracts.Queries.Search;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Infrastructure.Util;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks
{
    public class PersonTasks : IPersonTasks
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(PersonTasks));
        protected readonly INHibernateRepository<Person> personRepo;
        protected readonly INHibernateRepository<ProfileStatus> profileStatusRepo;
        protected readonly INHibernateRepository<Ethnicity> ethnicityRepo;
        protected readonly INHibernateRepository<PersonAlias> personAliasRepo;
        protected readonly INHibernateRepository<PersonRelationship> personRelationshipRepo;
        protected readonly INHibernateRepository<PersonRelationshipType> personRelationshipTypeRepo;
        protected readonly INHibernateRepository<ActiveScreening> activeScreeningRepo;
        protected readonly INHibernateRepository<PersonRestrictedNote> restrictedNoteRepo;
        protected readonly INHibernateRepository<AdminExportedPersonProfile> exportPersonRepo;
        protected readonly IPersonRelationshipTypeNameQuery personRelationshipTypeNameQuery;
        protected readonly IPersonDataTablesQuery personDataTablesQuery;
        protected readonly IPersonQueries personQueries;
        protected readonly IOrganizationTasks orgTasks;
        protected readonly IMergeStoredProcQueries mergeQueries;
        protected readonly ILuceneTasks luceneTasks;
        protected readonly IAttachedSourceQueries attachedSourceQueries;
        protected readonly IWordExportPersonService exportService;

        public PersonTasks(INHibernateRepository<Person> personRepo,
            INHibernateRepository<ProfileStatus> profileStatusRepo,
            INHibernateRepository<Ethnicity> ethnicityRepo,
            INHibernateRepository<PersonAlias> personAliasRepo,
            INHibernateRepository<PersonRelationship> personRelationshipRepo,
            INHibernateRepository<PersonRelationshipType> personRelationshipTypeRepo,
            INHibernateRepository<ActiveScreening> activeScreeningRepo,
            INHibernateRepository<PersonRestrictedNote> restrictedNoteRepo,
            INHibernateRepository<AdminExportedPersonProfile> exportPersonRepo,
            IPersonRelationshipTypeNameQuery personRelationshipTypeNameQuery,
            IPersonDataTablesQuery personDataTablesQuery,
            IPersonQueries personQueries,
            IOrganizationTasks orgTasks,
            IMergeStoredProcQueries mergeQueries,
            ILuceneTasks luceneTasks,
            IAttachedSourceQueries attachedSourceQueries,
            IWordExportPersonService exportService)
        {
            this.personRepo = personRepo;
            this.profileStatusRepo = profileStatusRepo;
            this.ethnicityRepo = ethnicityRepo;
            this.personAliasRepo = personAliasRepo;
            this.personRelationshipRepo = personRelationshipRepo;
            this.personRelationshipTypeRepo = personRelationshipTypeRepo;
            this.activeScreeningRepo = activeScreeningRepo;
            this.restrictedNoteRepo = restrictedNoteRepo;
            this.exportPersonRepo = exportPersonRepo;
            this.personRelationshipTypeNameQuery = personRelationshipTypeNameQuery;
            this.personDataTablesQuery = personDataTablesQuery;
            this.personQueries = personQueries;
            this.orgTasks = orgTasks;
            this.mergeQueries = mergeQueries;
            this.luceneTasks = luceneTasks;
            this.attachedSourceQueries = attachedSourceQueries;
            this.exportService = exportService;
        }

        public Person GetPerson(int personId)
        {
            return this.personRepo.Get(personId);
        }

        private Person GetPerson(ISession session, int personId)
        {
            return this.personQueries.GetPerson(session, personId);
        }

        public IList<Person> GetAllPersons()
        {
            return this.personRepo.GetAll();
        }

        public IList<Person> GetPersons(ISession session, int pageSize, int page)
        {
            return this.personQueries.GetPagedPersons(session, pageSize, page);
        }

        public int GetPersonsCount(ISession session)
        {
            return this.personQueries.GetPersonsCount(session);
        }

        public IList<Person> GetPersonsWithSameEthnicity(Person person, bool canAccessRestrictedProfiles)
        {
            if (person.Ethnicity != null && !string.Equals(person.Ethnicity.EthnicityName, "Unknown"))
            {
                IList<Person> persons = this.personQueries.GetPersonsByEthnicity(person.Ethnicity, canAccessRestrictedProfiles);
                persons.Remove(person);
                return persons;
            }
            return new List<Person>();
        }

        public Person SavePerson(Person person)
        {
            person = this.personRepo.SaveOrUpdate(person);

            BackgroundJob.Enqueue<IPersonTasks>(x => x.LuceneUpdatePersonQueueable(person.Id));

            return person;
        }

        public void LuceneUpdatePersonQueueable(int personId)
        {
            using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
            {
                this.luceneTasks.UpdatePerson(this.GetPerson(session, personId));
            }
        }

        public IList<SearchForPersonDTO> GetPersonsByName(string searchText, string username)
        {
            SearchTerm term = new SearchTerm(searchText, null, null);
            return this.personDataTablesQuery.GetPaginatedResults(0, 50, term, 0, null, null, username, true);
        }

        public IList<Person> GetPersonsWithUnmatchedMilitaryID()
        {
            IList<Person> persons =  this.personRepo.GetAll().Where(x => !x.Archive && !string.IsNullOrEmpty(x.MilitaryIDNumber)).ToList();
            IList<Person> unmatchedList = new List<Person>();
            foreach (Person p in persons)
                if (!IDNumber.IsRecognised(p.MilitaryIDNumber))
                    unmatchedList.Add(p);
            return unmatchedList;
        }

        public bool DeletePerson(Person person)
        {
            if (person != null)
            {
                this.log.Info("Deleting Person, Id: " + person.Id + ", Name: " + person.Name);
                this.luceneTasks.DeletePerson(person.Id);
                this.personRepo.Delete(person);
                return true;
            }
            return false;
        }

        public int GetPersonDataTablesCount(string searchText, string username, bool includeRestrictedProfiles)
        {
            SearchTerm term = new SearchTerm(searchText, this.orgTasks.GetAllRanks(), this.orgTasks.GetAllRoles());
            return this.personDataTablesQuery.GetSearchTotal(term, username, includeRestrictedProfiles);
        }

        public IList<SearchForPersonDTO> GetPersonDataTablesPaginated(int iDisplayStart, int iDisplayLength, string searchText,
            int iSortingCols, IList<int> iSortCol, IList<string> sSortDir, string username, bool includeRestrictedProfiles)
        {
            SearchTerm term = new SearchTerm(searchText, this.orgTasks.GetAllRanks(), this.orgTasks.GetAllRoles());
            return this.personDataTablesQuery.GetPaginatedResults(iDisplayStart, iDisplayLength, term, iSortingCols, iSortCol, sSortDir, username, includeRestrictedProfiles);
        }

        public IList<ProfileStatus> GetAllProfileStatuses()
        {
            return this.GetAllProfileStatuses(null);
        }

        public IList<ProfileStatus> GetAllProfileStatuses(ISession session)
        {
            return this.personQueries.GetAllProfileStatuses(session);
        }

        public ProfileStatus GetProfileStatus(int id)
        {
            return this.profileStatusRepo.Get(id);
        }

        public ProfileStatus GetProfileStatus(string name)
        {
            return this.GetProfileStatus(name, null);
        }

        public ProfileStatus GetProfileStatus(string name, ISession session)
        {
            return this.personQueries.GetProfileStatus(name, session);
        }

        public IList<Person> GetPersonsIncomplete()
        {
            return this.personQueries.GetIncompletePersons();
        }

        public IList<Ethnicity> GetEthnicities()
        {
            return this.ethnicityRepo.GetAll();
        }

        public Ethnicity GetEthnicity(int id)
        {
            return this.ethnicityRepo.Get(id);
        }

        public Ethnicity GetEthnicity(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("EthnicityName", name);
            return this.ethnicityRepo.FindOne(criteria);
        }

        public Ethnicity SaveEthnicity(Ethnicity e)
        {
            return this.ethnicityRepo.SaveOrUpdate(e);
        }

        public bool DeleteEthnicity(Ethnicity e)
        {
            if (e != null && e.Persons.Count < 1)
            {
                this.ethnicityRepo.Delete(e);
                return true;
            }
            return false;
        }

        public IList<object> GetEthnicitiesJson(string term)
        {
            IList<Ethnicity> ethnicities = (from eth in this.ethnicityRepo.GetAll()
                                            where eth.EthnicityName.ToUpper().Contains(term.ToUpper())
                                            orderby eth.ToString()
                                            select eth).ToList<Ethnicity>();

            IList<object> objList = new List<object>();
            foreach (Ethnicity e in ethnicities)
                objList.Add(new
                {
                    id = e.Id,
                    text = e.ToString()
                });
            return objList;
        }

        public PersonAlias GetPersonAlias(int id)
        {
            return this.personAliasRepo.Get(id);
        }

        public IList<PersonAlias> GetPersonAliases(Person person)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Person", person);
            return this.personAliasRepo.FindAll(criteria);
        }

        public PersonAlias SavePersonAlias(PersonAlias alias)
        {
            Person person = alias.Person;
            if (person != null)
            {
                person.AddPersonAlias(alias);

                if (!person.HasValidProfileStatus())
                    person.ProfileStatus = this.GetProfileStatus(ProfileStatus.ROUGH_OUTLINE);
                this.SavePerson(person);  // also updates lucene
            }
            return this.personAliasRepo.SaveOrUpdate(alias);
        }

        public void DeletePersonAlias(PersonAlias alias)
        {
            Person person = alias.Person;
            if (person != null)
            {
                person.RemovePersonAlias(alias);

                // queue update to Person index
                BackgroundJob.Enqueue<IPersonTasks>(x => x.LuceneUpdatePersonQueueable(person.Id));
            }
            this.personAliasRepo.Delete(alias);
        }

        public PersonRelationship GetPersonRelationship(int id)
        {
            return this.personRelationshipRepo.Get(id);
        }

        public PersonRelationship SavePersonRelationship(PersonRelationship relationship)
        {
            relationship.SubjectPerson.AddPersonRelationshipAsSubject(relationship);
            relationship.ObjectPerson.AddPersonRelationshipAsObject(relationship);

            Person subject = relationship.SubjectPerson;
            Person objectP = relationship.ObjectPerson;

            if (!subject.HasValidProfileStatus())
            {
                subject.ProfileStatus = this.GetProfileStatus(ProfileStatus.ROUGH_OUTLINE);
                this.SavePerson(subject);
            }
            if (!objectP.HasValidProfileStatus())
            {
                objectP.ProfileStatus = this.GetProfileStatus(ProfileStatus.ROUGH_OUTLINE);
                this.SavePerson(objectP);
            }

            return this.personRelationshipRepo.SaveOrUpdate(relationship);
        }

        public void DeletePersonRelationship(PersonRelationship relationship)
        {
            relationship.SubjectPerson.RemovePersonRelationshipAsSubject(relationship);
            relationship.ObjectPerson.RemovePersonRelationshipAsObject(relationship);
            this.personRelationshipRepo.Delete(relationship);
        }

        public PersonRelationshipType GetPersonRelationshipType(int id)
        {
            return this.personRelationshipTypeRepo.Get(id);
        }

        public IList<PersonRelationshipType> GetAllPersonRelationshipTypes()
        {
            return this.personRelationshipTypeRepo.GetAll()
                .Where(x => !x.Archive)
                .OrderBy(x => x.PersonRelationshipTypeName)
                .ToList<PersonRelationshipType>();
        }

        public IList<PersonRelationshipType> GetPersonRelationshipTypesByName(string term)
        {
            return this.personRelationshipTypeNameQuery.GetResults(term);
        }

        public PersonRelationshipType SavePersonRelationshipType(PersonRelationshipType type)
        {
            return this.personRelationshipTypeRepo.SaveOrUpdate(type);
        }

        public bool DeletePersonRelationshipType(PersonRelationshipType type)
        {
            if (type != null)
            {
                if (!type.PersonRelationships.Any())
                {
                    this.personRelationshipTypeRepo.Delete(type);
                    return true;
                }
            }
            return false;
        }

        public IList<Person> GetPersonsWanted()
        {
            return this.personQueries.GetPersonsWanted();
        }

        public ActiveScreening GetActiveScreening(int id)
        {
            return this.activeScreeningRepo.Get(id);
        }

        public ActiveScreening SaveActiveScreening(ActiveScreening a)
        {
            a.Person.AddActiveScreening(a);
            return this.activeScreeningRepo.SaveOrUpdate(a);
        }

        public void DeleteActiveScreening(ActiveScreening a)
        {
            a.Person.RemoveActiveScreening(a);
            this.activeScreeningRepo.Delete(a);
        }

        public int MergePersons(int toKeepPersonId, int toDeletePersonId, string userId, bool isProfilingChange)
        {
            int result = this.mergeQueries.MergePersons(toKeepPersonId, toDeletePersonId, userId, isProfilingChange);

            if (result == 1)
            {
                // queue update to Person index
                BackgroundJob.Enqueue<IPersonTasks>(x => x.LuceneUpdatePersonQueueable(toKeepPersonId));

                this.luceneTasks.DeletePerson(toDeletePersonId);
            }

            return result;
        }

        public IList<PersonSourceDTO> GetPersonSourceDTOs(int personId)
        {
            return this.attachedSourceQueries.GetPersonSourceDTOs(personId);
        }

        public PersonRestrictedNote GetPersonRestrictedNote(int id)
        {
            return this.restrictedNoteRepo.Get(id);
        }

        public PersonRestrictedNote SavePersonRestrictedNote(PersonRestrictedNote note)
        {
            note.Person.AddPersonRestrictedNote(note);

            if (!note.Person.HasValidProfileStatus())
            {
                note.Person.ProfileStatus = this.GetProfileStatus(ProfileStatus.ROUGH_OUTLINE);
                this.SavePerson(note.Person);
            }

            return this.restrictedNoteRepo.SaveOrUpdate(note);
        }

        public void DeletePersonRestrictedNote(PersonRestrictedNote note)
        {
            if (note != null)
            {
                note.Person.RemovePersonRestrictedNote(note);
                this.restrictedNoteRepo.Delete(note);
            }
        }

        public IList<Person> MatchPerson(SearchForPersonDTO dto)
        {
            IList<Person> results = new List<Person>();

            if (dto != null)
            {
                IList<LuceneSearchResult> luceneResults = new List<LuceneSearchResult>();

                if (!string.IsNullOrEmpty(dto.MilitaryIDNumber))
                {
                    luceneResults = luceneResults.Concat(this.luceneTasks.PersonSearch("MilitaryIDNumber:" + dto.MilitaryIDNumber, 10, true))
                        .ToList();
                }

                if (!string.IsNullOrEmpty(dto.FirstName) || !string.IsNullOrEmpty(dto.LastName))
                {
                    luceneResults = luceneResults.Concat(this.luceneTasks.PersonSearch(string.Join(" ", new string[] { dto.FirstName, dto.LastName }), 10, true))
                        .ToList();
                }

                results = luceneResults.OrderByDescending(x => x.Score).Select(x => this.GetPerson(x.GetPersonId())).ToList();
            }

            return results.Distinct().ToList();
        }

        public IDictionary<SearchForPersonDTO, IList<Person>> MatchPersons(IList<SearchForPersonDTO> dtos)
        {
            IDictionary<SearchForPersonDTO, IList<Person>> results = new Dictionary<SearchForPersonDTO, IList<Person>>();

            foreach (SearchForPersonDTO dto in dtos)
                results.Add(dto, this.MatchPerson(dto));

            return results;
        }

        public byte[] ExportDocument(Person person, bool includeBackground, AdminUser user, string clientDnsName, string clientIpAddress, string clientUserAgent)
        {
            // all exports are logged to this table
            AdminExportedPersonProfile ex = new AdminExportedPersonProfile()
            {
                Person = person,
                ExportDateTime = DateTime.Now,
                ExportedByAdminUser = user,
                ClientDnsName = clientDnsName,
                ClientIpAddress = clientIpAddress,
                ClientUserAgent = clientUserAgent
            };
            this.exportPersonRepo.SaveOrUpdate(ex);

            return this.exportService.GetExport(person, includeBackground);
        }
    }
}
