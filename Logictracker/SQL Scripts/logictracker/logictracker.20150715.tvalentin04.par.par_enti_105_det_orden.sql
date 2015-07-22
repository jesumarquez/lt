USE [logictracker]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

/*
Crear nueva tabla par_enti_##_det_orden con los campos:

FK par_enti_##_cab_orden
FK par_enti_58_insumos
Precio unitario
Cantidad
Descuento */
CREATE TABLE [dbo].[par.par_enti_105_det_orden](
	[id_parenti105] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti104] [int] NULL, 
	[rela_parenti58] [int] NULL, 
	[parenti105_precio_unitario] [float] NULL,
	[parenti105_cantidad] [float] NULL,
	[parenti105_descuento] [float] NULL,
 CONSTRAINT [parpar_enti_105_det_orden_PK] PRIMARY KEY CLUSTERED 
(
	[id_parenti105] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[par.par_enti_105_det_orden]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_105_det_orden.par_enti_104_cab_orden] FOREIGN KEY([rela_parenti104])
REFERENCES [dbo].[par.par_enti_104_cab_orden] ([id_parenti104])
GO
ALTER TABLE [dbo].[par.par_enti_105_det_orden] CHECK CONSTRAINT [FK_par.par_enti_105_det_orden.par_enti_104_cab_orden]
GO

ALTER TABLE [dbo].[par.par_enti_105_det_orden]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_105_det_orden.par_enti_58_insumos] FOREIGN KEY([rela_parenti58])
REFERENCES [dbo].[par.par_enti_58_insumos] ([id_parenti58])
GO
ALTER TABLE [dbo].[par.par_enti_105_det_orden] CHECK CONSTRAINT [FK_par.par_enti_105_det_orden.par_enti_58_insumos]
GO