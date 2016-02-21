using System.Collections.Generic;
using System.Dynamic;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Extensions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Persons
{
    public class PersonRelationship : Entity, IIncompleteDate
    {
        [Audited]
        public virtual Person SubjectPerson { get; set; }
        [Audited]
        public virtual Person ObjectPerson { get; set; }
        [Audited]
        public virtual PersonRelationshipType PersonRelationshipType { get; set; }
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
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }

        public virtual string RelationshipSummary()
        {
            return this.RelationshipSummary(null);
        }

        public virtual string RelationshipSummary(Person p)
        {
            return string.Join(" ", new string[]
            {
                this.SubjectPerson == p ? "(Profiled) " + this.SubjectPerson.Name : this.SubjectPerson.Name,
                this.PersonRelationshipType.PersonRelationshipTypeName,
                this.ObjectPerson == p ? "(profiled) " + this.ObjectPerson.Name : this.ObjectPerson.Name
            });
        }

        protected virtual string GetTimelineHeadlineText(Person p)
        {
            IList<string> parts = new List<string>();
            if (this.SubjectPerson != p)
                parts.Add(this.SubjectPerson.Name);
            parts.Add(this.PersonRelationshipType != null ? this.PersonRelationshipType.PersonRelationshipTypeName : "(no relationship type)");
            if (this.ObjectPerson != p)
                parts.Add(this.ObjectPerson.Name);
            return string.Join(" ", parts);
        }

        /// <summary>
        /// Returns TimelineJS date object.
        /// </summary>
        /// <param name="p">Subject of the timeline, in order not to duplicate text.</param>
        /// <returns></returns>
        public virtual object GetTimelineSlideObject(Person p)
        {
            dynamic o = new ExpandoObject();
            if (this.GetTimelineStartDateObject() != null)
                o.start_date = this.GetTimelineStartDateObject();
            if (this.GetTimelineEndDateObject() != null)
                o.end_date = this.GetTimelineEndDateObject();
            o.text = new
            {
                headline = this.GetTimelineHeadlineText(p),
                text = this.RelationshipSummary() + (!string.IsNullOrEmpty(this.Notes) ? "<p>" + this.Notes + "</p>" : string.Empty)
            };
            o.group = "Relationship";
            return o;
        }

        public override string ToString()
        {
            //return (this.SubjectPerson != null ? "Person(ID=" + this.SubjectPerson.Id.ToString() + ")" : string.Empty)
            //    + " " + this.PersonRelationshipType.ToString() + " "
            //    + (this.ObjectPerson != null ? "Person(ID=" + this.ObjectPerson.Id.ToString() + ")" : string.Empty);
            return "PersonRelationship(ID=" + this.Id.ToString() + ")";
        }
    }
}
