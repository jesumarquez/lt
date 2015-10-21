USE [logictracker]
GO

/****** Object:  Table [dbo].[ope.ope_tick_09_estado_x_viaje]    Script Date: 21/10/2015 10:55:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ope.ope_tick_09_estado_x_viaje](
	[id_opetick09] [int] IDENTITY(1,1) NOT NULL,
	[rela_opetick03] [int] NOT NULL,
	[rela_partick08] [int] NOT NULL,
	[opetick09_inicio] [datetime] NULL,
	[opetick09_fin] [datetime] NULL,
 CONSTRAINT [PK_ope.ope_tick_09_estado_x_ciclo] PRIMARY KEY CLUSTERED 
(
	[id_opetick09] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ope.ope_tick_09_estado_x_viaje]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_09_estado_x_ciclo_ope.ope_tick_03_cab_viajedistri] FOREIGN KEY([rela_opetick03])
REFERENCES [dbo].[ope.ope_tick_03_cab_viajedistri] ([id_opetick03])
GO

ALTER TABLE [dbo].[ope.ope_tick_09_estado_x_viaje] CHECK CONSTRAINT [FK_ope.ope_tick_09_estado_x_ciclo_ope.ope_tick_03_cab_viajedistri]
GO

ALTER TABLE [dbo].[ope.ope_tick_09_estado_x_viaje]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_09_estado_x_ciclo_par.par_tick_08_tbl_estado_logistico] FOREIGN KEY([rela_partick08])
REFERENCES [dbo].[par.par_tick_08_tbl_estado_logistico] ([id_partick08])
GO

ALTER TABLE [dbo].[ope.ope_tick_09_estado_x_viaje] CHECK CONSTRAINT [FK_ope.ope_tick_09_estado_x_ciclo_par.par_tick_08_tbl_estado_logistico]
GO

CREATE SYNONYM [dbo].[opetick09] FOR [dbo].[ope.ope_tick_09_estado_x_viaje]
GO


