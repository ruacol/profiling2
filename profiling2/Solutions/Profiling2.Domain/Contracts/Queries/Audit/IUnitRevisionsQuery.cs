using System.Collections.Generic;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Domain.Contracts.Queries.Audit
{
    public interface IUnitRevisionsQuery
    {
        IList<AuditTrailDTO> GetUnitRevisions(Unit u);

        IList<AuditTrailDTO> GetUnitCollectionRevisions<T>(Unit u);

        IList<ChangeActivityDTO> GetOldUnitAuditTrail(int unitId);
    }
}
