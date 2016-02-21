using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Contracts.Queries.Procs;
using Profiling2.Domain.Contracts.Queries.Suggestions;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Suggestions;
using Profiling2.Infrastructure.Suggestions;
using SharpArch.NHibernate.Contracts.Repositories;
using StackExchange.Profiling;

namespace Profiling2.Tasks
{
    public class SuggestionTasks : ISuggestionTasks
    {
        protected readonly INHibernateRepository<Event> eventRepo;
        protected readonly INHibernateRepository<AdminSuggestionFeaturePersonResponsibility> asfprRepo;
        protected readonly INHibernateRepository<AdminSuggestionPersonResponsibility> asprRepo;
        protected readonly ISuggestionEventForPersonQuery suggestionQuery;
        protected readonly IPersonRelationshipSuggestionsQuery personRelationshipSuggestionsQuery;
        protected readonly IEventRelationshipSuggestionsQuery eventRelationshipSuggestionsQuery;
        protected readonly IPersonNameSuggestionsQuery personNameSuggestionsQuery;
        protected readonly ISourceSuggestionsQuery sourceSuggestionsQuery;
        protected readonly ICareerLocationSuggestionsQuery careerLocationSuggestionsQuery;
        protected readonly IOrganizationResponsibilitySuggestionsQuery organizationResponsibleSuggestionsQuery;
        protected readonly IEventInSameLocationSuggestionsQuery eventInSameLocationSuggestionsQuery;

        public SuggestionTasks(INHibernateRepository<Event> eventRepo,
            INHibernateRepository<AdminSuggestionFeaturePersonResponsibility> asfprRepo,
            INHibernateRepository<AdminSuggestionPersonResponsibility> asprRepo,
            ISuggestionEventForPersonQuery suggestionQuery,
            IPersonRelationshipSuggestionsQuery personRelationshipSuggestionsQuery,
            IEventRelationshipSuggestionsQuery eventRelationshipSuggestionsQuery,
            IPersonNameSuggestionsQuery personNameSuggestionsQuery,
            ISourceSuggestionsQuery sourceSuggestionsQuery,
            ICareerLocationSuggestionsQuery careerLocationSuggestionsQuery,
            IOrganizationResponsibilitySuggestionsQuery organizationResponsibleSuggestionsQuery,
            IEventInSameLocationSuggestionsQuery eventInSameLocationSuggestionsQuery)
        {
            this.eventRepo = eventRepo;
            this.asfprRepo = asfprRepo;
            this.asprRepo = asprRepo;
            this.suggestionQuery = suggestionQuery;
            this.personRelationshipSuggestionsQuery = personRelationshipSuggestionsQuery;
            this.eventRelationshipSuggestionsQuery = eventRelationshipSuggestionsQuery;
            this.personNameSuggestionsQuery = personNameSuggestionsQuery;
            this.sourceSuggestionsQuery = sourceSuggestionsQuery;
            this.careerLocationSuggestionsQuery = careerLocationSuggestionsQuery;
            this.organizationResponsibleSuggestionsQuery = organizationResponsibleSuggestionsQuery;
            this.eventInSameLocationSuggestionsQuery = eventInSameLocationSuggestionsQuery;
        }

        public int GetSuggestionTotal(int personId)
        {
            return this.suggestionQuery.GetSuggestionTotal(personId);
        }

        public IList<SuggestionEventForPersonDTO> GetSuggestionResults(int iDisplayStart, int iDisplayLength, int personId)
        {
            return this.suggestionQuery.GetPaginatedResults(iDisplayStart, iDisplayLength, personId);
        }

        public AdminSuggestionPersonResponsibility SaveSuggestionPersonResponsibility(AdminSuggestionPersonResponsibility aspr)
        {
            return this.asprRepo.SaveOrUpdate(aspr);
        }

