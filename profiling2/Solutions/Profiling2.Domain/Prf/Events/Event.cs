using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Actions;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Suggestions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Events
{
    public class Event : Entity, IIncompleteDate
    {
        [Audited]
        public virtual string EventName { get; set; }
        [Audited]
        public virtual string NarrativeEn { get; set; }
        [Audited]
        public virtual string NarrativeFr { get; set; }
        [Audited]
        public virtual Location Location { get; set; }
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
        public virtual DateTime? Created { get; set; }
        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual EventVerifiedStatus EventVerifiedStatus { get; set; }

        [Audited]
        public virtual IList<PersonResponsibility> PersonResponsibilities { get; set; }
        [Audited]
        public virtual IList<EventRelationship> EventRelationshipsAsSubject { get; set; }
        [Audited]
        public virtual IList<EventRelationship> EventRelationshipsAsObject { get; set; }
        public virtual IList<Tag> Tags { get; set; }
        [Audited]
        public virtual IList<OrganizationResponsibility> OrganizationResponsibilities { get; set; }
        [Audited]
        public virtual IList<EventSource> EventSources { get; set; }
        public virtual IList<AdminExportedEventProfile> AdminExportedEventProfiles { get; set; }
        public virtual IList<AdminReviewedSource> AdminReviewedSources { get; set; }
        public virtual IList<AdminSuggestionPersonResponsibility> AdminSuggestionPersonResponsibilities { get; set; }
        [Audited]
        public virtual IList<ActionTaken> ActionTakens { get; set; }
        [Audited]
        public virtual IList<Violation> Violations { get; set; }
        public virtual IList<EventApproval> EventApprovals { get; set; }
        [Audited]
        public virtual IList<JhroCase> JhroCases { get; set; }

        public Event()
        {
            this.PersonResponsibilities = new List<PersonResponsibility>();
            this.EventRelationshipsAsSubject = new List<EventRelationship>();
            this.EventRelationshipsAsObject = new List<EventRelationship>();
            this.Tags = new List<Tag>();
            this.OrganizationResponsibilities = new List<OrganizationResponsibility>();
            this.EventSources = new List<EventSource>();
            this.AdminExportedEventProfiles = new List<AdminExportedEventProfile>();
            this.AdminReviewedSources = new List<AdminReviewedSource>();
            this.AdminSuggestionPersonResponsibilities = new List<AdminSuggestionPersonResponsibility>();
            this.ActionTakens = new List<ActionTaken>();
            this.Violations = new List<Violation>();
            this.EventApprovals = new List<EventApproval>();
            this.JhroCases = new List<JhroCase>();
        }

        public virtual string Headline
        {
            get
            {
                try
                {
                    string cats = string.Join(", ", this.Violations.Select(x => x.Name));

                    // deprecated field, but some Events may still not have their categories populated - so we fall back on this
                    if (string.IsNullOrEmpty(cats))
                        cats = this.EventName;

                    return cats + " - " + this.Location.LocationName + " (" + this.GetStartDateString() + (this.HasEndDate() ? " to " + this.GetEndDateString() : string.Empty) + ")";
                }
                catch (Exception)
                {
                    return "(couldn't find audit trail object)";
                }
            }
        }

        /// <summary>
        /// For audit purposes.
        /// </summary>
        public virtual string JhroCaseNumbers
        {
            get
            {
                return string.Join(", ", this.JhroCases.Select(x => x.CaseNumber));
            }
        }

        /// <summary>
        /// For audit purposes.
        /// </summary>
        public virtual string ViolationNames
        {
            get
            {
                return string.Join(", ", this.Violations.Select(x => x.Name));
            }
        }

        /// <summary>
        /// For audit purposes.
        /// </summary>
        public virtual string LocationNameSummary
        {
            get
            {
                try
                {
                    return this.Location != null ? this.Location.ToString() : string.Empty;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }

        public virtual string GetJhroCaseNumber()
        {
            if (this.JhroCases != null && this.JhroCases.Any())
            {
                return this.JhroCases[0].CaseNumber;
            }
            return null;
        }

        /// <summary>
        /// Scours attached entities for the text "TBC".
        /// </summary>
        /// <returns></returns>
        public virtual IList<ToBeConfirmedDTO> GetToBeConfirmedDTOs()
        {
            IList<ToBeConfirmedDTO> dtos = new List<ToBeConfirmedDTO>();

            foreach (PersonResponsibility pr in this.PersonResponsibilities.Where(x => !x.Archive && x.Person != null 
                && (x.Commentary.IContains("TBC") || x.Notes.IContains("TBC"))))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(PersonResponsibility).Name,
                    Priority = "High",
                    Summary = pr.Person.Name + " (" + pr.PersonResponsibilityType.ToString() + ")",
                    Text = string.Join("\n<hr />", new string[] { pr.Commentary, pr.Notes }.Where(x => !string.IsNullOrEmpty(x) && x.IContains("TBC")))
                });

            foreach (OrganizationResponsibility or in this.OrganizationResponsibilities.Where(x => !x.Archive && x.Organization != null 
                && (x.Commentary.IContains("TBC") || x.Notes.IContains("TBC"))))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(OrganizationResponsibility).Name,
                    Priority = "High",
                    Summary = or.Organization.OrgLongName + " (" + or.OrganizationResponsibilityType.ToString() + ") " + (or.Unit != null ? or.Unit.UnitName : string.Empty),
                    Text = string.Join("\n<hr />", new string[] { or.Commentary, or.Notes }.Where(x => !string.IsNullOrEmpty(x) && x.IContains("TBC")))
                });

            if (this.Notes.IContains("TBC"))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(Event).Name,
                    Priority = "Medium",
                    Summary = this.Headline + " Notes",
                    Text = this.Notes
                });

            foreach (ActionTaken at in this.ActionTakens
                .Where(x => !x.Archive && (x.Commentary.IContains("TBC") || x.Notes.IContains("TBC"))))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(ActionTaken).Name,
                    Priority = "Medium",
                    Summary = at.GetCompleteSummary(),
                    Text = string.Join("\n<hr />", new string[] { at.Commentary, at.Notes }.Where(x => !string.IsNullOrEmpty(x) && x.IContains("TBC")))
                });

            foreach (PersonResponsibility pr in this.PersonResponsibilities.Where(x => !x.Archive && x.Person != null
                && (x.Person.BackgroundInformation.IContains("TBC") || x.Person.Notes.IContains("TBC"))))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(Person).Name,
                    Priority = "Medium",
                    Summary = pr.Person.Name,
                    Text = string.Join("\n<hr />", new string[] { pr.Person.BackgroundInformation, pr.Person.Notes }.Where(x => !string.IsNullOrEmpty(x) && x.IContains("TBC")))
                });

            foreach (PersonAlias pa in this.PersonResponsibilities.Select(x => x.Person.PersonAliases)
                .Aggregate(new List<PersonAlias>(), (output, z) => output.Concat(z).ToList())
                .Where(x => !x.Archive && x.Notes.IContains("TBC")))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(PersonAlias).Name,
                    Priority = "Medium",
                    Summary = "Alias: " + pa.Name + " for person: " + pa.Person.Name,
                    Text = pa.Notes
                });

            foreach (OrganizationAlias oa in this.OrganizationResponsibilities.Select(x => x.Organization.OrganizationAliases)
                .Aggregate(new List<OrganizationAlias>(), (output, z) => output.Concat(z).ToList())
                .Where(x => !x.Archive && x.Notes.IContains("TBC")))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(OrganizationAlias).Name,
                    Priority = "Low",
                    Summary = "Alias: " + oa.OrgLongName + " for organization: " + oa.Organization.OrgLongName,
                    Text = oa.Notes
                });

            return dtos;
        }

        public virtual bool IsApproved()
        {
            return this.EventApprovals != null && this.EventApprovals.Any();
        }

        /// <summary>
        /// Useful for audit page.
        /// </summary>
        public virtual string EventVerifiedStatusName
        {
            get
            {
                return this.EventVerifiedStatus != null ? this.EventVerifiedStatus.EventVerifiedStatusName : string.Empty;
            }
        }

        public virtual bool IsPersonResponsible(Person p)
        {
            return this.PersonResponsibilities.Select(x => x.Person).Contains(p);
        }

        /// <summary>
        /// Return EventSources which haven't been archived.
        /// </summary>
        /// <returns></returns>
        public virtual IList<EventSource> GetUsableEventSources()
        {
            if (this.EventSources != null)
            {
                return this.EventSources.Where(x => !x.Archive && x.Source != null && !x.Source.Archive).ToList();
            }
            return null;
        }

        /// <summary>
        /// Logic copied from Profiling1 'save as word'.
        /// </summary>
        /// <returns></returns>
        public virtual bool HasAtLeastOneReliableSource()
        {
            IList<EventSource> list = this.GetUsableEventSources().Where(x => x.Reliability != null).ToList();
            if (list != null && list.Any())
            {
                if (list.Count == 1)
                {
                    return new string[] { Reliability.NAME_HIGH, Reliability.NAME_MODERATE }.Contains(list.First().Reliability.ReliabilityName);
                }
                return true;
            }
            return false;
        }

        public virtual IList<EventRelationship> GetEventRelationships()
        {
            return this.EventRelationshipsAsSubject.Concat(this.EventRelationshipsAsObject).ToList<EventRelationship>();
        }

        public virtual IList<string> GetCaseCodesInEventSources()
        {
            return this.EventSources.Where(x => x.HasCaseCode())
                .Select(x => x.GetCaseCodes())
                .Aggregate(new List<string>(), (x, y) => x.Concat(y).ToList())
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Return a float between 0 and 1 where 1 is identical and 0 is not.  Used as a filter in finding similar events.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual float GetViolationSimilarity(Event e)
        {
            if (e != null)
            {
                float f = 0;
                float part = 1 / (float)this.Violations.Count;
                foreach (Violation v in this.Violations)
                {
                    if (e.Violations.Contains(v))
                        f += part;
                }
                return f;
            }
            return 0;
        }

        /// <summary>
        /// Return a float between 0 and 1 where 1 is identical and 0 is not.  Used as a filter in finding similar events.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual float GetLocationSimilarity(Event e)
        {
            if (e != null)
            {
                if (e.Location == null && this.Location == null)
                    return 1;
                else if (e.Location != null && this.Location != null)
                    return e.Location.GetSimilarity(this.Location);
            }
            return 0;
        }

        public override string ToString()
        {
            return "Event(ID=" + this.Id + ")";
        }

        public virtual object ToShortHeadlineJSON()
        {
            return new
            {
                Id = this.Id,
                CaseCode = string.Join("; ", this.JhroCases.Select(x => x.CaseNumber)),
                Headline = this.Headline,
                VerifiedStatus = this.EventVerifiedStatus != null ? this.EventVerifiedStatus.EventVerifiedStatusName : null
            };
        }

        public virtual object ToShortJSON()
        {
            return new
                {
                    Id = this.Id,
                    CaseCode = string.Join("; ", this.JhroCases.Select(x => x.CaseNumber)),
                    StartDate = this.HasStartDate() ? this.GetStartDateString() : null,
                    EndDate = this.HasEndDate() ? this.GetEndDateString() : null,
                    Location = this.Location.ToShortJSON(),
                    Violations = this.Violations.Select(x => x.ToJSON()),
                    VerifiedStatus = this.EventVerifiedStatus != null ? this.EventVerifiedStatus.EventVerifiedStatusName : null
                };
        }

        public virtual object ToJSON(IList<SourceDTO> sources, int personId)
        {
            return new
                {
                    Id = this.Id,
                    CaseCode = string.Join("; ", this.JhroCases.Select(x => x.CaseNumber)),
                    StartDate = this.HasStartDate() ? this.GetStartDateString() : null,
                    EndDate = this.HasEndDate() ? this.GetEndDateString() : null,
                    Location = this.Location.ToShortJSON(),
                    Violations = this.Violations.Select(x => x.ToJSON()),
                    Narrative = !string.IsNullOrEmpty(this.NarrativeEn) ? this.NarrativeEn : this.NarrativeFr,
                    Notes = this.Notes,
                    VerifiedStatus = this.EventVerifiedStatus != null ? this.EventVerifiedStatus.EventVerifiedStatusName : null,
                    ActionsTaken = this.ActionTakens.Select(x => x.ToJSON()),
                    OthersResponsible = this.PersonResponsibilities.Where(x => !x.Archive && x.Person.Id != personId).Select(x => x.ToJSON()),
                    EventSources = this.EventSources.Select(x => x.ToJSON(sources.Where(y => y.SourceID == x.Source.Id).First())),
                    RelatedEvents = this.EventRelationshipsAsSubject.Where(x => !x.Archive).Select(x => x.ToJSON())
                        .Concat(this.EventRelationshipsAsObject.Where(x => !x.Archive).Select(x => x.ToJSON())),
                    Tags = this.Tags.Select(x => x.ToJSON())
                };
        }

        // data modification methods

        public virtual void AddJhroCase(JhroCase jc)
        {
            if (this.JhroCases.Contains(jc))
                return;

            this.JhroCases.Add(jc);
        }

        public virtual void AddEventSource(EventSource es)
        {
            if (this.EventSources.Contains(es))
                return;

            this.EventSources.Add(es);
        }

        public virtual void RemoveEventSource(EventSource es)
        {
            this.EventSources.Remove(es);
        }

        public virtual bool AddPersonResponsibility(PersonResponsibility pr)
        {
            if (this.PersonResponsibilities.Contains(pr))
                return false;

            this.PersonResponsibilities.Add(pr);
            pr.Person.AddPersonResponsibility(pr);
            return true;
        }

        public virtual void RemovePersonResponsibility(PersonResponsibility pr)
        {
            this.PersonResponsibilities.Remove(pr);
        }

        public virtual bool AddOrganizationResponsibility(OrganizationResponsibility or)
        {
            foreach (OrganizationResponsibility o in this.OrganizationResponsibilities)
            {
                if (o.Organization.Id == or.Organization.Id)
                {
                    if (o.Unit == null && or.Unit == null)
                        return false;
                    else if (o.Unit != null && or.Unit != null && o.Unit.Id == or.Unit.Id)
                        return false;
                }
            }

            this.OrganizationResponsibilities.Add(or);
            or.Organization.AddOrganizationResponsibility(or);
            return true;
        }

        public virtual void RemoveOrganizationResponsibility(OrganizationResponsibility or)
        {
            this.OrganizationResponsibilities.Remove(or);
        }

        public virtual void AddActionTaken(ActionTaken at)
        {
            this.ActionTakens.Add(at);
        }

        public virtual void RemoveActionTaken(ActionTaken at)
        {
            this.ActionTakens.Remove(at);
        }

        public virtual void AddEventRelationshipAsSubject(EventRelationship er)
        {
            if (this.EventRelationshipsAsSubject != null && this.EventRelationshipsAsSubject.Contains(er))
                return;

            this.EventRelationshipsAsSubject.Add(er);
        }

        public virtual void AddEventRelationshipAsObject(EventRelationship er)
        {
            if (this.EventRelationshipsAsObject != null && this.EventRelationshipsAsObject.Contains(er))
                return;

            this.EventRelationshipsAsObject.Add(er);
        }

        public virtual void RemoveEventRelationshipAsSubject(EventRelationship er)
        {
            this.EventRelationshipsAsSubject.Remove(er);
        }

        public virtual void RemoveEventRelationshipAsObject(EventRelationship er)
        {
            this.EventRelationshipsAsObject.Remove(er);
        }
    }
}
