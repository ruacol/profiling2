﻿SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[PRF_TR_PersonResponsibility_ProfileLastModified_I_U] ON [dbo].[PRF_PersonResponsibility]    
AFTER INSERT,UPDATE    
AS    
BEGIN
  SET NOCOUNT ON

	UPDATE P
	SET P.[ProfileLastModified] = GETDATE()
	FROM [dbo].[PRF_Person] AS P
	INNER JOIN
		(
			SELECT DISTINCT I.[PersonID] FROM INSERTED I
		) AS I
	ON P.[PersonID] = I.[PersonID]

  SET NOCOUNT OFF
END  