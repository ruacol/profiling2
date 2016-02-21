using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Queries.Search
{
    public interface IPersonRelationshipTypeNameQuery
    {
        IList<PersonRelationshipType> GetResults(string term);
    }
}
