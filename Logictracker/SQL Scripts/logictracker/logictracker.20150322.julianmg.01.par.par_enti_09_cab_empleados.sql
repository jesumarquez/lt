USE logictracker
GO

/****** Script para el comando SelectTopNRows de SSMS  ******/
ALTER TABLE [par.par_enti_09_cab_empleados]
ADD [rela_parenti08] [int] NULL

ALTER TABLE [dbo].[par.par_enti_09_cab_empleados]  
WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_09_cab_empleados.par_enti_08_cab_dispositivos_par] 
FOREIGN KEY([rela_parenti08])
REFERENCES [dbo].[par.par_enti_08_cab_dispositivos] ([id_parenti08])