using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class EventSourceViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "A source is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid source is required.")]
        public int? SourceId { get; set; }

        [Required(ErrorMessage = "An event is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid event is required.")]
        public int? EventId { get; set; }

        public int? ReliabilityId { get; set; }
        public string Commentary { get; set; }
        public string Notes { get; set; }

        // for display purposes only
        public string SourceName { get; set; }
        public bool SourceArchive { get; set; }
        public bool SourceIsRestricted { get; set; }
        public string EventHeadline { get; set; }
        public string ReliabilityName { get; set; }

        public SelectList Reliabilities { get; set; }

        public EventSourceViewModel() { }

        public EventSourceViewModel(EventSource es)
        {
            this.Id = es.Id;
            this.EventId = es.Event.Id;
            this.EventHeadline = es.Event.Headline;
            if (es.Reliability != null)
            {
                this.ReliabilityId = es.Reliability.Id;
                this.ReliabilityName = es.Reliability.ReliabilityName;
            }
            this.Commentary = es.Commentary;
            this.Notes = es.Notes;
        }

        public EventSourceViewModel(Event e)
        {
            this.EventId = e.Id;
            this.EventHeadline = e.Headline;
        }

        public void PopulateDropDowns(IEnumerable<Reliability> reliabilities)
        {
            this.Reliabilities = new SelectList(reliabilities, "Id", "ReliabilityName", this.ReliabilityId);
        }

        public void PopulateSource(SourceDTO s)
        {
            if (s != null)
            {
                this.SourceId = s.SourceID;
                this.SourceName = s.SourceName;
                this.SourceArchive = s.Archive;
                this.SourceIsRestricted = s.IsRestricted;
            }
        }
    }
}