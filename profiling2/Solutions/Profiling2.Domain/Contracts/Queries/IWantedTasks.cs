using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IWantedTasks
    {
        IList<Person> GetWantedCommanders();
    }
}
