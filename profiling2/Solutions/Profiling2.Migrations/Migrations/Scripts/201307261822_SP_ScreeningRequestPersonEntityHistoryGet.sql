SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SCR_SP_ScreeningRequestPersonEntityHistoryGet]      
(      
 @UserID NVARCHAR(450),      
 @ScreeningRequestPersonEntityID INT      
)      
AS      
BEGIN      
      
 DECLARE @AdminUserID INT      
 DECLARE @ScreeningEntityID INT      
       
 EXEC @AdminUserID = dbo.PRF_SP_AdminUserGetAdminUserID @UserID=@UserID      
       
 EXEC @ScreeningEntityID = dbo.SCR_SP_ScreeningEntityAdminUserGetByUser @AdminUserID=@AdminUserID, @RaiseError=1
       
 SELECT      
  SRER.[ResponseDateTime] AS [Date],      
  H.[Screened By] AS [Screened By],      
  '(' + R.[ReferenceNumber] + ') ' + R.[RequestName] AS [Request],      
  SR.[ScreeningResultName] AS [Color],      
  ISNULL(SRP.[Reason],'') AS [Reason],    
  ISNULL(SRP.[Commentary],'') AS [Comments]    
 FROM [dbo].[SCR_ScreeningRequestPersonEntity] AS Z
 INNER JOIN [dbo].[SCR_RequestPerson] AS A      
 ON Z.[RequestPersonID] = A.[RequestPersonID]
 AND A.[Archive] = 0
  INNER JOIN [dbo].[SCR_RequestPerson] AS RP      
  ON RP.[PersonID] = A.[PersonID]      
  AND RP.[Archive] = 0      
   INNER JOIN [dbo].[SCR_ScreeningRequestPersonEntity] AS SRP      
   ON SRP.[RequestPersonID] = RP.[RequestPersonID]      
   AND SRP.[ScreeningEntityID] = @ScreeningEntityID  
   AND SRP.[Archive] = 0      
    INNER JOIN [dbo].[SCR_ScreeningResult] AS SR      
    ON SR.[ScreeningResultID] = SRP.[ScreeningResultID]      
    AND SR.[Archive] = 0      
    CROSS APPLY      
    (      
     SELECT TOP 1      
      ISNULL(U.[UserName],U.[UserID]) AS [Screened By]      
     FROM [dbo].[SCR_ScreeningRequestPersonEntityHistory] AS H      
     INNER JOIN [dbo].[PRF_AdminUser] AS U      
     ON U.[AdminUserID] = H.[AdminUserID]   
     WHERE H.[ScreeningRequestPersonEntityID] = SRP.[ScreeningRequestPersonEntityID]      
     AND H.[Archive] = 0      
     ORDER BY H.[DateStatusReached] DESC      
    ) H      
   INNER JOIN [dbo].[SCR_Request] AS R      
   ON R.[RequestID] = RP.[RequestID]      
   AND R.[Archive] = 0      
    INNER JOIN [dbo].[SCR_ScreeningRequestEntityResponse] AS SRER      
    ON SRER.[RequestID] = R.[RequestID]      
    AND SRER.[ScreeningEntityID] = @ScreeningEntityID      
    AND SRER.[Archive] = 0      
 WHERE Z.[ScreeningRequestPersonEntityID] = @ScreeningRequestPersonEntityID      
 AND Z.[Archive] = 0      
 ORDER BY SRER.[ResponseDateTime] DESC      
END 