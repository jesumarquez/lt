USE [logictracker]
GO

ALTER TABLE dbo.[ope.ope_posi_10_datamart_tramos] ADD
	rela_parenti01 int NULL,
	rela_parenti02 int NULL
GO
ALTER TABLE [dbo].[ope.ope_posi_10_datamart_tramos]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_posi_10_datamart_tramos_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[ope.ope_posi_10_datamart_tramos] CHECK CONSTRAINT [FK_ope.ope_posi_10_datamart_tramos_par.par_enti_01_cab_empresas]
GO

ALTER TABLE [dbo].[ope.ope_posi_10_datamart_tramos]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_posi_10_datamart_tramos_par.par_enti_02_det_lineas] FOREIGN KEY([rela_parenti02])
REFERENCES [dbo].[par.par_enti_02_det_lineas] ([id_parenti02])
GO

ALTER TABLE [dbo].[ope.ope_posi_10_datamart_tramos] CHECK CONSTRAINT [FK_ope.ope_posi_10_datamart_tramos_par.par_enti_02_det_lineas]
GO



