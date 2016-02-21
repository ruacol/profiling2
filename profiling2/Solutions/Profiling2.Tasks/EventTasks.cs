using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using NHibernate;
using Profiling2.Domain.Contracts.Export.Profiling;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Queries.Procs;
using Profiling2.Domain.Contracts.Queries.Search;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks
{
    public class EventTasks : IEventTasks
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(EventTasks));
        protected readonly INHibernateRepository<Event> eventRepo;
        protected readonly INHibernateRepository<Violation> violationRepo;
        protected readonly INHibernateRepository<EventRelationshipType> eventRelationshipTypeRepo;
        protected readonly INHibernateRepository<EventRelationship> eventRelationshipRepo;
        protected readonly INHibernateRepository<AdminExportedEventProfile> exportEventRepo;
        protected readonly INHibernateRepository<Tag> tagRepo;
        protected readonly INHibernateRepository<EventVerifiedStatus> eventVerifiedStatusRepo;
        protected readonly IViolationDataTablesQuery violationDataTablesQuery;
        protected readonly IEventSearchQuery eventSearchQuery;
        protected readonly IEventQueries eventQueries;
        protected readonly IMergeStoredProcQueries mergeQueries;
        protected readonly ILuceneTasks luceneTasks;
        protected readonly IWordExportEventService exportService;

        public EventTasks(INHibernateRepository<Event> eventRepo,
            INHibernateRepository<Violation> violationRepo,
            INHibernateRepository<EventRelationshipType> eventRelationshipTypeRepo,
            INHibernateRepository<EventRelationship> eventRelationshipRepo,
            INHibernateRepository<AdminExportedEventProfile> exportEventRepo,
            INHibernateRepository<Tag> tagRepo,
            INHibernateRepository<EventVerifiedStatus> eventVerifiedStatusRepo,
            IViolationDataTablesQuery violationDataTablesQuery,
            IEventSearchQuery eventSearchQuery,
            IEventQueries eventQueries,
            IMergeStoredProcQueries mergeQueries,
            ILuceneTasks luceneTasks,
            IWordExportEventService exportService)
        {
            this.eventRepo = eventRepo;
            this.violationRepo = violationRepo;
            this.eventRelationshipTypeRepo = eventRelationshipTypeRepo;
            this.eventRelationshipRepo = eventRelationshipRepo;
            this.exportEventRepo = exportEventRepo;
            this.tagRepo = tagRepo;
            this.eventVerifiedStatusRepo = eventVerifiedStatusRepo;
            this.violationDataTablesQuery = violationDataTablesQuery;
            this.eventSearchQuery = eventSearchQuery;
            this.eventQueries = eventQueries;
            this.mergeQueries = mergeQueries;
            this.luceneTasks = luceneTasks;
            this.exportService = exportService;
        }

        public Event GetEvent(int id)
        {
            return this.eventRepo.Get(id);
        }

        public IList<Event> GetAllEvents()
        {
            return this.GetAllEvents(null);
        }

        public IList<Event> GetAllEvents(ISession session)
        {
            return this.eventQueries.GetAllEvents(session);
        }

        public IList<Event> GetEvents(string term)
        {
            return this.eventSearchQuery.GetResults(term).Distinct().ToList();
        }

        public Event SaveEvent(Event e)
        {
            e = this.eventRepo.SaveOrUpdate(e);
            this.luceneTasks.UpdateEvent(e);
            return e;
        }

        public bool DeleteEvent(Event e)
        {
            if (e != null)
            {
                if (e.PersonResponsibilities.Count == 0 && e.OrganizationResponsibilities.Count == 0
                    && e.Tags.Count == 0 && e.EventSources.Count == 0
                    && e.EventRelationshipsAsSubject.Count == 0 && e.EventRelationshipsAsObject.Count == 0
                    && e.ActionTakens.Count == 0)
                {
                    e.AdminExportedEventProfiles.Clear();
                    e.AdminReviewedSources.Clear();
                    e.AdminSuggestionPersonResponsibilities.Clear();

                    this.log.Info("Deleting Event, Id: " + e.Id);
                    this.luceneTasks.DeleteEvent(e.Id);
                    this.eventRepo.Delete(e);
                    return true;
                }
            }
            return false;
        }

        public IList<Event> SearchEventNotes(string term)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Notes", "%" + term + "%");
            return this.eventRepo.FindAll(criteria);
        }

        public Violation GetViolation(int id)
        {
            return this.violationRepo.Get(id);
        }

        public Violation GetViolation(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Name", name);
            return this.violationRepo.FindOne(criteria);
        }

        public int GetViolationDataTablesCount(string term)
        {
            return this.violationDataTablesQuery.GetSearchTotal(term);
        }

        public IList<ViolationDataTableView> GetViolationDataTablesPaginated(int iDisplayStart, int iDisplayLength, string searchText,
            int iSortingCols, IList<int> iSortCol, IList<string> sSortDir)
        {
            return this.violationDataTablesQuery.GetPaginatedResults(iDisplayStart, iDisplayLength, searchText, iSortingCols, iSortCol, sSortDir);
        }

        public Violation SaveViolation(Violation v)
        {
            return this.violationRepo.SaveOrUpdate(v);
        }

        public void DeleteViolation(Violation v)
        {
            // unset children
            foreach (Violation child in v.ChildrenViolations)
                child.ParentViolation = null;
            // unset events
            foreach (Event e in v.Events)
                e.Violations.Remove(v);
            this.violationRepo.Delete(v);
        }

        public IList<Violation> GetViolations()
        {
            return this.violationRepo.GetAll().OrderBy(x => x.Name).ToList<Violation>();
        }

        public IList<Violation> GetRootParentViolations()
        {
            return this.violationRepo.GetAll().Where(x => x.ParentViolation == null).ToList<Violation>();
        }

        public IList<object> GetViolationsJson(int eventId)
        {
            if (eventId > 0)
            {
                IList<object> list = new List<object>();

                Event e = this.GetEvent(eventId);
                if (e != null)
                {
                    foreach (Violation v in e.Violations)
                        list.Add(new { id = v.Id, text = v.Name });
                }

                return list;
            }
            else
            {
                IList<Violation> parents = (from v in this.GetViolations() orderby v.Name where v.ParentViolation == null select v).ToList<Violation>();
                IList<object> root = new List<object>();
                foreach (Violation p in parents)
                    root.Add(this.ViolationToJson(p));
                return root;
            }
        }

        public IList<object> GetViolationsJson(string term)
        {
            return this.GetViolationsJson(term, 0);
        }

        public IList<object> GetViolationsJson(string term, int eventId)
        {
            IList<object> list = new List<object>();
            IList<Violation> violations = null;

            if (eventId > 0 && this.GetEvent(eventId) != null)
            {
                violations = this.GetEvent(eventId).Violations;
            }
            else
            {
                violations = this.GetViolations();
            }

            foreach (Violation v in (from v in violations orderby v.Name where v.Name.ToUpper().Contains(term.ToUpper()) select v))
                list.Add(new { id = v.Id, text = v.Name });

            return list;
        }

        // recursive method used to populate select2 dropdown
        protected object ViolationToJson(Violation v)
        {
            if (v.ChildrenViolations != null && v.ChildrenViolations.Count > 0)
            {
                var node = new
                {
                    id = v.Id,
                    text = v.Name,
                    children = new List<object>()
                };
                foreach (Violation child in v.ChildrenViolations.OrderBy(x => x.Name))
                    node.children.Add(this.ViolationToJson(child));
                return node;
            }
            else
            {
                return new { id = v.Id, text = v.Name };
            }
        }

        public IDictionary<Violation, int> ScoreViolations(string eventName, string[] eventNameSeparators)
        {
            IDictionary<Violation, int> scores = new Dictionary<Violation, int>();
            foreach (Violation v in this.GetViolations())
            {
                IList<int> phraseScores = new List<int>();

                if (!string.IsNullOrEmpty(eventName))
                    foreach (string eventPhrase in eventName.Split(eventNameSeparators, StringSplitOptions.RemoveEmptyEntries))
                        phraseScores.Add(v.ScoreMatchingText(eventPhrase));

                if (phraseScores.Any())
                    scores.Add(v, phraseScores.Min());
            }
            return scores;
        }

        public IList<EventRelationshipType> GetEventRelationshipTypes()
        {
            return this.eventRelationshipTypeRepo.GetAll();
        }

        public EventRelationshipType GetEventRelationshipType(int id)
        {
            return this.eventRelationshipTypeRepo.Get(id);
        }

        public EventRelationship GetEventRelationship(int id)
        {
            return this.eventRelationshipRepo.Get(id);
        }

        public EventRelationship SaveEventRelationship(EventRelationship relationship)
        {
            relationship.SubjectEvent.AddEventRelationshipAsSubject(relationship);
            relationship.ObjectEvent.AddEventRelationshipAsObject(relationship);
            return this.eventRelationshipRepo.SaveOrUpdate(relationship);
        }

        public void DeleteEventRelationship(EventRelationship relationship)
        {
            relationship.SubjectEvent.RemoveEventRelationshipAsSubject(relationship);
            relationship.ObjectEvent.RemoveEventRelationshipAsObject(relationship);
            this.eventRelationshipRepo.Delete(relationship);
        }

        public int MergeEvents(int toKeepEventId, int toDeleteEventId, string userId, bool isProfilingChange)
        {
            int result = this.mergeQueries.MergeEvents(toKeepEventId, toDeleteEventId, userId, isProfilingChange);

            if (result == 1)
            {
                // get Event after merge in order to have updated attributes
                Event toKeep = this.GetEvent(toKeepEventId);

                this.luceneTasks.UpdateEvent(toKeep);
                this.luceneTasks.DeleteUnit(toDeleteEventId);
            }

            return result;
        }

        public byte[] ExportDocument(Event e, DateTime lastModifiedDate, AdminUser user, string clientDnsName, string clientIpAddress, string clientUserAgent)
        {
            // all exports are logged to this table
            AdminExportedEventProfile ex = new AdminExportedEventProfile()
            {
                Event = e,
                ExportDateTime = DateTime.Now,
                ExportedByAdminUser = user,
                ClientDnsName = clientDnsName,
                ClientIpAddress = clientIpAddress,
                ClientUserAgent = clientUserAgent
            };
            this.exportEventRepo.SaveOrUpdate(ex);

            return this.exportService.GetExport(e, lastModifiedDate);
        }

        public void ApproveEvent(Event e, AdminUser user)
        {
            if (e != null)
            {
                EventApproval approval = new EventApproval()
                {
                    Event = e,
                    AdminUser = user,
                    ApprovalDateTime = DateTime.Now
                };
                e.EventApprovals.Add(approval);
                this.SaveEvent(e);
            }
        }

        public IList<Event> GetUnapprovedEvents()
        {
            return this.eventQueries.GetUnapprovedEvents();
        }

        public IList<Tag> GetAllTags()
        {
            return this.tagRepo.GetAll().OrderBy(x => x.TagName).ToList();
        }

        public Tag GetTag(int id)
        {
            return this.tagRepo.Get(id);
        }

        public Tag GetTag(string tagName)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("TagName", tagName);
            return this.tagRepo.FindOne(criteria);
        }

        public Tag SaveTag(Tag tag)
        {
            return this.tagRepo.SaveOrUpdate(tag);
        }

        public void DeleteTag(Tag tag)
        {
            tag.Events.Clear();
            this.tagRepo.Delete(tag);
        }

        public IList<Tag> SearchTags(string term)
        {
            return this.eventSearchQuery.SearchTags(term);
        }

        public IList<EventVerifiedStatus> GetAllEventVerifiedStatuses()
        {
            return this.eventVerifiedStatusRepo.GetAll().OrderBy(x => x.EventVerifiedStatusName).ToList();
        }

        public EventVerifiedStatus GetEventVerifiedStatus(int id)
        {
            return this.eventVerifiedStatusRepo.Get(id);
        }

        public EventVerifiedStatus GetEventVerifiedStatus(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("EventVerifiedStatusName", name);
            return this.eventVerifiedStatusRepo.FindOne(criteria);
        }
    }
}
