SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PRF_SP_GetPersonProfile_CareerInformation]                              
(                              
 @PersonID INT                              
)                              
AS                              
BEGIN                              
 SELECT                              
  CASE                              
   WHEN (C.[YearOfStart] <> 0 OR C.[MonthOfStart] <> 0 OR C.[DayOfStart] <> 0)                              
   THEN 'From ' +                              
  CASE WHEN C.[YearOfStart] = 0                         
   THEN ''                         
   ELSE  CAST(C.[YearOfStart] AS NVARCHAR) + CASE WHEN C.[MonthOfStart] <> 0 OR C.[DayOfStart] <> 0 THEN '/' ELSE '' END                         
  END                        
  +                        
  CASE WHEN C.[MonthOfStart] = 0                         
   THEN ''                         
   ELSE CAST(C.MonthOfStart AS NVARCHAR) + CASE WHEN C.[DayOfStart] <> 0 THEN '/' ELSE '' END                         
  END                        
  +                        
  CASE WHEN C.[DayOfStart] = 0                         
   THEN ''                         
   ELSE CAST(C.[DayOfStart] AS NVARCHAR)                         
  END + ' '                        
   ELSE                              
    CASE                              
     WHEN (C.[YearAsOf] <> 0 OR C.[MonthAsOf] <> 0 OR C.[DayAsOf] <> 0)                              
     THEN 'As of ' +                        
  CASE WHEN C.[YearAsOf] = 0                         
   THEN ''                         
   ELSE  CAST(C.[YearAsOf] AS NVARCHAR) + CASE WHEN C.[MonthAsOf] <> 0 OR C.[DayAsOf] <> 0 THEN '/' ELSE '' END                         
  END                        
  +                        
  CASE WHEN C.[MonthAsOf] = 0                         
   THEN ''                         
   ELSE CAST(C.MonthAsOf AS NVARCHAR) + CASE WHEN C.[DayAsOf] <> 0 THEN '/' ELSE '' END                         
  END                        
  +                        
  CASE WHEN C.[DayAsOf] = 0                         
   THEN ''                         
   ELSE CAST(C.[DayAsOf] AS NVARCHAR)                         
  END + ' '                              
     ELSE ''                              
    END                              
  END AS [StartDate],                              
  CASE                              
   WHEN (C.[YearOfEnd] <> 0 OR C.[MonthOfEnd] <> 0 OR C.[DayOfEnd] <> 0)                              
   THEN           
  CASE           
   WHEN C.[YearAsOf] = 0 AND C.[YearOfStart] = 0 AND C.[MonthAsOf] = 0 AND C.[MonthOfStart] = 0 AND C.[DayAsOf] = 0 AND C.[DayOfStart] = 0           
   THEN 'U' ELSE 'u'          
  END          
  + 'ntil ' +                              
  CASE WHEN C.[YearOfEnd] = 0                         
   THEN ''                         
   ELSE  CAST(C.[YearOfEnd] AS NVARCHAR) + CASE WHEN C.[MonthOfEnd] <> 0 OR C.[DayOfEnd] <> 0 THEN '/' ELSE '' END                         
  END                        
  +                        
  CASE WHEN C.[MonthOfEnd] = 0                         
   THEN ''                         
   ELSE CAST(C.MonthOfEnd AS NVARCHAR) + CASE WHEN C.[DayOfEnd] <> 0 THEN '/' ELSE '' END                         
  END                        
  +                        
  CASE WHEN C.[DayOfEnd] = 0                         
   THEN ''                         
   ELSE CAST(C.[DayOfEnd] AS NVARCHAR)                         
  END + ' '                        
   ELSE ''                              
  END AS [EndDate],                        
  RO.[RoleName] AS [Role],
  C.[Job] AS [Job],
  R.[RankName] AS [Rank],                              
  ISNULL(U.[UnitName],'Unknown') AS [Unit],                           
  ISNULL(O.[OrgLongName],'Unknown') AS [Organization],                            
  ISNULL(C.[Commentary],'Unknown') AS [Commentary],                            
  L.[LocationName] AS [Location],                  
  C.[CareerID] AS [CareerID],
  C.[Acting] AS [Acting],
  C.[Defected] AS [Defected]
 FROM [dbo].[PRF_Career] AS C                              
 LEFT JOIN [dbo].[PRF_Rank] AS R                              
 ON C.[RankID] = R.[RankID]                              
 AND R.[Archive] = 0                              
 LEFT JOIN [dbo].[PRF_Location] AS L           
 ON C.[LocationID] = L.[LocationID]                              
 AND L.[Archive] = 0                              
 LEFT JOIN [dbo].[PRF_Organization] AS O                              
 ON C.[OrganizationID] = O.[OrganizationID]                            
 AND O.[Archive] = 0                              
 LEFT JOIN [dbo].[PRF_Unit] AS U        
 ON C.[UnitID] = U.[UnitID]        
 AND U.[Archive] = 0        
 LEFT JOIN [dbo].[PRF_Role] AS RO      
 ON C.[RoleID] = RO.[RoleID]      
 AND RO.[Archive] = 0  
 WHERE C.[PersonID] = @PersonID                              
 AND C.[Archive] = 0                              
 AND C.[IsCurrentCareer] = 0                
 ORDER BY                    
 CASE WHEN C.[YearOfStart] = 0 
	THEN 
		CASE WHEN C.[YearOfEnd] = 0
			THEN C.[YearAsOf]
			ELSE C.[YearOfEnd]
		END
	ELSE 
		C.[YearOfStart] 
 END DESC,            
 CASE WHEN C.[MonthOfStart] = 0 
	THEN 
		CASE WHEN C.[MonthOfEnd] = 0
			THEN C.[MonthAsOf]
			ELSE C.[MonthOfEnd]
		END
	ELSE 
		C.[MonthOfStart] 
 END DESC,            
 CASE WHEN C.[DayOfStart] = 0 
	THEN 
		CASE WHEN C.[DayOfEnd] = 0
			THEN C.[DayAsOf]
			ELSE C.[DayOfEnd]
		END
	ELSE 
		C.[DayOfStart] 
 END DESC                     
END 