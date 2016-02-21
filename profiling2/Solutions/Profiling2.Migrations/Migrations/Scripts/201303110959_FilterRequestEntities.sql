SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SCR_SP_RequestEntityGetForFk]    
AS    
BEGIN    
 SELECT    
  [RequestEntityID] AS [Id],    
  [RequestEntityName] AS [Value]    
 FROM [dbo].[SCR_RequestEntity]    
 WHERE [Archive] = 0
 AND [RequestEntityName] NOT IN ('Unknown', 'UNDP')
END  