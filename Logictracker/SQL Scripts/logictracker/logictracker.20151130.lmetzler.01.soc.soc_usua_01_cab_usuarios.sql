USE [logictracker]
GO

ALTER TABLE dbo.[soc.soc_usua_01_cab_usuarios] ADD
	rela_parenti09 int NULL
GO
ALTER TABLE dbo.[soc.soc_usua_01_cab_usuarios] SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE [dbo].[soc.soc_usua_01_cab_usuarios]  WITH CHECK ADD  CONSTRAINT [FK_soc.soc_usua_01_cab_usuarios_par.par_enti_09_cab_empleados] FOREIGN KEY([rela_parenti09])
REFERENCES [dbo].[par.par_enti_09_cab_empleados] ([id_parenti09])
GO

ALTER TABLE [dbo].[soc.soc_usua_01_cab_usuarios] CHECK CONSTRAINT [FK_soc.soc_usua_01_cab_usuarios_par.par_enti_09_cab_empleados]
GO

