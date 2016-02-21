using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Profiling2.Domain.Prf.Actions;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Web.Mvc.Helpers;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class PersonResponsibilityViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A person is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid person is required.")]
        public int? PersonId { get; set; }

        [Required(ErrorMessage = "An event is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid event is required.")]
        public int? EventId { get; set; }

        [Required(ErrorMessage = "At least one event category is required.")]
        public string ViolationIds { get; set; }

        [Required(ErrorMessage = "A responsibility type.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid responsibility type is required.")]
        public int? PersonResponsibilityTypeId { get; set; }

        public string Commentary { get; set; }
        public bool Archive { get; set; }
        public string Notes { get; set; }

        // for display purposes
        public string PersonName { get; set; }
        public string PersonResponsibilityTypeName { get; set; }
        public string PersonFunctionUnitSummary { get; set; }
        // linked to event
        public IList<ViolationViewModel> EventCategories { get; set; }
        public string EventHeadline { get; set; }
        public string EventStartDate { get; set; }
        public string EventEndDate { get; set; }
        public int EventLocationId { get; set; }
        public string EventLocationName { get; set; }
        public string EventLocationFullName { get; set; }
        public string EventNarrative { get; set; }
        public string EventVerifiedStatusName { get; set; }
        public IList<ActionTakenViewModel> ActionsTaken { get; set; }
        public IList<object> OthersResponsible { get; set; }
        public IList<EventSourceViewModel> EventSources { get; set; }
        public IList<object> RelatedEvents { get; set; }
        // linked to person
        public IList<object> Violations { get; set; }

        public SelectList PersonResponsibilityTypes { get; set; }

        public PersonResponsibilityViewModel()
        {
            this.CreateLists();
        }

        public PersonResponsibilityViewModel(Event e, IList<SourceDTO> sources)
        {
            this.CreateLists();
            this.SetEvent(e, sources);
        }

        public PersonResponsibilityViewModel(Person p)
        {
            this.CreateLists();
            this.SetPerson(p);
        }

        /// <summary>
        /// Instantiate ViewModel using given DTOs.
        /// </summary>
        /// <param name="pr"></param>
        /// <param name="sources">This must include all SourceDTOs attached to the PersonResponsibility's event.</param>
        public PersonResponsibilityViewModel(PersonResponsibility pr, IList<SourceDTO> sources)
        {
            this.CreateLists();
            this.Id = pr.Id;
            this.ViolationIds = string.Join(",", pr.Violations.Select(x => x.Id.ToString()));
            this.Violations = pr.Violations.Select(x => new { Id = x.Id, Name = x.Name }).ToList<object>();
            this.PersonResponsibilityTypeId = pr.PersonResponsibilityType.Id;
            this.Commentary = pr.Commentary;
            this.Archive = pr.Archive;
            this.Notes = pr.Notes;
            this.PersonResponsibilityTypeName = pr.PersonResponsibilityType.ToString();
            this.PersonFunctionUnitSummary = pr.GetPersonFunctionUnitSummary();
            this.SetPerson(pr.Person);
            this.SetEvent(pr.Event, sources);
        }

        protected void CreateLists()
        {
            this.EventCategories = new List<ViolationViewModel>();
            this.ActionsTaken = new List<ActionTakenViewModel>();
            this.OthersResponsible = new List<object>();
            this.EventSources = new List<EventSourceViewModel>();
            this.RelatedEvents = new List<object>();
            this.Violations = new List<object>();
        }

        protected void SetEvent(Event e, IList<SourceDTO> sources)
        {
            this.EventId = e.Id;
            this.EventCategories = e.Violations.Select(x => new ViolationViewModel(x)).ToList();
            this.EventHeadline = e.Headline;
            this.EventStartDate = new DateLabel(e.YearOfStart, e.MonthOfStart, e.DayOfStart, false).ToString();
            this.EventEndDate = new DateLabel(e.YearOfEnd, e.MonthOfEnd, e.DayOfEnd, false).ToString();
            this.EventLocationId = e.Location.Id;
            this.EventLocationName = e.Location.LocationName;
            this.EventLocationFullName = e.Location.ToString();
            this.EventNarrative = !string.IsNullOrEmpty(e.NarrativeEn) ? e.NarrativeEn : e.NarrativeFr;
            this.EventVerifiedStatusName = e.EventVerifiedStatus != null ? e.EventVerifiedStatus.EventVerifiedStatusName : null;

            foreach (ActionTaken action in e.ActionTakens)
                this.ActionsTaken.Add(new ActionTakenViewModel(action));
            foreach (PersonResponsibility responsibility in e.PersonResponsibilities)
                if (responsibility.Person.Id != this.PersonId)
                    this.OthersResponsible.Add(new
                    {
                        Id = responsibility.Id,
                        PersonId = responsibility.Person.Id,
                        PersonName = responsibility.Person.Name,
                        PersonFunctionUnitSummary = responsibility.GetPersonFunctionUnitSummary(),
                        EventId = responsibility.Event.Id,
                        Violations = responsibility.Violations.Select(x => new { Id = x.Id, Name = x.Name }).ToList<object>(),
                        PersonResponsibilityTypeName = responsibility.PersonResponsibilityType.ToString(),
                        Commentary = responsibility.Commentary
                    });
            foreach (EventSource es in e.EventSources)
            {
                EventSourceViewModel esvm = new EventSourceViewModel(es);
                esvm.PopulateSource(sources.Where(x => x.SourceID == es.Source.Id).First());
                this.EventSources.Add(esvm);
            }
            foreach (EventRelationship er in e.EventRelationshipsAsSubject)
            {
                this.RelatedEvents.Add(new
                {
                    Id = er.Id,
                    ObjectId = er.ObjectEvent.Id,
                    Headline = er.ObjectEvent.Headline,
                    Type = er.EventRelationshipType.ToString(),
                    Notes = er.Notes,
                    EventVerifiedStatusName = er.ObjectEvent.EventVerifiedStatus != null ? er.ObjectEvent.EventVerifiedStatus.EventVerifiedStatusName : null
                });
            }
            foreach (EventRelationship er in e.EventRelationshipsAsObject)
            {
                this.RelatedEvents.Add(new
                {
                    Id = er.Id,
                    SubjectId = er.SubjectEvent.Id,
                    Headline = er.SubjectEvent.Headline,
                    Type = er.EventRelationshipType.ToString(),
                    Notes = er.Notes,
                    EventVerifiedStatusName = er.SubjectEvent.EventVerifiedStatus != null ? er.SubjectEvent.EventVerifiedStatus.EventVerifiedStatusName : null
                });
            }
        }

        protected void SetPerson(Person p)
        {
            this.PersonId = p.Id;
            this.PersonName = p.Name;
        }

        public void PopulateDropDowns(IEnumerable<PersonResponsibilityType> personResponsibilityTypes)
        {
            this.PersonResponsibilityTypes = new SelectList(personResponsibilityTypes, "Id", "PersonResponsibilityTypeName", this.PersonResponsibilityTypeId);
        }
    }
}