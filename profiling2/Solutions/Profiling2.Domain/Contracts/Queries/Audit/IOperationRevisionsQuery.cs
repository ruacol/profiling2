using System.Collections.Generic;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Domain.Contracts.Queries.Audit
{
    public interface IOperationRevisionsQuery
    {
        IList<AuditTrailDTO> GetOperationRevisions(Operation op);

        IList<AuditTrailDTO> GetOperationCollectionRevisions<T>(Operation op);
    }
}
