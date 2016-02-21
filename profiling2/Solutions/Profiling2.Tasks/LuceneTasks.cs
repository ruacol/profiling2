using System;
using System.Collections.Generic;
using System.Linq;
using HrdbWebServiceClient.Domain;
using log4net;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Infrastructure.Search;
using StackExchange.Profiling;

namespace Profiling2.Tasks
{
    /// <summary>
    /// Each Lucene index has a single IndexWriter, implemented as a (lazy-loaded) singleton.  The IndexWriter stays open for the life of the worker thread
    /// (see [Entity]IndexWriterSingleton classes for implementation).  See BaseIndexer for when Commit() is called.
    /// 
    /// Searcher classes currently instantiate IndexSearchers using SearcherManager (which in turn are instantiated using the above mentioned singletons).
    /// TODO strategy to call SearcherManager.MaybeReopen() in order to refresh IndexReader/IndexSearcher.  Either that or ensure IndexWriter.Commit() is called
    /// after every change.
    /// </summary>
    public class LuceneTasks : ILuceneTasks
    {
        protected static ILog log = LogManager.GetLogger(typeof(LuceneTasks));
        protected readonly ILuceneIndexer<PersonIndexer> personIndexer;
        protected readonly IPersonSearcher personSearcher;
        protected readonly ILuceneIndexer<UnitIndexer> unitIndexer;
        protected readonly IUnitSearcher unitSearcher;
        protected readonly IScreeningResponseIndexer screeningResponseIndexer;
        protected readonly IScreeningResponseSearcher screeningResponseSearcher;
        protected readonly ILuceneIndexer<SourceIndexer> sourceIndexer;
        protected readonly ISourceSearcher sourceSearcher;
        protected readonly ILuceneIndexer<EventIndexer> eventIndexer;
        protected readonly IEventSearcher eventSearcher;
        protected readonly ILuceneIndexer<RequestIndexer> requestIndexer;
        protected readonly IRequestSearcher requestSearcher;

        public LuceneTasks(ILuceneIndexer<PersonIndexer> personIndexer, 
            IPersonSearcher personSearcher,
            ILuceneIndexer<UnitIndexer> unitIndexer,
            IUnitSearcher unitSearcher,
            IScreeningResponseIndexer screeningResponseIndexer,
            IScreeningResponseSearcher screeningResponseSearcher,
            ILuceneIndexer<SourceIndexer> sourceIndexer,
            ISourceSearcher sourceSearcher,
            ILuceneIndexer<EventIndexer> eventIndexer,
            IEventSearcher eventSearcher,
            ILuceneIndexer<RequestIndexer> requestIndexer,
            IRequestSearcher requestSearcher)
        {
            this.personIndexer = personIndexer;
            this.personSearcher = personSearcher;
            this.unitIndexer = unitIndexer;
            this.unitSearcher = unitSearcher;
            this.screeningResponseIndexer = screeningResponseIndexer;
            this.screeningResponseSearcher = screeningResponseSearcher;
            this.sourceIndexer = sourceIndexer;
            this.sourceSearcher = sourceSearcher;
            this.eventIndexer = eventIndexer;
            this.eventSearcher = eventSearcher;
            this.requestIndexer = requestIndexer;
            this.requestSearcher = requestSearcher;
        }

        // Create*Indexes methods utilise create=true when instantiating IndexWriter.  Necessary when creating index from scratch.  Clears
        // existing index if not called from scratch.

        public void CreatePersonIndexes(IList<Person> persons, bool create)
        {
            this.personIndexer.AddMultiple(persons);
        }

        public void CreateUnitIndexes(IList<Unit> units)
        {
            this.unitIndexer.AddMultiple(units.Where(x => !x.Archive));
        }

        public void CreateScreeningResponseIndexes(IList<ScreeningRequestPersonEntity> responses)
        {
            this.screeningResponseIndexer.AddMultiple(responses.Where(x => !x.Archive));
        }

        public void CreateEventIndexes(IList<Event> events)
        {
            this.eventIndexer.AddMultiple(events.Where(x => !x.Archive));
        }

