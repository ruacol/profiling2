SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PRF_SP_PersonMerge_NHibernate]                                    
(                                    
 @ToKeepPersonID INT,                                  
 @ToDeletePersonID INT,                                  
 @UserID NVARCHAR(450),                    
 @IsProfilingChange BIT = 1                                   
)                                    
AS                                    
BEGIN                  
 DECLARE @PersonAliasLastName NVARCHAR(500), @PersonAliasFirstName NVARCHAR(500)                  
 DECLARE @LastName NVARCHAR(500), @FirstName NVARCHAR(500)                  
 DECLARE @DayOfBirth INT, @MonthOfBirth INT, @YearOfBirth INT                  
 DECLARE @BirthVillage NVARCHAR(500)                  
 DECLARE @BirthRegionID INT                  
 DECLARE @ApproximateBirthDate NVARCHAR(255)                  
 DECLARE @EthnicityID INT                  
 DECLARE @Height NVARCHAR(255), @Weight NVARCHAR(255)                  
 DECLARE @BackgroundInformation NVARCHAR(MAX)                  
 DECLARE @MilitaryIDNumber NVARCHAR(255)                  
 DECLARE @Notes NVARCHAR(MAX)                  
 DECLARE @ProfileStatusID INT                  
 DECLARE @Archive BIT, @SetToRestrictedProfile BIT      
        
 IF @ToKeepPersonID = @ToDeletePersonID        
  BEGIN        
  RETURN 0        
  END        
                  
 BEGIN TRY                                    
  BEGIN TRANSACTION                                  
                                
     EXEC dbo.PRF_SP_CareerAudit @UserID=@UserID, @AdminAuditTypeID=2, @PersonID=@ToDeletePersonID, @CareerID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                
                                    
     UPDATE [dbo].[PRF_Career] SET [PersonID] = @ToKeepPersonID WHERE [PersonID] = @ToDeletePersonID                  
                      
                                  
     EXEC dbo.PRF_SP_PersonAliasAudit @UserID=@UserID, @AdminAuditTypeID=2, @PersonID=@ToDeletePersonID, @PersonAliasID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                
                                
     UPDATE [dbo].[PRF_PersonAlias] SET [PersonID] = @ToKeepPersonID WHERE [PersonID] = @ToDeletePersonID                  
                                
                                  
     EXEC dbo.PRF_SP_PersonPhotoAudit @UserID=@UserID, @AdminAuditTypeID=2, @PersonID=@ToDeletePersonID, @PersonPhotoID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                
                                
     UPDATE [dbo].[PRF_PersonPhoto] SET [PersonID] = @ToKeepPersonID WHERE [PersonID] = @ToDeletePersonID                                  
                                
                                
     EXEC dbo.PRF_SP_PersonRelationshipAudit @UserID=@UserID, @AdminAuditTypeID=2, @PersonID=@ToDeletePersonID, @PersonRelationshipID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                
                                  
     UPDATE [dbo].[PRF_PersonRelationship] SET [SubjectPersonID] = @ToKeepPersonID WHERE [SubjectPersonID] = @ToDeletePersonID                   
                       
     UPDATE [dbo].[PRF_PersonRelationship] SET [ObjectPersonID] = @ToKeepPersonID WHERE [ObjectPersonID] = @ToDeletePersonID                   
                  
                                  
     EXEC dbo.PRF_SP_PersonResponsibilityAudit @UserID=@UserID, @AdminAuditTypeID=2, @PersonID=@ToDeletePersonID, @PersonResponsibilityID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                
                                
     UPDATE [dbo].[PRF_PersonResponsibility] SET [PersonID] = @ToKeepPersonID WHERE [PersonID] = @ToDeletePersonID                  
                                
                                  
     EXEC dbo.PRF_SP_PersonSourceAudit @UserID=@UserID, @AdminAuditTypeID=2, @PersonID=@ToDeletePersonID, @PersonSourceID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                
                                
     UPDATE [dbo].[PRF_PersonSource] SET [PersonID] = @ToKeepPersonID WHERE [PersonID] = @ToDeletePersonID                  
                                
                   
     EXEC dbo.PRF_SP_ActionTakenAudit @UserID=@UserID, @AdminAuditTypeID=2, @PersonID=@ToDeletePersonID, @ActionTakenID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                
                                  
     UPDATE [dbo].[PRF_ActionTaken] SET [SubjectPersonID] = @ToKeepPersonID WHERE [SubjectPersonID] = @ToDeletePersonID                                  
                       
     UPDATE [dbo].[PRF_ActionTaken] SET [ObjectPersonID] = @ToKeepPersonID WHERE [ObjectPersonID] = @ToDeletePersonID                                  
                          
                          
     UPDATE [dbo].[PRF_AdminSuggestionPersonResponsibility] SET [PersonID] = @ToKeepPersonID WHERE [PersonID] = @ToDeletePersonID                      
                            
                            
     UPDATE [dbo].[PRF_AdminExportedPersonProfile] SET [PersonID] = @ToKeepPersonID WHERE [PersonID] = @ToDeletePersonID                            
                        
                              
	 UPDATE AS1
     SET AS1.[PersonID] = @ToKeepPersonID                  
     FROM [dbo].[PRF_ActiveScreening] AS AS1
     WHERE AS1.[PersonID] = @ToDeletePersonID
     UPDATE A                  
     SET A.[PersonID] = @ToKeepPersonID                  
     FROM [dbo].[PRF_AdminPersonImport] AS A                   
     WHERE A.[PersonID] = @ToDeletePersonID                  
     AND NOT EXISTS                  
  (                  
   SELECT 'X' FROM [dbo].[PRF_AdminPersonImport] AS I                   
   WHERE I.[PersonID] = @ToKeepPersonID                  
   AND I.[AdminPersonImportTypeID] = A.[AdminPersonImportTypeID]                  
   AND I.[PreviousID] = A.[PreviousID]                  
  )                  
                    
     DELETE FROM [dbo].[PRF_AdminPersonImport] WHERE [PersonID] = @ToDeletePersonID                  
                              
     UPDATE [dbo].[PRF_AdminReviewedSource] SET [AttachedToProfilePersonID] = @ToKeepPersonID WHERE [AttachedToProfilePersonID] = @ToDeletePersonID                  
                
                
     UPDATE RP                  
     SET RP.[Archive] = 0                
     FROM [dbo].[SCR_RequestPerson] AS RP                  
     WHERE RP.[PersonID] = @ToKeepPersonID                  
     AND RP.[Archive] = 1                
     AND EXISTS                  
     (                  
       SELECT 'X' FROM [dbo].[SCR_RequestPerson] AS A                  
       WHERE A.[PersonID] = @ToDeletePersonID                  
       AND A.[RequestID] = RP.[RequestID]                  
     )                    
                 
     UPDATE RP                  
     SET RP.[PersonID] = @ToKeepPersonID                  
     FROM [dbo].[SCR_RequestPerson] AS RP                  
     WHERE RP.[PersonID] = @ToDeletePersonID                  
     AND NOT EXISTS                  
     (                  
      SELECT 'X' FROM [dbo].[SCR_RequestPerson] AS A                  
      WHERE A.[PersonID] = @ToKeepPersonID                  
      AND A.[RequestID] = RP.[RequestID]                  
     )                  
                
 DELETE FROM [dbo].[SCR_AdminRequestPersonImport]                
 WHERE EXISTS                
  (                
   SELECT 'X' FROM [dbo].[SCR_RequestPerson] AS RP                
   WHERE RP.[RequestPersonID] = [dbo].[SCR_AdminRequestPersonImport].[RequestPersonID]                
   AND RP.[PersonID] = @ToDeletePersonID                
  )                
                  
 DELETE FROM [dbo].[SCR_RequestPersonHistory]                
 WHERE EXISTS                
  (                
   SELECT 'X' FROM [dbo].[SCR_RequestPerson] AS RP                
   WHERE RP.[RequestPersonID] = [dbo].[SCR_RequestPersonHistory].[RequestPersonID]                
   AND RP.[PersonID] = @ToDeletePersonID                
  )                   
                
 UPDATE E                
 SET E.[RequestPersonID] = B.[RequestPersonID]                
 FROM [dbo].[SCR_ScreeningRequestPersonEntity] AS E                
 INNER JOIN [dbo].[SCR_RequestPerson] AS A                
 ON E.[RequestPersonID] = A.[RequestPersonID]                
 AND A.[PersonID] = @ToDeletePersonID            
 INNER JOIN [dbo].[SCR_RequestPerson] AS B                
 ON A.[RequestID] = B.[RequestID]                
 AND B.[PersonID] = @ToKeepPersonID                
                 
 UPDATE R                
 SET R.[RequestPersonID] = B.[RequestPersonID]                
 FROM [dbo].[SCR_ScreeningRequestPersonRecommendation] AS R                
 INNER JOIN [dbo].[SCR_RequestPerson] AS A                
 ON R.[RequestPersonID] = A.[RequestPersonID]                
 AND A.[PersonID] = @ToDeletePersonID                
 INNER JOIN [dbo].[SCR_RequestPerson] AS B                
 ON A.[RequestID] = B.[RequestID]                
