using System.Collections.Generic;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Units;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.Proposed;

namespace Profiling2.Domain.Contracts.Tasks.Screenings
{
    public interface IRequestPersonTasks
    {
        RequestPerson GetRequestPerson(int id);

        RequestPerson SaveRequestPerson(RequestPerson rp);

        RequestPerson SaveRequestPerson(Request request, Person person);

        RequestPerson ArchiveRequestPerson(Request request, Person person);

        RequestPersonStatus GetRequestPersonStatus(string name);

        RequestPersonHistory SaveRequestPersonHistory(RequestPerson rp, string status, AdminUser user);

        bool NominateRequestPerson(RequestPerson rp, AdminUser user);

        bool WithdrawRequestPersonNomination(RequestPerson rp, AdminUser user);

        RequestProposedPerson GetRequestProposedPerson(int requestProposedPersonId);

        RequestProposedPerson SaveRequestProposedPerson(RequestProposedPerson rpp);

        RequestProposedPersonStatus GetRequestProposedPersonStatus(string name);

        RequestProposedPersonHistory SaveRequestProposedPersonHistory(RequestProposedPerson rpp, string status, AdminUser user);

        IList<RequestPerson> GetNominatedRequestPersons();

        /// <summary>
        /// Uses the date of latest screening to determine a person's career at the time of screening.
        /// Reasoning is that we're interested in the function and rank of individuals at the time they were screened.
        /// 
        /// Audit trail is used to filter down the number of eligible date-valid careers.
        /// </summary>
        /// <param name="rp"></param>
        /// <param name="useAuditTrail">Whether to use audit tables in order to trim down the number of eligible careers at time of screening.</param>
        /// <returns></returns>
        IList<Career> GetHistoricalCurrentCareers(RequestPerson rp, bool useAuditTrail);

        /// <summary>
        /// Get all RequestPersons that have been screened to completion (i.e. associated screening request is complete).
        /// 
        /// Useful for statistics.
        /// </summary>
        /// <returns></returns>
        IList<RequestPerson> GetCompletedRequestPersons();

        RequestUnit SaveRequestUnit(Request request, Unit unit);

        RequestUnit ArchiveRequestUnit(Request request, Unit unit);
    }
}