        public void CreateRequestIndexes(IList<Request> requests)
        {
            this.requestIndexer.AddMultiple(requests.Where(x => !x.Archive));
        }

        public void DeletePersonIndexes()
        {
            this.personIndexer.DeleteIndex();
            log.Info("Deleted person index.");
        }

        public void DeleteUnitIndexes()
        {
            this.unitIndexer.DeleteIndex();
            log.Info("Deleted unit index.");
        }

        public void DeleteSourceIndexes()
        {
            this.sourceIndexer.DeleteIndex();
            log.Info("Deleted source index.");
        }

        public void DeleteScreeningResponseIndexes()
        {
            this.screeningResponseIndexer.DeleteIndex();
            log.Info("Deleted screening response index.");
        }

        public void DeleteEventIndexes()
        {
            this.eventIndexer.DeleteIndex();
            log.Info("Deleted event index.");
        }

        public void DeleteRequestIndexes()
        {
            this.requestIndexer.DeleteIndex();
            log.Info("Deleted request index.");
        }

        public void AddPerson(Person person)
        {
            this.personIndexer.Add<Person>(person);
        }

        public void AddUnit(Unit unit)
        {
            this.unitIndexer.Add<Unit>(unit);
        }

        public void AddResponse(ScreeningRequestPersonEntity response)
        {
            this.screeningResponseIndexer.Add<ScreeningRequestPersonEntity>(response);
        }

        public void AddSource(Source source)
        {
            this.sourceIndexer.Add<Source>(source);
        }

        public void AddEvent(Event ev)
        {
            this.eventIndexer.Add<Event>(ev);
        }

        public void AddRequest(Request request)
        {
            this.requestIndexer.Add<Request>(request);
        }

        public void DeletePerson(int personId)
        {
            this.personIndexer.Delete<Person>(personId);

            // TODO delete that person's screening responses from screeningResponse index.  Realistically unlikely to happen.
        }

        public void DeleteUnit(int unitId)
        {
            this.unitIndexer.Delete<Unit>(unitId);
        }

        public void DeleteSource(int sourceId)
        {
            this.sourceIndexer.Delete<Source>(sourceId);
        }

        public void DeleteResponse(int personId, string screeningEntityName)
        {
            this.screeningResponseIndexer.DeleteResponse(personId, screeningEntityName);
        }

        public void DeleteEvent(int eventId)
        {
            this.eventIndexer.Delete<Event>(eventId);
        }

        public void DeleteRequest(int requestId)
        {
            this.requestIndexer.Delete<Request>(requestId);
        }

        public void UpdatePerson(Person person)
        {
            this.personIndexer.Update<Person>(person);
        }

        public void UpdatePersons(Organization org)
        {
            if (org != null && org.Careers != null)
            {
                foreach (Career c in org.Careers.Where(x => !x.Archive))
                    this.personIndexer.Update<Person>(c.Person);
            }
        }

        public void UpdatePersons(Unit unit)
        {
            if (unit != null && unit.Careers != null)
            {
                foreach (Career c in unit.Careers.Where(x => !x.Archive))
                    this.personIndexer.Update<Person>(c.Person);
            }
        }

        public void UpdatePersons(Rank rank)
        {
            if (rank != null && rank.Careers != null)
            {
                foreach (Career c in rank.Careers.Where(x => !x.Archive))
                    this.personIndexer.Update<Person>(c.Person);
            }
        }

        public void UpdatePersons(Role role)
        {
            if (role != null && role.Careers != null)
            {
                foreach (Career c in role.Careers.Where(x => !x.Archive))
                    this.personIndexer.Update<Person>(c.Person);
            }
        }

        public void UpdatePersons(Location loc)
        {
            if (loc != null && loc.Careers != null)
            {
                var profiler = MiniProfiler.Current;
                using (profiler.Step("Updating person index based on updated Location"))
                {
                    foreach (Career c in loc.Careers.Where(x => !x.Archive))
                        this.personIndexer.Update<Person>(c.Person);
                }
            }
        }

        public void UpdateUnit(Unit unit)
        {
            this.unitIndexer.Update<Unit>(unit);
        }

