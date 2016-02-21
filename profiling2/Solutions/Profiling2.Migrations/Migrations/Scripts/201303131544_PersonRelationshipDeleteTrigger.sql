SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[PRF_TR_PersonRelationship_ProfileLastModified_D] ON [dbo].[PRF_PersonRelationship]    
AFTER DELETE    
AS    
BEGIN    
  SET NOCOUNT ON
	UPDATE P
	SET P.[ProfileLastModified] = GETDATE()    
	FROM [dbo].[PRF_Person] AS P
	INNER JOIN
		(
			SELECT DISTINCT D.[SubjectPersonID] FROM DELETED D
		) AS D1
	ON P.[PersonID] = D1.[SubjectPersonID]

	UPDATE P
	SET P.[ProfileLastModified] = GETDATE()    
	FROM [dbo].[PRF_Person] AS P
	INNER JOIN
		(
			SELECT DISTINCT D.[ObjectPersonID] FROM DELETED D
		) AS D2
	ON P.[PersonID] = D2.[ObjectPersonID]
  SET NOCOUNT OFF
END  