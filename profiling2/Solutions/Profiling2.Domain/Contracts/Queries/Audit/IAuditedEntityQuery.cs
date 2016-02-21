using System;
using System.Collections.Generic;
using NHibernate.Envers;

namespace Profiling2.Domain.Contracts.Queries.Audit
{
    public interface IAuditedEntityQuery
    {
        IList<object[]> GetRawRevisions(Type type, RevisionType revisionType, DateTime? startDate, DateTime? endDate);
    }
}