        public void UpdateUnits(Organization org)
        {
            if (org != null && org.Units != null)
            {
                foreach (Unit u in org.Units.Where(x => !x.Archive))
                    this.unitIndexer.Update<Unit>(u);
            }
        }

        public void UpdateResponse(ScreeningRequestPersonEntity response)
        {
            this.screeningResponseIndexer.UpdateResponse(response);
        }

        public void UpdateSource(Source source)
        {
            this.sourceIndexer.Update<Source>(source);
        }

        public void UpdateEvent(Event ev)
        {
            this.eventIndexer.Update<Event>(ev);
        }

        public void UpdateRequest(Request request)
        {
            this.requestIndexer.Update<Request>(request);
        }

        public IList<LuceneSearchResult> PersonSearch(string term, int numResults, bool includeRestrictedProfiles)
        {
            if (!string.IsNullOrEmpty(term))
                return this.personSearcher.Search(term, numResults, includeRestrictedProfiles);
            else
                return new List<LuceneSearchResult>();
        }

        public IList<LuceneSearchResult> UnitSearch(string term, int numResults)
        {
            if (!string.IsNullOrEmpty(term))
                return this.unitSearcher.Search(term, numResults);
            else
                return new List<LuceneSearchResult>();
        }

        public IList<LuceneSearchResult> SourcePathExactSearch(string prefix, int numResults, 
            bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners, string sortField, bool descending)
        {
            if (!string.IsNullOrEmpty(prefix))
                return this.sourceSearcher.Search(null, prefix, false, null, null, numResults, 
                    canViewAndSearchAll, includeRestrictedSources, uploadedByUserId, owners, sortField, descending);
            else
                return new List<LuceneSearchResult>();
        }

        public IList<LuceneSearchResult> SourcePathPrefixSearch(string prefix, int numResults, 
            bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners, string sortField, bool descending)
        {
            if (!string.IsNullOrEmpty(prefix))
                return this.sourceSearcher.Search(null, prefix, true, null, null, numResults,
                    canViewAndSearchAll, includeRestrictedSources, uploadedByUserId, owners, sortField, descending);
            else
                return new List<LuceneSearchResult>();
        }

        public IList<LuceneSearchResult> GetAllSourcesWithCaseNumbers(string term, int numResults, bool includeRestrictedSources, string sortField, bool descending)
        {
            return this.sourceSearcher.AllSourcesWithCaseNumbers(term, numResults, includeRestrictedSources, sortField, descending);
        }

        public IList<LuceneSearchResult> ScreeningResponseSearch(string term, string screeningEntityName, int numResults)
        {
            if (!string.IsNullOrEmpty(term))
                return this.screeningResponseSearcher.Search(term, screeningEntityName, numResults);
            else
                return new List<LuceneSearchResult>();
        }

        public IList<LuceneSearchResult> SourceSearch(string term, string prefix, int numResults, 
            bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners, string sortField, bool descending)
        {
            return this.SourceSearch(term, prefix, null, null, numResults, 
                canViewAndSearchAll, includeRestrictedSources, uploadedByUserId, owners, sortField, descending);
        }

        public IList<LuceneSearchResult> SourceSearch(string term, string prefix, DateTime? start, DateTime? end, int numResults, 
            bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners, string sortField, bool descending)
        {
            if (!string.IsNullOrEmpty(term) || start.HasValue || end.HasValue)
                return this.sourceSearcher.Search(term, prefix, true, start, end, numResults, 
                    canViewAndSearchAll, includeRestrictedSources, uploadedByUserId, owners, sortField, descending);
            else
                return new List<LuceneSearchResult>();
        }

        public IDictionary<IDictionary<string, string>, long> SourceSearchFacets(string term, string prefix, DateTime? start, DateTime? end, int numResults, 
            bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners)
        {
            if (!string.IsNullOrEmpty(term) || start.HasValue || end.HasValue)
                return this.sourceSearcher.SearchGetFacets(term, prefix, true, start, end, numResults, 
                    canViewAndSearchAll, includeRestrictedSources, uploadedByUserId, owners);
            else
                return null;
        }