        // Counts from AdminSuggestionPersonResponsibility of how many times each feature has been accepted or declined.
        // Used to weigh future suggestions.
        public IList<FeatureProbabilityCalc> CountSuggestedFeatures()
        {
            IList<FeatureProbabilityCalc> probs = new List<FeatureProbabilityCalc>();

            // TODO won't scale once AdminSuggestionPersonResponsibility table grows
            IList<AdminSuggestionPersonResponsibility> suggestions = this.asprRepo.GetAll().Where(x => !x.Archive && x.SuggestedFeaturesList != null).ToList();

            // filter each of 19 features...
            foreach (AdminSuggestionFeaturePersonResponsibility f in this.asfprRepo.GetAll())
            {
                IEnumerable<AdminSuggestionPersonResponsibility> matchingSuggestions = suggestions.Where(x => x.IncludesFeatureID(f.Id));
                probs.Add(
                    new FeatureProbabilityCalc(
                        f, 
                        matchingSuggestions.Where(x => x.IsAccepted).Count(), 
                        matchingSuggestions.Where(x => !x.IsAccepted).Count()
                    )
                );
            }

            return probs;
        }

        public IDictionary<string, AdminSuggestionFeaturePersonResponsibility> GetAdminSuggestionFeaturePersonResponsibilities()
        {
            IDictionary<string, AdminSuggestionFeaturePersonResponsibility> dict = new Dictionary<string, AdminSuggestionFeaturePersonResponsibility>();
            foreach (AdminSuggestionFeaturePersonResponsibility asfpr in this.asfprRepo.GetAll().Where(x => !x.Archive))
                dict[asfpr.Code] = asfpr;
            return dict;
        }

        protected IList<SuggestedFeatureWithReason> GetPersonRelationshipSuggestions(Person p)
        {
            PersonRelationshipSuggestions s = new PersonRelationshipSuggestions(p, this.personRelationshipSuggestionsQuery.GetPersonRelationships(p));
            return s.Suggestions;
        }

        protected IList<SuggestedFeatureWithReason> GetEventRelationshipSuggestions(Person p)
        {
            EventRelationshipSuggestions s = new EventRelationshipSuggestions(p, this.eventRelationshipSuggestionsQuery.GetEventRelationships(p));
            return s.Suggestions;
        }

        protected IList<SuggestedFeatureWithReason> GetLastNameSuggestions(Person p)
        {
            LastNameSuggestions s = new LastNameSuggestions(p, this.personNameSuggestionsQuery.GetEventsByLastName(p));
            return s.Suggestions;
        }

        protected IList<SuggestedFeatureWithReason> GetFirstNameSuggestions(Person p)
        {
            FirstNameSuggestions s = new FirstNameSuggestions(p, this.personNameSuggestionsQuery.GetEventsByFirstName(p));
            return s.Suggestions;
        }

        protected IList<SuggestedFeatureWithReason> GetSourceSuggestions(Person p)
        {
            SourceSuggestions s = new SourceSuggestions(p, this.sourceSuggestionsQuery.GetEventSources(p));
            return s.Suggestions;
        }

        protected IList<SuggestedFeatureWithReason> GetAliasSuggestions(Person p)
        {
            AliasSuggestions s = new AliasSuggestions(p, this.personNameSuggestionsQuery.GetEventsByAlias(p));
            return s.Suggestions;
        }

        protected IList<SuggestedFeatureWithReason> GetCareerLocationSuggestions(Person p)
        {
            CareerLocationSuggestions s = new CareerLocationSuggestions(p, this.careerLocationSuggestionsQuery.GetEvents(p));
            return s.Suggestions;
        }

        protected IList<SuggestedFeatureWithReason> GetCareerInResponsibleOrganizationSuggestions(Person p)
        {
            CareerInResponsibleOrganizationSuggestions s = new CareerInResponsibleOrganizationSuggestions(p, this.organizationResponsibleSuggestionsQuery.GetOrganizationResponsibilitiesLinkedByOrganization(p));
            return s.Suggestions;
        }

