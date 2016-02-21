SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PRF_SP_Suggestion_EventForPersonCount_NHibernate]                          
(                          
 @PersonID INT                          
)                          
AS                          
BEGIN                      
 DECLARE @Suggestions TABLE                        
 (                        
  [EventID] INT PRIMARY KEY,                         
  [EventName] NVARCHAR(MAX),                        
  [Score] INT,                        
  [Features] XML,                  
  [SuggestionReason] NVARCHAR(MAX)                        
 )                          
                              
 INSERT INTO @Suggestions ([EventID],[EventName],[Score],[Features],[SuggestionReason])                          
 EXEC [dbo].[PRF_SP_Suggestion_EventForPerson] @PersonID=@PersonID                              
                          
 SELECT COUNT(*) AS Count FROM @Suggestions                         
END