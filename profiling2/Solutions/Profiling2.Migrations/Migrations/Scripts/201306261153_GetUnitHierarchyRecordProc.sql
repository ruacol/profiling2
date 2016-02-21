SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PRF_SP_UnitCentric_GetUnitHierarchyRecord]            
(            
 @UnitHierarchyID INT,                          
 @UserID NVARCHAR(450)        
)                                                  
AS                                                  
BEGIN                                                  
 SELECT              
  UH.[ParentUnitID] AS [ParentUnitID],        
  ISNULL(PU.[UnitName],'') AS [ParentUnit],        
  UH.[UnitID] AS [UnitID],        
  U.[UnitName] AS [Unit],        
  O.[OrganizationID] AS [OrganizationID],    
  CASE        
   WHEN O.[OrganizationID] IS NULL THEN ''        
   ELSE        
    CASE        
     WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = ISNULL(O.[OrgShortName],'') AND UN.[Archive] = 0)        
     THEN ISNULL(O.[OrgLongName],'')        
     ELSE O.[OrgShortName]        
    END        
  END AS [Organization],    
  UH.[DayOfStart] AS [DayOfStart],    
  UH.[MonthOfStart] AS [MonthOfStart],    
  UH.[YearOfStart] AS [YearOfStart],    
  UH.[DayOfEnd] AS [DayOfEnd],    
  UH.[MonthOfEnd] AS [MonthOfEnd],    
  UH.[YearOfEnd] AS [YearOfEnd],      
  UH.[DayAsOf] AS [DayAsOf],    
  UH.[MonthAsOf] AS [MonthAsOf],    
  UH.[YearAsOf] AS [YearAsOf],      
  CASE                          
   WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = ISNULL(UH.[Commentary],'') AND UN.[Archive] = 0)                          
   THEN ''                          
   ELSE ISNULL(UH.[Commentary],'')                          
  END AS [Commentary],              
  L.[LocationID] AS [LocationID],                
  CASE      
   WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = L.[LocationName] AND UN.[Archive] = 0)      
   THEN ''      
   ELSE L.[LocationName]      
  END      
  +           
  CASE           
   WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = L.[Territory] AND UN.[Archive] = 0)          
   OR L.[Territory] IS NULL          
   THEN ''          
   ELSE ', ' + L.[Territory]          
  END          
  +          
  CASE          
   WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = L.[Province] AND UN.[Archive] = 0)      
   THEN ''      
   ELSE ', ' + L.[Province]      
  END AS [Location],    
  UT.[UnitHierarchyTypeID] AS [UnitHierarchyTypeID],    
  CASE    
   WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = UT.[UnitHierarchyTypeName] AND UN.[Archive] = 0)    
   THEN ''    
   ELSE UT.[UnitHierarchyTypeName]    
  END AS [UnitHierarchyTypeName],  
  CASE                          
   WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = ISNULL(UH.[Notes],'') AND UN.[Archive] = 0)                          
   THEN ''                          
   ELSE ISNULL(UH.[Notes],'')                          
  END AS [Notes]  
 FROM [dbo].[PRF_UnitHierarchy] AS UH             
 INNER JOIN [dbo].[PRF_UnitHierarchyType] AS UT    
 ON UH.[UnitHierarchyTypeID] = UT.[UnitHierarchyTypeID]    
 AND UT.[Archive] = 0    
 INNER JOIN [dbo].[PRF_Unit] AS U        
 ON U.[UnitID] = UH.[UnitID]          
 AND U.[Archive] = 0                                               
 LEFT JOIN [dbo].[PRF_Unit] AS PU        
 ON PU.[UnitID] = UH.[ParentUnitID]                                                  
 AND PU.[Archive] = 0                                                  
 INNER JOIN [dbo].[PRF_Location] AS L                               
 ON L.[LocationID] = UH.[LocationID]                                                  
 AND L.[Archive] = 0                                                  
 LEFT JOIN [dbo].[PRF_Organization] AS O        
 ON O.[OrganizationID] = U.[OrganizationID]                      
 AND O.[Archive] = 0                      
 WHERE UH.[UnitHierarchyID] = @UnitHierarchyID     
 AND UH.[Archive] = 0                                                  
END 