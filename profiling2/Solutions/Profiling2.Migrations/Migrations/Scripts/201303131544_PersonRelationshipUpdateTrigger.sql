SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[PRF_TR_PersonRelationship_ProfileLastModified_I_U] ON [dbo].[PRF_PersonRelationship]    
AFTER INSERT,UPDATE    
AS    
BEGIN 
  SET NOCOUNT ON   
	UPDATE P
	SET P.[ProfileLastModified] = GETDATE()
	FROM [dbo].[PRF_Person] AS P
	INNER JOIN
		(
			SELECT DISTINCT I.[SubjectPersonID] FROM INSERTED I
		) AS I1
	ON P.[PersonID] = I1.[SubjectPersonID]

	UPDATE P
	SET P.[ProfileLastModified] = GETDATE()
	FROM [dbo].[PRF_Person] AS P
	INNER JOIN
		(
			SELECT DISTINCT I.[ObjectPersonID] FROM INSERTED I
		) AS I2
	ON P.[PersonID] = I2.[ObjectPersonID]
  SET NOCOUNT OFF
END  