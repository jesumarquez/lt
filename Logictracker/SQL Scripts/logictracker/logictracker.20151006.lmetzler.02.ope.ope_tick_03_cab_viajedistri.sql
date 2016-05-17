USE [logictracker]
GO

ALTER TABLE dbo.[ope.ope_tick_03_cab_viajedistri] ADD
	rela_partick09 int NULL
GO
ALTER TABLE dbo.[ope.ope_tick_03_cab_viajedistri] SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE [dbo].[ope.ope_tick_03_cab_viajedistri]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_03_cab_viajedistri_par.par_tick_09_cab_tipo_ciclo_logistico] FOREIGN KEY([rela_partick09])
REFERENCES [dbo].[par.par_tick_09_cab_tipo_ciclo_logistico] ([id_partick09])
GO

ALTER TABLE [dbo].[ope.ope_tick_03_cab_viajedistri] CHECK CONSTRAINT [FK_ope.ope_tick_03_cab_viajedistri_par.par_tick_09_cab_tipo_ciclo_logistico]
GO

