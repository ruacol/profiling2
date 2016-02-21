SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PRF_SP_Search_SearchForPerson_NHibernate]                            
(            
 @ExactName NVARCHAR(MAX) = NULL,                            
 @PartialName NVARCHAR(MAX) = NULL,                            
 @MilitaryID NVARCHAR(20) = NULL,  
 @AlternativeMilitaryID NVARCHAR(20) = NULL,  
 @RankID INT = NULL,  
 @RoleID INT = NULL,  
 @YearOfBirth INT = 0,                  
 @MonthOfBirth INT = 0,                  
 @DayOfBirth INT = 0,                  
 @UserID NVARCHAR(450),                        
 @Separator NVARCHAR(10),                         
 @MaximumRows INT = NULL,                                                
 @StartRowIndex INT = NULL,      
 @IncludeRestrictedProfiles BIT = 0       
)                            
AS                            
BEGIN                           
                      
-- ****************************************************************                                            
-- STORED PROCEDURE [dbo].[PRF_SP_Search_SearchForPersonCount] DEPENDS ON THE STRUCTURE OF THE RESULT SET                                            
-- DO NOT MODIFY THE RESULT SET WITHOUT MODIFYING THE DEPENDENT STORED PROCEDURES                                            
-- ****************************************************************                          
                       
 DECLARE @PartialNamesToCheck TABLE ( [ID] INT IDENTITY (1,1), [Name] NVARCHAR(500) )                                        
 DECLARE @ExactNamesToCheck TABLE ( [ID] INT IDENTITY (1,1), [Name] NVARCHAR(500) )                                        
 DECLARE @PersonsFound TABLE ( [PersonID] INT, [SCORE] DECIMAL(5,2) )                        
 DECLARE @MilitaryIDWeight DECIMAL(5,2), @RankIDWeight DECIMAL(5,2), @PartialNameWeight DECIMAL(5,2)   
 DECLARE @BirthDateWeight DECIMAL(5,2), @RoleIDWeight DECIMAL(5,2), @ExactNameWeight DECIMAL(5,2)  
 DECLARE @PartialNameCount INT            
                        
                        
                        
 -- *****************************************************                        
 -- ********* VALIDATION CHECK OF PARAMETERS ************                        
 -- *****************************************************                        
                        
 --no search criteria input                          
 IF ((@ExactName IS NULL AND @PartialName IS NULL AND @MilitaryID IS NULL AND @RankID IS NULL AND @RoleID IS NULL) AND (@YearOfBirth = 0 AND @MonthOfBirth = 0 AND @DayOfBirth = 0))
  BEGIN                          
  RETURN                          
  END                          
                        
                        
                        
                        
 -- *****************************************************                        
 -- ************** CONFIGURE ALGORITHM ******************                        
 -- *****************************************************                        
                        
 SELECT                        
  @MilitaryIDWeight = 5.0,                        
  @RankIDWeight = 1.0,                        
  @RoleIDWeight = 1.0,  
  @PartialNameWeight = 10.0,                  
  @ExactNameWeight = 15.0,                  
  @BirthDateWeight = 1.0                  
                        
                        
 -- *****************************************************                        
 -- ********* CREATE A TABLES WITH SEARCH TERMS **********                        
 -- *****************************************************                        
                        
 --Creates table of partial name matches                            
 --Takes a string where each word is separated, e.g. by ;;                                        
 --Converts "This;;is;;a;;phrase into a table of words                                        
 ; WITH SplitSearchTerm AS                                        
 (                                          
  SELECT CAST(0 AS BIGINT) AS [FirstPos],CHARINDEX(@Separator,@PartialName) [SecondPos]                                      
  UNION ALL                                          
  SELECT [SecondPos]+LEN(@Separator),CHARINDEX(@Separator,@PartialName,[SecondPos]+LEN(@Separator))                              
  FROM SplitSearchTerm                                         
  WHERE [SecondPos]>0         
 )                                        
 INSERT INTO @PartialNamesToCheck                                        
 SELECT                   
  [Name]                                        
 FROM                                        
  (                                        
  SELECT                             
    SUBSTRING                                          
     ( --starting substring                                          
     @PartialName,                                 
     FirstPos,                                          
     COALESCE(NULLIF(SecondPos,0),LEN(@PartialName)+LEN(@Separator))-FirstPos                                          
     ) --ending substring                                          
    AS [Name]                                          
  FROM  SplitSearchTerm                                        
  ) AS [NAMES]                                        
 --ignore blank fields                                    
 WHERE [NAMES].[Name] <> ''                                    
                        
 SELECT @PartialNameCount = COUNT(*) FROM @PartialNamesToCheck            
            
 --Creates table of exact name matches            
 --Takes a string where each word is separated, e.g. by ;;                                        
 --Converts "This;;is;;a;;phrase into a table of words                                        
 ; WITH SplitSearchTerm AS                                        
 (                                          
  SELECT CAST(0 AS BIGINT) AS [FirstPos],CHARINDEX(@Separator,@ExactName) [SecondPos]                                          
  UNION ALL                                          
  SELECT [SecondPos]+LEN(@Separator),CHARINDEX(@Separator,@ExactName,[SecondPos]+LEN(@Separator))                                          
  FROM SplitSearchTerm                                         
  WHERE [SecondPos]>0                                          
 )                                        
 INSERT INTO @ExactNamesToCheck                                        
 SELECT                                        
  [Name]                                        
 FROM                                        
  (                                        
  SELECT                                        
    SUBSTRING                                          
     ( --starting substring                                          
     @ExactName,                                          
     FirstPos,                                          
     COALESCE(NULLIF(SecondPos,0),LEN(@ExactName)+LEN(@Separator))-FirstPos                                          
     ) --ending substring                                          
    AS [Name]                                          
  FROM  SplitSearchTerm                                        
  ) AS [NAMES]                                        
 --ignore blank fields                                    
 WHERE [NAMES].[Name] <> ''                                    
                        
                         
                        
 -- *****************************************************                        
 -- ********* RETRIEVING MATCHING PERSONS ***************                        
 -- *****************************************************                        
