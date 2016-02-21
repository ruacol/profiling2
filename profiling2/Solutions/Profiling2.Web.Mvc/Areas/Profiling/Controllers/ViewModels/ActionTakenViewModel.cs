using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AutoMapper;
using Profiling2.Domain.Prf.Actions;
using Profiling2.Domain.Prf.Events;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class ActionTakenViewModel : PeriodBaseViewModel
    {
        public int Id { get; set; }
        public int? SubjectPersonId { get; set; }
        public int? ObjectPersonId { get; set; }

        [Required(ErrorMessage = "An action taken is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid action taken is required.")]
        public int ActionTakenTypeId { get; set; }

        [Required(ErrorMessage = "An event is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid event is required.")]
        public int EventId { get; set; }

        public string Commentary { get; set; }
        public string Notes { get; set; }
        public bool Archive { get; set; }

        public SelectList ActionTakenTypes { get; set; }

        // for display purposes only
        public string EventHeadline { get; set; }
        public string SubjectPersonName { get; set; }
        public string ObjectPersonName { get; set; }
        public string ActionTakenTypeName { get; set; }
        public bool ActionTakenTypeIsRemedial { get; set; }
        public bool ActionTakenTypeIsDisciplinary { get; set; }
        public string ActionTakenSummary { get; set; }

        public ActionTakenViewModel() { }

        public ActionTakenViewModel(Event e)
        {
            this.EventId = e.Id;
            this.EventHeadline = e.Headline;
        }

        public ActionTakenViewModel(ActionTaken at)
        {
            this.Id = at.Id;
            if (at.SubjectPerson != null)
            {
                this.SubjectPersonId = at.SubjectPerson.Id;
                this.SubjectPersonName = at.SubjectPerson.Name;
            }
            if (at.ObjectPerson != null)
            {
                this.ObjectPersonId = at.ObjectPerson.Id;
                this.ObjectPersonName = at.ObjectPerson.Name;
            }
            this.ActionTakenTypeId = at.ActionTakenType.Id;
            this.ActionTakenTypeName = at.ActionTakenType.ToString();
            this.ActionTakenTypeIsRemedial = at.ActionTakenType.IsRemedial;
            this.ActionTakenTypeIsDisciplinary = at.ActionTakenType.IsDisciplinary;
            this.ActionTakenSummary = at.GetCompleteSummary();
            this.EventId = at.Event.Id;
            this.EventHeadline = at.Event.Headline;
            Mapper.Map(at, this);
        }

        public void PopulateDropDowns(IEnumerable<ActionTakenType> actionTakenTypes)
        {
            this.ActionTakenTypes = new SelectList(actionTakenTypes, "Id", "ActionTakenTypeName", this.ActionTakenTypeId);
        }
    }
}