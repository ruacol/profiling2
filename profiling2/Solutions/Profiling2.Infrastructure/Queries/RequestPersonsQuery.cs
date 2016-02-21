using System.Collections.Generic;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class RequestPersonsQuery : NHibernateQuery, IRequestPersonsQuery
    {
        // TODO doesn't use most-recent RequestPerson history - this needs to be done further down the line.
        // can be done with cross apply.
        // TODO same for most-recent RequestStatus history.
        public IList<RequestPerson> GetNominatedRequestPersons()
        {
            return this.GetRequestPersons(RequestPersonStatus.NAME_NOMINATED);
        }

        public IList<RequestPerson> GetNominationWithdrawnRequestPersons()
        {
            return this.GetRequestPersons(RequestPersonStatus.NAME_NOMINATION_WITHDRAWN);
        }

        protected IList<RequestPerson> GetRequestPersons(string requestPersonStatus)
        {
            RequestPerson rpAlias = null;
            RequestPersonHistory rphAlias = null;
            RequestPersonStatus rpsAlias = null;
            Request rAlias = null;
            RequestHistory rhAlias = null;
            RequestStatus rsAlias = null;

            return Session.QueryOver<RequestPerson>(() => rpAlias)
                .JoinAlias(() => rpAlias.RequestPersonHistories, () => rphAlias)
                .JoinAlias(() => rphAlias.RequestPersonStatus, () => rpsAlias)
                .JoinAlias(() => rpAlias.Request, () => rAlias)
                .JoinAlias(() => rAlias.RequestHistories, () => rhAlias)
                .JoinAlias(() => rhAlias.RequestStatus, () => rsAlias)
                .WhereRestrictionOn(() => rsAlias.RequestStatusName)
                    .IsIn(new string[] { RequestStatus.NAME_SENT_FOR_SCREENING, RequestStatus.NAME_SCREENING_IN_PROGRESS, RequestStatus.NAME_SENT_FOR_CONSOLIDATION })
                .And(() => rpsAlias.RequestPersonStatusName == requestPersonStatus)
                .TransformUsing(Transformers.DistinctRootEntity)
                .List<RequestPerson>();
        }
    }
}
