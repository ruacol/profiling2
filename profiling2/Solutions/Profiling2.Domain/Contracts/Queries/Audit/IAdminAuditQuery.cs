using System;
using System.Collections.Generic;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Contracts.Queries.Audit
{
    public interface IAdminAuditQuery
    {
        IList<AdminAudit> GetAdminAudits(string tableName, int auditTypeId, DateTime startDate, DateTime endDate);
    }
}
