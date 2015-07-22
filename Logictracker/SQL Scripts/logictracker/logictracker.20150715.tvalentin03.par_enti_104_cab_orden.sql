/*
Crear nueva tabla par_enti_##_cab_orden con los campos:
Código de pedido
Fecha de alta
Fecha de pedido
Fecha de entrega
FK par_enti_01_cab_empresas
FK par_enti_02_det_lineas
FK par_enti_44_pto_entrega
FK par_enti_09_cab_empleados
Inicio ventana horaria
Fin ventana horaria
*/

USE [logictracker]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO


CREATE TABLE [dbo].[par.par_enti_104_cab_orden](
	[id_parenti104] [int] IDENTITY(1,1) NOT NULL,
	[parenti104_codigo_pedido] [int] NULL,
	[parenti104_fecha_alta] [datetime] NULL,
	[parenti104_fecha_pedido] [datetime] NULL,
	[parenti104_fecha_entrega] [datetime] NULL,
	[rela_parenti01] [int] NULL, 
	[rela_parenti02] [int] NULL,
	[rela_parenti44] [int] NULL,
	[rela_parenti09] [int] NULL,
	[parenti104_inicio_ventana_horaria] [time](7) NULL,
	[parenti104_fin_ventana_horaria] [time](7) NULL,
 CONSTRAINT [par.par_enti_104_cab_orden_PK] PRIMARY KEY CLUSTERED 
(
	[id_parenti104] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[par.par_enti_104_cab_orden]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_104_cab_orden.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO
ALTER TABLE [dbo].[par.par_enti_104_cab_orden] CHECK CONSTRAINT [FK_par.par_enti_104_cab_orden.par_enti_01_cab_empresas]
GO

ALTER TABLE [dbo].[par.par_enti_104_cab_orden]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_104_cab_orden.par_enti_02_det_lineas] FOREIGN KEY([rela_parenti02])
REFERENCES [dbo].[par.par_enti_02_det_lineas] ([id_parenti02])
GO
ALTER TABLE [dbo].[par.par_enti_104_cab_orden] CHECK CONSTRAINT [FK_par.par_enti_104_cab_orden.par_enti_02_det_lineas]
GO

ALTER TABLE [dbo].[par.par_enti_104_cab_orden]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_104_cab_orden.par_enti_44_pto_entrega] FOREIGN KEY([rela_parenti44])
REFERENCES [dbo].[par.par_enti_44_pto_entrega] ([id_parenti44])
GO
ALTER TABLE [dbo].[par.par_enti_104_cab_orden] CHECK CONSTRAINT [FK_par.par_enti_104_cab_orden.par_enti_44_pto_entrega]
GO

ALTER TABLE [dbo].[par.par_enti_104_cab_orden]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_104_cab_orden.par_enti_09_cab_empleados] FOREIGN KEY([rela_parenti09])
REFERENCES [dbo].[par.par_enti_09_cab_empleados] ([id_parenti09])
GO
ALTER TABLE [dbo].[par.par_enti_104_cab_orden] CHECK CONSTRAINT [FK_par.par_enti_104_cab_orden.par_enti_09_cab_empleados]
GO