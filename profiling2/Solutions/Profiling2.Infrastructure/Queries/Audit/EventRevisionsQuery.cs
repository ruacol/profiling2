using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers;
using NHibernate.Envers.Query;
using NHibernate.Transform;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Events;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public class EventRevisionsQuery : NHibernateAuditQuery, IEventRevisionsQuery
    {
        public IList<AuditTrailDTO> GetEventRevisions(Event e)
        {
            IList<object[]> objects = AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(Event), false, true)
                .Add(AuditEntity.Id().Eq(e.Id))
                .GetResultList<object[]>();
            return this.AddDifferences<Event>(this.TransformToDto(objects));
        }

        public IList<AuditTrailDTO> GetEventCollectionRevisions<T>(Event e)
        {
            IList<object[]> objects = AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(T), false, true)
                .Add(AuditEntity.Property("Event").Eq(e))
                .GetResultList<object[]>();
            return this.AddDifferences<T>(this.TransformToDto(objects));
        }

        public IList<AuditTrailDTO> GetEventRelationshipRevisions(Event e)
        {
            IList<object[]> subjects = AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(EventRelationship), false, true)
                .Add(AuditEntity.Property("SubjectEvent").Eq(e))
                .GetResultList<object[]>();

            IList<object[]> objects = AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(EventRelationship), false, true)
                .Add(AuditEntity.Property("ObjectEvent").Eq(e))
                .GetResultList<object[]>();

            return this.AddDifferences<EventRelationship>(this.TransformToDto(subjects.Concat(objects).ToList()));
        }

        public IList<ChangeActivityDTO> GetOldEventAuditTrail(int eventId)
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
                    FROM PRF_Event AS E INNER JOIN PRF_AdminAudit AS A 
                        ON E.EventID = A.ChangedRecordID  
                        AND A.ChangedTableName = 'Event'    
                    WHERE E.EventID = :EventID
                    UNION ALL
                    SELECT A.AdminAuditID
                    FROM PRF_PersonResponsibility AS PR INNER JOIN PRF_AdminAudit AS A 
                        ON PR.PersonResponsibilityID = A.ChangedRecordID  
                        AND A.ChangedTableName = 'PersonResponsibility'    
                    WHERE PR.EventID = :EventID
                    UNION ALL
                    SELECT A.AdminAuditID
                    FROM PRF_OrganizationResponsibility AS OResp INNER JOIN PRF_AdminAudit AS A 
                        ON OResp.OrganizationResponsibilityID = A.ChangedRecordID  
                        AND A.ChangedTableName = 'OrganizationResponsibility'    
                    WHERE OResp.EventID = :EventID
                    UNION ALL
                    SELECT A.AdminAuditID
                    FROM PRF_EventSource AS ES INNER JOIN PRF_AdminAudit AS A 
                        ON ES.EventSourceID = A.ChangedRecordID  
                        AND A.ChangedTableName = 'EventSource'
                    WHERE ES.EventID = :EventID
                    UNION ALL
                    SELECT A.AdminAuditID
                    FROM PRF_ActionTaken AS AT INNER JOIN PRF_AdminAudit AS A 
                        ON AT.ActionTakenID = A.ChangedRecordID  
                        AND A.ChangedTableName = 'ActionTaken'
                    WHERE AT.EventID = :EventID
                    UNION ALL
                    SELECT A.AdminAuditID
                    FROM PRF_EventRelationship AS ER INNER JOIN PRF_AdminAudit AS A 
                        ON ER.EventRelationshipID = A.ChangedRecordID  
                        AND A.ChangedTableName = 'EventRelationship'    
                    WHERE ER.SubjectEventID = :EventID OR ER.ObjectEventID = :EventID
                ) AS AuditTrail
                ON AuditTrail.AdminAuditID = A.AdminAuditID
                INNER JOIN PRF_AdminUser AS U ON A.AdminUserID = U.AdminUserID
                INNER JOIN PRF_AdminAuditType AS AT ON A.AdminAuditTypeID = AT.AdminAuditTypeID
                ORDER BY A.ChangedDateTime DESC
            ";
            return Session.CreateSQLQuery(sql)
                .SetInt32("EventID", eventId)
                .SetResultTransformer(Transformers.AliasToBean(typeof(ChangeActivityDTO)))
                .List<ChangeActivityDTO>();
        }

        public int GetEventCount(DateTime date)
        {
            int revision = this.GetRevisionNumberForDate(date);

            return AuditReaderFactory.Get(Session).CreateQuery()
                .ForEntitiesAtRevision(typeof(Event), Convert.ToInt64(revision))
                .Add(AuditEntity.Property("Archive").Eq(false))
                .GetResultList().Count;
        }
    }
}
