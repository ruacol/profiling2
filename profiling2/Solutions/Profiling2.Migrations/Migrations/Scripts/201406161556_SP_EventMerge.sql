SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PRF_SP_EventMerge]                                        
(                                        
 @ToKeepEventID INT,                                      
 @ToDeleteEventID INT,                                      
 @UserID NVARCHAR(450),                        
 @IsProfilingChange BIT = 1                                       
)                                        
AS                                        
BEGIN                      
 DECLARE @EventName NVARCHAR(500)    
 DECLARE @NarrativeEn NVARCHAR(MAX), @NarrativeFr NVARCHAR(MAX), @Notes NVARCHAR(MAX)    
 DECLARE @DayOfStart INT, @MonthOfStart INT, @YearOfStart INT, @DayOfEnd INT, @MonthOfEnd INT, @YearOfEnd INT                       
 DECLARE @LocationID INT                      
 DECLARE @Archive BIT    
            
 IF @ToKeepEventID = @ToDeleteEventID            
  BEGIN            
  RETURN 0            
  END            
                      
 BEGIN TRY                                        
  BEGIN TRANSACTION                                      
                                    
     
     EXEC dbo.PRF_SP_OrganizationResponsibilityAudit @UserID=@UserID, @AdminAuditTypeID=2, @EventID=@ToDeleteEventID, @OrganizationResponsibilityID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                    
                                    
     UPDATE [dbo].[PRF_OrganizationResponsibility] SET [EventID] = @ToKeepEventID WHERE [EventID] = @ToDeleteEventID                       
         
         
     EXEC dbo.PRF_SP_PersonResponsibilityAudit @UserID=@UserID, @AdminAuditTypeID=2, @EventID=@ToDeleteEventID, @PersonResponsibilityID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                    
                                    
     UPDATE [dbo].[PRF_PersonResponsibility] SET [EventID] = @ToKeepEventID WHERE [EventID] = @ToDeleteEventID                            
     
     
     
     EXEC dbo.PRF_SP_EventRelationshipAudit @UserID=@UserID, @AdminAuditTypeID=2, @EventID=@ToDeleteEventID, @EventRelationshipID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                    
                                      
     UPDATE [dbo].[PRF_EventRelationship] SET [SubjectEventID] = @ToKeepEventID WHERE [SubjectEventID] = @ToDeleteEventID                       
                           
     UPDATE [dbo].[PRF_EventRelationship] SET [ObjectEventID] = @ToKeepEventID WHERE [ObjectEventID] = @ToDeleteEventID                        
     
     
     
     EXEC dbo.PRF_SP_ActionTakenAudit @UserID=@UserID, @AdminAuditTypeID=2, @EventID=@ToDeleteEventID, @ActionTakenID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                    
                                        
     UPDATE [dbo].[PRF_ActionTaken] SET [EventID] = @ToKeepEventID WHERE [EventID] = @ToDeleteEventID                      
     
     
     
     EXEC dbo.PRF_SP_EventSourceAudit @UserID=@UserID, @AdminAuditTypeID=2, @EventID=@ToDeleteEventID, @EventSourceID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                    
                                        
     UPDATE [dbo].[PRF_EventSource] SET [EventID] = @ToKeepEventID WHERE [EventID] = @ToDeleteEventID                      
         
         
         
     EXEC dbo.PRF_SP_EventTagAudit @UserID=@UserID, @AdminAuditTypeID=2, @EventID=@ToDeleteEventID, @EventTagID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                    
                                        
     UPDATE [dbo].[PRF_EventTag] SET [EventID] = @ToKeepEventID WHERE [EventID] = @ToDeleteEventID                      
         
         
    
     UPDATE [dbo].[PRF_AdminExportedEventProfile] SET [EventID] = @ToKeepEventID WHERE [EventID] = @ToDeleteEventID                                                 
                                      
                              
                              
     UPDATE [dbo].[PRF_AdminSuggestionPersonResponsibility] SET [EventID] = @ToKeepEventID WHERE [EventID] = @ToDeleteEventID         
                                  
                                  
     UPDATE [dbo].[PRF_AdminReviewedSource] SET [AttachedToProfileEventID] = @ToKeepEventID WHERE [AttachedToProfileEventID] = @ToDeleteEventID                      
                    
     /* doesn't update PRF_EventViolation_AUD */
     UPDATE [dbo].[PRF_EventViolation] SET [EventID] = @ToKeepEventID WHERE [EventID] = @ToDeleteEventID
                
                      
 SELECT TOP 1                      
  @EventName = K.[EventName],    
  @DayOfStart = CASE WHEN K.[DayOfStart] <> 0 THEN K.[DayOfStart] ELSE D.[DayOfStart] END,                      
  @MonthOfStart = CASE WHEN K.[MonthOfStart] <> 0 THEN K.[MonthOfStart] ELSE D.[MonthOfStart] END,                      
  @YearOfStart = CASE WHEN K.[YearOfStart] <> 0 THEN K.[YearOfStart] ELSE D.[YearOfStart] END,                      
  @DayOfEnd = CASE WHEN K.[DayOfEnd] <> 0 THEN K.[DayOfEnd] ELSE D.[DayOfEnd] END,                      
  @MonthOfEnd = CASE WHEN K.[MonthOfEnd] <> 0 THEN K.[MonthOfEnd] ELSE D.[MonthOfEnd] END,                      
  @YearOfEnd = CASE WHEN K.[YearOfEnd] <> 0 THEN K.[YearOfEnd] ELSE D.[YearOfEnd] END,                        
  @LocationID = CASE WHEN K.[LocationID] IS NOT NULL THEN K.[LocationID] ELSE D.[LocationID] END,                                     
  @NarrativeEn = CASE WHEN LTRIM(RTRIM(ISNULL(K.[NarrativeEn],''))) = '' THEN LTRIM(RTRIM(ISNULL(D.[NarrativeEn],''))) ELSE LTRIM(RTRIM(ISNULL(K.[NarrativeEn],''))) END,
  @NarrativeFr = CASE WHEN LTRIM(RTRIM(ISNULL(K.[NarrativeFr],''))) = '' THEN LTRIM(RTRIM(ISNULL(D.[NarrativeFr],''))) ELSE LTRIM(RTRIM(ISNULL(K.[NarrativeFr],''))) END, 
  @Notes = ISNULL(K.[Notes],'') + ' ' + ISNULL(D.[Notes],'') + ' ' +                       
   CASE WHEN K.[DayOfStart] <> 0 AND D.[DayOfStart] <> 0 AND K.[DayOfStart] <> D.[DayOfStart] THEN 'Merged event''s day of start was ' + CAST(D.[DayOfStart] AS NVARCHAR(2)) + '. ' ELSE '' END +                      
   CASE WHEN K.[MonthOfStart] <> 0 AND D.[MonthOfStart] <> 0 AND K.[MonthOfStart] <> D.[MonthOfStart] THEN 'Merged event''s month of start was ' + CAST(D.[MonthOfStart] AS NVARCHAR(2)) + '. ' ELSE '' END +                      
   CASE WHEN K.[YearOfStart] <> 0 AND D.[YearOfStart] <> 0 AND K.[YearOfStart] <> D.[YearOfStart] THEN 'Merged event''s year of start was ' + CAST(D.[YearOfStart] AS NVARCHAR(4)) + '. ' ELSE '' END +                      
   CASE WHEN K.[DayOfEnd] <> 0 AND D.[DayOfEnd] <> 0 AND K.[DayOfEnd] <> D.[DayOfEnd] THEN 'Merged event''s day of start was ' + CAST(D.[DayOfEnd] AS NVARCHAR(2)) + '. ' ELSE '' END +                      
   CASE WHEN K.[MonthOfEnd] <> 0 AND D.[MonthOfEnd] <> 0 AND K.[MonthOfEnd] <> D.[MonthOfEnd] THEN 'Merged event''s month of start was ' + CAST(D.[MonthOfEnd] AS NVARCHAR(2)) + '. ' ELSE '' END +                      
   CASE WHEN K.[YearOfEnd] <> 0 AND D.[YearOfEnd] <> 0 AND K.[YearOfEnd] <> D.[YearOfEnd] THEN 'Merged event''s year of start was ' + CAST(D.[YearOfEnd] AS NVARCHAR(4)) + '. ' ELSE '' END +                         
   CASE WHEN K.[LocationID] IS NOT NULL AND D.[LocationID] IS NOT NULL AND K.[LocationID] <> D.[LocationID] THEN 'Merged event''s location was ' + L.[LocationName] + '. ' ELSE '' END +                      
   CASE WHEN LTRIM(RTRIM(ISNULL(K.[NarrativeEn],''))) <> '' AND LTRIM(RTRIM(ISNULL(D.[NarrativeEn],''))) <> '' AND K.[NarrativeEn] <> D.[NarrativeEn] THEN 'Merged event''s english narrative was ' + D.[NarrativeEn] + '. ' ELSE '' END +                     
   CASE WHEN LTRIM(RTRIM(ISNULL(K.[NarrativeFr],''))) <> '' AND LTRIM(RTRIM(ISNULL(D.[NarrativeFr],''))) <> '' AND K.[NarrativeFr] <> D.[NarrativeFr] THEN 'Merged event''s english narrative was ' + D.[NarrativeFr] + '. ' ELSE '' END,    
  @Archive = 0    
 FROM [dbo].[PRF_Event] AS D                      
 INNER JOIN [dbo].[PRF_Event] AS K                      
 ON D.[EventID] = @ToDeleteEventID                      
 AND K.[EventID] = @ToKeepEventID                      
 LEFT JOIN [dbo].[PRF_Location] AS L    
 ON L.[LocationID] = D.[LocationID]                      
                      
 SELECT @NarrativeEn = CASE WHEN LTRIM(RTRIM(ISNULL(@NarrativeEn,''))) = '' THEN NULL ELSE LTRIM(RTRIM(@NarrativeEn)) END                      
 SELECT @NarrativeFr = CASE WHEN LTRIM(RTRIM(ISNULL(@NarrativeFr,''))) = '' THEN NULL ELSE LTRIM(RTRIM(@NarrativeFr)) END                       
 SELECT @Notes = CASE WHEN LTRIM(RTRIM(ISNULL(@Notes,''))) = '' THEN NULL ELSE LTRIM(RTRIM(@Notes)) END                      
                       
 EXEC dbo.PRF_SP_EventUpdate      
  @EventID=@ToKeepEventID,        
  @EventName=@EventName,        
  @NarrativeEn=@NarrativeEn,        
  @NarrativeFr=@NarrativeFr,        
  @DayOfStart=@DayOfStart,        
  @MonthOfStart=@MonthOfStart,        
  @YearOfStart=@YearOfStart,        
  @DayOfEnd=@DayOfEnd,        
  @MonthOfEnd=@MonthOfEnd,        
  @YearOfEnd=@YearOfEnd,        
  @LocationID=@LocationID,        
  @Notes=@Notes,        
  @UserID=@UserID,        
  @Archive=@Archive,    
  @IsProfilingChange=@IsProfilingChange      
       
                        
 EXEC dbo.PRF_SP_EventAudit @UserID=@UserID, @AdminAuditTypeID=3, @EventID=@ToDeleteEventID, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                    
                           
     DELETE TOP (1) FROM [dbo].[PRF_Event] WHERE [EventID]=@ToDeleteEventID                      
  COMMIT TRANSACTION                                      
 END TRY                                        
 BEGIN CATCH                                        
                                      
  IF @@TRANCOUNT > 0                                      
   ROLLBACK TRANSACTION                                     
      
  RETURN 0                                        
                                      
 END CATCH                                        
 RETURN 1                                        
END 