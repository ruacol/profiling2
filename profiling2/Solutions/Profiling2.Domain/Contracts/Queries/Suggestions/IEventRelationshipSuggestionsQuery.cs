using System.Collections.Generic;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Queries.Suggestions
{
    public interface IEventRelationshipSuggestionsQuery
    {
        IList<EventRelationship> GetEventRelationships(Person p);
    }
}
