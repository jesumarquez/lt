/*
En la tabla par_enti_59_proveedor, agregar:

nombre_contacto
telefono_contacto
contacto_mail
FK a par_enti_05_mov_referencias
*/


USE [logictracker]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO


ALTER TABLE [dbo].[par.par_enti_59_proveedor]  
ADD parenti59_nombre_contacto VARCHAR(255) NULL 
GO
ALTER TABLE [dbo].[par.par_enti_59_proveedor]  
ADD parenti59_telefono_contacto VARCHAR(50) NULL
GO
ALTER TABLE [dbo].[par.par_enti_59_proveedor]  
ADD parenti59_contacto_mail VARCHAR(128) NULL 
GO
ALTER TABLE [dbo].[par.par_enti_59_proveedor]  
ADD rela_parenti05 INT NULL 
GO

ALTER TABLE [dbo].[par.par_enti_59_proveedor]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_59_proveedor_par.par_enti_05_mov_referencias] FOREIGN KEY([rela_parenti05])
REFERENCES [dbo].[par.par_enti_05_mov_referencias] ([id_parenti05])
GO
ALTER TABLE [dbo].[par.par_enti_59_proveedor] CHECK CONSTRAINT [FK_par.par_enti_59_proveedor_par.par_enti_05_mov_referencias]
GO