using System.Collections.Generic;
using NHibernate.Envers;
using NHibernate.Envers.Query;
using NHibernate.Transform;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public class UnitRevisionsQuery : NHibernateAuditQuery, IUnitRevisionsQuery
    {
        public IList<AuditTrailDTO> GetUnitRevisions(Unit u)
        {
            IList<object[]> objects = AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(Unit), false, true)
                .Add(AuditEntity.Id().Eq(u.Id))
                .GetResultList<object[]>();
            return this.AddDifferences<Unit>(this.TransformToDto(objects));
        }

        public IList<AuditTrailDTO> GetUnitCollectionRevisions<T>(Unit u)
        {
            IList<object[]> objects = AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(T), false, true)
                .Add(AuditEntity.Property("Unit").Eq(u))
                .GetResultList<object[]>();
            return this.AddDifferences<T>(this.TransformToDto(objects));
        }

        public IList<ChangeActivityDTO> GetOldUnitAuditTrail(int unitId)
        {
            string sql = @"
                SELECT  
                    A.AdminAuditID AS [LogNo],      
                    ISNULL(U.UserName,U.UserID) AS [Who],      
                    AT.[AdminAuditTypeName] 
                        + CASE WHEN RIGHT(AT.AdminAuditTypeName, 1) = 'E' THEN 'D ' ELSE 'ED ' END 
                        + A.ChangedTableName 
                        + ' ID = ' + CAST(A.ChangedRecordID AS NVARCHAR(12)) AS [What],      
                    A.ChangedDateTime AS [When],      
                    CASE WHEN A.ChangedColumns IS NULL THEN '' ELSE A.ChangedColumns END AS [PreviousValues],
                    CASE WHEN A.IsProfilingChange = 0 THEN 'Yes' ELSE '' END AS [NonProfilingChange]       
                FROM PRF_AdminAudit AS A      
                INNER JOIN
                (
                    SELECT A.AdminAuditID
                    FROM PRF_Unit AS U INNER JOIN PRF_AdminAudit AS A 
                        ON U.UnitID = A.ChangedRecordID  
                        AND A.ChangedTableName = 'Unit'    
                    WHERE U.UnitID = :UnitID
                ) AS AuditTrail
                ON AuditTrail.AdminAuditID = A.AdminAuditID
                INNER JOIN PRF_AdminUser AS U ON A.AdminUserID = U.AdminUserID
                INNER JOIN PRF_AdminAuditType AS AT ON A.AdminAuditTypeID = AT.AdminAuditTypeID
                ORDER BY A.ChangedDateTime DESC
            ";
            return Session.CreateSQLQuery(sql)
                .SetInt32("UnitID", unitId)
                .SetResultTransformer(Transformers.AliasToBean(typeof(ChangeActivityDTO)))
                .List<ChangeActivityDTO>();
        }
    }
}
