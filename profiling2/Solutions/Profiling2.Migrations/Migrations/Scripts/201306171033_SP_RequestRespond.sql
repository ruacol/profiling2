SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SCR_SP_RequestRespond]                  
(                  
 @UserID NVARCHAR(450),                    
 @RequestID INT,                  
 @Notes NVARCHAR(MAX) = NULL        
)                  
AS                  
BEGIN                  
 DECLARE @ScreeningEntityID INT        
 DECLARE @Today DATETIME    
 DECLARE @RequestHistoryID INT    
 DECLARE @AdminUserID INT    
 DECLARE @ScreeningRequestEntityResponse INT    
    
 --Get the internal UserID, external UserID doesn't exist it will be added                  
 EXEC @AdminUserID = dbo.PRF_SP_AdminUserGetAdminUserID @UserID=@UserID                
    
 --Retrieve which screening entity this user is part of        
 EXEC @ScreeningEntityID = dbo.SCR_SP_ScreeningEntityAdminUserGetByUser @AdminUserID=@AdminUserID,@UserID=NULL,@RaiseError=0    
 --user wasn't part of screening entity      
 IF @ScreeningEntityID = -1      
  BEGIN      
  RAISERROR('User was not part of any screening entity', 16, 1)    
  RETURN -1    
  END      
    
 IF EXISTS    
 (    
  SELECT 'X' FROM    
  (    
   SELECT     
    RP.[RequestPersonID],    
    SRP.[ScreeningRequestPersonEntityID]    ,
    SRP.[ScreeningResultID]      
   FROM [dbo].[SCR_RequestPerson] AS RP    
   LEFT JOIN [dbo].[SCR_ScreeningRequestPersonEntity] AS SRP    
   ON RP.[RequestPersonID] = SRP.[RequestPersonID]    
   AND SRP.[ScreeningEntityID] = @ScreeningEntityID    
   AND SRP.[Archive] = 0    
   WHERE RP.[RequestID] = @RequestID    
   AND RP.[Archive] = 0    
  ) O LEFT JOIN [dbo].[SCR_ScreeningResult] SR ON SR.ScreeningResultID = O.ScreeningResultID
  WHERE O.[ScreeningRequestPersonEntityID] IS NULL
  OR SR.ScreeningResultName = 'Pending'
  OR SR.ScreeningResultID IS NULL
 )    
  BEGIN    
  RAISERROR('Not all persons have been responded to in this screening request', 16, 1)    
  RETURN -1    
  END    
           
 IF EXISTS    
  (    
   SELECT 'X' FROM [dbo].[SCR_ScreeningRequestEntityResponse]    
   WHERE [RequestID] = @RequestID    
   AND [ScreeningEntityID] = @ScreeningEntityID    
   AND [Archive] = 0    
  )    
  BEGIN    
  RAISERROR('The screening entity has already submitted a response to this request', 16, 1)    
  RETURN -1    
  END    
                      
 SET @Today = GETDATE()                  
    
 --formalize screening response                  
 EXEC @ScreeningRequestEntityResponse = [dbo].[SCR_SP_ScreeningRequestEntityResponseAdd]    
  @RequestID=@RequestID,    
  @ScreeningEntityID=@ScreeningEntityID,    
  @Notes=@Notes    
    
 --all screening entities have responded    
 IF     
 (    
  (    
   SELECT COUNT(DISTINCT SE.[ScreeningEntityID])    
   FROM [dbo].[SCR_ScreeningEntity] AS SE    
   INNER JOIN [dbo].[SCR_ScreeningRequestEntityResponse] AS SRER    
   ON SRER.[RequestID] = @RequestID    
   AND SRER.[ScreeningEntityID] = SE.[ScreeningEntityID]    
   AND SRER.[Archive] = 0    
   WHERE SE.[Archive] = 0    
  )    
   >=     
  (    
   SELECT COUNT(*) FROM [dbo].[SCR_ScreeningEntity]    
   WHERE [Archive] = 0    
  )    
 )    
  BEGIN     
  --update status because all parties have responded    
  EXEC @RequestHistoryID = [dbo].[SCR_SP_RequestHistoryAdd]                 
   @AdminUserID=@AdminUserID,               
   @RequestStatusID=4, --screening by all entites    
   @RequestID=@RequestID,                  
   @Notes=@Notes,                  
   @DateStatusReached=@Today,                  
   @Archive=0        
  END    
 ELSE    
  BEGIN    
  --update status because some parties have responded    
  EXEC @RequestHistoryID = [dbo].[SCR_SP_RequestHistoryAdd]                 
   @AdminUserID=@AdminUserID,               
   @RequestStatusID=11, --screening in progress    
   @RequestID=@RequestID,                  
   @Notes=@Notes,                  
   @DateStatusReached=@Today,                  
   @Archive=0        
  END    
                  
 RETURN @RequestID                  
                  
END 