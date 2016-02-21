using System.Collections.Generic;
using NHibernate.Envers;
using NHibernate.Envers.Query;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public class OperationRevisionsQuery : NHibernateAuditQuery, IOperationRevisionsQuery
    {
        public IList<AuditTrailDTO> GetOperationRevisions(Operation op)
        {
            IList<object[]> objects = AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(Operation), false, true)
                .Add(AuditEntity.Id().Eq(op.Id))
                .GetResultList<object[]>();
            return this.AddDifferences<Operation>(this.TransformToDto(objects));
        }

        public IList<AuditTrailDTO> GetOperationCollectionRevisions<T>(Operation op)
        {
            IList<object[]> objects = AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(T), false, true)
                .Add(AuditEntity.Property("Operation").Eq(op))
                .GetResultList<object[]>();
            return this.AddDifferences<T>(this.TransformToDto(objects));
        }
    }
}
