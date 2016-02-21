SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PRF_SP_Search_SearchForPersonCount_NHibernate]                      
(                                              
 @PartialName NVARCHAR(MAX) = NULL,                            
 @ExactName NVARCHAR(MAX) = NULL,                            
 @MilitaryID NVARCHAR(20) = NULL,   
 @AlternativeMilitaryID NVARCHAR(20) = NULL,                            
 @RankID INT = NULL,  
 @RoleID INT = NULL,  
 @YearOfBirth INT = 0,  
 @MonthOfBirth INT = 0,  
 @DayOfBirth INT = 0,  
 @UserID NVARCHAR(450),  
 @Separator NVARCHAR(10),    
 @IncludeRestrictedProfiles BIT = 0    
)                                              
AS                                              
BEGIN                                         
 DECLARE @Persons TABLE                                            
 (                                     
  [PersonID] INT PRIMARY KEY,                      
  [FirstName] NVARCHAR(500),                      
  [LastName] NVARCHAR(500),                      
  [MilitaryIDNumber] NVARCHAR(255),            
  [Aliases] NVARCHAR(MAX),                      
  [Score] DECIMAL(5,2),                      
  [RowNumber] INT                      
 )                             
                                                  
 INSERT INTO @Persons ([PersonID],[FirstName],[LastName],[MilitaryIDNumber],[Aliases],[Score],[RowNumber])                      
 EXEC [dbo].[PRF_SP_Search_SearchForPerson]                      
  @PartialName=@PartialName,          
  @ExactName=@ExactName,          
  @MilitaryID=@MilitaryID,  
  @AlternativeMilitaryID=@AlternativeMilitaryID,  
  @RankID=@RankID,  
  @RoleID=@RoleID,  
  @YearOfBirth=@YearOfBirth,          
  @MonthOfBirth=@MonthOfBirth,          
  @DayOfBirth=@DayOfBirth,        
  @UserID=@UserID,    
  @Separator=@Separator,    
  @IncludeRestrictedProfiles=@IncludeRestrictedProfiles    
                                              
 SELECT COUNT(*) AS Count FROM @Persons                                            
END