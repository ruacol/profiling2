using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AutoMapper;
using Profiling2.Domain.Prf.Events;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class EventViewModel : PeriodBaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "At least one event category is required for this event.")]
        public string ViolationIds { get; set; }

        [AllowHtml]
        public string NarrativeEn { get; set; }

        [AllowHtml]
        public string NarrativeFr { get; set; }

        [Required(ErrorMessage = "A location is required for this event.")]
        public int? LocationId { get; set; }

        public string Notes { get; set; }

        public int? EventVerifiedStatusId { get; set; }

        public string JhroCaseIds { get; set; }

        public string TagIds { get; set; }

        public SelectList EventVerifiedStatuses { get; set; }

        // this property just used to populate initial selected values in view
        public IList<Violation> Violations { get; set; }

        // this property just used to populate initial selected value in view
        public string LocationText { get; set; }

        // display only (for events with no categories)
        public string EventName { get; set; }

        public EventViewModel() { }

        public EventViewModel(Event e)
        {
            Mapper.Map(e, this);
        }

        public void PopulateDropDowns(IEnumerable<EventVerifiedStatus> statuses)
        {
            this.EventVerifiedStatuses = new SelectList(statuses, "Id", "EventVerifiedStatusName", this.EventVerifiedStatusId);
        }
    }
}