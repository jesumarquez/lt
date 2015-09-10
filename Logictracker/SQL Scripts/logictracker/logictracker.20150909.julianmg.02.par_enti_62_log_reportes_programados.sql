GO
PRINT N'Starting rebuilding table [dbo].[par.par_enti_106_det_paramreportesprog]...';

GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_par.par_enti_62_log_reportes_programados](
	[id_parenti62] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti56] [int] NOT NULL,
	[parenti62_inicio] [datetime] NOT NULL,
	[parenti62_fin] [datetime] NULL,
	[parenti62_cantidad_filas] [int] NOT NULL,
	[parenti62_error] [bit] NOT NULL,
 CONSTRAINT [PK_par.par_enti_62_log_reportes_programados] PRIMARY KEY CLUSTERED 
(
	[id_parenti62] ASC
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
