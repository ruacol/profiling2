using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Queries.Suggestions
{
    public interface IPersonRelationshipSuggestionsQuery
    {
        IList<PersonRelationship> GetPersonRelationships(Person p);
    }
}