AND B.[PersonID] = @ToKeepPersonID                
                 
 UPDATE F                
 SET F.[RequestPersonID] = B.[RequestPersonID]                
 FROM [dbo].[SCR_ScreeningRequestPersonFinalDecision] AS F                
 INNER JOIN [dbo].[SCR_RequestPerson] AS A                
 ON F.[RequestPersonID] = A.[RequestPersonID]                
 AND A.[PersonID] = @ToDeletePersonID                
 INNER JOIN [dbo].[SCR_RequestPerson] AS B                
 ON A.[RequestID] = B.[RequestID]                
 AND B.[PersonID] = @ToKeepPersonID                
                  
 DELETE FROM [dbo].[SCR_RequestPerson] WHERE [PersonID] = @ToDeletePersonID                  
             
             
 UPDATE [dbo].[SCR_PersonFinalDecision]            
 SET [PersonID] = @ToKeepPersonID            
 WHERE [PersonID] = @ToDeletePersonID            
            
                  
 SELECT TOP 1                  
  @PersonAliasLastName = D.[LastName],                                
  @PersonAliasFirstName = D.[FirstName],                                 
  @LastName = K.[LastName],                  
  @FirstName = K.[FirstName],                  
  @DayOfBirth = CASE WHEN K.[DayOfBirth] <> 0 THEN K.[DayOfBirth] ELSE D.[DayOfBirth] END,                  
  @MonthOfBirth = CASE WHEN K.[MonthOfBirth] <> 0 THEN K.[MonthOfBirth] ELSE D.[MonthOfBirth] END,                  
  @YearOfBirth = CASE WHEN K.[YearOfBirth] <> 0 THEN K.[YearOfBirth] ELSE D.[YearOfBirth] END,                  
  @BirthVillage = CASE WHEN LTRIM(RTRIM(ISNULL(K.[BirthVillage],''))) <> '' THEN K.[BirthVillage] ELSE D.[BirthVillage] END,                                
  @BirthRegionID = CASE WHEN K.[BirthRegionID] IS NOT NULL THEN K.[BirthRegionID] ELSE D.[BirthRegionID] END,                                 
  @ApproximateBirthDate = CASE WHEN LTRIM(RTRIM(ISNULL(K.[ApproximateBirthDate],''))) <> '' THEN K.[ApproximateBirthDate] ELSE D.[ApproximateBirthDate] END,                                
  @EthnicityID = CASE WHEN K.[EthnicityID] IS NOT NULL THEN K.[EthnicityID] ELSE D.[EthnicityID] END,                                
  @Height = CASE WHEN LTRIM(RTRIM(ISNULL(K.[Height],''))) <> '' THEN K.[Height] ELSE D.[Height] END,                                
  @Weight = CASE WHEN LTRIM(RTRIM(ISNULL(K.[Weight],''))) <> '' THEN K.[Weight] ELSE D.[Weight] END,                                
  @BackgroundInformation = ISNULL(K.[BackgroundInformation],'') + ' ' + ISNULL(D.[BackgroundInformation],''),                                
  @MilitaryIDNumber = ISNULL(K.[MilitaryIDNumber],'') + ' / ' + ISNULL(D.[MilitaryIDNumber],''),                                
  @Notes = ISNULL(K.[Notes],'') + ' ' + ISNULL(D.[Notes],'') + ' ' +                  
   CASE WHEN K.[DayOfBirth] <> 0 AND D.[DayOfBirth] <> 0 AND K.[DayOfBirth] <> D.[DayOfBirth] THEN 'Merged person''s day of birth was ' + CAST(D.[DayOfBirth] AS NVARCHAR(2)) + '. ' ELSE '' END +                  
   CASE WHEN K.[MonthOfBirth] <> 0 AND D.[MonthOfBirth] <> 0 AND K.[MonthOfBirth] <> D.[MonthOfBirth] THEN 'Merged person''s month of birth was ' + CAST(D.[MonthOfBirth] AS NVARCHAR(2)) + '. ' ELSE '' END +                  
   CASE WHEN K.[YearOfBirth] <> 0 AND D.[YearOfBirth] <> 0 AND K.[YearOfBirth] <> D.[YearOfBirth] THEN 'Merged person''s year of birth was ' + CAST(D.[YearOfBirth] AS NVARCHAR(4)) + '. ' ELSE '' END +                  
   CASE WHEN LTRIM(RTRIM(ISNULL(K.[BirthVillage],''))) <> '' AND LTRIM(RTRIM(ISNULL(D.[BirthVillage],''))) <> '' AND K.[BirthVillage] <> D.[BirthVillage] THEN 'Merged person''s birth village was ' + D.[BirthVillage] + '. ' ELSE '' END +                  
   CASE WHEN K.[BirthRegionID] IS NOT NULL AND D.[BirthRegionID] IS NOT NULL AND K.[BirthRegionID] <> D.[BirthRegionID] THEN 'Merged person''s birth region was ' + R.[RegionName] + '. ' ELSE '' END +                  
   CASE WHEN LTRIM(RTRIM(ISNULL(K.[ApproximateBirthDate],''))) <> '' AND LTRIM(RTRIM(ISNULL(D.[ApproximateBirthDate],''))) <> '' AND K.[ApproximateBirthDate] <> D.[ApproximateBirthDate]             
  THEN 'Merged person''s approximate birth date was ' + D.[ApproximateBirthDate] + '. ' ELSE '' END +                     
   CASE WHEN K.[EthnicityID] IS NOT NULL AND D.[EthnicityID] IS NOT NULL AND K.[EthnicityID] <> D.[EthnicityID] THEN 'Merged person''s ethnicity was ' + E.[EthnicityName] + '. ' ELSE '' END +                  
   CASE WHEN LTRIM(RTRIM(ISNULL(K.[Height],''))) <> '' AND LTRIM(RTRIM(ISNULL(D.[Height],''))) <> '' AND K.[Height] <> D.[Height] THEN 'Merged person''s height was ' + D.[Height] + '. ' ELSE '' END +                  
   CASE WHEN LTRIM(RTRIM(ISNULL(K.[Weight],''))) <> '' AND LTRIM(RTRIM(ISNULL(D.[Weight],''))) <> '' AND K.[Weight] <> D.[Weight] THEN 'Merged person''s weight was ' + D.[Weight] + '. ' ELSE '' END +                  
   'Merged person''s profile status was ' + PS.[ProfileStatusName] + '. ',                              
  @ProfileStatusID = K.[ProfileStatusID],                              
  @Archive = 0,      
  @SetToRestrictedProfile = CASE WHEN D.[IsRestrictedProfile] = 1 AND K.[IsRestrictedProfile] = 0 THEN 1 ELSE 0 END      
 FROM [dbo].[PRF_Person] AS D                  
 INNER JOIN [dbo].[PRF_Person] AS K                  
 ON D.[PersonID] = @ToDeletePersonID                  
 AND K.[PersonID] = @ToKeepPersonID                  
 LEFT JOIN [dbo].[PRF_Ethnicity] AS E                  
 ON E.[EthnicityID] = D.[EthnicityID]                  
 LEFT JOIN [dbo].[PRF_Region] AS R                  
 ON R.[RegionID] = D.[BirthRegionID]                  
