SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[PRF_TR_PersonResponsibility_ProfileLastModified_D] ON [dbo].[PRF_PersonResponsibility]    
AFTER DELETE    
AS    
BEGIN
  SET NOCOUNT ON

	UPDATE P
	SET P.[ProfileLastModified] = GETDATE()    
	FROM [dbo].[PRF_Person] AS P
	INNER JOIN
		(
			SELECT DISTINCT D.[PersonID] FROM DELETED D
		) AS D
	ON P.[PersonID] = D.[PersonID]

  SET NOCOUNT OFF
END 