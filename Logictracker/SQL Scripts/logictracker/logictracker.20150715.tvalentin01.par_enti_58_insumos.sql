/*En la tabla par_enti_58_insumos, agregar:

FK a par_enti_59_proveedor
rela_parenti59 not null
Volumen
Peso

*/

USE [logictracker]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO


ALTER TABLE [dbo].[par.par_enti_58_insumos] 
ADD rela_parenti59 int NULL 
GO
ALTER TABLE [dbo].[par.par_enti_58_insumos] 
ADD parenti58_volumen float NULL DEFAULT 0
GO
ALTER TABLE [dbo].[par.par_enti_58_insumos] 
ADD parenti58_peso float NULL DEFAULT 0
GO

ALTER TABLE [dbo].[par.par_enti_58_insumos]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_58_insumos_par.par_enti_59_proveedor] FOREIGN KEY([rela_parenti59])
REFERENCES [dbo].[par.par_enti_59_proveedor] ([id_parenti59])
GO
ALTER TABLE [dbo].[par.par_enti_58_insumos] CHECK CONSTRAINT [FK_par.par_enti_58_insumos_par.par_enti_59_proveedor]
GO
