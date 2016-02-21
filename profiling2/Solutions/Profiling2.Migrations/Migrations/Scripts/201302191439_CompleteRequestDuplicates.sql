SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SCR_SP_ScreeningRequestPersonFinalDecisionGetCompletedRequest]    
(    
 @UserID NVARCHAR(450),    
 @RequestID INT    
)    
AS    
BEGIN    
    
 DECLARE @AdminUserID INT                                          
                                      
 --Get the internal UserID, external UserID doesn't exist it will be added                                          
 EXEC @AdminUserID = dbo.PRF_SP_AdminUserGetAdminUserID @UserID=@UserID                                          
    
 SELECT    
  ISNULL(P.[FirstName],'') AS [FirstName],    
  ISNULL(P.[LastName],'') AS [LastName],    
  ISNULL(P.[MilitaryIDNumber],'') AS [MilitaryIDNumber],    
  --only show support status if request has been completed
  CASE WHEN H.[RequestHistoryID] IS NULL THEN '' ELSE ISNULL(S.[ScreeningSupportStatusName],'') END AS [ScreeningSupportStatusName]    
 FROM [dbo].[SCR_RequestPerson] AS RP    
 INNER JOIN [dbo].[PRF_Person] AS P    
 ON RP.[PersonID] = P.[PersonID]    
 AND P.[Archive] = 0    
 LEFT JOIN [dbo].[SCR_ScreeningRequestPersonFinalDecision] AS F    
 ON F.[RequestPersonID] = RP.[RequestPersonID]    
 AND F.[Archive] = 0    
 LEFT JOIN [dbo].[SCR_ScreeningSupportStatus] AS S    
 ON S.[ScreeningSupportStatusID] = F.[ScreeningSupportStatusID] 
 AND S.[Archive] = 0    
  CROSS APPLY      
    (      
      SELECT TOP 1 *      
      FROM [dbo].[SCR_RequestHistory] AS X  
      WHERE X.[RequestID] = RP.[RequestID]  
      AND X.[RequestStatusID] = 8 --Completed  
      AND X.[Archive] = 0           
    ) H 
 WHERE RP.[Archive] = 0    
 AND RP.[RequestID] = @RequestID   
END