INSERT INTO @PersonsFound                        
([PersonID],[Score])                        
SELECT                        
 M.[PersonID] AS [PersonID],                        
 SUM(M.[Score]) AS [Score]                        
FROM                        
(                        
                  
                  
 --Matching Birth Date                  
 SELECT                  
  [PersonID] AS [PersonID],                  
  CASE WHEN @YearOfBirth <> 0 AND [YearOfBirth] = @YearOfBirth THEN @BirthDateWeight ELSE 0 END                  
  +                  
  CASE WHEN @MonthOfBirth <> 0 AND [MonthOfBirth] = @MonthOfBirth THEN @BirthDateWeight ELSE 0 END                  
  +                  
  CASE WHEN @DayOfBirth <> 0 AND [DayOfBirth] = @DayOfBirth THEN @BirthDateWeight ELSE 0 END                  
  AS [Score]                  
 FROM [dbo].[PRF_Person]                  
 WHERE [Archive] = 0                  
 AND                  
  (                  
   (@YearOfBirth <> 0 AND [YearOfBirth] = @YearOfBirth)                  
  OR                  
   (@MonthOfBirth <> 0 AND [MonthOfBirth] = @MonthOfBirth)                  
  OR                  
   (@DayOfBirth <> 0 AND [DayOfBirth] = @DayOfBirth)                  
  )       
 AND (@IncludeRestrictedProfiles = 1 OR [IsRestrictedProfile] = 0)                 
                     
                  
                  
                  
UNION ALL                  
                  
                  
                  
                  
 --Matching Military ID                        
 SELECT                        
  [PersonID] AS [PersonID],                        
  @MilitaryIDWeight AS [Score]                        
 FROM [dbo].[PRF_Person]                        
 WHERE [MilitaryIDNumber] IS NOT NULL  
 AND  
  (  
   (@MilitaryID IS NOT NULL AND REPLACE(REPLACE(REPLACE([MilitaryIDNumber], '/', ''), '-', ''), '.', '') LIKE '%' + @MilitaryID + '%')  
  OR  
   (@AlternativeMilitaryID IS NOT NULL AND REPLACE(REPLACE(REPLACE([MilitaryIDNumber], '/', ''), '-', ''), '.', '') LIKE '%' + @AlternativeMilitaryID + '%')  
  )  
 AND [Archive] = 0        
 AND (@IncludeRestrictedProfiles = 1 OR [IsRestrictedProfile] = 0)       
        
                        
                  
                  
                  
 UNION ALL                        
                        
                  
                  
 --Matching Rank                        
 SELECT                        
  P.[PersonID] AS [PersonID],                        
  @RankIDWeight AS [Score]              
 FROM [dbo].[PRF_Person] AS P                        
 INNER JOIN [dbo].[PRF_Career] AS C                        
 ON P.[PersonID] = C.[PersonID]                        
 AND C.[IsCurrentCareer] = 1 --only if current career                        
 AND @RankID IS NOT NULL                        
 AND C.[RankID] = @RankID                        
 AND C.[Archive] = 0                        
 WHERE P.[Archive] = 0        
 AND (@IncludeRestrictedProfiles = 1 OR P.[IsRestrictedProfile] = 0)                       
                        
                
                
                
                
 UNION ALL                        
                        
                  
                  
 --Matching Role                   
 SELECT                        
  P.[PersonID] AS [PersonID],                        
  @RoleIDWeight AS [Score]              
 FROM [dbo].[PRF_Person] AS P                        
 INNER JOIN [dbo].[PRF_Career] AS C                        
 ON P.[PersonID] = C.[PersonID]                        
 AND C.[IsCurrentCareer] = 1 --only if current career                        
 AND @RoleID IS NOT NULL AND C.[RoleID] IS NOT NULL  
 AND C.[RoleID] = @RoleID                        
 AND C.[Archive] = 0                        
 WHERE P.[Archive] = 0        
 AND (@IncludeRestrictedProfiles = 1 OR P.[IsRestrictedProfile] = 0)                       
                        
                
                
                
                
 UNION ALL                       
                
                
                
                
                        
 --Matching Partial Names                        
 SELECT                        
  P.[PersonID] AS [PersonID],                        
  CASE                         
   WHEN (@PartialNameCount / CAST(COUNT(DISTINCT N.[ID]) AS DECIMAL(5,2))) = 1 THEN @PartialNameWeight --Matched all names in terms (or some names were repeated)                        
   ELSE @PartialNameWeight / (@PartialNameCount / CAST(COUNT(DISTINCT N.[ID]) AS DECIMAL(5,2))) --Matched only some names in terms                        
  END AS [Score]                        
 FROM [dbo].[PRF_Person] AS P                        
  LEFT JOIN [dbo].[PRF_PersonAlias] AS PA                    
  ON PA.[PersonID] = P.[PersonID]                    
  AND PA.[Archive] = 0                    
 INNER JOIN @PartialNamesToCheck AS N                        
 ON ISNULL(P.[FirstName],'') COLLATE Latin1_general_CI_AI LIKE '%' + N.[Name] + '%' COLLATE Latin1_general_CI_AI                
 OR ISNULL(P.[LastName],'') COLLATE Latin1_general_CI_AI LIKE '%' + N.[Name] + '%' COLLATE Latin1_general_CI_AI                        
 OR (                    
  PA.[PersonAliasID] IS NOT NULL                     
 AND                     
  (                    
   ISNULL(PA.[FirstName],'') COLLATE Latin1_general_CI_AI LIKE '%' + N.[Name] + '%' COLLATE Latin1_general_CI_AI                    
  OR                    
   ISNULL(PA.[LastName],'') COLLATE Latin1_general_CI_AI LIKE '%' + N.[Name] + '%' COLLATE Latin1_general_CI_AI                    
  )                    
 )                    
 WHERE P.[Archive] = 0        
 AND (@IncludeRestrictedProfiles = 1 OR P.[IsRestrictedProfile] = 0)                      
 GROUP BY P.[PersonID]                       
            
            
            
            
            
UNION ALL            
            
            
            
            
 --Matching Exact Names                        
 SELECT                        
  P.[PersonID] AS [PersonID],                        
  MAX(@ExactNameWeight) AS [Score]                        
 FROM [dbo].[PRF_Person] AS P            
  LEFT JOIN [dbo].[PRF_PersonAlias] AS PA                    
  ON PA.[PersonID] = P.[PersonID]                    
  AND PA.[Archive] = 0                    
 INNER JOIN @ExactNamesToCheck AS N            
 ON N.[Name] =           
  CASE          
   WHEN P.[FirstName] IS NOT NULL AND P.[LastName] IS NOT NULL THEN P.[FirstName] + ' ' + P.[LastName]          
   ELSE           
    CASE WHEN P.[FirstName] IS NULL THEN '' ELSE P.[FirstName] + ' ' END + ISNULL(P.[LastName],'')          
  END          
 OR (                    
  PA.[PersonAliasID] IS NOT NULL                     
 AND                     
  (                    
   N.[Name] =             
   CASE          
    WHEN PA.[FirstName] IS NOT NULL AND PA.[LastName] IS NOT NULL THEN PA.[FirstName] + ' ' + PA.[LastName]          
    ELSE           
     CASE WHEN PA.[FirstName] IS NULL THEN '' ELSE PA.[FirstName] + ' ' END + ISNULL(PA.[LastName],'')          
   END             
  )                    
 )            
 WHERE P.[Archive] = 0         
 AND (@IncludeRestrictedProfiles = 1 OR P.[IsRestrictedProfile] = 0)                     
 GROUP BY P.[PersonID]                       
                    
                    
                     
) M                        
GROUP BY M.[PersonID]                        
        
                        
 -- *****************************************************                        
 -- *************** RETURNING RESULTS *******************                        
 -- *****************************************************                        
