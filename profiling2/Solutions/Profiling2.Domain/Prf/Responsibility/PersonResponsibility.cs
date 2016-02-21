using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using ScorePhrase;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Responsibility
{
    public class PersonResponsibility : Entity
    {
        [Audited]
        public virtual Person Person { get; set; }
        [Audited]
        public virtual Event Event { get; set; }
        [Audited]
        public virtual IList<Violation> Violations { get; set; }
        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual PersonResponsibilityType PersonResponsibilityType { get; set; }
        [Audited]
        public virtual string Commentary { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }
        public virtual DateTime? Created { get; set; }

        public PersonResponsibility()
        {
            this.Violations = new List<Violation>();
        }

        /// <summary>
        /// Get person's career at the time of the event.
        /// </summary>
        /// <returns></returns>
        protected virtual Career GetPersonCareer()
        {
            if (this.Person != null && this.Event != null)
            {
                DateTime? timeToCheck = null;
                if (this.Event.HasStartDate())
                {
                    timeToCheck = this.Event.GetStartDateTime();
                }
                else if (this.Event.HasEndDate())
                {
                    timeToCheck = this.Event.GetEndDateTime();
                }

                if (timeToCheck.HasValue)
                {
                    IList<Career> candidates = this.Person.GetCareersAtTimeOf(timeToCheck.Value);
                    if (candidates != null && candidates.Any())
                    {
                        // filter out careers where the function is 'appointed' or 'promoted'
                        candidates = candidates.Where(x =>
                        {
                            if (x.Role != null && !string.IsNullOrEmpty(x.Role.RoleName))
                                return !x.Role.RoleName.Contains("ppointed") && !x.Role.RoleName.Contains("romoted");
                            else
                                return true;
                        }).ToList();

                        if (candidates.Where(x => x.IsCurrentCareer).Any())
                        {
                            return candidates.Where(x => x.IsCurrentCareer).OrderByDescending(x => x.GetSortDate()).First();
                        }
                        else
                        {
                            return candidates.First();
                        }
                    }
                }
                else
                {
                    return this.Person.CurrentCareer;
                }
            }
            return null;
        }

        /// <summary>
        /// Get person's function at the time of the event.
        /// </summary>
        /// <returns></returns>
        public virtual string GetPersonFunctionUnitSummary()
        {
            Career c = this.GetPersonCareer();
            return c == null ? null : c.FunctionUnitSummary;
        }

        /// <summary>
        /// Return the Levenshtein distance (edit distance) between the given PersonResponsibility's commentary and function fields.
        /// 
        /// Of temporary use during cleanup of PersonResponsibility.Commentary field when virtual function field introduced.
        /// </summary>
        /// <returns></returns>
        public virtual int GetEditDistanceBetweenCommentaryAndFunction()
        {
            if (!string.IsNullOrEmpty(this.Commentary) && !string.IsNullOrEmpty(this.GetPersonFunctionUnitSummary()))
            {
                return LevenshteinDistance.Compute(this.Commentary, this.GetPersonFunctionUnitSummary());
            }
            return int.MaxValue;
        }

        public virtual string PrintPersonResponsibilityType()
        {
            return this.PersonResponsibilityType != null ? this.PersonResponsibilityType.PersonResponsibilityTypeName : string.Empty;
        }

        /// <summary>
        /// Returns TimelineJS date object.
        /// </summary>
        /// <returns></returns>
        public virtual object GetTimelineSlideObject()
        {
            dynamic o = new ExpandoObject();
            if (this.Event.GetTimelineStartDateObject() != null)
                o.start_date = this.Event.GetTimelineStartDateObject();
            if (this.Event.GetTimelineEndDateObject() != null)
                o.end_date = this.Event.GetTimelineEndDateObject();
            o.text = new
            {
                headline = string.Join(", ", this.Violations.Select(x => x.Name)),
                text = this.PrintPersonResponsibilityType() + " responsibility; " + (this.Event.Location != null ? this.Event.Location.ToString() : "no known location")
            };
            o.group = "Responsibility";
            return o;
        }

        public override string ToString()
        {
            //return (this.Person != null ? "Person(ID=" + this.Person.Id.ToString() + ")" : string.Empty)
            //    + " has " + this.PersonResponsibilityType.ToString() + " responsibility type for "
            //    + (this.Event != null ? "Event(ID=" + this.Event.Id.ToString() + ")" : string.Empty);
            return "PersonResponsibility(ID=" + this.Id.ToString() + ")";
        }

        public virtual object ToJSON()
        {
            Career c = this.GetPersonCareer();
            return new
            {
                Id = this.Id,
                EventId = this.Event.Id,
                Person = this.Person.ToJSON(),
                PersonFunctionUnitSummary = this.GetPersonFunctionUnitSummary(),
                Absent = c == null ? false : c.Absent,
                Nominated = c == null ? false : c.Nominated,
                Defected = c == null ? false : c.Defected,
                Violations = this.Violations.Select(x => x.ToJSON()),
                PersonResponsibilityType = this.PersonResponsibilityType.ToJSON(),
                Commentary = this.Commentary,
                Notes = this.Notes
            };
        }

        // data modification methods

        public virtual void AddViolation(Violation v)
        {
            if (this.Violations.Contains(v))
                return;
            this.Violations.Add(v);
        }
    }
}
