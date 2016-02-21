using System.Collections.Generic;
using HrdbWebServiceClient.Domain;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IEventMatchingTasks
    {
        IDictionary<string, IList<object>> FindMatchingEventCandidates(JhroCase jhroCase, HrdbCase hrdbCase);

        IList<object> FindSimilarEvents(Event e);
    }
}
