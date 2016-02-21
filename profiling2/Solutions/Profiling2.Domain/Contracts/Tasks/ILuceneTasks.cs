using System;
using System.Collections.Generic;
using HrdbWebServiceClient.Domain;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.PersonEntity;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface ILuceneTasks
    {
        /// <summary>
        /// Creates entries in the Lucene person index of given persons.  Note this is a long-running task that holds a lock on the index while running.
        /// </summary>
        /// <param name="persons"></param>
        /// <param name="create">Specify true to create fresh index (will clear existing index if exists).  Otherwise specify false when updating index.</param>
        void CreatePersonIndexes(IList<Person> persons, bool create);

        void CreateUnitIndexes(IList<Unit> units);

        void CreateScreeningResponseIndexes(IList<ScreeningRequestPersonEntity> responses);

        void CreateEventIndexes(IList<Event> events);

        void CreateRequestIndexes(IList<Request> requests);

        void DeletePersonIndexes();

        void DeleteUnitIndexes();

        void DeleteScreeningResponseIndexes();

        void DeleteSourceIndexes();

        void DeleteEventIndexes();

        void DeleteRequestIndexes();

        void AddPerson(Person person);

        void AddUnit(Unit unit);

        void AddResponse(ScreeningRequestPersonEntity response);

        void AddSource(Source source);

        void AddEvent(Event ev);

        void AddRequest(Request request);

        void DeletePerson(int personId);

        void DeleteUnit(int unitId);

        void DeleteSource(int sourceId);

        void DeleteResponse(int personId, string screeningEntityName);

        void DeleteEvent(int eventId);

        void DeleteRequest(int requestId);

        void UpdatePerson(Person person);

        /// <summary>
        /// Update Lucene Person index for each person that has a Career that references this Organisation.
        /// </summary>
        /// <param name="org"></param>
        void UpdatePersons(Organization org);

        /// <summary>
        /// Update Lucene Person index for each person that has a Career that references this Unit.
        /// </summary>
        /// <param name="unit"></param>
        void UpdatePersons(Unit unit);

        void UpdatePersons(Rank rank);

        void UpdatePersons(Role role);

        void UpdatePersons(Location loc);

        void UpdateUnit(Unit unit);

        void UpdateUnits(Organization org);

        void UpdateResponse(ScreeningRequestPersonEntity response);

        void UpdateSource(Source source);

        void UpdateEvent(Event ev);

        void UpdateRequest(Request request);

        /// <summary>
        /// Runs search term through QueryParser, Query and IndexSearcher to perform a Lucene-based index search.
        /// 
        /// For results to be up to date, index must be updated as soon as domain models change.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="numResults"></param>
        /// <param name="includeRestrictedProfiles"></param>
        /// <returns></returns>
        IList<LuceneSearchResult> PersonSearch(string term, int numResults, bool includeRestrictedProfiles);

        IList<LuceneSearchResult> UnitSearch(string term, int numResults);

        IList<LuceneSearchResult> SourcePathExactSearch(string prefix, int numResults, bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners, string sortField, bool descending);

        IList<LuceneSearchResult> SourcePathPrefixSearch(string prefix, int numResults, bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners, string sortField, bool descending);

        IList<LuceneSearchResult> GetAllSourcesWithCaseNumbers(string term, int numResults, bool includeRestrictedSources, string sortField, bool descending);

        IList<LuceneSearchResult> ScreeningResponseSearch(string term, string screeningEntityName, int numResults);

        IList<LuceneSearchResult> SourceSearch(string term, string prefix, int numResults, bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners, string sortField, bool descending);

        IList<LuceneSearchResult> SourceSearch(string term, string prefix, DateTime? start, DateTime? end, int numResults, bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners, string sortField, bool descending);

        /// <summary>
        /// Run search, but only return facet count.
        /// </summary>
        /// <param name="term"></param>
        /// <param name="prefix"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="numResults"></param>
        /// <param name="includeRestrictedSources"></param>
        /// <returns></returns>
        IDictionary<IDictionary<string, string>, long> SourceSearchFacets(string term, string prefix, DateTime? start, DateTime? end, int numResults, bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners);

        /// <summary>
        /// Get the highest Source ID in the index.
        /// </summary>
        /// <returns></returns>
        int GetMaxSourceID();

        /// <summary>
        /// Use MoreLikeThis Lucene extension to find similar sources.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        IList<LuceneSearchResult> GetSourcesLikeThis(int sourceId, int numResults);

        IList<LuceneSearchResult> EventSearch(string term, int numResults, string sortField, bool descending);

        IList<LuceneSearchResult> EventSearch(string term, DateTime? start, DateTime? end, int numResults, string sortField, bool descending);

        /// <summary>
        /// Search screening requests.
        /// </summary>
        /// <param name="term"></param>
        /// <param name="numResults"></param>
        /// <param name="user">When not null, filters results as if user were only a screening request initiator.</param>
        /// <param name="sortField"></param>
        /// <param name="descending"></param>
        /// <returns></returns>
        IList<LuceneSearchResult> RequestSearch(string term, int numResults, AdminUser user, string sortField, bool descending);

        IList<LuceneSearchResult> FindMatchingEventCandidates(DateTime? start, DateTime? end, string caseCode);

        IList<LuceneSearchResult> FindMatchingEventCandidates(DateTime? start, DateTime? end, string caseCode, string townVillage, string subregion, string region);
    }
}
