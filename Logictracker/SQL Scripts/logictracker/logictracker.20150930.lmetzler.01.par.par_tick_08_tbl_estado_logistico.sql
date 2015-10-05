USE [logictracker]
GO

CREATE TABLE [dbo].[par.par_tick_08_tbl_estado_logistico](
	[id_partick08] [int] IDENTITY(1,1) NOT NULL,
	[partick08_descripcion] [varchar](255) NOT NULL,
	[partick08_demora] [smallint] NULL,
	[rela_pareven01_inicio] [int] NOT NULL,
	[rela_parenti01] [int] NOT NULL,
	[rela_parenti11] [int] NULL,
	[partick08_baja] [bit] NOT NULL,
	[partick08_productivo] [bit] NULL,
	[rela_pareven01_fin] [int] NULL,
	[rela_parenti10_inicio] [int] NULL,
	[rela_parenti10_fin] [int] NULL,
 CONSTRAINT [PK_par.par_tick_08_tbl_estado_logistico] PRIMARY KEY CLUSTERED 
(
	[id_partick08] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[par.par_tick_08_tbl_estado_logistico]  WITH CHECK ADD  CONSTRAINT [FK_par.par_tick_08_tbl_estado_logistico_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[par.par_tick_08_tbl_estado_logistico] CHECK CONSTRAINT [FK_par.par_tick_08_tbl_estado_logistico_par.par_enti_01_cab_empresas]
GO

ALTER TABLE [dbo].[par.par_tick_08_tbl_estado_logistico]  WITH CHECK ADD  CONSTRAINT [FK_par.par_tick_08_tbl_estado_logistico_par.par_enti_10_tbl_tiporeferencia] FOREIGN KEY([rela_parenti10_inicio])
REFERENCES [dbo].[par.par_enti_10_tbl_tiporeferencia] ([id_parenti10])
GO

ALTER TABLE [dbo].[par.par_tick_08_tbl_estado_logistico] CHECK CONSTRAINT [FK_par.par_tick_08_tbl_estado_logistico_par.par_enti_10_tbl_tiporeferencia]
GO

ALTER TABLE [dbo].[par.par_tick_08_tbl_estado_logistico]  WITH CHECK ADD  CONSTRAINT [FK_par.par_tick_08_tbl_estado_logistico_par.par_enti_10_tbl_tiporeferencia1] FOREIGN KEY([rela_parenti10_fin])
REFERENCES [dbo].[par.par_enti_10_tbl_tiporeferencia] ([id_parenti10])
GO

ALTER TABLE [dbo].[par.par_tick_08_tbl_estado_logistico] CHECK CONSTRAINT [FK_par.par_tick_08_tbl_estado_logistico_par.par_enti_10_tbl_tiporeferencia1]
GO

ALTER TABLE [dbo].[par.par_tick_08_tbl_estado_logistico]  WITH CHECK ADD  CONSTRAINT [FK_par.par_tick_08_tbl_estado_logistico_par.par_enti_11_tbl_icono] FOREIGN KEY([rela_parenti11])
REFERENCES [dbo].[par.par_enti_11_tbl_icono] ([id_parenti11])
GO

ALTER TABLE [dbo].[par.par_tick_08_tbl_estado_logistico] CHECK CONSTRAINT [FK_par.par_tick_08_tbl_estado_logistico_par.par_enti_11_tbl_icono]
GO

ALTER TABLE [dbo].[par.par_tick_08_tbl_estado_logistico]  WITH CHECK ADD  CONSTRAINT [FK_par.par_tick_08_tbl_estado_logistico_par.par_even_01_tbl_mensajes] FOREIGN KEY([rela_pareven01_inicio])
REFERENCES [dbo].[par.par_even_01_tbl_mensajes] ([id_pareven01])
GO

ALTER TABLE [dbo].[par.par_tick_08_tbl_estado_logistico] CHECK CONSTRAINT [FK_par.par_tick_08_tbl_estado_logistico_par.par_even_01_tbl_mensajes]
GO

ALTER TABLE [dbo].[par.par_tick_08_tbl_estado_logistico]  WITH CHECK ADD  CONSTRAINT [FK_par.par_tick_08_tbl_estado_logistico_par.par_even_01_tbl_mensajes1] FOREIGN KEY([rela_pareven01_fin])
REFERENCES [dbo].[par.par_even_01_tbl_mensajes] ([id_pareven01])
GO

ALTER TABLE [dbo].[par.par_tick_08_tbl_estado_logistico] CHECK CONSTRAINT [FK_par.par_tick_08_tbl_estado_logistico_par.par_even_01_tbl_mensajes1]
GO

CREATE SYNONYM [dbo].[partick08] FOR [dbo].[par.par_tick_08_tbl_estado_logistico]
GO
