SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PRF_SP_UnitMerge_NHibernate]                                      
(                                      
 @ToKeepUnitID INT,                                    
 @ToDeleteUnitID INT,                                    
 @UserID NVARCHAR(450),                      
 @IsProfilingChange BIT = 1                                     
)                                      
AS                                      
BEGIN                    
 DECLARE @UnitName NVARCHAR(500)  
 DECLARE @BackgroundInformation NVARCHAR(MAX), @Notes NVARCHAR(MAX)  
 DECLARE @Archive BIT, @SetToRestrictedProfile BIT        
          
 IF @ToKeepUnitID = @ToDeleteUnitID          
  BEGIN          
  RETURN 0          
  END          
                    
 BEGIN TRY                                      
  BEGIN TRANSACTION                                    
                                  
     EXEC dbo.PRF_SP_CareerAudit @UserID=@UserID, @AdminAuditTypeID=2, @UnitID=@ToDeleteUnitID, @CareerID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                  
                                      
     UPDATE [dbo].[PRF_Career] SET [UnitID] = @ToKeepUnitID WHERE [UnitID] IS NOT NULL AND [UnitID] = @ToDeleteUnitID                    
                        
                                    
     EXEC dbo.PRF_SP_OrganizationResponsibilityAudit @UserID=@UserID, @AdminAuditTypeID=2, @UnitID=@ToDeleteUnitID, @OrganizationResponsibilityID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                  
                                  
     UPDATE [dbo].[PRF_OrganizationResponsibility] SET [UnitID] = @ToKeepUnitID WHERE [UnitID] IS NOT NULL AND [UnitID] = @ToDeleteUnitID                    
                                  
                                    
     EXEC dbo.PRF_SP_UnitHierarchyAudit @UserID=@UserID, @AdminAuditTypeID=2, @UnitID=@ToDeleteUnitID, @UnitHierarchyID=NULL, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                  
                                    
     UPDATE [dbo].[PRF_UnitHierarchy] SET [UnitID] = @ToKeepUnitID WHERE [UnitID] = @ToDeleteUnitID                     
                         
     UPDATE [dbo].[PRF_UnitHierarchy] SET [ParentUnitID] = @ToKeepUnitID WHERE [ParentUnitID] IS NOT NULL AND [ParentUnitID] = @ToDeleteUnitID  
                    
     UPDATE [dbo].[PRF_UnitLocation] SET [UnitID] = @ToKeepUnitID WHERE [UnitID] IS NOT NULL AND [UnitID] = @ToDeleteUnitID 
     
     UPDATE [dbo].[PRF_UnitAlias] SET [UnitID] = @ToKeepUnitID WHERE [UnitID] IS NOT NULL AND [UnitID] = @ToDeleteUnitID
     
     UPDATE [dbo].[PRF_UnitSource] SET [UnitID] = @ToKeepUnitID WHERE [UnitID] IS NOT NULL AND [UnitID] = @ToDeleteUnitID 
	                                
     UPDATE A                    
     SET A.[UnitID] = @ToKeepUnitID                    
     FROM [dbo].[PRF_AdminUnitImport] AS A                     
     WHERE A.[UnitID] = @ToDeleteUnitID                    
     AND NOT EXISTS                    
  (                    
   SELECT 'X' FROM [dbo].[PRF_AdminUnitImport] AS I                     
   WHERE I.[UnitID] = @ToKeepUnitID                    
   AND I.[AdminUnitImportTypeID] = A.[AdminUnitImportTypeID]                    
   AND I.[PreviousID] = A.[PreviousID]                    
  )                    
                      
     DELETE FROM [dbo].[PRF_AdminUnitImport] WHERE [UnitID] = @ToDeleteUnitID                    
              
                    
 SELECT TOP 1                    
  @UnitName = K.[UnitName],                    
  @BackgroundInformation = ISNULL(K.[BackgroundInformation],'') + ' ' + ISNULL(D.[BackgroundInformation],''),                                  
  @Notes = ISNULL(K.[Notes],'') + ' ' + ISNULL(D.[Notes],'') +                     
   CASE  
 WHEN D.[UnitName] <> K.[UnitName]  
 THEN ' Merged unit''s name was ' + D.[UnitName] ELSE '' END,  
  @Archive = 0  
 FROM [dbo].[PRF_Unit] AS D                    
 INNER JOIN [dbo].[PRF_Unit] AS K                    
 ON D.[UnitID] = @ToDeleteUnitID                    
 AND K.[UnitID] = @ToKeepUnitID                    
                                       
 SELECT @BackgroundInformation = CASE WHEN LTRIM(RTRIM(ISNULL(@BackgroundInformation,''))) = '' THEN NULL ELSE LTRIM(RTRIM(@BackgroundInformation)) END                    
 SELECT @Notes = CASE WHEN LTRIM(RTRIM(ISNULL(@Notes,''))) = '' THEN NULL ELSE LTRIM(RTRIM(@Notes)) END                    
                     
 EXEC dbo.PRF_SP_UnitUpdate  
     @UnitID=@ToKeepUnitID,                    
     @UnitName=@UnitName,                    
     @BackgroundInformation=@BackgroundInformation,                    
     @Notes=@Notes,                    
     @UserID=@UserID,            
     @Archive=@Archive,                    
     @IsProfilingChange=@IsProfilingChange    
     
 EXEC dbo.PRF_SP_UnitAudit @UserID=@UserID, @AdminAuditTypeID=3, @UnitID=@ToDeleteUnitID, @NewXml=NULL, @IsProfilingChange=@IsProfilingChange                                  
                         
     DELETE TOP (1) FROM [dbo].[PRF_Unit] WHERE [UnitID]=@ToDeleteUnitID                    
  COMMIT TRANSACTION                                    
 END TRY                                      
 BEGIN CATCH                                      
                                    
  IF @@TRANCOUNT > 0                                    
   ROLLBACK TRANSACTION                                   
    
  SELECT 0 AS Result                                      
                                    
 END CATCH                                      
 SELECT 1 AS Result                                    
END 