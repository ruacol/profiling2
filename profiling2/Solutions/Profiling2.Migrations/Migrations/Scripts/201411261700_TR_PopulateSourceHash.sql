SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [PRF_TR_Source_Created] ON [PRF_Source]    
AFTER INSERT  
AS    
BEGIN    
    UPDATE S
    SET S.[Hash] = master.sys.fn_repl_hash_binary(S.[FileData])
    FROM INSERTED I, PRF_Source S
    WHERE I.[SourceID] = S.[SourceID]
END
                