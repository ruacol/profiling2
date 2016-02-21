using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Profiling2.Domain.Prf.Events;
using System.Collections.Generic;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class EventRelationshipViewModel : PeriodBaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A subject event is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid subject event is required.")]
        public int? SubjectEventId { get; set; }

        [Required(ErrorMessage = "An object event is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid object event is required.")]
        public int? ObjectEventId { get; set; }

        [Required(ErrorMessage = "A relationship type is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid relationship type is required.")]
        public int EventRelationshipTypeId { get; set; }

        public bool Archive { get; set; }
        public string Notes { get; set; }

        // for form population purposes only
        public SelectList EventRelationshipTypeList { get; set; }

        public EventRelationshipViewModel() { }

        public EventRelationshipViewModel(EventRelationship er)
        {
            if (er != null)
            {
                this.Id = er.Id;
                if (er.SubjectEvent != null)
                    this.SubjectEventId = er.SubjectEvent.Id;
                if (er.ObjectEvent != null)
                    this.ObjectEventId = er.ObjectEvent.Id;
                if (er.EventRelationshipType != null)
                    this.EventRelationshipTypeId = er.EventRelationshipType.Id;
                this.Archive = er.Archive;
                this.Notes = er.Notes;
            }
        }

        public void PopulateDropDowns(IEnumerable<EventRelationshipType> types)
        {
            if (this.EventRelationshipTypeId > 0)
                this.EventRelationshipTypeList = new SelectList(types, "Id", "EventRelationshipTypeName", this.EventRelationshipTypeId);
            else
                this.EventRelationshipTypeList = new SelectList(types, "Id", "EventRelationshipTypeName");
        }
    }
}