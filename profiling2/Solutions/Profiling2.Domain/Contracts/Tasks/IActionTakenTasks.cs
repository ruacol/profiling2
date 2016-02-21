using System.Collections.Generic;
using Profiling2.Domain.Prf.Actions;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IActionTakenTasks
    {
        IList<ActionTakenType> GetActionTakenTypes();

        IList<ActionTakenType> GetActionTakenTypesByName(string term);

        ActionTakenType GetActionTakenType(int id);

        ActionTakenType SaveActionTakenType(ActionTakenType type);

        bool DeleteActionTakenType(ActionTakenType type);

        ActionTaken GetActionTaken(int id);

        ActionTaken SaveActionTaken(ActionTaken at);

        void DeleteActionTaken(ActionTaken at);

        IList<ActionTaken> GetWantedActionsTaken();
    }
}
