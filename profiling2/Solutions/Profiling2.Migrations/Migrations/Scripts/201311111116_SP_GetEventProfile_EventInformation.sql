SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PRF_SP_GetEventProfile_EventInformation]              
(              
 @EventID INT              
)              
AS              
BEGIN              
 SELECT  
  E.[EventName] AS [EventName],  
  ISNULL(E.[NarrativeEn],'') AS [NarrativeEn],  
  ISNULL(E.[NarrativeFr],'') AS [NarrativeFr],  
   CASE WHEN E.[YearOfStart] = 0 AND E.[MonthOfStart] = 0 AND E.[DayOfStart] = 0 THEN ''  
   ELSE          
     CASE WHEN E.[YearOfStart] = 0             
   THEN ''             
   ELSE  CAST(E.[YearOfStart] AS NVARCHAR) + CASE WHEN E.[MonthOfStart] <> 0 OR E.[DayOfStart] <> 0 THEN '/' ELSE '' END             
     END            
     +            
     CASE WHEN E.[MonthOfStart] = 0             
   THEN ''             
   ELSE CAST(E.MonthOfStart AS NVARCHAR) + CASE WHEN E.[DayOfStart] <> 0 THEN '/' ELSE '' END             
     END            
     +            
     CASE WHEN E.[DayOfStart] = 0             
   THEN ''             
   ELSE CAST(E.[DayOfStart] AS NVARCHAR)             
     END          
   END AS [DateOfStart],      
   CASE WHEN E.[YearOfEnd] = 0 AND E.[MonthOfEnd] = 0 AND E.[DayOfEnd] = 0 THEN ''  
   ELSE          
     CASE WHEN E.[YearOfEnd] = 0             
   THEN ''             
   ELSE  CAST(E.[YearOfEnd] AS NVARCHAR) + CASE WHEN E.[MonthOfEnd] <> 0 OR E.[DayOfEnd] <> 0 THEN '/' ELSE '' END             
     END            
     +            
     CASE WHEN E.[MonthOfEnd] = 0             
   THEN ''             
   ELSE CAST(E.MonthOfEnd AS NVARCHAR) + CASE WHEN E.[DayOfEnd] <> 0 THEN '/' ELSE '' END             
     END            
     +            
     CASE WHEN E.[DayOfEnd] = 0             
   THEN ''             
   ELSE CAST(E.[DayOfEnd] AS NVARCHAR)             
     END          
   END AS [DateOfEnd],       
      L.[LocationName]     
     +     
     CASE     
      WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS U WHERE U.[UnknownValue] = L.[Territory] AND U.[Archive] = 0)    
      OR L.[Territory] IS NULL    
      THEN ''    
      ELSE ', ' + L.[Territory]    
     END    
     +    
     CASE    
      WHEN R.RegionID IS NULL    
      THEN ''
      ELSE ', ' + R.[RegionName]
     END  AS [Location],  
  ISNULL(E.[Notes],'') AS [Notes]  
 FROM [dbo].[PRF_Event] AS E  
 INNER JOIN [dbo].[PRF_Location] AS L  
 ON L.[LocationID] = E.[LocationID] 
 AND L.[Archive] = 0   
 LEFT OUTER JOIN [dbo].[PRF_Region] AS R
 ON L.[RegionID] = R.[RegionID]
 WHERE E.[EventID] = @EventID  
 AND E.[Archive] = 0  
END