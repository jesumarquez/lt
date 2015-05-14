/****** Object:  Index [IX_Distrito_Base_Transportista_Estado]    Script Date: 11/10/2014 7:01:53 PM ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_03_cab_coches]') AND name = N'IX_Distrito_Base_Transportista_Estado')
DROP INDEX [IX_Distrito_Base_Transportista_Estado] ON [dbo].[par.par_enti_03_cab_coches]
GO

/****** Object:  Index [IX_Distrito_Base_Tipo_Transportista_Estado]    Script Date: 11/10/2014 7:01:53 PM ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_03_cab_coches]') AND name = N'IX_Distrito_Base_Tipo_Transportista_Estado')
DROP INDEX [IX_Distrito_Base_Tipo_Transportista_Estado] ON [dbo].[par.par_enti_03_cab_coches]
GO

/****** Object:  Index [IX_Distrito_Base_Tipo_Estado]    Script Date: 11/10/2014 7:01:53 PM ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_03_cab_coches]') AND name = N'IX_Distrito_Base_Tipo_Estado')
DROP INDEX [IX_Distrito_Base_Tipo_Estado] ON [dbo].[par.par_enti_03_cab_coches]
GO


