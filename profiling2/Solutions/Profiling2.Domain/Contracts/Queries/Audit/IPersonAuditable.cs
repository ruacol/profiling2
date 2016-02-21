using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Queries.Audit
{
    /// <summary>
    /// NOTE implementing classes need to be registered manually in ComponentRegistrar.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPersonAuditable<T>
    {
        CompareLogic CompareLogic { get; }

        IList<object[]> GetRawRevisions(Person target);
    }
}
