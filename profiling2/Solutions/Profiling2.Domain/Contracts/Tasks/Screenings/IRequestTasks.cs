using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using Profiling2.Domain.Scr;

namespace Profiling2.Domain.Contracts.Tasks.Screenings
{
    public interface IRequestTasks
    {
        Request Get(int requestId);

        /// <summary>
        /// Get requests that have a valid conditionality status, i.e. past the initiation stage and not rejected or deleted.
        /// </summary>
        /// <returns></returns>
        IList<Request> GetValidRequests();

        IList<HistoryViewModel> GetRequestHistory(int requestId);

        IQueryable<Request> GetRequestsQueryable();

        IList<Request> GetAllRequests(ISession session);

        IList<Request> GetRequestsRequiringResponse();

        Request SaveOrUpdateRequest(Request request);

        bool DeleteRequest(Request request);

        string GetNextReferenceNumber();

        string GetNextReferenceNumber(int num);

        IEnumerable<RequestEntity> GetRequestEntities();

        RequestEntity GetRequestEntity(int requestEntityId);

        bool SetRequestEntity(int requestEntityId, int userId);

        IEnumerable<RequestType> GetRequestTypes();

        RequestType GetRequestType(int requestTypeId);

        IEnumerable<RequestStatus> GetRequestStatuses();

        RequestStatus GetRequestStatus(string name);

        RequestHistory SaveRequestHistory(int requestId, int requestStatusId, string username);

        RequestHistory SaveRequestHistory(int requestId, int requestStatusId, string username, string notes);

        IList<Request> GetRequestsForConsolidation();

        IDictionary<RequestType, int> GetRequestCountByType(DateTime start, DateTime end);
        
        IList<Request> GetRequestsForFinalization();

        IList<Request> GetRequestsForValidation();
    }
}
