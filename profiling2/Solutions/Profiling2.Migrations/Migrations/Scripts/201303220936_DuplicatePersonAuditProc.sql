SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PRF_SP_Summaries_ProfileChangeActivity_NHibernate]      
(  
 @PersonID INT  
)  
AS      
BEGIN      
 ;    
 WITH ProfileAuditTrail    
 AS    
 (    
  SELECT    
   A.[AdminAuditID]  
  FROM [dbo].[PRF_Person] AS P    
  INNER JOIN [dbo].[PRF_AdminAudit] AS A    
  ON P.[PersonID] = A.[ChangedRecordID]    
  AND A.[ChangedTableName] = 'Person'    
  WHERE P.[PersonID] = @PersonID    
  AND P.[Archive] = 0    
 UNION ALL    
  SELECT    
   A.[AdminAuditID]  
  FROM [dbo].[PRF_Career] AS C    
  INNER JOIN [dbo].[PRF_AdminAudit] AS A    
  ON C.[CareerID] = A.[ChangedRecordID]    
  AND A.[ChangedTableName] = 'Career'    
  WHERE C.[PersonID] = @PersonID    
  AND C.[Archive] = 0    
 UNION ALL    
  SELECT    
   A.[AdminAuditID]  
  FROM [dbo].[PRF_PersonAlias] AS PA    
  INNER JOIN [dbo].[PRF_AdminAudit] AS A    
  ON PA.[PersonAliasID] = A.[ChangedRecordID]    
  AND A.[ChangedTableName] = 'PersonAlias'    
  WHERE PA.[PersonID] = @PersonID    
  AND PA.[Archive] = 0    
 UNION ALL    
  SELECT    
   A.[AdminAuditID]  
  FROM [dbo].[PRF_PersonPhoto] AS PP    
  INNER JOIN [dbo].[PRF_AdminAudit] AS A    
  ON PP.[PersonPhotoID] = A.[ChangedRecordID]    
  AND A.[ChangedTableName] = 'PersonPhoto'    
  WHERE PP.[PersonID] = @PersonID    
  AND PP.[Archive] = 0    
 UNION ALL    
  SELECT    
   A.[AdminAuditID]  
  FROM [dbo].[PRF_PersonRelationship] AS PPR    
  INNER JOIN [dbo].[PRF_AdminAudit] AS A    
  ON PPR.[PersonRelationshipID] = A.[ChangedRecordID]    
  AND A.[ChangedTableName] = 'PersonRelationship'    
  WHERE (PPR.[SubjectPersonID] = @PersonID OR PPR.[ObjectPersonID] = @PersonID)    
  AND PPR.[Archive] = 0    
 UNION ALL    
  SELECT    
   A.[AdminAuditID]  
  FROM [dbo].[PRF_PersonResponsibility] AS PR    
  INNER JOIN [dbo].[PRF_AdminAudit] AS A    
  ON PR.[PersonResponsibilityID] = A.[ChangedRecordID]    
  AND A.[ChangedTableName] = 'PersonResponsibility'    
  WHERE PR.[PersonID] = @PersonID    
  AND PR.[Archive] = 0    
 UNION ALL    
  SELECT    
   A.[AdminAuditID]  
  FROM [dbo].[PRF_PersonSource] AS PS    
  INNER JOIN [dbo].[PRF_AdminAudit] AS A    
  ON PS.[PersonSourceID] = A.[ChangedRecordID]    
  AND A.[ChangedTableName] = 'PersonSource'    
  WHERE PS.[PersonID] = @PersonID    
  AND PS.[Archive] = 0    
 UNION ALL    
  SELECT    
   A.[AdminAuditID]  
  FROM [dbo].[PRF_ActionTaken] AS AT    
  INNER JOIN [dbo].[PRF_AdminAudit] AS A    
  ON AT.[ActionTakenID] = A.[ChangedRecordID]    
  AND A.[ChangedTableName] = 'ActionTaken'    
  WHERE (AT.[SubjectPersonID] = @PersonID OR AT.[ObjectPersonID] = @PersonID)    
  AND AT.[Archive] = 0     
 )    
 SELECT  
  A.[AdminAuditID] AS [LogNo],      
  ISNULL(U.[UserName],U.[UserID]) AS [Who],      
  AT.[AdminAuditTypeName] +      
  CASE WHEN RIGHT(AT.[AdminAuditTypeName], 1) = 'E' THEN 'D ' ELSE 'ED ' END +      
  A.[ChangedTableName] + ' ID = ' + CAST(A.[ChangedRecordID] AS NVARCHAR(12)) AS [What],      
  A.[ChangedDateTime] AS [When],      
  CASE       
   WHEN A.[ChangedColumns] IS NULL       
   THEN ''      
   ELSE A.[ChangedColumns].query('/ChangedRecord/*')      
  END AS [PreviousValues],
  CASE
   WHEN A.[IsProfilingChange] = 0
   THEN 'Yes'
   ELSE ''
  END AS [NonProfilingChange]       
 FROM [dbo].[PRF_AdminAudit] AS A      
 INNER JOIN ProfileAuditTrail AS P  
 ON P.[AdminAuditID] = A.[AdminAuditID]  
 INNER JOIN [dbo].[PRF_AdminUser] AS U      
 ON A.[AdminUserID] = U.[AdminUserID]
 INNER JOIN [dbo].[PRF_AdminAuditType] AS AT      
 ON A.[AdminAuditTypeID] = AT.[AdminAuditTypeID]      
 AND AT.[Archive] = 0      
 WHERE A.[Archive] = 0      
 AND    
 (     
  (A.[AdminAuditTypeID] <> 2)    
 OR    
  (A.[ChangedColumns].value('count(/ChangedRecord/*)', 'INT') > 0)    
 )    
 ORDER BY A.[ChangedDateTime] DESC      
END