        protected IList<SuggestedFeatureWithReason> GetCareerInResponsibleUnitSuggestions(Person p)
        {
            CareerInResponsibleUnitSuggestions s = new CareerInResponsibleUnitSuggestions(p, this.organizationResponsibleSuggestionsQuery.GetOrganizationResponsibilitiesLinkedByUnit(p));
            return s.Suggestions;
        }

        protected IList<SuggestedFeatureWithReason> GetEventInSameLocationSuggestions(Person p)
        {
            EventInSameLocationSuggestions s = new EventInSameLocationSuggestions(p, this.eventInSameLocationSuggestionsQuery.GetEvents(p));
            return s.Suggestions;
        }

        public IList<SuggestionEventForPersonDTO> GetSuggestionsRefactored(Person p, IList<int> enabledIds)
        {
            var profiler = MiniProfiler.Current;

            Suggester s = new Suggester();

            IDictionary<string, AdminSuggestionFeaturePersonResponsibility> features = this.GetAdminSuggestionFeaturePersonResponsibilities();

            using(profiler.Step("set counts of previous suggestions"))
                s.SetFeatureProbabilityCalcs(this.CountSuggestedFeatures());

            using (profiler.Step("get person relationship suggestions"))
                s.AddSuggestedFeatures(this.GetPersonRelationshipSuggestions(p));

            if (enabledIds.Contains(features[AdminSuggestionFeaturePersonResponsibility.RESPONSIBLE_FOR_RELATED_EVENT].Id))
                using (profiler.Step("get event relationship suggestions"))
                    s.AddSuggestedFeatures(this.GetEventRelationshipSuggestions(p));

            if (enabledIds.Contains(features[AdminSuggestionFeaturePersonResponsibility.LAST_NAME_APPEARS].Id))
                using (profiler.Step("get last name suggestions"))
                    s.AddSuggestedFeatures(this.GetLastNameSuggestions(p));

            if (enabledIds.Contains(features[AdminSuggestionFeaturePersonResponsibility.FIRST_NAME_APPEARS].Id))
                using (profiler.Step("get first name suggestions"))
                    s.AddSuggestedFeatures(this.GetFirstNameSuggestions(p));

            if (enabledIds.Contains(features[AdminSuggestionFeaturePersonResponsibility.ALIAS_APPEARS].Id))
                using (profiler.Step("get alias suggestions"))
                    s.AddSuggestedFeatures(this.GetAliasSuggestions(p));

            if (enabledIds.Contains(features[AdminSuggestionFeaturePersonResponsibility.COMMON_SOURCE].Id))
                using (profiler.Step("get shared source suggestions"))
                    s.AddSuggestedFeatures(this.GetSourceSuggestions(p));

            if (enabledIds.Contains(features[AdminSuggestionFeaturePersonResponsibility.CAREER_IN_LOCATION].Id))
                using (profiler.Step("get career location suggestions"))
                    s.AddSuggestedFeatures(this.GetCareerLocationSuggestions(p));

            if (enabledIds.Contains(features[AdminSuggestionFeaturePersonResponsibility.CAREER_IN_ORG_RESPONSIBLE].Id))
                using (profiler.Step("get career in responsible org suggestions"))
                    s.AddSuggestedFeatures(this.GetCareerInResponsibleOrganizationSuggestions(p));

            if (enabledIds.Contains(features[AdminSuggestionFeaturePersonResponsibility.CAREER_IN_UNIT_RESPONSIBLE].Id))
                using (profiler.Step("get career in responsible unit suggestions"))
                    s.AddSuggestedFeatures(this.GetCareerInResponsibleUnitSuggestions(p));

            if (enabledIds.Contains(features[AdminSuggestionFeaturePersonResponsibility.RESPONSIBILITY_IN_LOCATION].Id))
                using (profiler.Step("get event in same location suggestions"))
                    s.AddSuggestedFeatures(this.GetEventInSameLocationSuggestions(p));

            IList<SuggestionEventForPersonDTO> dtos;
            using (profiler.Step("Suggester.Suggest"))
                dtos = s.Suggest(p);

            return dtos;
        }
    }
}
