using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Actions;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Suggestions;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Domain.Scr.PersonFinalDecision;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Persons
{
    public class Person : Entity
    {
        /// <summary>
        /// 'Standard' last name.  Aliases or alternative spellings may be stored as a PersonAlias entity.
        /// </summary>
        [Audited]
        public virtual string LastName { get; set; }

        [Audited]
        public virtual string FirstName { get; set; }

        [Audited]
        public virtual int DayOfBirth { get; set; }

        [Audited]
        public virtual int MonthOfBirth { get; set; }

        [Audited]
        public virtual int YearOfBirth { get; set; }

        [Audited]
        public virtual string BirthVillage { get; set; }

        [Audited]
        public virtual Region BirthRegion { get; set; }

        /// <summary>
        /// Records incomplete or uncertain knowledge regarding birth date.
        /// 
        /// Doesn't seem much used, candidate for deprecation.
        /// </summary>
        [Audited]
        public virtual string ApproximateBirthDate { get; set; }

        [Audited]
        public virtual Ethnicity Ethnicity { get; set; }

        [Audited]
        public virtual string Height { get; set; }

        [Audited]
        public virtual string Weight { get; set; }

        /// <summary>
        /// Current catch-all field for textual information and analysis.
        /// </summary>
        [Audited]
        public virtual string BackgroundInformation { get; set; }

        /// <summary>
        /// Text that can be classified as public.
        /// </summary>
        [Audited]
        public virtual string PublicSummary { get; set; }

        /// <summary>
        /// Date of last change to PublicSummary.
        /// </summary>
        [Audited]
        public virtual DateTime? PublicSummaryDate { get; set; }

        /// <summary>
        /// Identifying numbers, separated by comma.
        /// 
        /// Not necessarily of military origin - name is a holdover from previous generation of the database schema.
        /// </summary>
        [Audited]
        public virtual string MilitaryIDNumber { get; set; }

        [Audited]
        public virtual bool Archive { get; set; }

        [Audited]
        public virtual string Notes { get; set; }

        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual ProfileStatus ProfileStatus { get; set; }

        /// <summary>
        /// Auto-modified by some stored procedures.
        /// </summary>
        public virtual DateTime ProfileLastModified { get; set; }

        /// <summary>
        /// Marks higher classification level than other profiles.
        /// </summary>
        [Audited]
        public virtual bool IsRestrictedProfile { get; set; }

        /// <summary>
        /// Auto-populated by database trigger.
        /// </summary>
        public virtual DateTime? Created { get; set; }

        /// <summary>
        /// Non-empty if this Person was imported from a previous database.
        /// </summary>
        public virtual IList<AdminPersonImport> AdminPersonImports { get; set; }

        [Audited]
        public virtual IList<PersonAlias> PersonAliases { get; set; }

        [Audited]
        public virtual IList<PersonRelationship> PersonRelationshipAsSubject { get; set; }

        [Audited]
        public virtual IList<PersonRelationship> PersonRelationshipAsObject { get; set; }

        [Audited]
        public virtual IList<PersonPhoto> PersonPhotos { get; set; }

        [Audited]
        public virtual IList<PersonSource> PersonSources { get; set; }

        /// <summary>
        /// Non-empty if this Person has ever been suggested by the event suggestion algorithm as being responsible for an Event.
        /// </summary>
        public virtual IList<AdminSuggestionPersonResponsibility> AdminSuggestionPersonResponsibilities { get; set; }

        [Audited]
        public virtual IList<ActionTaken> ActionsTakenAsSubject { get; set; }

        [Audited]
        public virtual IList<ActionTaken> ActionsTakenAsObject { get; set; }

        [Audited]
        public virtual IList<PersonResponsibility> PersonResponsibilities { get; set; }

        [Audited]
        public virtual IList<Career> Careers { get; set; }

        /// <summary>
        /// Log of occassions when this profile has been exported.
        /// </summary>
        public virtual IList<AdminExportedPersonProfile> AdminExportedPersonProfiles { get; set; }

        /// <summary>
        /// Log of occassions when this profile has been reviewed in source search results.
        /// </summary>
        public virtual IList<AdminReviewedSource> AdminReviewedSources { get; set; }

        /// <summary>
        /// Instances of when this Person has been screened.
        /// </summary>
        public virtual IList<RequestPerson> RequestPersons { get; set; }

        /// <summary>
        /// Log of when this Person has been actively researched and updated by a human.  Useful when applying the 3 month rule.
        /// </summary>
        public virtual IList<ActiveScreening> ActiveScreenings { get; set; }

        /// <summary>
        /// Text that is only visible to Profiling Internationals and Profiling Nationals.
        /// </summary>
        [Audited]
        public virtual IList<PersonRestrictedNote> PersonRestrictedNotes { get; set; }

        public Person()
        {
            this.AdminPersonImports = new List<AdminPersonImport>();
            this.PersonAliases = new List<PersonAlias>();
            this.PersonRelationshipAsSubject = new List<PersonRelationship>();
            this.PersonRelationshipAsObject = new List<PersonRelationship>();
            this.PersonPhotos = new List<PersonPhoto>();
            this.PersonSources = new List<PersonSource>();
            this.AdminSuggestionPersonResponsibilities = new List<AdminSuggestionPersonResponsibility>();
            this.ActionsTakenAsSubject = new List<ActionTaken>();
            this.ActionsTakenAsObject = new List<ActionTaken>();
            this.PersonResponsibilities = new List<PersonResponsibility>();
            this.Careers = new List<Career>();
            this.AdminExportedPersonProfiles = new List<AdminExportedPersonProfile>();
            this.AdminReviewedSources = new List<AdminReviewedSource>();
            this.RequestPersons = new List<RequestPerson>();
            this.ActiveScreenings = new List<ActiveScreening>();
            this.PersonRestrictedNotes = new List<PersonRestrictedNote>();
        }

        public virtual string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(this.FirstName))
                {
                    if (!string.IsNullOrEmpty(this.LastName))
                        return this.FirstName + " " + this.LastName;
                    else
                        return this.FirstName;
                }
                else
                {
                    return this.LastName;
                }
            }
        }

        /// <summary>
        /// Printable version of person's DOB.  Uses forward slash as separator, and a dash if a value is not known.
        /// 
        /// Priority fields are [{Year, Month, Day}]OfBirth; if empty or all zero, uses ApproximateBirthDate.  
        /// </summary>
        public virtual string DateOfBirth
        {
            get
            {
                if (this.YearOfBirth > 0 || this.MonthOfBirth > 0 || this.DayOfBirth > 0)
                {
                    string y = this.YearOfBirth > 0 ? this.YearOfBirth.ToString() : "-";
                    string m = "-";
                    if (this.MonthOfBirth > 9)
                        m = this.MonthOfBirth.ToString();
                    else if (this.MonthOfBirth > 0)
                        m = "0" + this.MonthOfBirth.ToString();
                    string d = "-";
                    if (this.DayOfBirth > 9)
                        d = this.DayOfBirth.ToString();
                    else if (this.DayOfBirth > 0)
                        d = "0" + this.DayOfBirth.ToString();
                    return string.Join("/", new string[] { y, m, d });
                }
                else if (!string.IsNullOrEmpty(this.ApproximateBirthDate))
                    return this.ApproximateBirthDate;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Printable version of person's POB.  Combines BirthVillage and BirthRegion.
        /// </summary>
        public virtual string PlaceOfBirth
        {
            get
            {
                if (!string.IsNullOrEmpty(this.BirthVillage))
                    return this.BirthVillage;
                else if (this.BirthRegion != null)
                    return this.BirthRegion.ToString();
                else
                    return string.Empty;
            }
        }

        public virtual IList<Career> GetCurrentCareers()
        {
            if (this.Careers != null)
                return this.Careers.Where(x => x.IsCurrentCareer && !x.Archive)
                        .OrderByDescending(x => x.GetSortDate())
                        .ToList();
            return null;
        }

        public virtual IList<Career> GetNonCurrentCareers()
        {
            if (this.Careers != null)
                return this.Careers.Where(x => !x.IsCurrentCareer && !x.Archive)
                        .OrderByDescending(x => x.GetSortDate())
                        .ToList();
            return null;
        }

        public virtual Career CurrentCareer
        {
            get
            {
                if (this.GetCurrentCareers() != null && this.GetCurrentCareers().Any())
                    return this.GetCurrentCareers().First();
                if (this.GetNonCurrentCareers() != null && this.GetNonCurrentCareers().Any())
                    return this.GetNonCurrentCareers().First();
                return null;
            }
        }

        public virtual string CurrentRank
        {
            get
            {
                if (this.CurrentCareer != null)
                    if (this.CurrentCareer != null && this.CurrentCareer.Rank != null)
                        return this.CurrentCareer.Rank.ToString();
                return string.Empty;
            }
        }

        public virtual int CurrentRankSortOrder
        {
            get
            {
                if (this.CurrentCareer != null)
                    if (this.CurrentCareer.Rank != null)
                        return this.CurrentCareer.Rank.SortOrder;
                return int.MaxValue;
            }
        }

        protected virtual ScreeningRequestPersonFinalDecision LatestScreeningRequestPersonFinalDecision
        {
            get
            {
                IEnumerable<ScreeningRequestPersonFinalDecision> decisions = (from rp in this.RequestPersons
                                                                              where rp.GetScreeningRequestPersonFinalDecision() != null
                                                                              orderby rp.GetScreeningRequestPersonFinalDecision().MostRecentHistory.DateStatusReached
                                                                              select rp.GetScreeningRequestPersonFinalDecision());
                if (decisions != null && decisions.Count() > 0)
                    return decisions.Last();
                return null;
            }
        }

        public virtual string LatestScreeningResult
        {
            get
            {
                ScreeningRequestPersonFinalDecision srpfd = this.LatestScreeningRequestPersonFinalDecision;
                if (srpfd != null)
                    return srpfd.ScreeningResult.ToString();
                return string.Empty;
            }
        }

        public virtual string LatestScreeningSupportStatus
        {
            get
            {
                ScreeningRequestPersonFinalDecision srpfd = this.LatestScreeningRequestPersonFinalDecision;
                if (srpfd != null)
                    return srpfd.ScreeningSupportStatus.ToString();
                return string.Empty;
            }
        }

        public virtual DateTime? LatestScreeningFinalDecisionDate
        {
            get
            {
                ScreeningRequestPersonFinalDecision srpfd = this.LatestScreeningRequestPersonFinalDecision;
                if (srpfd != null)
                    return srpfd.MostRecentDate;
                return null;
            }
        }

        public virtual bool LatestScreeningFinalDecisionContainsWaiver
        {
            get
            {
                ScreeningRequestPersonFinalDecision srpfd = this.LatestScreeningRequestPersonFinalDecision;
                if (srpfd != null)
                    return srpfd.ContainsWaiver();
                return false;
            }
        }

        /// <summary>
        /// Get all careers active during the given date, sorted most-recent first.
        /// Note many careers list 'as of' dates with no 'end date', so effectively we are returned careers with very old 'as of' dates -
        /// the most recent career is the most 'correct' (i.e. containing the function and rank this individual held at the given date).
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public virtual IList<Career> GetCareersAtTimeOf(DateTime date)
        {
            if (this.Careers != null && this.Careers.Any())
                return this.Careers
                    .Where(x => x.WasActiveOn(date))
                    .OrderByDescending(x => x.IsCurrentCareer)
                    .ThenByDescending(x => x.GetSortDate())
                    .ToList();
            return null;
        }

        public virtual string GetLatestColourCoding(string screeningEntityName)
        {
            ScreeningRequestPersonEntity srpe = this.GetLatestScreeningEntityResponse(screeningEntityName);
            if (srpe != null)
                return srpe.ScreeningResult.ToString();
            return string.Empty;
        }

        /// <summary>
        /// [DEPRECATED] Determines whether the colour coding from a screening entity is still valid according to the 3 month rule.
        /// Uses internal 'active screening' dates.  To be eligible for the 3 month rule, the active screening date must
        /// have been for a request that was finalized.
        /// </summary>
        /// <param name="excluding"></param>
        /// <param name="screeningEntityName"></param>
        /// <returns></returns>
        public virtual bool HasValidColourCoding(Request excluding, string screeningEntityName)
        {
            if (!string.IsNullOrEmpty(this.GetLatestColourCoding(screeningEntityName)))
            {
                ActiveScreening s = this.GetLatestActiveScreening(excluding);
                if (s != null && s.Request != null)
                    return s.Request.HasBeenCompleted && s.DateActivelyScreened >= DateTime.Now.AddMonths(-3);
            }
            return false;
        }

        /// <summary>
        /// Used to pre-populate responses for current screening requests.
        /// </summary>
        /// <param name="screeningEntityName"></param>
        /// <returns></returns>
        public virtual ScreeningRequestPersonEntity GetLatestScreeningEntityResponse(string screeningEntityName)
        {
            if (!string.IsNullOrEmpty(screeningEntityName))
            {
                //IEnumerable<RequestPerson> list = this.RequestPersons.Where(x => !x.Archive)
                //    .OrderByDescending(x => x.GetMostRecentScreeningRequestPersonEntity(screeningEntityName) != null ? x.GetMostRecentScreeningRequestPersonEntity(screeningEntityName).MostRecentHistory.DateStatusReached as DateTime? : null);
                //if (list != null && list.Any())
                //    return list.First().GetMostRecentScreeningRequestPersonEntity(screeningEntityName);
                ScreeningRequestPersonEntity mostRecent = null;
                foreach (RequestPerson rp in this.RequestPersons.Where(x => !x.Archive))
                {
                    if (mostRecent == null)
                        mostRecent = rp.GetMostRecentScreeningRequestPersonEntity(screeningEntityName);
                    else
                    {
                        ScreeningRequestPersonEntity current = rp.GetMostRecentScreeningRequestPersonEntity(screeningEntityName);
                        if (current != null && current.MostRecentHistory != null && mostRecent.MostRecentHistory != null)
                            if (current.MostRecentHistory.DateStatusReached >= mostRecent.MostRecentHistory.DateStatusReached)
                                mostRecent = current;
                    }
                }

                return mostRecent;
            }
            return null;
        }

        public virtual ActiveScreening GetLatestActiveScreening(Request excluding)
        {
            IEnumerable<ActiveScreening> activeScreenings = this.ActiveScreenings.Where(x => x.Request != excluding);
            if (activeScreenings.Any())
                return activeScreenings.OrderByDescending(x => x.DateActivelyScreened).First();
            return null;
        }

        public virtual bool IsResponsibleFor(int eventId)
        {
            return this.PersonResponsibilities.Select(x => x.Event).Select(x => x.Id).Contains(eventId);
        }

        public virtual bool WasSuggestedEventDeclined(int eventId)
        {
            return this.AdminSuggestionPersonResponsibilities.Where(x => x.Event.Id == eventId & !x.IsAccepted).Any();
        }

        /// <summary>
        /// Get actions taken linked to this person both directly and those listed in events for which this person is responsible.
        /// </summary>
        /// <returns></returns>
        public virtual IList<ActionTaken> GetActionTakens()
        {
            return this.PersonResponsibilities.Select(x => x.Event).Select(y => y.ActionTakens).Aggregate(new List<ActionTaken>(), (output, z) => output.Concat(z).ToList())
                .Concat(this.ActionsTakenAsSubject)
                .Concat(this.ActionsTakenAsObject)
                .Distinct()
                .ToList();
        }

        public virtual IList<PersonRelationship> GetPersonRelationships()
        {
            return this.PersonRelationshipAsSubject.Concat(this.PersonRelationshipAsObject).ToList();
        }

        /// <summary>
        /// Scours attached entities for the text "TBC".
        /// </summary>
        /// <returns></returns>
        public virtual IList<ToBeConfirmedDTO> GetToBeConfirmedDTOs()
        {
            IList<ToBeConfirmedDTO> dtos = new List<ToBeConfirmedDTO>();

            foreach (PersonResponsibility pr in this.PersonResponsibilities.Where(x => !x.Archive && x.Event != null 
                && (x.Commentary.IContains("TBC") || x.Notes.IContains("TBC"))))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(PersonResponsibility).Name,
                    Priority = "High",
                    Summary = pr.Event.Headline + " (" + pr.PersonResponsibilityType.ToString() + ")",
                    Text = string.Join("\n<hr />", new string[] { pr.Commentary, pr.Notes }.Where(x => !string.IsNullOrEmpty(x) && x.IContains("TBC")))
                });

            foreach (Career c in this.Careers.Where(x => !x.Archive && (x.Commentary.IContains("TBC") || x.Notes.IContains("TBC"))))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(Career).Name,
                    Priority = "High",
                    Summary = c.RankOrganizationLocationSummary + " - " + c.FunctionUnitSummary,
                    Text = string.Join("\n<hr />", new string[] { c.Commentary, c.Notes }.Where(x => !string.IsNullOrEmpty(x) && x.IContains("TBC")))
                });

            foreach (PersonResponsibility pr in this.PersonResponsibilities.Where(x => !x.Archive && x.Event != null && x.Event.Notes.IContains("TBC")))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(Event).Name,
                    Priority = "High",
                    Summary = pr.Event.Headline,
                    Text = pr.Event.Notes
                });

            if (this.BackgroundInformation.IContains("TBC"))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(Person).Name,
                    Priority = "Medium",
                    Summary = this.Name + " Background Information",
                    Text = this.BackgroundInformation
                });

            if (this.Notes.IContains("TBC"))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(Person).Name,
                    Priority = "Medium",
                    Summary = this.Name + " Notes",
                    Text = this.Notes
                });

            foreach (ActionTaken at in this.GetActionTakens()
                .Where(x => !x.Archive && (x.Commentary.IContains("TBC") || x.Notes.IContains("TBC"))))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(ActionTaken).Name,
                    Priority = "Medium",
                    Summary = at.GetCompleteSummary(),
                    Text = string.Join("\n<hr />", new string[] { at.Commentary, at.Notes }.Where(x => !string.IsNullOrEmpty(x) && x.IContains("TBC")))
                });

            foreach (PersonAlias pa in this.PersonAliases.Where(x => !x.Archive && x.Notes.IContains("TBC")))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(PersonAlias).Name,
                    Priority = "Medium",
                    Summary = pa.Name,
                    Text = pa.Notes
                });

            foreach (PersonRelationship pr in this.PersonRelationshipAsSubject.Concat(this.PersonRelationshipAsObject)
                .Where(x => !x.Archive && x.Notes.IContains("TBC")))
                dtos.Add(new ToBeConfirmedDTO()
                {
                    Type = typeof(PersonRelationship).Name,
                    Priority = "Low",
                    Summary = pr.RelationshipSummary(),
                    Text = pr.Notes
                });

            return dtos;
        }

        /// <summary>
        /// Check all careers for this person for the given organisation.
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public virtual bool WasEverAMemberOf(Organization org)
        {
            if (this.Careers != null)
                foreach (Career c in this.Careers.Where(x => !x.Archive))
                    if (c.Organization == org)
                        return true;
            return false;
        }

        /// <summary>
        /// Checks whether this Person's ProfileStatus matches one of the hard-coded valid ProfileStatuses (i.e. not FARDC2007List).
        /// </summary>
        /// <returns></returns>
        public virtual bool HasValidProfileStatus()
        {
            if (this.ProfileStatus != null)
                return new string[] { ProfileStatus.COMPLETE, ProfileStatus.IN_PROGRESS, ProfileStatus.ROUGH_OUTLINE }.Contains(this.ProfileStatus.ProfileStatusName);
            return false;
        }

        /// <summary>
        /// Returns TimelineJS date objects; fills out career start/end dates if missing.
        /// </summary>
        /// <returns></returns>
        public virtual IList<object> GetCareerTimelineSlideObjects(bool fillEndDates)
        {
            IList<Career> careers = this.Careers.Where(x => !x.Archive && x.HasDate()).OrderBy(x => x.GetSortDate()).ToList();
            if (!fillEndDates)
            {
                return careers.Select(x => x.GetTimelineSlideObject()).ToList();
            }
            IList<object> timeline = new List<object>();
            for (int i = 0; i < careers.Count; i++)
            {
                Career c = careers[i];
                dynamic o = c.GetTimelineSlideObject();
                if (o.GetType().GetProperty("end_date") == null)
                {
                    // get start date from next career
                    if (i + 1 < careers.Count)
                    {
                        Career next = careers[i + 1];
                        object nextStartDate = next.GetTimelineStartDateObject();
                        if (nextStartDate != null)
                        {
                            o.end_date = nextStartDate;
                        }
                    }
                }

                timeline.Add((object)o);
            }
            return timeline;
        }

        // collection modification below

        public virtual void AddPersonResponsibility(PersonResponsibility pr)
        {
            if (this.PersonResponsibilities.Contains(pr))
                return;

            this.PersonResponsibilities.Add(pr);
            pr.Event.AddPersonResponsibility(pr);
        }

        public virtual void RemovePersonResponsibility(PersonResponsibility pr)
        {
            this.PersonResponsibilities.Remove(pr);
        }

        public virtual void AddPersonAlias(PersonAlias alias)
        {
            if (this.PersonAliases.Contains(alias))
                return;

            this.PersonAliases.Add(alias);
        }

        public virtual void RemovePersonAlias(PersonAlias alias)
        {
            this.PersonAliases.Remove(alias);
        }

        public virtual void AddPhoto(Photo photo)
        {
            foreach (PersonPhoto p in this.PersonPhotos)
                if (p.Photo.Id == photo.Id)
                    return;

            this.PersonPhotos.Add(new PersonPhoto() { Person = this, Photo = photo });
        }

        public virtual void RemovePhoto(Photo photo)
        {
            List<PersonPhoto> photosToRemove = new List<PersonPhoto>();
            foreach (PersonPhoto p in this.PersonPhotos)
                if (p.Photo.Id == photo.Id)
                    photosToRemove.Add(p);

            foreach (PersonPhoto p in photosToRemove)
                this.PersonPhotos.Remove(p);
        }

        public virtual void AddPersonRelationshipAsSubject(PersonRelationship relationship)
        {
            if (this.PersonRelationshipAsSubject.Contains(relationship))
                return;

            this.PersonRelationshipAsSubject.Add(relationship);
        }

        public virtual void AddPersonRelationshipAsObject(PersonRelationship relationship)
        {
            if (this.PersonRelationshipAsObject.Contains(relationship))
                return;

            this.PersonRelationshipAsObject.Add(relationship);
        }

        public virtual void RemovePersonRelationshipAsSubject(PersonRelationship relationship)
        {
            this.PersonRelationshipAsSubject.Remove(relationship);
        }

        public virtual void RemovePersonRelationshipAsObject(PersonRelationship relationship)
        {
            this.PersonRelationshipAsObject.Remove(relationship);
        }

        public virtual void AddCareer(Career career)
        {
            if (this.Careers.Contains(career))
                return;

            this.Careers.Add(career);
        }

        public virtual void RemoveCareer(Career career)
        {
            this.Careers.Remove(career);
        }

        public virtual void AddPersonSource(PersonSource ps)
        {
            if (this.PersonSources.Contains(ps))
                return;

            this.PersonSources.Add(ps);
        }

        public virtual void RemovePersonSource(PersonSource ps)
        {
            this.PersonSources.Remove(ps);
        }

        public virtual void AddActiveScreening(ActiveScreening a)
        {
            if (this.ActiveScreenings.Contains(a))
                return;

            this.ActiveScreenings.Add(a);
        }

        public virtual void RemoveActiveScreening(ActiveScreening a)
        {
            this.ActiveScreenings.Remove(a);
        }

        public virtual void AddPersonRestrictedNote(PersonRestrictedNote n)
        {
            if (this.PersonRestrictedNotes.Contains(n))
                return;

            this.PersonRestrictedNotes.Add(n);
        }

        public virtual void RemovePersonRestrictedNote(PersonRestrictedNote n)
        {
            this.PersonRestrictedNotes.Remove(n);
        }

        public override string ToString()
        {
            return this.Name + " (ID=" + this.Id + ")";
        }

        public virtual object ToJSON()
        {
            return new
                {
                    Id = this.Id,
                    Name = this.Name
                };
        }
    }
}
