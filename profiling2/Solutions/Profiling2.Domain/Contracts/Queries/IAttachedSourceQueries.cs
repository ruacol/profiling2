using System.Collections.Generic;
using Profiling2.Domain.DTO;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IAttachedSourceQueries
    {
        IList<PersonSourceDTO> GetPersonSourceDTOs(int personId);
    }
}