        public int GetMaxSourceID()
        {
            return this.sourceSearcher.GetMaxSourceID();
        }

        public IList<LuceneSearchResult> GetSourcesLikeThis(int sourceId, int numResults)
        {
            return this.sourceSearcher.GetSourcesLikeThis(sourceId, numResults);
        }

        public IList<LuceneSearchResult> EventSearch(string term, int numResults, string sortField, bool descending)
        {
            return this.EventSearch(term, null, null, numResults, sortField, descending);
        }

        public IList<LuceneSearchResult> EventSearch(string term, DateTime? start, DateTime? end, int numResults, string sortField, bool descending)
        {
            if (!string.IsNullOrEmpty(term) || start.HasValue || end.HasValue)
                return this.eventSearcher.Search(term, start, end, numResults, sortField, descending);
            else
                return new List<LuceneSearchResult>();
        }

        public IList<LuceneSearchResult> RequestSearch(string term, int numResults, AdminUser user, string sortField, bool descending)
        {
            return this.requestSearcher.Search(term, numResults, user, sortField, descending);
        }

        protected class DatePeriodForSearch
        {
            public DateTime? StartFilter { get; set; }
            public DateTime? EndFilter { get; set; }

            public DatePeriodForSearch(DateTime? start, DateTime? end)
            {
                // search events 1 day on either side of actual specified date
                this.StartFilter = start.HasValue ? start : end;
                if (this.StartFilter.HasValue)
                    this.StartFilter = this.StartFilter.Value.Subtract(TimeSpan.FromDays(1));
                this.EndFilter = end.HasValue ? end : start;
                if (this.EndFilter.HasValue)
                    this.EndFilter = this.EndFilter.Value.Add(TimeSpan.FromDays(1));
            }
        }

        public IList<LuceneSearchResult> FindMatchingEventCandidates(DateTime? start, DateTime? end, string caseCode)
        {
            DatePeriodForSearch dateFilters = new DatePeriodForSearch(start, end);

            // search fields indexed by Lucene for mention of the case code, filtered by the above dates...
            return this.EventSearch(caseCode, dateFilters.StartFilter, dateFilters.EndFilter, 50, "StartDateDisplay", true);
        }

        public IList<LuceneSearchResult> FindMatchingEventCandidates(DateTime? start, DateTime? end, string caseCode,
            string townVillage, string subregion, string region)
        {
            // search Lucene fields filtered by date and case code...
            IList<LuceneSearchResult> candidates = this.FindMatchingEventCandidates(start, end, caseCode);

            DatePeriodForSearch dateFilters = new DatePeriodForSearch(start, end);

            // ...Town/Village
            IList<LuceneSearchResult> townCandidates = null;
            if (!string.IsNullOrEmpty(townVillage))
            {
                townCandidates = this.EventSearch(townVillage, dateFilters.StartFilter, dateFilters.EndFilter, 50, "StartDateDisplay", true);
            }
            if (townCandidates != null && townCandidates.Any())
            {
                candidates = candidates.Concat(townCandidates).ToList();
            }
            else
            {
                // ...Subregion
                IList<LuceneSearchResult> subregionCandidates = null;
                if (!string.IsNullOrEmpty(subregion))
                {
                    subregionCandidates = this.EventSearch(subregion, dateFilters.StartFilter, dateFilters.EndFilter, 50, "StartDateDisplay", true);
                }
                if (subregionCandidates != null && subregionCandidates.Any())
                {
                    candidates = candidates.Concat(subregionCandidates).ToList();
                }
                else
                {
                    // ...Region
                    IList<LuceneSearchResult> regionCandidates = null;
                    if (!string.IsNullOrEmpty(region))
                    {
                        regionCandidates = this.EventSearch(region, dateFilters.StartFilter, dateFilters.EndFilter, 50, "StartDateDisplay", true);
                    }
                    if (regionCandidates != null && regionCandidates.Any())
                    {
                        candidates = candidates.Concat(regionCandidates).ToList();
                    }
                }
            }

            return candidates;
        }
    }
}
