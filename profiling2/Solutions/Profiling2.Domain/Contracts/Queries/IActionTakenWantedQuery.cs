using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Actions;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IActionTakenWantedQuery
    {
        IList<ActionTaken> GetWantedActionsTaken();

        IList<ActionTakenType> GetActionTakenTypes(string term);
    }
}
