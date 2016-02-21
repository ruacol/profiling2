SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PRF_SP_Search_SearchForEvent]                
(                
 @PartialName NVARCHAR(MAX) = NULL,  
 @LocationID INT = NULL,  
 @RegionID INT = NULL,  
 @YearOfStart INT = 0,  
 @YearOfEnd INT = 0,  
 @MonthOfStart INT = 0,  
 @MonthOfEnd INT = 0,  
 @DayOfStart INT = 0,  
 @DayOfEnd INT = 0,  
 @UserID NVARCHAR(450),  
 @Separator NVARCHAR(10),  
 @MaximumRows INT = NULL,                                                
 @StartRowIndex INT = NULL    
)                
AS                
BEGIN                
  
-- ****************************************************************                                            
-- STORED PROCEDURE [dbo].[PRF_SP_Search_SearchForEventCount] DEPENDS ON THE STRUCTURE OF THE RESULT SET                                            
-- DO NOT MODIFY THE RESULT SET WITHOUT MODIFYING THE DEPENDENT STORED PROCEDURES                                            
-- ****************************************************************                          
 DECLARE @PartialNamesToCheck TABLE ( [ID] INT IDENTITY (1,1), [Name] NVARCHAR(500) )                                        
 DECLARE @EventsFound TABLE ( [EventID] INT, [SCORE] DECIMAL(5,2) )                        
 DECLARE @LocationIDWeight DECIMAL(5,2), @RegionIDWeight DECIMAL(5,2), @PartialNameWeight DECIMAL(5,2)  
 DECLARE @DateWeight DECIMAL(5,2), @NarrativeWeight DECIMAL(5,2)  
 DECLARE @PartialNameCount INT  
  
 -- *****************************************************                        
 -- ********* VALIDATION CHECK OF PARAMETERS ************                        
 -- *****************************************************                        
                        
 --no search criteria input                          
 IF ((@PartialName IS NULL AND @LocationID IS NULL AND @RegionID IS NULL) AND (@YearOfStart = 0 AND @YearOfEnd = 0 AND @MonthOfEnd = 0 AND @MonthOfStart = 0 AND @DayOfEnd = 0 AND @DayOfStart = 0))  
  BEGIN                          
  RETURN                          
  END                          
  
  
  
  
 -- *****************************************************                        
 -- ************** CONFIGURE ALGORITHM ******************                        
 -- *****************************************************                        
                        
 SELECT                        
  @LocationIDWeight = 5.0,  
  @RegionIDWeight = 1.0,                        
  @PartialNameWeight = 10.0,                  
  @NarrativeWeight = 1.0,  
  @DateWeight = 1.0                  
  
  
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
  
  
 -- *****************************************************                        
 -- ********* RETRIEVING MATCHING PERSONS ***************                        
 -- *****************************************************                        
INSERT INTO @EventsFound                        
([EventID],[Score])                        
SELECT                        
 M.[EventID] AS [EventID],                        
 SUM(M.[Score]) AS [Score]                        
