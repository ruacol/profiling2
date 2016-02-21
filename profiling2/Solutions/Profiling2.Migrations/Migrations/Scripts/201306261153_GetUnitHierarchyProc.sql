SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PRF_SP_UnitCentric_GetUnitHierarchy]          
(          
 @UnitID INT = NULL,
 @UnitHierarchyID INT = NULL,
 @UserID NVARCHAR(450)      
)                                                
AS                                                
BEGIN                            
 IF @UnitID IS NULL AND @UnitHierarchyID IS NULL
  BEGIN
  RETURN
  END

                    
 SELECT            
  CASE WHEN UH.[ParentUnitID] IS NOT NULL AND UH.[ParentUnitID] = @UnitID THEN 1 ELSE 0 END AS [IsParentUnit],      
  CASE WHEN UH.[UnitID] = @UnitID THEN 1 ELSE 0 END AS [IsMainUnit],      
  UH.[ParentUnitID] AS [ParentUnitID],      
  ISNULL(PU.[UnitName],'') AS [ParentUnit],      
  UH.[UnitID] AS [UnitID],      
  U.[UnitName] AS [Unit],      
  CASE      
   WHEN O.[OrganizationID] IS NULL THEN ''      
   ELSE      
    CASE      
     WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = ISNULL(O.[OrgShortName],'') AND UN.[Archive] = 0)      
     THEN ISNULL(O.[OrgLongName],'')      
     ELSE O.[OrgShortName]      
    END      
  END AS [Organization],      
  UH.[UnitHierarchyID],                                               
  CASE                                                
   WHEN (UH.[YearOfStart] <> 0 OR UH.[MonthOfStart] <> 0 OR UH.[DayOfStart] <> 0)                                                
   THEN 'From ' +                                                
  CASE WHEN UH.[YearOfStart] = 0                                           
   THEN ''                                           
   ELSE  CAST(UH.[YearOfStart] AS NVARCHAR) + CASE WHEN UH.[MonthOfStart] <> 0 OR UH.[DayOfStart] <> 0 THEN '/' ELSE '' END                                           
  END                                          
  +                                          
  CASE WHEN UH.[MonthOfStart] = 0                                           
   THEN ''                                           
   ELSE CAST(UH.MonthOfStart AS NVARCHAR) + CASE WHEN UH.[DayOfStart] <> 0 THEN '/' ELSE '' END                                           
  END                                          
  +                                          
  CASE WHEN UH.[DayOfStart] = 0                                           
   THEN ''                                           
   ELSE CAST(UH.[DayOfStart] AS NVARCHAR)                                           
  END + ' '                                          
   ELSE                                                
    CASE                                                
     WHEN (UH.[YearAsOf] <> 0 OR UH.[MonthAsOf] <> 0 OR UH.[DayAsOf] <> 0)                                                
     THEN 'As of ' +                                          
  CASE WHEN UH.[YearAsOf] = 0                                           
   THEN ''                                           
   ELSE  CAST(UH.[YearAsOf] AS NVARCHAR) + CASE WHEN UH.[MonthAsOf] <> 0 OR UH.[DayAsOf] <> 0 THEN '/' ELSE '' END                                           
  END                                          
  +                                          
  CASE WHEN UH.[MonthAsOf] = 0                                           
   THEN ''                                           
   ELSE CAST(UH.MonthAsOf AS NVARCHAR) + CASE WHEN UH.[DayAsOf] <> 0 THEN '/' ELSE '' END                                           
  END                                          
  +                                          
  CASE WHEN UH.[DayAsOf] = 0                                           
   THEN ''                                           
   ELSE CAST(UH.[DayAsOf] AS NVARCHAR)                                           
  END + ' '                                                
     ELSE ''                                                
    END                                                
  END AS [StartDate],                                                
  CASE                                                
   WHEN (UH.[YearOfEnd] <> 0 OR UH.[MonthOfEnd] <> 0 OR UH.[DayOfEnd] <> 0)                                                
   THEN                     
  CASE                             
   WHEN UH.[YearAsOf] = 0 AND UH.[YearOfStart] = 0 AND UH.[MonthAsOf] = 0 AND UH.[MonthOfStart] = 0 AND UH.[DayAsOf] = 0 AND UH.[DayOfStart] = 0                             
   THEN 'U' ELSE 'u'         
  END                            
  + 'ntil ' +                                                
  CASE WHEN UH.[YearOfEnd] = 0                                           
   THEN ''                                           
   ELSE  CAST(UH.[YearOfEnd] AS NVARCHAR) + CASE WHEN UH.[MonthOfEnd] <> 0 OR UH.[DayOfEnd] <> 0 THEN '/' ELSE '' END                                           
  END                                          
  +                                          
  CASE WHEN UH.[MonthOfEnd] = 0                                           
   THEN ''                                           
   ELSE CAST(UH.MonthOfEnd AS NVARCHAR) + CASE WHEN UH.[DayOfEnd] <> 0 THEN '/' ELSE '' END                                           
  END                                          
  +                                          
  CASE WHEN UH.[DayOfEnd] = 0                                           
   THEN ''                                           
   ELSE CAST(UH.[DayOfEnd] AS NVARCHAR)                                       
  END + ' '                         
   ELSE ''                                                
  END AS [EndDate],                     
  CASE                        
   WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = ISNULL(UH.[Commentary],'') AND UN.[Archive] = 0)                        
   THEN ''                        
   ELSE ISNULL(UH.[Commentary],'')                        
  END AS [Commentary],                        
  CASE                        
   WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = L.[LocationName] AND UN.[Archive] = 0)                        
   THEN ''                        
   ELSE L.[LocationName]                        
  END AS [Location],  
  CASE  
   WHEN EXISTS (SELECT 'X' FROM [dbo].[PRF_AdminUnknown] AS UN WHERE UN.[UnknownValue] = UT.[UnitHierarchyTypeName] AND UN.[Archive] = 0)  
   THEN ''  
   ELSE UT.[UnitHierarchyTypeName]  
  END AS [UnitHierarchyTypeName]  
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
 WHERE 
	(
		(@UnitID IS NOT NULL AND (UH.[UnitID] = @UnitID OR UH.[ParentUnitID] = @UnitID))
	OR 
		(@UnitHierarchyID IS NOT NULL AND (UH.[UnitHierarchyID] = @UnitHierarchyID))
	)
 AND UH.[Archive] = 0                                                
 ORDER BY                                      
 CASE WHEN UH.[YearOfStart] = 0 THEN UH.[YearAsOf] ELSE UH.[YearOfStart] END DESC,                              
 CASE WHEN UH.[MonthOfStart] = 0 THEN UH.[MonthAsOf] ELSE UH.[MonthOfStart] END DESC,                              
 CASE WHEN UH.[DayOfStart] = 0 THEN UH.[DayAsOf] ELSE UH.[DayOfStart] END DESC,                              
 UH.[YearOfEnd] DESC,                                          
 UH.[MonthOfEnd] DESC,                     
 UH.[DayOfEnd] DESC,  
 UT.[UnitHierarchyTypeID] ASC                                         
END 