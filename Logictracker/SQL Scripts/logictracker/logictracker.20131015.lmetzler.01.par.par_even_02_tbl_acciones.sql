BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.[par.par_even_02_tbl_acciones] ADD
	rela_parenti04 int NULL,
	rela_parenti37 int NULL,
	pareven02_reporta_rela04 bit NULL,
	pareven02_reporta_rela37 bit NULL
GO
ALTER TABLE dbo.[par.par_even_02_tbl_acciones] SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE [dbo].[par.par_even_02_tbl_acciones]  WITH CHECK ADD  CONSTRAINT [FK_par.par_even_02_tbl_acciones_par.par_enti_04_cab_departamento] FOREIGN KEY([rela_parenti04])
REFERENCES [dbo].[par.par_enti_04_cab_departamento] ([id_parenti04])
GO

ALTER TABLE [dbo].[par.par_even_02_tbl_acciones] CHECK CONSTRAINT [FK_par.par_even_02_tbl_acciones_par.par_enti_04_cab_departamento]
GO

ALTER TABLE [dbo].[par.par_even_02_tbl_acciones]  WITH CHECK ADD  CONSTRAINT [FK_par.par_even_02_tbl_acciones_par.par_enti_37_cab_centro_costos] FOREIGN KEY([rela_parenti37])
REFERENCES [dbo].[par.par_enti_37_cab_centro_costos] ([id_parenti37])
GO

ALTER TABLE [dbo].[par.par_even_02_tbl_acciones] CHECK CONSTRAINT [FK_par.par_even_02_tbl_acciones_par.par_enti_37_cab_centro_costos]
GO

COMMIT