SELECT                      
 [PersonID],                      
 [FirstName],                      
 [LastName],                      
 [MilitaryIDNumber],                      
  (                        
   SELECT            
    CASE WHEN PA.[FirstName] IS NULL THEN '' ELSE PA.[FirstName] + ' ' END          
    +          
    CASE WHEN PA.[LastName] IS NULL THEN '' ELSE PA.[LastName] + '; ' END          
  FROM                        
    [dbo].[PRF_PersonAlias] AS PA                        
   WHERE                        
    PA.[PersonID] = A.[PersonID]                        
   FOR XML PATH ('')                        
  ) AS [Aliases],                        
 [Score],                      
 [RowNumber]                      
FROM                      
 (                        
 SELECT       
  P.[PersonID],                        
  ISNULL(P.[FirstName],'') AS [FirstName],                        
  ISNULL(P.[LastName],'') AS [LastName],                        
  ISNULL(P.[MilitaryIDNumber],'') AS [MilitaryIDNumber],                 
  F.[Score] AS [Score],                      
  ROW_NUMBER() OVER(ORDER BY F.[Score] DESC) AS [RowNumber]                      
 FROM [dbo].[PRF_Person] AS P                        
 INNER JOIN @PersonsFound AS F                        
 ON P.[PersonID] = F.[PersonID]                        
 ) AS A                      
 WHERE                                             
  (                                            
   (@MaximumRows IS NULL OR @StartRowIndex IS NULL)                                            
  OR                                            
   [RowNumber] BETWEEN (@StartRowIndex + 1) AND (@MaximumRows + @StartRowIndex)                                                
  )                                            
 ORDER BY [RowNumber] ASC                      
                             
END