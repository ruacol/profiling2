using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.Scr;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IRequestQueries
    {
        /// <summary>
        /// Select requests which have a current, valid, request status.
        /// </summary>
        /// <returns></returns>
        IList<Request> GetValidRequests();

        IList<Request> GetAllRequests(ISession session);

        IList<Request> GetRequestsRequiringResponse();
    }
}
