using System.Collections.Generic;
using Profiling2.Domain.DTO;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IObjectSourceDuplicatesQuery
    {
        IList<ObjectSourceDuplicateDTO> GetPersonSourceDuplicates();

        IList<ObjectSourceDuplicateDTO> GetEventSourceDuplicates();

        IList<ObjectSourceDuplicateDTO> GetOrganizationSourceDuplicates();
    }
}
