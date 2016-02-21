using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Units;
using Profiling2.Domain.Scr.Attach;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Domain.Scr.Proposed;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr
{
    public class Request : Entity
    {
        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual RequestEntity RequestEntity { get; set; }
        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual RequestType RequestType { get; set; }
        [Audited]
        public virtual string RequestName { get; set; }
        [Audited]
        public virtual string ReferenceNumber { get; set; }
        [Audited]
        public virtual string Notes { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual DateTime? RespondBy { get; set; }
        [Audited]
        public virtual bool RespondImmediately { get; set; }

        public virtual IList<RequestHistory> RequestHistories { get; set; }
        public virtual IList<RequestProposedPerson> ProposedPersons { get; set; }
        public virtual IList<RequestPerson> Persons { get; set; }
        public virtual IList<RequestUnit> Units { get; set; }
        public virtual IList<RequestAttachment> RequestAttachments { get; set; }
        public virtual IList<ScreeningRequestEntityResponse> ScreeningRequestEntityResponses { get; set; }

        public Request()
        {
            this.RequestHistories = new List<RequestHistory>();
            this.ProposedPersons = new List<RequestProposedPerson>();
            this.Persons = new List<RequestPerson>();
            this.Units = new List<RequestUnit>();
            this.RequestAttachments = new List<RequestAttachment>();
            this.ScreeningRequestEntityResponses = new List<ScreeningRequestEntityResponse>();
        }

        public virtual string Headline
        {
            get
            {
                return this.ReferenceNumber + (!string.IsNullOrEmpty(this.RequestName) ? ": " + this.RequestName : string.Empty);
            }
        }

        protected virtual RequestHistory MostRecentHistory
        {
            get
            {
                if (this.RequestHistories != null && this.RequestHistories.Count > 0)
                    return (from rh in this.RequestHistories
                            orderby rh.DateStatusReached
                            select rh).Last<RequestHistory>();
                else
                    return null;
            }
        }

        public virtual AdminUser Creator
        {
            get
            {
                RequestHistory createdHistory = this.GetHistory(RequestStatus.NAME_CREATED);
                if (createdHistory != null)
                    return createdHistory.AdminUser;
                return null;
            }
        }

        public virtual DateTime GetCreatedDate()
        {
            RequestHistory createdHistory = this.GetHistory(RequestStatus.NAME_CREATED);
            if (createdHistory != null)
                return createdHistory.DateStatusReached;
            return default(DateTime);
        }

        public virtual string RejectedReason
        {
            get
            {
                if (this.CurrentStatus != null && string.Equals(this.CurrentStatus.RequestStatusName, RequestStatus.NAME_REJECTED))
                    return this.MostRecentHistory.Notes;  // does it count as rejected if there is a RequestHistory = rejected that is not the most recent?
                else
                    return string.Empty;
            }
        }

        public virtual RequestStatus CurrentStatus
        {
            get
            {
                return this.MostRecentHistory != null ? this.MostRecentHistory.RequestStatus : null;
            }
        }

        public virtual DateTime? CurrentStatusDate
        {
            get
            {
                if (this.MostRecentHistory != null)
                {
                    // select date of first occurrence of this request status; caters for case when an admin
                    // edits a request, resulting in multiple histories with the same status.
                    return (from rh in this.RequestHistories
                            where rh.RequestStatus == this.MostRecentHistory.RequestStatus
                            orderby rh.DateStatusReached
                            select rh).First().DateStatusReached;
                }
                return null;
            }
        }

        public virtual AdminUser CurrentStatusUser
        {
            get
            {
                if (this.MostRecentHistory != null)
                {
                    return (from rh in this.RequestHistories
                            where rh.RequestStatus == this.MostRecentHistory.RequestStatus
                            orderby rh.DateStatusReached
                            select rh).First().AdminUser;
                }
                return null;
            }
        }

        public virtual bool RequiresFinalDecision
        {
            get
            {
                if (this.MostRecentHistory != null && this.MostRecentHistory.RequestStatus != null)
                    if (string.Equals(this.MostRecentHistory.RequestStatus.RequestStatusName, RequestStatus.NAME_SENT_FOR_FINAL_DECISION))
                        return true;
                return false;
            }
        }

        public virtual bool RequiresConsolidation
        {
            get
            {
                if (this.MostRecentHistory != null && this.MostRecentHistory.RequestStatus != null)
                {
                    // TODO search history for completed or rejected status?
                    if (new string[] { RequestStatus.NAME_SENT_FOR_SCREENING, RequestStatus.NAME_SCREENING_IN_PROGRESS, RequestStatus.NAME_SENT_FOR_CONSOLIDATION }
                        .Contains(this.MostRecentHistory.RequestStatus.RequestStatusName))
                        return true;
                }
                return false;
            }
        }

        public virtual bool RequiresResponse
        {
            get
            {
                if (this.MostRecentHistory != null && this.MostRecentHistory.RequestStatus != null)
                {
                    if (new string[] { RequestStatus.NAME_SENT_FOR_SCREENING, RequestStatus.NAME_SCREENING_IN_PROGRESS }
                        .Contains(this.MostRecentHistory.RequestStatus.RequestStatusName))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Checks if this request has ever, in its history, had a status of 'Sent for validation'.
        /// </summary>
        public virtual bool HasBeenSentForValidation
        {
            get
            {
                foreach (RequestHistory h in this.RequestHistories)
                    if (h.RequestStatus != null && string.Equals(h.RequestStatus.RequestStatusName, RequestStatus.NAME_SENT_FOR_VALIDATION))
                        return true;
                return false;
            }
        }

        public virtual bool IsSentForValidation
        {
            get
            {
                return this.CurrentStatus != null && string.Equals(this.CurrentStatus.RequestStatusName, RequestStatus.NAME_SENT_FOR_VALIDATION);
            }
        }

        public virtual bool HasBeenForwardedForScreening
        {
            get
            {
                foreach (RequestHistory h in this.RequestHistories)
                    if (h.RequestStatus != null && string.Equals(h.RequestStatus.RequestStatusName, RequestStatus.NAME_SENT_FOR_SCREENING))
                        return true;
                return false;
            }
        }

        public virtual bool HasBeenCompleted
        {
            get
            {
                foreach (RequestHistory h in this.RequestHistories)
                    if (h.RequestStatus != null && string.Equals(h.RequestStatus.RequestStatusName, RequestStatus.NAME_COMPLETED))
                        return true;
                return false;
            }
        }

        public virtual bool HasProposedPersons
        {
            get
            {
                if (this.ProposedPersons != null)
                    return this.ProposedPersons.Where(x => !x.Archive).Any();
                return false;
            }
        }

        public virtual string GetSortableReferenceNumber()
        {
            if (!string.IsNullOrEmpty(this.ReferenceNumber))
            {
                string[] parts = this.ReferenceNumber.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < parts.Length; i++)
                    if (parts[i].Length == 1)
                        parts[i] = "0" + parts[i];

                return string.Join("-", parts);
            }
            return null;
        }

        public virtual RequestHistory GetHistory(string name)
        {
            if (this.RequestHistories != null && this.RequestHistories.Count > 0)
                return (from rh in this.RequestHistories
                        where rh.RequestStatus.RequestStatusName == name
                        orderby rh.DateStatusReached
                        select rh).First<RequestHistory>();
            return null;
        }

        public virtual DateTime? GetLatestScreeningDate()
        {
            if (this.RequestHistories != null && this.RequestHistories.Count > 0)
            {
                RequestHistory latest = (from rh in this.RequestHistories
                                         where new string[] { 
                                                RequestStatus.NAME_CREATED,
                                                RequestStatus.NAME_SENT_FOR_VALIDATION,
                                                RequestStatus.NAME_SENT_FOR_SCREENING, 
                                                RequestStatus.NAME_SCREENING_IN_PROGRESS, 
                                                RequestStatus.NAME_SENT_FOR_CONSOLIDATION 
                                            }.Contains(rh.RequestStatus.RequestStatusName)
                                         orderby rh.DateStatusReached descending
                                         select rh).First<RequestHistory>();

                if (latest == this.MostRecentHistory)
                    return DateTime.Now;
                else
                    return latest.DateStatusReached;
            }
            return null;
        }

        public virtual DateTime? ResponseDate(string screeningEntityName)
        {
            if (!string.IsNullOrEmpty(screeningEntityName))
            {
                IEnumerable<ScreeningRequestEntityResponse> list = this.ScreeningRequestEntityResponses
                    .Where(x => string.Equals(x.ScreeningEntity.ScreeningEntityName, screeningEntityName))
                    .OrderByDescending(x => x.ResponseDateTime);
                if (list.Any())
                    return list.First().ResponseDateTime;
            }
            return null;
        }

        public virtual bool HasResponded(string screeningEntityName)
        {
            if (!string.IsNullOrEmpty(screeningEntityName))
                foreach (ScreeningRequestEntityResponse r in this.ScreeningRequestEntityResponses.Where(x => !x.Archive))
                    if (string.Equals(r.ScreeningEntity.ScreeningEntityName, screeningEntityName))
                        return true;
            return false;
        }

        /// <summary>
        /// Generally, users can only add/remove individuals to/from requests associated with their own 'request entity'. This applies mainly to 
        /// initiators.  Initiators may also only view requests associated with their own request entity.
        /// 
        /// Checks whether given user is allowed to modify this screening request (during the initiation and validation stages).
        /// i.e., add and remove persons, propose new persons to attach to the screening request.
        /// 
        /// This method was ported from stored procedure logic in Profiling1.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual bool UserHasPermission(AdminUser user)
        {
            if (user != null)
                if (this.Creator == user  // (initiator) can view requests they created
                    || user.HasSameRequestEntityAs(this.Creator)  // (initiator) can view requests created by others from the same request entity
                    || user.RequestEntities.Contains(this.RequestEntity)  // (initiator) can view requests when they are a member of the requests' assigned request entity
                    || user.ScreeningEntities.Count > 0)  // any screening entity member can view any request
                    return true;
            return false;
        }

        public virtual RequestPerson AddPerson(Profiling2.Domain.Prf.Persons.Person person)
        {
            IEnumerable<RequestPerson> rps = this.Persons.Where(x => x.Person == person);

            RequestPerson rp = null;
            if (rps != null && rps.Count() > 0)
            {
                rp = rps.First();
                rp.Archive = false;
            }
            else
            {
                rp = new RequestPerson();
                rp.Person = person;
                rp.Request = this;
                this.Persons.Add(rp);
            }
            return rp;
        }

        public virtual RequestPerson GetPerson(Profiling2.Domain.Prf.Persons.Person person)
        {
            IEnumerable<RequestPerson> rps = this.Persons.Where(x => x.Person == person);
            if (rps != null && rps.Count() > 0)
                return rps.First();
            return null;
        }

        public virtual void AddProposedPerson(RequestProposedPerson rpp)
        {
            if (rpp != null && !this.ProposedPersons.Contains(rpp))
                this.ProposedPersons.Add(rpp);
        }

        public virtual void AddHistory(RequestHistory rh)
        {
            if (!this.RequestHistories.Contains(rh))
                this.RequestHistories.Add(rh);
        }

        public virtual RequestUnit AddUnit(Unit unit)
        {
            IEnumerable<RequestUnit> rus = this.Units.Where(x => x.Unit == unit);

            RequestUnit ru = null;
            if (rus != null && rus.Count() > 0)
            {
                ru = rus.First();
                ru.Archive = false;
            }
            else
            {
                ru = new RequestUnit();
                ru.Unit = unit;
                ru.Request = this;
                this.Units.Add(ru);
            }
            return ru;
        }

        public virtual RequestUnit GetUnit(Unit unit)
        {
            IEnumerable<RequestUnit> rus = this.Units.Where(x => x.Unit == unit);
            if (rus != null && rus.Count() > 0)
                return rus.First();
            return null;
        }
    }
}
