using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.Prf;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public class AdminAuditQuery : NHibernateQuery, IAdminAuditQuery
    {
        public IList<AdminAudit> GetAdminAudits(string tableName, int auditTypeId, DateTime startDate, DateTime endDate)
        {
            AdminAuditType typeAlias = null;

            return Session.QueryOver<AdminAudit>()
                .JoinAlias(x => x.AdminAuditType, () => typeAlias, JoinType.InnerJoin)
                .Where(x => x.ChangedTableName == tableName)
                .And(() => typeAlias.Id == auditTypeId)
                .And(Restrictions.On<AdminAudit>(x => x.ChangedDateTime).IsBetween(startDate).And(endDate))
                .OrderBy(x => x.ChangedDateTime).Asc
                .List<AdminAudit>();
        }
    }
}
