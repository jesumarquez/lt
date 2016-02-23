USE [logictracker]

GO
ALTER TABLE dbo.[ope.ope_tick_03_cab_viajedistri] ADD
	rela_parenti17 int NULL
GO

ALTER TABLE [dbo].[ope.ope_tick_03_cab_viajedistri]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_03_cab_viajedistri_par.par_enti_17_tbl_tipocoche] FOREIGN KEY([rela_parenti17])
REFERENCES [dbo].[par.par_enti_17_tbl_tipocoche] ([id_parenti17])
GO

ALTER TABLE [dbo].[ope.ope_tick_03_cab_viajedistri] CHECK CONSTRAINT [FK_ope.ope_tick_03_cab_viajedistri_par.par_enti_17_tbl_tipocoche]
GO

