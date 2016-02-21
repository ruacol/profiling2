using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Scr;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class RequestQueries : NHibernateQuery, IRequestQueries
    {
        protected IList<Request> GetRequests(string[] statusNames)
        {
            Request rAlias = null;
            RequestHistory rhAlias = null;
            RequestStatus rsAlias = null;

            var currentRequestHistoryIds = QueryOver.Of<RequestHistory>()
                .Where(rh => rh.Request.Id == rAlias.Id)
                .And(rh => !rh.Archive)
                .OrderBy(rh => rh.DateStatusReached).Desc
                .Select(rh => rh.Id)
                .Take(1);

            return Session.QueryOver<Request>(() => rAlias)
                .JoinAlias(() => rAlias.RequestHistories, () => rhAlias)
                .JoinAlias(() => rhAlias.RequestStatus, () => rsAlias)
                .WhereRestrictionOn(() => rsAlias.RequestStatusName).IsIn(statusNames)
                .And(() => !rAlias.Archive)
                .WithSubquery.WhereProperty(() => rhAlias.Id).In(currentRequestHistoryIds)
                .OrderBy(() => rAlias.ReferenceNumber).Desc
                .TransformUsing(Transformers.DistinctRootEntity)
                .List<Request>();
        }

        public IList<Request> GetValidRequests()
        {
            return this.GetRequests(new string[] {
                        RequestStatus.NAME_SENT_FOR_VALIDATION,
                        RequestStatus.NAME_SENT_FOR_SCREENING, 
                        RequestStatus.NAME_SCREENING_IN_PROGRESS, 
                        RequestStatus.NAME_SENT_FOR_CONSOLIDATION,
                        RequestStatus.NAME_SENT_FOR_FINAL_DECISION,
                        RequestStatus.NAME_COMPLETED
                    });
        }

        public IList<Request> GetAllRequests(ISession session)
        {
            ISession thisSession = session == null ? Session : session;
            return thisSession.QueryOver<Request>().Where(x => !x.Archive).List();
        }

        public IList<Request> GetRequestsRequiringResponse()
        {
            return this.GetRequests(new string[] {
                        RequestStatus.NAME_SENT_FOR_SCREENING, 
                        RequestStatus.NAME_SCREENING_IN_PROGRESS
                    });
        }
    }
}
