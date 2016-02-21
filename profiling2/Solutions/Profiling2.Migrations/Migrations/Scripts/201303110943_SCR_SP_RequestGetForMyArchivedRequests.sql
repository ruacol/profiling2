SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SCR_SP_RequestGetForMyArchivedRequests]                                
(                          
 @UserID NVARCHAR(450),                                
 @RequestID INT = NULL                      
)                                
AS                                
BEGIN                                
                          
 DECLARE @AdminUserID INT                            
                        
 --Get the internal UserID, external UserID doesn't exist it will be added                            
 EXEC @AdminUserID = dbo.PRF_SP_AdminUserGetAdminUserID @UserID=@UserID                            
                          
 SELECT                                
  R.[RequestID] AS [RequestID],                      
  R.[ReferenceNumber] AS [Reference Number],                                
  R.[RequestName] AS [Name],                                
  ISNULL(R.[Notes],'') AS [Description],                                
  RE.[RequestEntityName] AS [Requesting Entity],                                
  RT.[RequestTypeName] AS [Type],  
  ISNULL(R.[RespondBy],'') AS [Respond By],
  S.[Status] AS [Status],                                
  S.[Date] AS [Status Reached On],                      
  --Can only be deleted if in created status                       
  CASE                 
   WHEN S.[StatusID] = 1 --Created                
   OR S.[StatusID] = 10 --Edited                
   THEN 1 ELSE 0                 
  END AS [IsDeletable],           
  CASE WHEN S.[StatusID] = 8 THEN 1 ELSE 0 END AS [IsCompleted],                 
  CASE WHEN S.[StatusID] = 13 THEN 1 ELSE 0 END AS [IsRejected],    
  S.[Who] AS [Who]                     
 FROM [dbo].[SCR_Request] AS R                                
 INNER JOIN [dbo].[SCR_RequestEntity] AS RE                                
 ON R.[RequestEntityID] = RE.[RequestEntityID]                                
 AND (RE.[Archive] = 0)                                
 INNER JOIN [dbo].[SCR_RequestType] AS RT                                
 ON R.[RequestTypeID] = RT.[RequestTypeID]                                
 AND (RT.[Archive] = 0)                                
 CROSS APPLY                        
  (                        
  SELECT TOP 1                           
   RS.[RequestStatusID] AS [StatusID],                           
   RS.[RequestStatusName] AS [Status],                                
   RH.[DateStatusReached] AS [Date],                  
   ISNULL(U.[UserName],U.[UserID]) AS [Who]                                
  FROM [dbo].[SCR_RequestHistory] AS RH                                
  INNER JOIN [dbo].[SCR_RequestStatus] AS RS                                
  ON RH.[RequestStatusID] = RS.[RequestStatusID]                                
  AND (RS.[Archive] = 0)                   
  AND (RS.[RequestStatusID] <> 10) --ignore edited status                    
  INNER JOIN [dbo].[PRF_AdminUser] AS U                  
  ON U.[AdminUserID] = RH.[AdminUserID]                  
  AND U.[Archive] = 0                         
  WHERE (RH.[RequestID] = R.[RequestID])                              
  AND (RH.[Archive] = 0)                                
  ORDER BY  
   CASE WHEN (RH.[RequestStatusID] = 8 /* completed */ OR RH.[RequestStatusID] = 13 /* rejected */) THEN 1 ELSE 0 END DESC,  
   RH.[DateStatusReached] DESC                                   
  ) AS S                                
 WHERE (R.[Archive] = 0)                                
 AND (@RequestID IS NULL OR R.[RequestID] = @RequestID)                        
 AND                    
 (                    
  --check that user has privileges to view this request                        
  EXISTS                        
   (                        
    SELECT 'X'                        
    FROM [dbo].[SCR_RequestHistory] AS RH                        
    WHERE [AdminUserID] = @AdminUserID                        
    AND RH.[RequestID] = R.[RequestID]                        
    AND RH.[RequestStatusID] = 1 --created                        
    AND (RH.[Archive] = 0)                        
   )
  OR EXISTS
   -- user is of same request entity as creator
   (                      
    SELECT 'X'                        
    FROM [dbo].[SCR_RequestHistory] AS RH
    INNER JOIN [dbo].[SCR_RequestEntityAdminUser] AS CREATOR 
	ON RH.[AdminUserID] = CREATOR.[AdminUserID]
    INNER JOIN [dbo].[SCR_RequestEntityAdminUser] AS VIEWER 
	ON CREATOR.RequestEntityID = VIEWER.[RequestEntityID] AND VIEWER.[AdminUserID] = @AdminUserID
    WHERE RH.[RequestID] = R.[RequestID]                        
    AND RH.[RequestStatusID] = 1 --created                        
    AND RH.[Archive] = 0
   ) 
  OR EXISTS                      
   (                      
    SELECT 'X' FROM [dbo].[SCR_RequestEntityAdminUser] AS RU                      
    WHERE RE.[RequestEntityID] = RU.[RequestEntityID]                      
    AND RU.[AdminUserID] = @AdminUserID                      
    AND RU.[Archive] = 0                      
   )                          
  OR EXISTS            
   (            
    SELECT 'X' FROM [dbo].[SCR_ScreeningEntityAdminUser] AS SEAU            
    WHERE [AdminUserID] = @AdminUserID            
    AND [Archive] = 0            
   )            
 )      
 AND S.[Date] < (GETDATE()-31)                      
 ORDER BY [Status Reached On] DESC        
                       
END