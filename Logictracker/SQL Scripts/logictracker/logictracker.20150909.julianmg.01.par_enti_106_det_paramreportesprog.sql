GO
PRINT N'Starting rebuilding table [dbo].[par.par_enti_106_det_paramreportesprog]...';

GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_par.par_enti_106_det_paramreportesprog](
	[id_parenti106] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti56] [int] NULL,
	[parenti106_parametro] [varchar](20) NOT NULL,
	[parenti106_valor] [int] NOT NULL,
 CONSTRAINT [tmp_ms_xx_constraint_PK_par.par_enti_106_det_paramreportesprog] PRIMARY KEY CLUSTERED 
([id_parenti106] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

IF EXISTS (SELECT TOP 1 1 FROM  [dbo].[par.par_enti_106_det_paramreportesprog])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_par.par_enti_106_det_paramreportesprog] ON;
        INSERT INTO [dbo].[tmp_ms_xx_par.par_enti_106_det_paramreportesprog] ([id_parenti106], [rela_parenti56], [parenti106_parametro], [parenti106_valor])
        SELECT   [id_parenti106],
                 [rela_parenti56],
                 [parenti106_parametro],
                 [parenti106_valor]
        FROM     [dbo].[par.par_enti_106_det_paramreportesprog]
        ORDER BY [id_parenti106] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_par.par_enti_106_det_paramreportesprog] OFF;
    END

DROP TABLE [dbo].[par.par_enti_106_det_paramreportesprog];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_par.par_enti_106_det_paramreportesprog]', N'par.par_enti_106_det_paramreportesprog';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_par.par_enti_106_det_paramreportesprog]', N'PK_par.par_enti_106_det_paramreportesprog', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

GO
