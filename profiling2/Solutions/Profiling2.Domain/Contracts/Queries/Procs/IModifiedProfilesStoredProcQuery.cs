using System;
using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Queries.Procs
{
    public interface IModifiedProfilesStoredProcQuery
    {
        IList<ModifiedProfilesAuditDTO> GetRows(DateTime startDate, DateTime endDate);
    }
}
