using System.Collections.Generic;
using System.Linq;
using HrdbWebServiceClient.Domain;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.DTO;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Tasks
{
    public class EventMatchingTasks : IEventMatchingTasks
    {
        protected readonly IEventTasks eventTasks;
        protected readonly ILuceneTasks luceneTasks;
        protected readonly ISourceAttachmentTasks sourceAttachmentTasks;
        protected readonly ILocationTasks locationTasks;
        protected readonly IJhroCaseQueries jhroCaseQueries;
        protected readonly IPersonTasks personTasks;
        protected readonly IOrganizationTasks orgTasks;

        public EventMatchingTasks(IEventTasks eventTasks,
            ILuceneTasks luceneTasks,
            ISourceAttachmentTasks sourceAttachmentTasks,
            ILocationTasks locationTasks,
            IJhroCaseQueries jhroCaseQueries,
            IPersonTasks personTasks,
            IOrganizationTasks orgTasks)
        {
            this.eventTasks = eventTasks;
            this.luceneTasks = luceneTasks;
            this.sourceAttachmentTasks = sourceAttachmentTasks;
            this.locationTasks = locationTasks;
            this.jhroCaseQueries = jhroCaseQueries;
            this.personTasks = personTasks;
            this.orgTasks = orgTasks;
        }

        public IDictionary<string, IList<object>> FindMatchingEventCandidates(JhroCase jhroCase, HrdbCase hrdbCase)
        {
            IDictionary<string, IList<object>> results = new Dictionary<string, IList<object>>();

            string caseCode = jhroCase != null ? jhroCase.CaseNumber : (hrdbCase != null ? hrdbCase.CaseCode : null);

            // search Event.Notes field for mention of the case code
            results["CodeInEventNotes"] = this.eventTasks.SearchEventNotes(caseCode).Select(x => x.ToShortHeadlineJSON()).ToList();

            // search EventSource commentaries and notes for mention of the case code
            results["CodeInEventSourceCommentary"] = this.sourceAttachmentTasks.SearchEventSources(caseCode)
                .Select(x => x.Event)
                .Distinct()
                .Select(x => x.ToShortHeadlineJSON())
                .ToList();
            
            // search fields indexed by Lucene
            if (hrdbCase != null)
                results["DateAndLocation"] = this.luceneTasks
                    .FindMatchingEventCandidates(hrdbCase.StartDate.HasValue ? hrdbCase.StartDate : null, 
                        hrdbCase.EndDate.HasValue ? hrdbCase.EndDate : null, 
                        hrdbCase.CaseCode, hrdbCase.TownVillage, hrdbCase.Subregion, hrdbCase.Region)
                    .Select(x => this.eventTasks.GetEvent(new EventDataTableView(x).Id).ToShortHeadlineJSON())
                    .Distinct()
                    .ToList();

            return results;
        }

        public IList<object> FindSimilarEvents(Event e)
        {
            IList<Event> results = new List<Event>();

            results = this.luceneTasks.FindMatchingEventCandidates(e.HasStartDate() ? e.GetStartDateTime() : null, 
                    e.HasEndDate() ? e.GetEndDateTime() : null, 
                    e.GetJhroCaseNumber())
                .Select(x => this.eventTasks.GetEvent(new EventDataTableView(x).Id))
                .Distinct()
                .ToList();

            return results.Where(x => x.Id != e.Id)  // don't include self
                .Where(x => e.GetViolationSimilarity(x) >= 0.5)  // filter out matches with dissimilar violations
                .Where(x => e.GetLocationSimilarity(x) >= 0.4)  // filter out locations
                .Select(x => x.ToShortHeadlineJSON())
                .ToList();
        }
    }
}
