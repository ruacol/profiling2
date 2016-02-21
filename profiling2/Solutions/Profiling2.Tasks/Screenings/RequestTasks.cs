using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Attach;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.Proposed;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks.Screenings
{
    public class RequestTasks : IRequestTasks
    {
        private readonly IUserTasks userTasks;
        private readonly INHibernateRepository<Request> requestRepository;
        private readonly INHibernateRepository<RequestEntity> requestEntityRepository;
        private readonly INHibernateRepository<RequestType> requestTypeRepository;
        private readonly INHibernateRepository<RequestStatus> requestStatusRepository;
        private readonly INHibernateRepository<RequestHistory> requestHistoryRepository;
        private readonly IRequestQueries requestQueries;
        private readonly ILuceneTasks luceneTasks;

        public RequestTasks(IUserTasks userTasks,
            INHibernateRepository<Request> requestRepository,
            INHibernateRepository<RequestEntity> requestEntityRepository,
            INHibernateRepository<RequestType> requestTypeRepository,
            INHibernateRepository<RequestStatus> requestStatusRepository,
            INHibernateRepository<RequestHistory> requestHistoryRepository,
            INHibernateRepository<RequestPerson> requestPersonRepository,
            INHibernateRepository<RequestPersonStatus> requestPersonStatusRepository,
            IRequestQueries requestQueries,
            ILuceneTasks luceneTasks)
        {
            this.userTasks = userTasks;
            this.requestRepository = requestRepository;
            this.requestEntityRepository = requestEntityRepository;
            this.requestTypeRepository = requestTypeRepository;
            this.requestStatusRepository = requestStatusRepository;
            this.requestHistoryRepository = requestHistoryRepository;
            this.requestQueries = requestQueries;
            this.luceneTasks = luceneTasks;
        }

        public Request Get(int requestId)
        {
            return this.requestRepository.Get(requestId);
        }

        public IList<Request> GetValidRequests()
        {
            return this.requestQueries.GetValidRequests();
        }

        public IList<HistoryViewModel> GetRequestHistory(int requestId)
        {
            IList<HistoryViewModel> histories = new List<HistoryViewModel>();
            Request r = this.Get(requestId);
            if (r != null)
            {
                foreach (RequestHistory rh in r.RequestHistories.Where(x => !x.Archive))
                    histories.Add(new HistoryViewModel()
                    {
                        Status = rh.RequestStatus.ToString(),
                        Date = rh.DateStatusReached,
                        User = rh.AdminUser,
                        Type = typeof(Request),
                        Notes = rh.Notes
                    });

                foreach (RequestPerson rp in r.Persons)
                    foreach (RequestPersonHistory rph in rp.RequestPersonHistories.Where(x => !x.Archive))
                        histories.Add(new HistoryViewModel()
                        {
                            Status = rph.RequestPersonStatus + " (" + rp.Person.Name + ")",
                            Date = rph.DateStatusReached,
                            User = rph.AdminUser,
                            Type = typeof(RequestPerson),
                            Notes = rph.Notes
                        });

                foreach (RequestProposedPerson rpp in r.ProposedPersons)
                    foreach (RequestProposedPersonHistory rpph in rpp.RequestProposedPersonHistories.Where(x => !x.Archive))
                        histories.Add(new HistoryViewModel()
                        {
                            Status = rpph.RequestProposedPersonStatus + " (" + rpp.PersonName + ")",
                            Date = rpph.DateStatusReached,
                            User = rpph.AdminUser,
                            Type = typeof(RequestProposedPerson),
                            Notes = rpph.Notes
                        });

                foreach (RequestAttachment ra in r.RequestAttachments)
                    foreach (RequestAttachmentHistory rah in ra.Histories.Where(x => !x.Archive))
                        histories.Add(new HistoryViewModel()
                        {
                            Status = rah.RequestAttachmentStatus + " (" + ra.Attachment.FileName + ")",
                            Date = rah.DateStatusReached,
                            User = rah.AdminUser,
                            Type = typeof(RequestAttachment),
                            Notes = rah.Notes
                        });
            }
            return histories.OrderBy(x => x.Date).ToList<HistoryViewModel>();
        }

        public IQueryable<Request> GetRequestsQueryable()
        {
            return this.GetAllRequests(null).AsQueryable<Request>();
        }
        
        public IList<Request> GetAllRequests(ISession session)
        {
            return this.requestQueries.GetAllRequests(session);
        }

        public IList<Request> GetRequestsRequiringResponse()
        {
            return this.requestQueries.GetRequestsRequiringResponse();
        }

        public Request SaveOrUpdateRequest(Request request)
        {
            if (request.Id > 0)
                this.luceneTasks.UpdateRequest(request);
            return this.requestRepository.SaveOrUpdate(request);
        }

        public bool DeleteRequest(Request request)
        {
            if (request != null)
                if (new string[] { RequestStatus.NAME_CREATED, RequestStatus.NAME_EDITED }.Contains(request.CurrentStatus.RequestStatusName))
                {
                    //this.requestRepository.Delete(request);
                    request.Archive = true;
                    this.requestRepository.SaveOrUpdate(request);

                    this.luceneTasks.DeleteRequest(request.Id);

                    return true;
                }
            return false;
        }

        public string GetNextReferenceNumber()
        {
            return this.GetNextReferenceNumber(1);
        }

        public string GetNextReferenceNumber(int num)
        {
            string refNum = DateTime.Now.ToString("yyyy-MM-dd") + "-" + num.ToString();
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("ReferenceNumber", refNum);
            Request request = this.requestRepository.FindOne(criteria);
            if (request != null)
                return this.GetNextReferenceNumber(num + 1);
            return refNum;
        }

        public IEnumerable<RequestEntity> GetRequestEntities()
        {
            return this.requestEntityRepository.GetAll();
        }

        public RequestEntity GetRequestEntity(int requestEntityId)
        {
            return this.requestEntityRepository.Get(requestEntityId);
        }

        // will only allow one RequestEntity per user.
        public bool SetRequestEntity(int requestEntityId, int userId)
        {
            RequestEntity re = this.requestEntityRepository.Get(requestEntityId);
            AdminUser u = this.userTasks.GetAdminUser(userId);
            if (u != null)
            {
                foreach (RequestEntity e in this.requestEntityRepository.GetAll())
                    u.RemoveRequestEntity(e);
                if (re != null)
                    u.AddRequestEntity(re);
                return true;
            }
            return false;
        }

        public IEnumerable<RequestType> GetRequestTypes()
        {
            return this.requestTypeRepository.GetAll();
        }

        public RequestType GetRequestType(int requestTypeId)
        {
            return this.requestTypeRepository.Get(requestTypeId);
        }

        public IEnumerable<RequestStatus> GetRequestStatuses()
        {
            return this.requestStatusRepository.GetAll();
        }

        public RequestStatus GetRequestStatus(string name)
        {
            // this works because the name column has a unique index
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("RequestStatusName", name);
            return this.requestStatusRepository.FindOne(criteria);
        }

        public RequestHistory SaveRequestHistory(int requestId, int requestStatusId, string username)
        {
            return this.SaveRequestHistory(requestId, requestStatusId, username, string.Empty);
        }

        public RequestHistory SaveRequestHistory(int requestId, int requestStatusId, string username, string notes)
        {
            Request request = this.requestRepository.Get(requestId);

            RequestHistory rh = new RequestHistory();
            rh.AdminUser = this.userTasks.GetAdminUser(username);
            rh.DateStatusReached = DateTime.Now;
            rh.Request = request;
            rh.RequestStatus = this.requestStatusRepository.Get(requestStatusId);
            rh.Notes = notes;
            request.AddHistory(rh);
            rh = this.requestHistoryRepository.Save(rh);

            if (string.Equals(RequestStatus.NAME_CREATED, rh.RequestStatus.RequestStatusName))
                this.luceneTasks.AddRequest(request);
            else
                this.luceneTasks.UpdateRequest(request);

            return rh;
        }

        public IList<Request> GetRequestsForConsolidation()
        {
            string[] statusesForConsolidation = { RequestStatus.NAME_SENT_FOR_SCREENING, RequestStatus.NAME_SCREENING_IN_PROGRESS, RequestStatus.NAME_SENT_FOR_CONSOLIDATION };
            IList<Request> requests = this.requestRepository.GetAll();
            return requests.Where(x => statusesForConsolidation.Contains(x.CurrentStatus.RequestStatusName)).ToList();
        }

        public IDictionary<RequestType, int> GetRequestCountByType(DateTime start, DateTime end)
        {
            IList<Request> requests = this.requestRepository.GetAll()
                .Where(x => !x.Archive 
                    && x.CurrentStatus.RequestStatusName == RequestStatus.NAME_COMPLETED
                    && x.CurrentStatusDate >= start
                    && x.CurrentStatusDate <= end)
                .ToList();

            IDictionary<RequestType, int> dict = new Dictionary<RequestType, int>();
            foreach (RequestType rt in this.GetRequestTypes())
                dict.Add(rt, requests.Where(x => x.RequestType == rt).Count());

            return dict;
        }

        public IList<Request> GetRequestsForFinalization()
        {
            string[] statusesForFinalization = { RequestStatus.NAME_SENT_FOR_FINAL_DECISION };
            IList<Request> requests = this.requestRepository.GetAll();
            return requests.Where(x => statusesForFinalization.Contains(x.CurrentStatus.RequestStatusName)).ToList();
        }

        public IList<Request> GetRequestsForValidation()
        {
            IList<Request> requests = this.requestRepository.GetAll();
            return requests.Where(x => string.Equals(RequestStatus.NAME_SENT_FOR_VALIDATION, x.CurrentStatus.RequestStatusName)
                || (!x.HasBeenForwardedForScreening && x.HasBeenSentForValidation && string.Equals(RequestStatus.NAME_EDITED, x.CurrentStatus.RequestStatusName)))
            .ToList();
        }
    }
}
