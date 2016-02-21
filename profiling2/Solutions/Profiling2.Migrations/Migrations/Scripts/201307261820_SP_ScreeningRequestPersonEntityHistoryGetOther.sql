SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SCR_SP_ScreeningRequestPersonEntityHistoryGetOther]        
(        
 @UserID NVARCHAR(450),        
 @ScreeningRequestPersonEntityID INT        
)        
AS        
BEGIN        
        
 DECLARE @AdminUserID INT        
 DECLARE @ScreeningEntityID INT        
 DECLARE @PersonID INT
         
 EXEC @AdminUserID = dbo.PRF_SP_AdminUserGetAdminUserID @UserID=@UserID        
         
 EXEC @ScreeningEntityID = dbo.SCR_SP_ScreeningEntityAdminUserGetByUser @AdminUserID=@AdminUserID, @RaiseError=1  

 SELECT TOP 1 @PersonID = P.[PersonID]
 FROM [dbo].[PRF_Person] AS P 
 INNER JOIN [dbo].[SCR_RequestPerson] AS RP
 ON P.[PersonID] = RP.[PersonID]
 AND RP.[Archive] = 0
		INNER JOIN [dbo].[SCR_ScreeningRequestPersonEntity] AS S
		ON S.[RequestPersonID] = RP.[RequestPersonID]
		AND S.[ScreeningRequestPersonEntityID] = @ScreeningRequestPersonEntityID
		AND S.[Archive] = 0
 WHERE P.[Archive] = 0
         
	SELECT   
		
		CASE 
			--if a response has been provided or the request marked as complete, then not in progress
			WHEN ER.[ScreeningRequestEntityResponseID] IS NOT NULL OR RH.[RequestHistoryID] IS NOT NULL
			THEN ''
			ELSE 'In progress'
		END AS [In Progress],
		ISNULL(ER.[ResponseDateTime],H.[Date]) AS [Date],        
		SE.[ScreeningEntityName] AS [Screened By],        
		'(' + R.[ReferenceNumber] + ') ' + R.[RequestName] AS [Request],        
		SR.[ScreeningResultName] AS [Color],        
		ISNULL(S.[Reason],'') AS [Reason],      
		ISNULL(S.[Commentary],'') AS [Comments]      
	FROM [dbo].[SCR_ScreeningRequestPersonEntity] AS S
	INNER JOIN [dbo].[SCR_RequestPerson] AS RP
	ON RP.[RequestPersonID] = S.[RequestPersonID]  
	AND RP.[PersonID] = @PersonID
	AND RP.[Archive] = 0  
		INNER JOIN [dbo].[SCR_Request] AS R        
		ON R.[RequestID] = RP.[RequestID]        
		AND R.[Archive] = 0        
			LEFT JOIN [dbo].[SCR_ScreeningRequestEntityResponse] AS ER
			ON ER.[RequestID] = R.[RequestID]        
			AND ER.[ScreeningEntityID] = S.[ScreeningEntityID]
			AND ER.[Archive] = 0            		
			LEFT JOIN [dbo].[SCR_RequestHistory] AS RH
			ON RH.[RequestID] = R.[RequestID]
			AND RH.[RequestStatusID] = 8 --Completed
			AND RH.[Archive] = 0
    INNER JOIN [dbo].[SCR_ScreeningResult] AS SR        
    ON SR.[ScreeningResultID] = S.[ScreeningResultID]        
    AND SR.[Archive] = 0       
    INNER JOIN [dbo].[SCR_ScreeningEntity] AS SE
    ON SE.[ScreeningEntityID] = S.[ScreeningEntityID]
    AND SE.[Archive] = 0
    CROSS APPLY        
    (        
     SELECT TOP 1        
      ISNULL(U.[UserName],U.[UserID]) AS [Screened By],
      H.[DateStatusReached] AS [Date]
     FROM [dbo].[SCR_ScreeningRequestPersonEntityHistory] AS H        
     INNER JOIN [dbo].[PRF_AdminUser] AS U        
     ON U.[AdminUserID] = H.[AdminUserID]      
     WHERE H.[ScreeningRequestPersonEntityID] = S.[ScreeningRequestPersonEntityID]        
     AND H.[Archive] = 0        
     ORDER BY H.[DateStatusReached] DESC        
    ) H        
 WHERE S.[ScreeningEntityID] <> @ScreeningEntityID
 AND S.[Archive] = 0        
 ORDER BY ISNULL(ER.[ResponseDateTime],H.[Date]) DESC        
END 