FROM                        
(                        
                  
                  
 --Matching Start and End Date                  
 SELECT                  
  [EventID] AS [EventID],                  
  CASE WHEN @YearOfStart <> 0 AND [YearOfStart] = @YearOfStart THEN @DateWeight ELSE 0 END                  
  +                  
  CASE WHEN @MonthOfStart <> 0 AND [MonthOfStart] = @MonthOfStart THEN @DateWeight ELSE 0 END                  
  +                  
  CASE WHEN @DayOfStart <> 0 AND [DayOfStart] = @DayOfStart THEN @DateWeight ELSE 0 END                  
  +  
  CASE WHEN @YearOfEnd <> 0 AND [YearOfEnd] = @YearOfEnd THEN @DateWeight ELSE 0 END                  
  +                  
  CASE WHEN @MonthOfEnd <> 0 AND [MonthOfEnd] = @MonthOfEnd THEN @DateWeight ELSE 0 END                  
  +                  
  CASE WHEN @DayOfEnd <> 0 AND [DayOfEnd] = @DayOfEnd THEN @DateWeight ELSE 0 END                    
  AS [Score]                  
 FROM [dbo].[PRF_Event]  
 WHERE [Archive] = 0  
 AND                  
  (                  
   (@YearOfStart <> 0 AND [YearOfStart] = @YearOfStart)                  
  OR                  
   (@MonthOfStart <> 0 AND [MonthOfStart] = @MonthOfStart)                  
  OR                  
   (@DayOfStart <> 0 AND [DayOfStart] = @DayOfStart)                  
  OR  
   (@YearOfEnd <> 0 AND [YearOfEnd] = @YearOfEnd)                  
  OR                  
   (@MonthOfEnd <> 0 AND [MonthOfEnd] = @MonthOfEnd)                  
  OR                  
   (@DayOfEnd <> 0 AND [DayOfEnd] = @DayOfEnd)                    
  )       
                     
                  
                  
                  
UNION ALL                  
                  
                  
                  
                  
 --Matching Location ID                        
 SELECT                        
  [EventID] AS [EventID],                        
  @LocationIDWeight AS [Score]                        
 FROM [dbo].[PRF_Event]  
 WHERE (@LocationID IS NOT NULL AND [LocationID] = @LocationID)  
 AND [Archive] = 0        
  
        
                        
                  
                  
                  
 UNION ALL                        
   
   
   
   
   
   
   
   
   
  --Matching Region ID                        
 SELECT                        
  [EventID] AS [EventID],                        
  @RegionIDWeight AS [Score]                        
 FROM [dbo].[PRF_Event] AS E  
 INNER JOIN [dbo].[PRF_Location] AS L  
 ON E.[LocationID] = L.[LocationID]  
 AND (@RegionID IS NOT NULL AND L.[RegionID] = @RegionID)  
 AND L.[Archive]= 0  
 WHERE E.[Archive] = 0        
  
        
                        
                  
                  
                  
 UNION ALL                        
   
                        
                  
                  
                
                
                
                
        
                        
 --Matching Partial Names                        
 SELECT                        
  E.[EventID] AS [EventID],                        
  CASE                         
   WHEN (@PartialNameCount / CAST(COUNT(DISTINCT N.[ID]) AS DECIMAL(5,2))) = 1 THEN @PartialNameWeight --Matched all names in terms (or some names were repeated)                        
   ELSE @PartialNameWeight / (@PartialNameCount / CAST(COUNT(DISTINCT N.[ID]) AS DECIMAL(5,2))) --Matched only some names in terms                        
  END AS [Score]                        
 FROM [dbo].[PRF_Event] AS E  
 INNER JOIN @PartialNamesToCheck AS N                        
 ON E.[EventName] COLLATE Latin1_general_CI_AI LIKE '%' + N.[Name] + '%' COLLATE Latin1_general_CI_AI                
 WHERE E.[Archive] = 0  
 GROUP BY E.[EventID]                       
            
            
            
            
            
UNION ALL            
            
            
            
            
            
            
            
            
            
            
            
 --Matching Narratives  
 SELECT                        
  E.[EventID] AS [EventID],                        
  CASE                         
   WHEN (@PartialNameCount / CAST(COUNT(DISTINCT N.[ID]) AS DECIMAL(5,2))) = 1 THEN @NarrativeWeight --Matched all names in terms (or some names were repeated)  
   ELSE @NarrativeWeight / (@PartialNameCount / CAST(COUNT(DISTINCT N.[ID]) AS DECIMAL(5,2))) --Matched only some names in terms  
  END AS [Score]                        
 FROM [dbo].[PRF_Event] AS E  
 INNER JOIN @PartialNamesToCheck AS N                        
 ON (ISNULL(E.[NarrativeEn],'') COLLATE Latin1_general_CI_AI LIKE '%' + N.[Name] + '%' COLLATE Latin1_general_CI_AI)  
 OR (ISNULL(E.[NarrativeFr],'') COLLATE Latin1_general_CI_AI LIKE '%' + N.[Name] + '%' COLLATE Latin1_general_CI_AI)  
 WHERE E.[Archive] = 0  
 GROUP BY E.[EventID]                       
   
   
   
   
   
   
   
   
   
                    
                     
) M                        
GROUP BY M.[EventID]                        
        
                        
 -- *****************************************************                        
 -- *************** RETURNING RESULTS *******************                        
 -- *****************************************************                        
SELECT                      
 [EventID],  
 [EventName],  
 [Narrative],  
 [Location],  
 [StartDate],  
 [EndDate],  
 [Score],                      
 [RowNumber]                      
FROM                      
 (                        
 SELECT                        
  E.[EventID],                        
  E.[EventName] AS [EventName],  
  ISNULL(E.[NarrativeEn],ISNULL(E.[NarrativeFr],'')) AS [Narrative],  
  ISNULL(L.[LocationName],'') +       
   CASE       
    WHEN EXISTS (SELECT 'X' FROM PRF_AdminUnknown WHERE [UnknownValue] = ISNULL([Territory],''))      
    OR [Territory] IS NULL      
    THEN ''      
    ELSE ', ' + [Territory]      
   END AS [Location],                        
  CASE  
   WHEN E.[DayOfStart] = 0 AND E.[MonthOfStart] = 0 AND E.[YearOfStart] = 0 THEN ''  
   ELSE CAST(E.[YearOfStart] AS NVARCHAR(4)) + '/' + CAST(E.[MonthOfStart] AS NVARCHAR(2)) + '/' + CAST(E.[DayOfStart] AS NVARCHAR(2))  
  END AS [StartDate],  
  CASE  
   WHEN E.[DayOfEnd] = 0 AND E.[MonthOfEnd] = 0 AND E.[YearOfEnd] = 0 THEN ''  
   ELSE CAST(E.[YearOfEnd] AS NVARCHAR(4)) + '/' + CAST(E.[MonthOfEnd] AS NVARCHAR(2)) + '/' + CAST(E.[DayOfEnd] AS NVARCHAR(2))  
  END AS [EndDate],    
  F.[Score] AS [Score],                      
  ROW_NUMBER() OVER(ORDER BY F.[Score] DESC) AS [RowNumber]                      
 FROM [dbo].[PRF_Event] AS E                        
 LEFT JOIN [dbo].[PRF_Location] AS L  
 ON L.[LocationID] = E.[LocationID]  
 AND L.[Archive] = 0  
 INNER JOIN @EventsFound AS F                        
 ON E.[EventID] = F.[EventID]                        
 ) AS A                      
 WHERE                                             
  (                                            
   (@MaximumRows IS NULL OR @StartRowIndex IS NULL)                                            
  OR                                            
   [RowNumber] BETWEEN (@StartRowIndex + 1) AND (@MaximumRows + @StartRowIndex)                                                
  )                                            
 ORDER BY [RowNumber] ASC                      
                             
END