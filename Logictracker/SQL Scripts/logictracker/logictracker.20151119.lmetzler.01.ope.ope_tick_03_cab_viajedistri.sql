USE [logictracker]
GO

ALTER TABLE dbo.[ope.ope_tick_03_cab_viajedistri] ADD
	rela_parenti07 int NULL
GO
ALTER TABLE dbo.[ope.ope_tick_03_cab_viajedistri] SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE [dbo].[ope.ope_tick_03_cab_viajedistri]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_03_cab_viajedistri_par.par_enti_07_tbl_transportistas] FOREIGN KEY([rela_parenti07])
REFERENCES [dbo].[par.par_enti_07_tbl_transportistas] ([id_parenti07])
GO

ALTER TABLE [dbo].[ope.ope_tick_03_cab_viajedistri] CHECK CONSTRAINT [FK_ope.ope_tick_03_cab_viajedistri_par.par_enti_07_tbl_transportistas]
GO