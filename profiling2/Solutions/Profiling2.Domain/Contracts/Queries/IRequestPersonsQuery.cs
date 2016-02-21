using System.Collections.Generic;
using Profiling2.Domain.Scr.Person;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IRequestPersonsQuery
    {
        IList<RequestPerson> GetNominatedRequestPersons();

        IList<RequestPerson> GetNominationWithdrawnRequestPersons();
    }
}
