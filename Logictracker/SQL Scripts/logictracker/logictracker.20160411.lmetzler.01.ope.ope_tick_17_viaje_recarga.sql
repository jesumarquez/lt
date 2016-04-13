USE [logictracker]
GO

/****** Object:  Table [dbo].[ope.ope_tick_17_viaje_recarga]    Script Date: 11/04/2016 11:20:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ope.ope_tick_17_viaje_recarga](
	[id_opetick17] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti01] [int] NOT NULL,
	[rela_parenti02] [int] NOT NULL,
	[rela_parenti03] [int] NOT NULL,
	[opetick17_interno] [varchar](64) NOT NULL,
	[opetick17_patente] [varchar](64) NOT NULL,
	[opetick17_fecha] [datetime] NOT NULL,
	[opetick17_accion] [varchar](64) NOT NULL,
	[opetick17_inicio] [datetime] NOT NULL,
	[opetick17_fin] [datetime] NOT NULL,
	[opetick17_duracion] [float] NOT NULL,
 CONSTRAINT [PK_ope.ope_tick_17_viaje_recarga] PRIMARY KEY CLUSTERED 
(
	[id_opetick17] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ope.ope_tick_17_viaje_recarga]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_17_viaje_recarga_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[ope.ope_tick_17_viaje_recarga] CHECK CONSTRAINT [FK_ope.ope_tick_17_viaje_recarga_par.par_enti_01_cab_empresas]
GO

ALTER TABLE [dbo].[ope.ope_tick_17_viaje_recarga]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_17_viaje_recarga_par.par_enti_02_det_lineas] FOREIGN KEY([rela_parenti02])
REFERENCES [dbo].[par.par_enti_02_det_lineas] ([id_parenti02])
GO

ALTER TABLE [dbo].[ope.ope_tick_17_viaje_recarga] CHECK CONSTRAINT [FK_ope.ope_tick_17_viaje_recarga_par.par_enti_02_det_lineas]
GO

ALTER TABLE [dbo].[ope.ope_tick_17_viaje_recarga]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_17_viaje_recarga_par.par_enti_03_cab_coches] FOREIGN KEY([rela_parenti03])
REFERENCES [dbo].[par.par_enti_03_cab_coches] ([id_parenti03])
GO

ALTER TABLE [dbo].[ope.ope_tick_17_viaje_recarga] CHECK CONSTRAINT [FK_ope.ope_tick_17_viaje_recarga_par.par_enti_03_cab_coches]
GO

CREATE SYNONYM [dbo].[opetick17] FOR [dbo].[ope.ope_tick_17_viaje_recarga]
GO

