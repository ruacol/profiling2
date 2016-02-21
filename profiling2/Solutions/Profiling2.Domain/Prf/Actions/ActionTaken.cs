using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Extensions;
using SharpArch.Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;

namespace Profiling2.Domain.Prf.Actions
{
    public class ActionTaken : Entity, IIncompleteDate
    {
        [Audited]
        public virtual Person SubjectPerson { get; set; }
        [Audited]
        public virtual Person ObjectPerson { get; set; }
        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual ActionTakenType ActionTakenType { get; set; }
        [Audited]
        public virtual Event Event { get; set; }
        [Audited]
        public virtual int DayOfStart { get; set; }
        [Audited]
        public virtual int MonthOfStart { get; set; }
        [Audited]
        public virtual int YearOfStart { get; set; }
        [Audited]
        public virtual int DayOfEnd { get; set; }
        [Audited]
        public virtual int MonthOfEnd { get; set; }
        [Audited]
        public virtual int YearOfEnd { get; set; }
        [Audited]
        public virtual string Commentary { get; set; }
        [Audited]
        public virtual string Notes { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }

        public virtual bool IsOtherType()
        {
            return this.ActionTakenType != null && string.Equals(this.ActionTakenType.ActionTakenTypeName, "Other", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Summarise this ActionTaken as "[Subject] [ActionTakenType] [Object]".
        /// 
        /// Where ActionTakenType=Other, Commentary is used in place of ActionTakenType.
        /// </summary>
        /// <returns></returns>
        public virtual string GetSubjectTypeObjectSummary()
        {
            IList<string> components = new List<string>();

            if (this.SubjectPerson != null)
                components.Add(this.SubjectPerson.Name);
            if (this.ActionTakenType != null)
            {
                if (this.IsOtherType())
                    components.Add(this.Commentary);
                else
                    components.Add(this.ActionTakenType.ActionTakenTypeName);
            }
            if (this.ObjectPerson != null)
                components.Add(this.ObjectPerson.Name);

            return string.Join(" ", (from s in components
                                     where !string.IsNullOrEmpty(s)
                                     select s).ToArray<string>()
                                    );
        }

        /// <summary>
        /// Summary of this ActionTaken, including commentary.
        /// </summary>
        public virtual string GetCompleteSummary()
        {
            string summary = this.GetSubjectTypeObjectSummary();
            if (string.IsNullOrEmpty(summary))
            {
                return this.Commentary;
            }
            else
            {
                if (this.IsOtherType())
                    return summary;
                else
                    return summary + "." + this.Commentary;
            }
        }

        public virtual string GetTimelineSlideText()
        {
            string s = "<em>In relation to " + this.Event.Headline + "</em>";
            s += "<p>";
            s += this.GetSubjectTypeObjectSummary();
            s += "</p>";
            s += !string.IsNullOrEmpty(this.Commentary) ? "<p>" + this.Commentary + "</p>" : string.Empty;
            return s;
        }

        /// <summary>
        /// Returns TimelineJS date object.
        /// </summary>
        /// <returns></returns>
        public virtual object GetTimelineSlideObject()
        {
            dynamic o = new ExpandoObject();
            if (this.GetTimelineStartDateObject() != null)
                o.start_date = this.GetTimelineStartDateObject();
            if (this.GetTimelineEndDateObject() != null)
                o.end_date = this.GetTimelineEndDateObject();
            o.text = new
            {
                headline = !string.IsNullOrEmpty(this.ActionTakenType.ActionTakenTypeName) ? this.ActionTakenType.ActionTakenTypeName : "no action taken type",
                text = this.GetTimelineSlideText()
            };
            o.group = "Action taken";
            return o;
        }

        public override string ToString()
        {
            return (this.SubjectPerson != null ? "Subject Person(ID=" + this.SubjectPerson.Id.ToString() + ")" : string.Empty)
                + " ActionTakenType=" + this.ActionTakenType.ToString() + " "
                + (this.ObjectPerson != null ? "Object Person(ID=" + this.ObjectPerson.Id.ToString() + ")" : string.Empty);
        }

        public virtual object ToJSON()
        {
            return new
                {
                    Id = this.Id,
                    SubjectPerson = this.SubjectPerson != null ? this.SubjectPerson.ToJSON() : null,
                    ObjectPerson = this.ObjectPerson != null ? this.ObjectPerson.ToJSON() : null,
                    ActionTakenType = this.ActionTakenType.ToJSON(),
                    EventId = this.Event.Id,
                    StartDate = this.HasStartDate() ? this.GetStartDateString() : null,
                    EndDate = this.HasEndDate() ? this.GetEndDateString() : null,
                    Commentary = this.Commentary,
                    Notes = this.Notes
                };
        }
    }
}
