using System;
using System.Collections.Generic;
using System.Dynamic;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Units;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Careers
{
    public class Career : Entity, IIncompleteDate, IAsOfDate
    {
        [Audited]
        public virtual Person Person { get; set; }
        [Audited]
        public virtual Organization Organization { get; set; }
        [Audited]
        public virtual Location Location { get; set; }
        [Audited]
        public virtual Rank Rank { get; set; }
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
        public virtual string Job { get; set; }
        [Audited]
        public virtual string Commentary { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }
        [Audited]
        public virtual int DayAsOf { get; set; }
        [Audited]
        public virtual int MonthAsOf { get; set; }
        [Audited]
        public virtual int YearAsOf { get; set; }
        [Audited]
        public virtual bool IsCurrentCareer { get; set; }
        [Audited]
        public virtual Unit Unit { get; set; }
        [Audited]
        public virtual Role Role { get; set; }
        public virtual DateTime? Created { get; set; }
        [Audited]
        public virtual bool Defected { get; set; }
        [Audited]
        public virtual bool Acting { get; set; }
        [Audited]
        public virtual bool Absent { get; set; }
        [Audited]
        public virtual bool Nominated { get; set; }

        public virtual IList<AdminCareerImport> AdminCareerImports { get; set; }

        public Career()
        {
            this.AdminCareerImports = new List<AdminCareerImport>();
        }

        public virtual string Function
        {
            get
            {
                string s = string.Empty;
                if (this.Role != null && this.IsValidName(this.Role.RoleName))
                {
                    s += (this.Acting ? "Acting " : string.Empty) + this.Role.RoleName;
                    // TODO deprecate references to Job once values have been cleaned from database
                    if (this.IsValidName(this.Job))
                        s += " (" + this.Job + ")";
                }
                else
                    s += this.Job;
                return s;
            }
        }

        public virtual string FunctionUnitSummary
        {
            get
            {
                string s = this.Function;
                if (this.Unit != null && this.IsValidName(this.Unit.UnitName))
                {
                    if (!string.IsNullOrEmpty(s))
                        s += " of ";
                    s += this.Unit.UnitName;
                }
                return s;
            }
        }

        public virtual string RankOrganizationLocationSummary
        {
            get
            {
                string s = string.Empty;
                if (this.Rank != null && this.IsValidName(this.Rank.RankName))
                {
                    s += "<span title=\"" + this.Rank.RankNameFr + "\">" + this.Rank.RankName + "</span>";
                    if (this.Organization != null && (this.IsValidName(this.Organization.OrgShortName) || this.IsValidName(this.Organization.OrgLongName)))
                        s += " of ";
                }
                if (this.Organization != null)
                {
                    if (this.IsValidName(this.Organization.OrgShortName))
                        s += "<span title=\"" + this.Organization.OrgLongName + "\">" + this.Organization.OrgShortName + "</span>";
                    else if (this.IsValidName(this.Organization.OrgLongName))
                        s += this.Organization.OrgLongName;
                }
                if (this.Location != null && this.IsValidName(this.Location.LocationName))
                {
                    if (string.IsNullOrEmpty(s))
                        s += "Based in ";
                    else
                        s += " based in ";
                    s += this.Location.LocationName;
                }
                return s;
            }
        }

        /// <summary>
        /// Summary of rank, organization and location.  
        /// 
        /// Similar to RankOrganizationLocationSummary, except no HTML. 
        /// 
        /// TODO refactor both.
        /// </summary>
        /// <returns></returns>
        public virtual string GetRankOrganizationLocationNoHtmlSummary()
        {
            string s = string.Empty;
            if (this.Rank != null && this.IsValidName(this.Rank.RankName))
            {
                s += this.Rank.RankName;
                if (this.Organization != null && (this.IsValidName(this.Organization.OrgShortName) || this.IsValidName(this.Organization.OrgLongName)))
                    s += " of ";
            }
            if (this.Organization != null)
            {
                s += this.Organization.ToString();
            }
            if (this.Location != null && this.IsValidName(this.Location.LocationName))
            {
                if (string.IsNullOrEmpty(s))
                    s += "Based in ";
                else
                    s += " based in ";
                s += this.Location.LocationName;
            }
            return s;
        }

        /// <summary>
        /// Printable date summary which doesn't attempt to fill in empty values.
        /// 
        /// Similar to GetDateSummary() in EntityDateExtensions, but incorporates AsOfDate.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetCareerDateSummary()
        {
            string s = string.Empty;

            if (this.HasStartDate())
            {
                s += "From " + this.GetStartDateString();
            }
            else if (this.HasAsOfDate())
            {
                s += "As of " + this.GetAsOfDateString();
            }
            if (this.HasEndDate())
            {
                s += " until " + this.GetEndDateString();
            }
            if (!string.IsNullOrEmpty(s))
            {
                s = s.Trim();
                if (!char.IsUpper(s[0]))
                {
                    s = s.Substring(0, 1).ToUpper() + s.Substring(1);
                }
                s += ". ";
            }

            return s;
        }

        /// <summary>
        /// Printable summary of this career.  Shouldn't contain HTML markup.
        /// </summary>
        /// <returns></returns>
        public virtual string GetFullSummary()
        {
            string s = string.Empty;
            if (this.Defected)
            {
                if (this.Organization != null)
                {
                    s += "Defected from " + this.Organization.ToString() + ". ";
                }

                s += "Prior to defection";
                if (this.Rank != null && this.IsValidName(this.Rank.RankName))
                {
                    s += " was " + this.Rank.RankName;
                }
                s += this.GetCareerDateSummary();

                if (!string.IsNullOrEmpty(this.FunctionUnitSummary))
                {
                    s += this.FunctionUnitSummary + ". ";
                }

                if (this.Location != null && this.IsValidName(this.Location.LocationName))
                {
                    s += this.Location.LocationName;
                }
            }
            else
            {
                s += this.GetCareerDateSummary();

                if (!string.IsNullOrEmpty(this.FunctionUnitSummary))
                {
                    s += this.FunctionUnitSummary;
                    if (!string.IsNullOrEmpty(this.GetRankOrganizationLocationNoHtmlSummary()))
                    {
                        s += " / " + this.GetRankOrganizationLocationNoHtmlSummary();
                    }
                    s += ".";
                }
                else if (!string.IsNullOrEmpty(this.GetRankOrganizationLocationNoHtmlSummary()))
                {
                    s += this.GetRankOrganizationLocationNoHtmlSummary() + ".";
                }
            }

            if (this.Absent)
            {
                s += " Absent during some or all of this period.";
            }

            return s;
        }

        /// <summary>
        /// Select careers which were valid (or 'active') at the given date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public virtual bool WasActiveOn(DateTime date)
        {
            DateTime? start = this.GetStartDateTime();
            if (start.HasValue && start.Value > date)
                return false;

            DateTime? end = this.GetEndDateTime();
            if (end.HasValue && end.Value < date)
                return false;

            // only test 'as of' date if there is no start date present
            if (!this.HasStartDate())
            {
                // The 'as of' date was formerly only used when there existed no start/end date.  However PersonResponsibility.GetPersonFunctionUnitSummary()
                // started returning invalid results which were fixed by elevating the 'as of' date, i.e. allowing it to filter dates considered active
                // even in the presence of a start or end date.
                DateTime? asOf = this.GetAsOfDate();
                if (asOf.HasValue && asOf.Value > date)
                    return false;
            }

            return true;
        }

        public virtual bool HasDate()
        {
            return this.HasStartDate() || this.HasEndDate() || this.HasAsOfDate();
        }

        /// <summary>
        /// Returns TimelineJS date object.
        /// </summary>
        /// <returns></returns>
        protected virtual object GetTimelineStartDateObject()
        {
            if (this.HasStartDate())
                return this.GetIncompleteStartDate();
            else if (this.HasAsOfDate())
                return this.GetIncompleteAsOfDate();
            else if (this.HasEndDate())
                return this.GetIncompleteEndDate();
            return null;
        }

        /// <summary>
        /// Returns TimelineJS date object.
        /// </summary>
        /// <returns></returns>
        protected virtual object GetTimelineEndDateObject()
        {
            if (this.HasEndDate())
                return this.GetIncompleteEndDate();
            return null;
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
                headline = !string.IsNullOrEmpty(this.FunctionUnitSummary) ? this.FunctionUnitSummary : this.RankOrganizationLocationSummary,
                text = this.RankOrganizationLocationSummary
            };
            o.group = "Career";
            return o;
        }

        /// <summary>
        /// Detects values in database that count as invalid.
        /// In Profiling1, invalid values stored in PRF_AdminUnknown.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected bool IsValidName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                if (!string.Equals(name, "0")
                    && !string.Equals(name, "00")
                    && !string.Equals(name, "-")
                    && !string.Equals(name, "n/a")
                    && !string.Equals(name, "unknown", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(name, "Not relevant"))  // there is an entry in PRF_Rank with this value
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            return "Career(ID = " + this.Id + ")";
            //return (this.Person != null ? "Person(ID=" + this.Person.Id.ToString() + ") has " : string.Empty)
            //    + (this.Rank != null ? " Rank(ID=" + this.Rank.Id.ToString() + ")" : string.Empty)
            //    + (this.Role != null ? " Role(ID=" + this.Role.Id.ToString() + ")" : string.Empty)
            //    + (this.Job != null ? " Function(ID=" + this.Job.ToString() + ")" : string.Empty)
            //    + (this.Organization != null ? " in Organization(ID=" + this.Organization.Id.ToString() + ")" : string.Empty)
            //    + (this.Unit != null ? " in Unit(ID=" + this.Unit.Id.ToString() + ")" : string.Empty)
            //    + (this.Location != null ? " in Location(ID=" + this.Location.Id.ToString() + ")" : string.Empty);
        }
    }
}
