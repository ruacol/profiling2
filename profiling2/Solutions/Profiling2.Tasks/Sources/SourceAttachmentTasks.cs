using System;
using System.Collections.Generic;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks.Sources
{
    public class SourceAttachmentTasks : ISourceAttachmentTasks
    {
        protected readonly INHibernateRepository<PersonSource> personSourceRepo;
        protected readonly INHibernateRepository<EventSource> eventSourceRepo;
        protected readonly INHibernateRepository<OrganizationSource> orgSourceRepo;
        protected readonly INHibernateRepository<UnitSource> unitSourceRepo;
        protected readonly INHibernateRepository<OperationSource> operationSourceRepo;
        protected readonly INHibernateRepository<AdminSourceSearch> adminSourceSearchRepository;
        protected readonly INHibernateRepository<AdminReviewedSource> adminReviewedSourceRepository;
        protected readonly ISourceCollectionsQuery sourceCollectionsQuery;
        protected readonly IAdminSourceSearchQuery adminSourceSearchQuery;
        protected readonly ISourceTasks sourceTasks;
        protected readonly IPersonTasks personTasks;

        public SourceAttachmentTasks(INHibernateRepository<PersonSource> personSourceRepo,
            INHibernateRepository<EventSource> eventSourceRepo,
            INHibernateRepository<OrganizationSource> orgSourceRepo,
            INHibernateRepository<UnitSource> unitSourceRepo,
            INHibernateRepository<OperationSource> operationSourceRepo,
            INHibernateRepository<AdminSourceSearch> adminSourceSearchRepository,
            INHibernateRepository<AdminReviewedSource> adminReviewedSourceRepository,
            ISourceCollectionsQuery sourceCollectionsQuery,
            IAdminSourceSearchQuery adminSourceSearchQuery,
            ISourceTasks sourceTasks,
            IPersonTasks personTasks)
        {
            this.personSourceRepo = personSourceRepo;
            this.eventSourceRepo = eventSourceRepo;
            this.orgSourceRepo = orgSourceRepo;
            this.unitSourceRepo = unitSourceRepo;
            this.operationSourceRepo = operationSourceRepo;
            this.adminSourceSearchRepository = adminSourceSearchRepository;
            this.adminReviewedSourceRepository = adminReviewedSourceRepository;
            this.sourceCollectionsQuery = sourceCollectionsQuery;
            this.adminSourceSearchQuery = adminSourceSearchQuery;
            this.sourceTasks = sourceTasks;
            this.personTasks = personTasks;
        }

        // AdminSourceSearches

        public AdminSourceSearch GetAdminSourceSearch(int adminSourceSearchId)
        {
            return this.adminSourceSearchRepository.Get(adminSourceSearchId);
        }

        public AdminSourceSearch SaveOrUpdateAdminSourceSearch(AdminSourceSearch adminSourceSearch)
        {
            return this.adminSourceSearchRepository.SaveOrUpdate(adminSourceSearch);
        }

        public IList<int> GetAdminSourceSearchIds(AdminSourceSearch adminSourceSearch)
        {
            return this.adminSourceSearchQuery.GetAdminSourceSearchIds(adminSourceSearch);
        }

        // AdminReviewedSources

        public AdminReviewedSource GetOrCreateAdminReviewedSource(int sourceId, int adminSourceSearchId)
        {
            Source source = this.sourceTasks.GetSource(sourceId);
            AdminSourceSearch ass = this.adminSourceSearchRepository.Get(adminSourceSearchId);

            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Source", source);
            criteria.Add("AdminSourceSearch", ass);
            AdminReviewedSource ars = this.adminReviewedSourceRepository.FindOne(criteria);
            if (ars == null)
            {
                // TODO when creating a new Reviewed record, this source might be marked relevant in another
                // equivalent search.  However this new Review will be more recent.
                // We could prompt the user to remind them to mark it as relevant when they review the source...
                ars = new AdminReviewedSource();
                ars.ReviewedDateTime = DateTime.Now;
                ars.Source = source;
                ars.AdminSourceSearch = ass;
                ars.Archive = false;
                ars = this.adminReviewedSourceRepository.SaveOrUpdate(ars);
            }
            return ars;
        }

        public IList<AdminReviewedSource> GetReviewsForSource(int sourceId)
        {
            return this.sourceCollectionsQuery.GetReviewsForSource(sourceId);
        }

        // AdminSourceImports

        public IList<AdminSourceImport> GetAdminImportsForSource(int sourceId)
        {
            return this.sourceCollectionsQuery.GetAdminImportsForSource(sourceId);
        }

        // SourceRelationships

        public SourceRelationship GetParentSourceOf(int sourceId)
        {
            return this.sourceCollectionsQuery.GetParentSourceOf(sourceId);
        }

        public IList<SourceRelationship> GetChildSourcesOf(int sourceId)
        {
            return this.sourceCollectionsQuery.GetChildSourcesOf(sourceId);
        }

        // PersonSources

        public PersonSource GetPersonSource(int id)
        {
            return this.personSourceRepo.Get(id);
        }

        public IList<PersonSource> GetPersonSources(int sourceId)
        {
            return this.sourceCollectionsQuery.GetPersonSources(sourceId);
        }

        public IList<PersonSource> GetPersonSources(Person person, Source source)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Person", person);
            criteria.Add("Source", source);
            return this.personSourceRepo.FindAll(criteria);
        }

        public PersonSource SavePersonSource(PersonSource ps)
        {
            ps.Person.AddPersonSource(ps);
            ps.Source.AddPersonSource(ps);

            if (!ps.Person.HasValidProfileStatus())
            {
                ps.Person.ProfileStatus = this.personTasks.GetProfileStatus(ProfileStatus.ROUGH_OUTLINE);
                this.personTasks.SavePerson(ps.Person);
            }

            return this.personSourceRepo.SaveOrUpdate(ps);
        }

        public void DeletePersonSource(int id)
        {
            this.DeletePersonSource(this.personSourceRepo.Get(id));
        }

        private void DeletePersonSource(PersonSource ps)
        {
            if (ps != null)
            {
                ps.Source.RemovePersonSource(ps);
                ps.Person.RemovePersonSource(ps);
                this.personSourceRepo.Delete(ps);
            }
        }

        // EventSources

        public EventSource GetEventSource(int id)
        {
            return this.eventSourceRepo.Get(id);
        }

        public IList<EventSource> GetEventSources(int sourceId)
        {
            return this.sourceCollectionsQuery.GetEventSources(sourceId);
        }

        public IList<EventSource> GetEventSources(Event ev, Source source)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Event", ev);
            criteria.Add("Source", source);
            return this.eventSourceRepo.FindAll(criteria);
        }

        public IList<EventSource> SearchEventSources(string term)
        {
            return this.sourceCollectionsQuery.SearchEventSources(term);
        }

        public EventSource SaveEventSource(EventSource es)
        {
            es.Event.AddEventSource(es);
            es.Source.AddEventSource(es);
            return this.eventSourceRepo.SaveOrUpdate(es);
        }

        public void DeleteEventSource(int id)
        {
            this.DeleteEventSource(this.eventSourceRepo.Get(id));
        }

        public void DeleteEventSource(EventSource es)
        {
            if (es != null)
            {
                es.Source.RemoveEventSource(es);
                es.Event.RemoveEventSource(es);
                this.eventSourceRepo.Delete(es);
            }
        }

        // OrganizationSources

        public IList<OrganizationSource> GetOrganizationSources(Organization org, Source source)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Organization", org);
            criteria.Add("Source", source);
            return this.orgSourceRepo.FindAll(criteria);
        }

        public OrganizationSource SaveOrganizationSource(OrganizationSource os)
        {
            os.Organization.AddOrganizationSource(os);
            os.Source.AddOrganizationSource(os);
            return this.orgSourceRepo.SaveOrUpdate(os);
        }

        public void DeleteOrganizationSource(int id)
        {
            this.DeleteOrganizationSource(this.orgSourceRepo.Get(id));
        }

        private void DeleteOrganizationSource(OrganizationSource os)
        {
            if (os != null)
            {
                os.Source.RemoveOrganizationSource(os);
                os.Organization.RemoveOrganizationSource(os);
                this.orgSourceRepo.Delete(os);
            }
        }

        // UnitSources

        public UnitSource GetUnitSource(int id)
        {
            return this.unitSourceRepo.Get(id);
        }

        public IList<UnitSource> GetUnitSources(int sourceId)
        {
            return this.sourceCollectionsQuery.GetUnitSources(sourceId);
        }

        public UnitSource SaveUnitSource(UnitSource us)
        {
            us.Unit.AddUnitSource(us);
            us.Source.AddUnitSource(us);
            return this.unitSourceRepo.SaveOrUpdate(us);
        }

        public void DeleteUnitSource(int id)
        {
            this.DeleteUnitSource(this.unitSourceRepo.Get(id));
        }

        private void DeleteUnitSource(UnitSource us)
        {
            if (us != null)
            {
                us.Source.RemoveUnitSource(us);
                us.Unit.RemoveUnitSource(us);
                this.unitSourceRepo.Delete(us);
            }
        }

        // OperationSources

        public OperationSource GetOperationSource(int id)
        {
            return this.operationSourceRepo.Get(id);
        }

        public IList<OperationSource> GetOperationSources(int sourceId)
        {
            return this.sourceCollectionsQuery.GetOperationSources(sourceId);
        }

        public OperationSource SaveOperationSource(OperationSource us)
        {
            us.Operation.AddOperationSource(us);
            us.Source.AddOperationSource(us);
            return this.operationSourceRepo.SaveOrUpdate(us);
        }

        public void DeleteOperationSource(int id)
        {
            this.DeleteOperationSource(this.operationSourceRepo.Get(id));
        }

        private void DeleteOperationSource(OperationSource os)
        {
            if (os != null)
            {
                os.Source.RemoveOperationSource(os);
                os.Operation.RemoveOperationSource(os);
                this.operationSourceRepo.Delete(os);
            }
        }
    }
}
