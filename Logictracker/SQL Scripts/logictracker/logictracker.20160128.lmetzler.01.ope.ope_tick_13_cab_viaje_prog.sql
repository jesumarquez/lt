USE [logictracker]
GO

/****** Object:  Table [dbo].[ope.ope_tick_13_cab_viaje_prog]    Script Date: 28/01/2016 11:47:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ope.ope_tick_13_cab_viaje_prog](
	[id_opetick13] [int] IDENTITY(1,1) NOT NULL,
	[opetick13_codigo] [varchar](32) NOT NULL,
	[opetick13_horas] [float] NULL,
	[opetick13_km] [float] NULL,
	[rela_parenti01] [int] NOT NULL,
	[rela_parenti07] [int] NULL,
	[rela_parenti17] [int] NULL,
 CONSTRAINT [PK_ope.ope_tick_13_cab_viaje_prog] PRIMARY KEY CLUSTERED 
(
	[id_opetick13] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ope.ope_tick_13_cab_viaje_prog]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_13_cab_viaje_prog_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[ope.ope_tick_13_cab_viaje_prog] CHECK CONSTRAINT [FK_ope.ope_tick_13_cab_viaje_prog_par.par_enti_01_cab_empresas]
GO

ALTER TABLE [dbo].[ope.ope_tick_13_cab_viaje_prog]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_13_cab_viaje_prog_par.par_enti_07_tbl_transportistas] FOREIGN KEY([rela_parenti07])
REFERENCES [dbo].[par.par_enti_07_tbl_transportistas] ([id_parenti07])
GO

ALTER TABLE [dbo].[ope.ope_tick_13_cab_viaje_prog] CHECK CONSTRAINT [FK_ope.ope_tick_13_cab_viaje_prog_par.par_enti_07_tbl_transportistas]
GO

ALTER TABLE [dbo].[ope.ope_tick_13_cab_viaje_prog]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_13_cab_viaje_prog_par.par_enti_17_tbl_tipocoche] FOREIGN KEY([rela_parenti17])
REFERENCES [dbo].[par.par_enti_17_tbl_tipocoche] ([id_parenti17])
GO

ALTER TABLE [dbo].[ope.ope_tick_13_cab_viaje_prog] CHECK CONSTRAINT [FK_ope.ope_tick_13_cab_viaje_prog_par.par_enti_17_tbl_tipocoche]
GO

CREATE SYNONYM [dbo].[opetick13] FOR [dbo].[ope.ope_tick_13_cab_viaje_prog]
GO

