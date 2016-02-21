using System;
using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Units;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.Proposed;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks.Screenings
{
    public class RequestPersonTasks : IRequestPersonTasks
    {
        private readonly INHibernateRepository<RequestPerson> requestPersonRepository;
        private readonly INHibernateRepository<RequestPersonStatus> requestPersonStatusRepository;
        private readonly INHibernateRepository<RequestPersonHistory> requestPersonHistoryRepository;
        private readonly INHibernateRepository<RequestProposedPerson> proposedRepo;
        private readonly INHibernateRepository<RequestProposedPersonStatus> proposedStatusRepo;
        private readonly INHibernateRepository<RequestProposedPersonHistory> proposedHistoryRepo;
        private readonly INHibernateRepository<RequestUnit> requestUnitRepository;
        private readonly IRequestPersonsQuery requestPersonsQuery;
        private readonly IAuditTasks auditTasks;

        public RequestPersonTasks(INHibernateRepository<RequestPerson> requestPersonRepository,
            INHibernateRepository<RequestPersonStatus> requestPersonStatusRepository,
            INHibernateRepository<RequestPersonHistory> requestPersonHistoryRepository,
            INHibernateRepository<RequestProposedPerson> proposedRepo,
            INHibernateRepository<RequestProposedPersonStatus> proposedStatusRepo,
            INHibernateRepository<RequestProposedPersonHistory> proposedHistoryRepo,
            INHibernateRepository<RequestUnit> requestUnitRepository,
            IRequestPersonsQuery requestPersonsQuery,
            IAuditTasks auditTasks)
        {
            this.requestPersonRepository = requestPersonRepository;
            this.requestPersonStatusRepository = requestPersonStatusRepository;
            this.requestPersonHistoryRepository = requestPersonHistoryRepository;
            this.proposedRepo = proposedRepo;
            this.proposedStatusRepo = proposedStatusRepo;
            this.proposedHistoryRepo = proposedHistoryRepo;
            this.requestUnitRepository = requestUnitRepository;
            this.requestPersonsQuery = requestPersonsQuery;
            this.auditTasks = auditTasks;
        }

        public RequestPerson GetRequestPerson(int id)
        {
            return this.requestPersonRepository.Get(id);
        }

        public RequestPerson SaveRequestPerson(RequestPerson rp)
        {
            return this.requestPersonRepository.SaveOrUpdate(rp);
        }

        public RequestPerson SaveRequestPerson(Request request, Person person)
        {
            if (request != null && person != null)
            {
                RequestPerson rp = request.AddPerson(person);
                return this.requestPersonRepository.SaveOrUpdate(rp);
            }
            return null;
        }

        public RequestPerson ArchiveRequestPerson(Request request, Person person)
        {
            if (request != null && person != null)
            {
                RequestPerson rp = request.GetPerson(person);
                if (rp != null)
                {
                    rp.Archive = true;
                    return this.requestPersonRepository.SaveOrUpdate(rp);
                }
            }
            return null;
        }

        public RequestPersonStatus GetRequestPersonStatus(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("RequestPersonStatusName", name);
            return this.requestPersonStatusRepository.FindOne(criteria);
        }

        public RequestPersonHistory SaveRequestPersonHistory(RequestPerson rp, string status, AdminUser user)
        {
            if (rp != null && !string.IsNullOrEmpty(status) && user != null)
            {
                RequestPersonHistory h = new RequestPersonHistory();
                h.RequestPerson = rp;
                h.RequestPersonStatus = this.GetRequestPersonStatus(status);
                h.AdminUser = user;
                h.DateStatusReached = DateTime.Now;
                rp.AddRequestPersonHistory(h);
                return this.requestPersonHistoryRepository.SaveOrUpdate(h);
            }
            return null;
        }

        public bool NominateRequestPerson(RequestPerson rp, AdminUser user)
        {
            RequestPersonHistory rph = this.SaveRequestPersonHistory(rp, RequestPersonStatus.NAME_NOMINATED, user);
            return rph != null;
        }

        public bool WithdrawRequestPersonNomination(RequestPerson rp, AdminUser user)
        {
            RequestPersonHistory rph = this.SaveRequestPersonHistory(rp, RequestPersonStatus.NAME_NOMINATION_WITHDRAWN, user);
            return rph != null;
        }

        public RequestProposedPerson GetRequestProposedPerson(int requestProposedPersonId)
        {
            return this.proposedRepo.Get(requestProposedPersonId);
        }

        public RequestProposedPerson SaveRequestProposedPerson(RequestProposedPerson rpp)
        {
            if (rpp != null && rpp.Request != null)
            {
                rpp.Request.AddProposedPerson(rpp);
                rpp = this.proposedRepo.SaveOrUpdate(rpp);
            }
            return rpp;
        }

        public RequestProposedPersonStatus GetRequestProposedPersonStatus(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("RequestProposedPersonStatusName", name);
            return this.proposedStatusRepo.FindOne(criteria);
        }

        public RequestProposedPersonHistory SaveRequestProposedPersonHistory(RequestProposedPerson rpp, string status, AdminUser user)
        {
            if (rpp != null && !string.IsNullOrEmpty(status) && user != null)
            {
                RequestProposedPersonHistory h = new RequestProposedPersonHistory();
                h.RequestProposedPerson = rpp;
                h.RequestProposedPersonStatus = this.GetRequestProposedPersonStatus(status);
                h.AdminUser = user;
                h.DateStatusReached = DateTime.Now;
                rpp.AddRequestProposedPersonHistory(h);
                return this.proposedHistoryRepo.SaveOrUpdate(h);
            }
            return null;
        }

        public IList<RequestPerson> GetNominatedRequestPersons()
        {
            return this.requestPersonsQuery.GetNominatedRequestPersons()
                // only RequestPersons whose current status is nominated
                .Where(x => x.MostRecentHistory.RequestPersonStatus.RequestPersonStatusName == RequestPersonStatus.NAME_NOMINATED
                    // only in requests that are currently being screened
                    && new string[] { RequestStatus.NAME_SENT_FOR_SCREENING, RequestStatus.NAME_SCREENING_IN_PROGRESS, RequestStatus.NAME_SENT_FOR_CONSOLIDATION }
                        .Contains(x.Request.CurrentStatus.RequestStatusName))
                .ToList();
        }

        public IList<Career> GetHistoricalCurrentCareers(RequestPerson rp, bool useAuditTrail)
        {
            if (rp != null && rp.Request != null)
            {
                // get historical date
                DateTime? date = rp.Request.GetLatestScreeningDate();

                if (date.HasValue)
                {
                    IList<Career> list = new List<Career>();

                    // all valid (date-wise) careers; need to figure out the person's correct function/s at given date
                    IList<Career> candidates = rp.Person.GetCareersAtTimeOf(date.Value);

                    if (candidates != null && candidates.Any())
                    {
                        // if any date-valid careers are marked current, select those
                        list.Concat(candidates.Where(x => x.IsCurrentCareer));

                        // if this is an old request and there are no date-valid careers marked current, then select the most recent
                        // TODO as requests age and their subject profiles advance in career, their careers will no longer be marked current.
                        // this is a problem for those with multiple functions, as the logic below will only ensure the most recent is returned.
                        if (list.Count == 0)
                        {
                            list.Add(candidates.First());
                            candidates.RemoveAt(0);
                        }

                        if (useAuditTrail)
                        {
                            // select the remaining date-valid careers
                            candidates = candidates.Where(x => !x.IsCurrentCareer).ToList();

                            // of the remaining candidates, include only if that career was flagged 'current' at time of screening.
                            // for this we need to check audit trail.
                            // IMPORTANT: this assumes the that the 'current' careers in the audit trail were correct - not always the case!
                            IList<int> oldCareerIds = this.auditTasks.GetHistoricalCurrentCareers(rp.Person, date.Value).Select(x => x.Id).ToList();
                            foreach (Career c in candidates)
                                if (oldCareerIds.Contains(c.Id)
                                    || c.IsCurrentCareer)  // if our given date is recent, we can rely on the IsCurrentCareer flag
                                    list.Add(c);
                        }
                    }

                    return list;
                }
            }
            return null;
        }

        public IList<RequestPerson> GetCompletedRequestPersons()
        {
            return this.requestPersonRepository.GetAll()
                .Where(x => !x.Archive 
                    && !x.Request.Archive 
                    && x.Request.CurrentStatus != null 
                    && string.Equals(x.Request.CurrentStatus.RequestStatusName, RequestStatus.NAME_COMPLETED)
                    && x.GetScreeningRequestPersonFinalDecision() != null)
                .ToList();
        }

        public RequestUnit SaveRequestUnit(Request request, Unit unit)
        {
            if (request != null && unit != null)
            {
                RequestUnit ru = request.AddUnit(unit);
                return this.requestUnitRepository.SaveOrUpdate(ru);
            }
            return null;
        }

        public RequestUnit ArchiveRequestUnit(Request request, Unit unit)
        {
            if (request != null && unit != null)
            {
                RequestUnit ru = request.GetUnit(unit);
                if (ru != null)
                {
                    ru.Archive = true;
                    return this.requestUnitRepository.SaveOrUpdate(ru);
                }
            }
            return null;
        }
    }
}
