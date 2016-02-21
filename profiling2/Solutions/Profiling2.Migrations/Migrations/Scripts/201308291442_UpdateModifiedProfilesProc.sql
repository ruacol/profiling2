SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PRF_SP_Reports_ProfilesModifiedLastWeek_NHibernate] 
(
  @StartDate DATETIME,
  @EndDate DATETIME
)         
AS        
BEGIN            
 ; WITH AuditTrail          
 AS          
 (          
  SELECT      
   A.[AdminUserID],          
   A.[ChangedDateTime],      
   A.[ChangedRecordID] AS [PersonID]      
  FROM [dbo].[PRF_AdminAudit] AS A          
  WHERE A.[ChangedTableName] = 'Person'          
  AND A.[AdminAuditTypeID] = 2 --UPDATE      
  AND A.[ChangedDateTime] BETWEEN @StartDate AND @EndDate          
  AND A.[Archive] = 0      
 UNION ALL          
  SELECT          
   A.[AdminUserID],          
   A.[ChangedDateTime],      
   C.[PersonID] AS [PersonID]      
  FROM [dbo].[PRF_Career] AS C          
  INNER JOIN [dbo].[PRF_AdminAudit] AS A          
  ON C.[CareerID] = A.[ChangedRecordID]          
  AND A.[ChangedTableName] = 'Career'      
  AND A.[ChangedDateTime] BETWEEN @StartDate AND @EndDate      
  WHERE C.[Archive] = 0          
 UNION ALL          
  SELECT          
   A.[AdminUserID],          
   A.[ChangedDateTime],      
   PA.[PersonID] AS [PersonID]      
  FROM [dbo].[PRF_PersonAlias] AS PA      
  INNER JOIN [dbo].[PRF_AdminAudit] AS A          
  ON PA.[PersonAliasID] = A.[ChangedRecordID]          
  AND A.[ChangedTableName] = 'PersonAlias'          
  AND A.[ChangedDateTime] BETWEEN @StartDate AND @EndDate      
  WHERE PA.[Archive] = 0          
 UNION ALL          
  SELECT          
   A.[AdminUserID],          
   A.[ChangedDateTime],      
   PP.[PersonID] AS [PersonID]      
  FROM [dbo].[PRF_PersonPhoto] AS PP          
  INNER JOIN [dbo].[PRF_AdminAudit] AS A          
  ON PP.[PersonPhotoID] = A.[ChangedRecordID]          
  AND A.[ChangedTableName] = 'PersonPhoto'          
  AND A.[ChangedDateTime] BETWEEN @StartDate AND @EndDate      
  WHERE PP.[Archive] = 0          
 UNION ALL          
  SELECT          
   A.[AdminUserID],          
   A.[ChangedDateTime],      
   PPR.[SubjectPersonID] AS [PersonID]      
  FROM [dbo].[PRF_PersonRelationship] AS PPR          
  INNER JOIN [dbo].[PRF_AdminAudit] AS A          
  ON PPR.[PersonRelationshipID] = A.[ChangedRecordID]          
  AND A.[ChangedTableName] = 'PersonRelationship'          
  AND A.[ChangedDateTime] BETWEEN @StartDate AND @EndDate      
  WHERE PPR.[Archive] = 0          
 UNION ALL          
  SELECT          
   A.[AdminUserID],          
   A.[ChangedDateTime],      
   PPR.[ObjectPersonID] AS [PersonID]      
  FROM [dbo].[PRF_PersonRelationship] AS PPR          
  INNER JOIN [dbo].[PRF_AdminAudit] AS A          
  ON PPR.[PersonRelationshipID] = A.[ChangedRecordID]          
  AND A.[ChangedTableName] = 'PersonRelationship'          
  AND A.[ChangedDateTime] BETWEEN @StartDate AND @EndDate      
  WHERE PPR.[Archive] = 0          
 UNION ALL          
  SELECT          
   A.[AdminUserID],          
   A.[ChangedDateTime],      
   PR.[PersonID] AS [PersonID]      
  FROM [dbo].[PRF_PersonResponsibility] AS PR          
  INNER JOIN [dbo].[PRF_AdminAudit] AS A          
  ON PR.[PersonResponsibilityID] = A.[ChangedRecordID]          
  AND A.[ChangedTableName] = 'PersonResponsibility'          
  AND A.[ChangedDateTime] BETWEEN @StartDate AND @EndDate      
  WHERE PR.[Archive] = 0          
 UNION ALL          
  SELECT          
   A.[AdminUserID],          
   A.[ChangedDateTime],      
   PS.[PersonID] AS [PersonID]      
  FROM [dbo].[PRF_PersonSource] AS PS      
  INNER JOIN [dbo].[PRF_AdminAudit] AS A          
  ON PS.[PersonSourceID] = A.[ChangedRecordID]          
  AND A.[ChangedTableName] = 'PersonSource'          
  AND A.[ChangedDateTime] BETWEEN @StartDate AND @EndDate      
  WHERE PS.[Archive] = 0          
 UNION ALL          
  SELECT          
   A.[AdminUserID],          
   A.[ChangedDateTime],      
   AT.[SubjectPersonID] AS [PersonID]      
  FROM [dbo].[PRF_ActionTaken] AS AT          
  INNER JOIN [dbo].[PRF_AdminAudit] AS A          
  ON AT.[ActionTakenID] = A.[ChangedRecordID]          
  AND A.[ChangedTableName] = 'ActionTaken'     
  AND A.[ChangedDateTime] BETWEEN @StartDate AND @EndDate      
  WHERE AT.[Archive] = 0           
 UNION ALL          
  SELECT          
   A.[AdminUserID],          
   A.[ChangedDateTime],      
   AT.[ObjectPersonID] AS [PersonID]      
  FROM [dbo].[PRF_ActionTaken] AS AT          
  INNER JOIN [dbo].[PRF_AdminAudit] AS A          
  ON AT.[ActionTakenID] = A.[ChangedRecordID]          
  AND A.[ChangedTableName] = 'ActionTaken'          
  AND A.[ChangedDateTime] BETWEEN @StartDate AND @EndDate      
  WHERE AT.[Archive] = 0           
 )       
 SELECT      
  A.[When] AS [When],        
  ISNULL(U.[UserName],U.[UserID]) AS [Who],          
  P.[PersonID] AS [PersonID],          
  CASE WHEN P.[FirstName] IS NULL THEN '' ELSE P.[FirstName] + ' ' END + ISNULL(P.[LastName],'')AS [Person]  
 FROM [dbo].[PRF_Person] AS P   
 INNER JOIN       
  (      
  SELECT      
  A.[AdminUserID],      
     MAX(A.[ChangedDateTime]) AS [When],      
  A.[PersonID]       
  FROM AuditTrail AS A      
  GROUP BY A.[AdminUserID],A.[PersonID]      
  ) A      
 ON A.[PersonID] = P.[PersonID]      
 INNER JOIN [dbo].[PRF_AdminUser] AS U      
 ON A.[AdminUserID] = U.[AdminUserID] 
 WHERE P.[Archive] = 0      
 ORDER BY A.[When] DESC, U.[AdminUserID] ASC, [Person] ASC    
END