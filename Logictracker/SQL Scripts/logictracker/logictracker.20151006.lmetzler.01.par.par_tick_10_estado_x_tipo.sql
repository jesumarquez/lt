USE [logictracker]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[par.par_tick_10_estado_x_tipo](
	[id_partick10] [int] IDENTITY(1,1) NOT NULL,
	[rela_partick08] [int] NOT NULL,
	[rela_partick09] [int] NOT NULL,
 CONSTRAINT [PK_par.par_tick_10_estado_x_tipo] PRIMARY KEY CLUSTERED 
(
	[id_partick10] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[par.par_tick_10_estado_x_tipo]  WITH CHECK ADD  CONSTRAINT [FK_par.par_tick_10_estado_x_tipo_par.par_tick_08_tbl_estado_logistico] FOREIGN KEY([rela_partick08])
REFERENCES [dbo].[par.par_tick_08_tbl_estado_logistico] ([id_partick08])
GO

ALTER TABLE [dbo].[par.par_tick_10_estado_x_tipo] CHECK CONSTRAINT [FK_par.par_tick_10_estado_x_tipo_par.par_tick_08_tbl_estado_logistico]
GO

ALTER TABLE [dbo].[par.par_tick_10_estado_x_tipo]  WITH CHECK ADD  CONSTRAINT [FK_par.par_tick_10_estado_x_tipo_par.par_tick_09_cab_tipo_ciclo_logistico] FOREIGN KEY([rela_partick09])
REFERENCES [dbo].[par.par_tick_09_cab_tipo_ciclo_logistico] ([id_partick09])
GO

ALTER TABLE [dbo].[par.par_tick_10_estado_x_tipo] CHECK CONSTRAINT [FK_par.par_tick_10_estado_x_tipo_par.par_tick_09_cab_tipo_ciclo_logistico]
GO

CREATE SYNONYM [dbo].[partick10] FOR [dbo].[par.par_tick_10_estado_x_tipo]
GO