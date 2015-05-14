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
ALTER TABLE dbo.[ope.ope_tick_03_cab_viajedistri] ADD
	rela_parenti37 int NULL,
	rela_parenti99 int NULL,
	opetick03_motivo varchar(128) NULL,
	opetick03_comentario varchar(128) NULL
GO
ALTER TABLE dbo.[ope.ope_tick_03_cab_viajedistri] SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE [dbo].[ope.ope_tick_03_cab_viajedistri]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_03_cab_viajedistri_par.par_enti_37_cab_centro_costos] FOREIGN KEY([rela_parenti37])
REFERENCES [dbo].[par.par_enti_37_cab_centro_costos] ([id_parenti37])
GO

ALTER TABLE [dbo].[ope.ope_tick_03_cab_viajedistri] CHECK CONSTRAINT [FK_ope.ope_tick_03_cab_viajedistri_par.par_enti_37_cab_centro_costos]
GO

ALTER TABLE [dbo].[ope.ope_tick_03_cab_viajedistri]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_03_cab_viajedistri_par.par_enti_99_cab_subcentro_costos] FOREIGN KEY([rela_parenti99])
REFERENCES [dbo].[par.par_enti_99_cab_subcentro_costos] ([id_parenti99])
GO

ALTER TABLE [dbo].[ope.ope_tick_03_cab_viajedistri] CHECK CONSTRAINT [FK_ope.ope_tick_03_cab_viajedistri_par.par_enti_99_cab_subcentro_costos]
GO

COMMIT
