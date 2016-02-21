using System;
using System.Collections.Generic;
using NHibernate.Envers;
using NHibernate.Envers.Query;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public class AuditedEntityQuery : NHibernateQuery, IAuditedEntityQuery
    {
        public IList<object[]> GetRawRevisions(Type type, RevisionType revisionType, DateTime? startDate, DateTime? endDate)
        {
            IAuditQuery q = AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(type, false, true)
                .Add(AuditEntity.RevisionType().Eq(revisionType))
                .AddOrder(AuditEntity.RevisionProperty("REVTSTMP").Desc());
            if (startDate.HasValue && endDate.HasValue)
                q = q.Add(AuditEntity.RevisionProperty("REVTSTMP").Between(startDate, endDate));
            return q.GetResultList<object[]>();
        }
    }
}