INNER JOIN [dbo].[PRF_ProfileStatus] AS PS                  
 ON PS.[ProfileStatusID] = D.[ProfileStatusID]                  
                   
 EXEC dbo.PRF_SP_PersonAliasAdd                   
     @PersonID=@ToKeepPersonID,                   
     @LastName=@PersonAliasLastName,                   
     @FirstName=@PersonAliasFirstName,                   
     @Notes='Generated automatically during person merge',                   
     @UserID=@UserID,                   
     @Archive=0,                  
     @IsProfilingChange=@IsProfilingChange                  
                  
 IF LTRIM(RTRIM(ISNULL(@BirthVillage,''))) = '' BEGIN SET @BirthVillage = NULL END                  
 IF LTRIM(RTRIM(ISNULL(@ApproximateBirthDate,''))) = '' BEGIN SET @ApproximateBirthDate = NULL END                  
 IF LTRIM(RTRIM(ISNULL(@Height,''))) = '' BEGIN SET @Height = NULL END                  
 IF LTRIM(RTRIM(ISNULL(@Weight,''))) = '' BEGIN SET @Weight = NULL END                  
 SELECT @BackgroundInformation = CASE WHEN LTRIM(RTRIM(ISNULL(@BackgroundInformation,''))) = '' THEN NULL ELSE LTRIM(RTRIM(@BackgroundInformation)) END                  
 SELECT @MilitaryIDNumber = CASE                  
    WHEN LEFT(@MilitaryIDNumber, 2) = ' /' THEN LTRIM(RTRIM(RIGHT(@MilitaryIDNumber, LEN(@MilitaryIDNumber)-2)))                  
    WHEN RIGHT(@MilitaryIDNumber, 2) = '/ ' THEN LTRIM(RTRIM(LEFT(@MilitaryIDNumber, LEN(@MilitaryIDNumber)-2)))                  
    ELSE LTRIM(RTRIM(@MilitaryIDNumber))                  
   END                  
 SELECT @Notes = CASE WHEN LTRIM(RTRIM(ISNULL(@Notes,''))) = '' THEN NULL ELSE LTRIM(RTRIM(@Notes)) END                  
                   
 EXEC dbo.PRF_SP_PersonUpdate                  
     @PersonID=@ToKeepPersonID,                  
     @LastName=@LastName,                  
     @FirstName=@FirstName,                  
     @DayOfBirth=@DayOfBirth,                  
     @MonthOfBirth=@MonthOfBirth,                  
     @YearOfBirth=@YearOfBirth,                  
     @BirthVillage=@BirthVillage,          
     @BirthRegionID=@BirthRegionID,                  
     @ApproximateBirthDate=@ApproximateBirthDate,                  
     @EthnicityID=@EthnicityID,                  
     @Height=@Height,                  
     @Weight=@Weight,                  
     @BackgroundInformation=@BackgroundInformation,                  
     @MilitaryIDNumber=@MilitaryIDNumber,                  
     @Notes=@Notes,                  
     @ProfileStatusID=@ProfileStatusID,                  
     @UserID=@UserID,          
     @Archive=@Archive,                  
     @IsProfilingChange=@IsProfilingChange  
   
 --set to restricted only if the deleted profile was restricted, the but kept profile was not              
 IF @SetToRestrictedProfile = 1  
  BEGIN  
  EXEC dbo.PRF_SP_PersonUpdate_IsRestrictedProfile @PersonID=@ToKeepPersonID, @UserID=@UserID, @IsRestrictedProfile=1, @IsProfilingChange=@IsProfilingChange  
  END  
                    
 EXEC dbo.PRF_SP_PersonAudit @UserID=@UserID, @AdminAuditTypeID=3, @PersonID=@ToDeletePersonID, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                
                       
     DELETE TOP (1) FROM [dbo].[PRF_Person] WHERE [PersonID]=@ToDeletePersonID                  
  COMMIT TRANSACTION                                  
 END TRY                                    
 BEGIN CATCH                                    
                                  
  IF @@TRANCOUNT > 0                                  
   ROLLBACK TRANSACTION                                 
  
  SELECT 0 AS Result
                                  
 END CATCH                                    
 SELECT 1 AS Result
END 
