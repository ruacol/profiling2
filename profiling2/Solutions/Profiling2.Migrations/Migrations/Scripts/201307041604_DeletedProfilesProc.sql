SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PRF_SP_Reports_DeletedProfiles_NHibernate]      
AS      
BEGIN      
 SELECT      
  A.[AdminAuditID] AS [LogNo],      
  ISNULL(U.[UserName],U.[UserID]) AS [Who],    
	A.[ChangedDateTime] AS [When],        
  CAST(A.[ChangedRecordID] AS NVARCHAR(12)) AS [PersonID],
  CASE       
   WHEN A.[ChangedColumns] IS NULL       
   THEN ''      
   ELSE 
	CASE
		WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = A.[ChangedColumns].value('(/ChangedRecord/FirstName)[1]', 'NVARCHAR(MAX)') AND UN.[Archive] = 0)
		THEN ''
		ELSE A.[ChangedColumns].value('(/ChangedRecord/FirstName)[1]', 'NVARCHAR(MAX)') + ' '
	END
	+
	CASE
		WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = A.[ChangedColumns].value('(/ChangedRecord/LastName)[1]', 'NVARCHAR(MAX)') AND UN.[Archive] = 0)
		THEN ''
		ELSE A.[ChangedColumns].value('(/ChangedRecord/LastName)[1]', 'NVARCHAR(MAX)')
	END	
   END AS [Person],  
  CASE  
   WHEN A.[IsProfilingChange] = 0  
   THEN 'Yes'  
   ELSE ''  
  END AS [NonProfilingChange]  
 FROM [dbo].[PRF_AdminAudit] AS A      
 INNER JOIN [dbo].[PRF_AdminUser] AS U      
 ON A.[AdminUserID] = U.[AdminUserID]      
 WHERE A.[AdminAuditTypeID] = 3
 AND A.[ChangedTableName] = 'Person'
 AND A.[Archive] = 0
 ORDER BY A.[ChangedDateTime] DESC      
END