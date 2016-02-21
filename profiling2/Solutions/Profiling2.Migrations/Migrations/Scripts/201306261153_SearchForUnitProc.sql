SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PRF_SP_Search_SearchForUnit]                        
(        
 @ExactName NVARCHAR(MAX) = NULL,                        
 @PartialName NVARCHAR(MAX) = NULL,                        
 @OrganizationID INT = NULL,                        
 @UserID NVARCHAR(450),                    
 @Separator NVARCHAR(10),                     
 @MaximumRows INT = NULL,                                            
 @StartRowIndex INT = NULL
)                        
AS                        
BEGIN                       
                  
-- ****************************************************************                                        
-- STORED PROCEDURE [dbo].[PRF_SP_Search_SearchForUnitCount] DEPENDS ON THE STRUCTURE OF THE RESULT SET                                        
-- DO NOT MODIFY THE RESULT SET WITHOUT MODIFYING THE DEPENDENT STORED PROCEDURES                                        
-- ****************************************************************                      
                   
 DECLARE @PartialNamesToCheck TABLE ( [ID] INT IDENTITY (1,1), [Name] NVARCHAR(500) )                                    
 DECLARE @ExactNamesToCheck TABLE ( [ID] INT IDENTITY (1,1), [Name] NVARCHAR(500) )                                    
 DECLARE @UnitsFound TABLE ( [UnitID] INT, [SCORE] DECIMAL(5,2) )                    
 DECLARE @OrganizationIDWeight DECIMAL(5,2), @PartialNameWeight DECIMAL(5,2), @ExactNameWeight DECIMAL(5,2)
 DECLARE @PartialNameCount INT        
                    
                    
                    
 -- *****************************************************                    
 -- ********* VALIDATION CHECK OF PARAMETERS ************                    
 -- *****************************************************                    
                    
 --no search criteria input                      
 IF (@ExactName IS NULL AND @PartialName IS NULL AND @OrganizationID IS NULL)
  BEGIN                      
  RETURN                      
  END                      
                    
                    
                    
                    
 -- *****************************************************                    
 -- ************** CONFIGURE ALGORITHM ******************                    
 -- *****************************************************                    
                    
 SELECT                    
  @OrganizationIDWeight = 1.0,                    
  @PartialNameWeight = 10.0,              
  @ExactNameWeight = 15.0

                    
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
INSERT INTO @UnitsFound                    
([UnitID],[Score])                    
SELECT                    
 M.[UnitID] AS [UnitID],                    
 SUM(M.[Score]) AS [Score]                    
FROM                    
(                    
              
              
              
              
                    
              
              
 --Matching Organization 
 SELECT                    
  U.[UnitID] AS [UnitID],                    
  @OrganizationIDWeight AS [Score]          
 FROM [dbo].[PRF_Unit] AS U
 WHERE @OrganizationID IS NOT NULL
 AND U.[OrganizationID] = @OrganizationID
 AND U.[Archive] = 0    

                    
            
            
            
            
 UNION ALL                    
            
            
            
            
            
                    
 --Matching Partial Names                    
 SELECT                    
  U.[UnitID] AS [UnitID],                    
  CASE                     
   WHEN (@PartialNameCount / CAST(COUNT(DISTINCT N.[ID]) AS DECIMAL(5,2))) = 1 THEN @PartialNameWeight --Matched all names in terms (or some names were repeated)                    
   ELSE @PartialNameWeight / (@PartialNameCount / CAST(COUNT(DISTINCT N.[ID]) AS DECIMAL(5,2))) --Matched only some names in terms                    
  END AS [Score]                    
 FROM [dbo].[PRF_Unit] AS U
 INNER JOIN @PartialNamesToCheck AS N                    
 ON U.[UnitName] COLLATE Latin1_general_CI_AI LIKE '%' + N.[Name] + '%' COLLATE Latin1_general_CI_AI            
 WHERE U.[Archive] = 0    
 GROUP BY U.[UnitID]                   
        
        
        
        
        
UNION ALL        
        
        
        
        
 --Matching Exact Names                    
 SELECT                    
  U.[UnitID] AS [UnitID],                    
  MAX(@ExactNameWeight) AS [Score]                    
 FROM [dbo].[PRF_Unit] AS U
 INNER JOIN @ExactNamesToCheck AS N        
 ON N.[Name] = U.[UnitName]
 WHERE U.[Archive] = 0     
 GROUP BY U.[UnitID]                   
                
                
                 
) M                    
GROUP BY M.[UnitID]                    
    
                    
 -- *****************************************************                    
 -- *************** RETURNING RESULTS *******************                    
 -- *****************************************************                    
SELECT                  
 [UnitID],                  
 [UnitName],                  
 [BackgroundInformation],                  
 [Score],                  
 [RowNumber]
FROM                  
 (                    
 SELECT                    
  U.[UnitID] AS [UnitID],                    
  U.[UnitName] AS [UnitName],
  ISNULL(U.[BackgroundInformation],'') AS [BackgroundInformation],
  F.[Score] AS [Score],                  
  ROW_NUMBER() OVER(ORDER BY F.[Score] DESC) AS [RowNumber]                  
 FROM [dbo].[PRF_Unit] AS U
 INNER JOIN @UnitsFound AS F                    
 ON U.[UnitID] = F.[UnitID]                    
 ) AS A                  
 WHERE                                         
  (                                        
   (@MaximumRows IS NULL OR @StartRowIndex IS NULL)                                        
  OR                                        
   [RowNumber] BETWEEN (@StartRowIndex + 1) AND (@MaximumRows + @StartRowIndex)                                            
  )                                        
 ORDER BY [RowNumber] ASC                  
                         
END