USE [logictracker]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ope.ope_tick_15_cab_stock_vehicular](
	[id_opetick15] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti01] [int] NOT NULL,
	[rela_parenti17] [int] NOT NULL,
	[rela_parenti89] [int] NOT NULL,
 CONSTRAINT [PK_ope.ope_tick_15_cab_stock_vehicular] PRIMARY KEY CLUSTERED 
(
	[id_opetick15] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ope.ope_tick_15_cab_stock_vehicular]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_15_cab_stock_vehicular_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[ope.ope_tick_15_cab_stock_vehicular] CHECK CONSTRAINT [FK_ope.ope_tick_15_cab_stock_vehicular_par.par_enti_01_cab_empresas]
GO

ALTER TABLE [dbo].[ope.ope_tick_15_cab_stock_vehicular]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_15_cab_stock_vehicular_par.par_enti_17_tbl_tipocoche] FOREIGN KEY([rela_parenti17])
REFERENCES [dbo].[par.par_enti_17_tbl_tipocoche] ([id_parenti17])
GO

ALTER TABLE [dbo].[ope.ope_tick_15_cab_stock_vehicular] CHECK CONSTRAINT [FK_ope.ope_tick_15_cab_stock_vehicular_par.par_enti_17_tbl_tipocoche]
GO

ALTER TABLE [dbo].[ope.ope_tick_15_cab_stock_vehicular]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_15_cab_stock_vehicular_par.par_enti_89_cab_zona] FOREIGN KEY([rela_parenti89])
REFERENCES [dbo].[par.par_enti_89_cab_zona] ([id_parenti89])
GO

ALTER TABLE [dbo].[ope.ope_tick_15_cab_stock_vehicular] CHECK CONSTRAINT [FK_ope.ope_tick_15_cab_stock_vehicular_par.par_enti_89_cab_zona]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ope.ope_tick_16_det_stock_vehicular](
	[id_opetick16] [int] IDENTITY(1,1) NOT NULL,
	[rela_opetick15] [int] NOT NULL,
	[rela_parenti03] [int] NOT NULL,
 CONSTRAINT [PK_ope.ope_tick_16_det_stock_vehicular] PRIMARY KEY CLUSTERED 
(
	[id_opetick16] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ope.ope_tick_16_det_stock_vehicular]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_16_det_stock_vehicular_ope.ope_tick_15_cab_stock_vehicular] FOREIGN KEY([rela_opetick15])
REFERENCES [dbo].[ope.ope_tick_15_cab_stock_vehicular] ([id_opetick15])
GO

ALTER TABLE [dbo].[ope.ope_tick_16_det_stock_vehicular] CHECK CONSTRAINT [FK_ope.ope_tick_16_det_stock_vehicular_ope.ope_tick_15_cab_stock_vehicular]
GO

ALTER TABLE [dbo].[ope.ope_tick_16_det_stock_vehicular]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_16_det_stock_vehicular_par.par_enti_03_cab_coches] FOREIGN KEY([rela_parenti03])
REFERENCES [dbo].[par.par_enti_03_cab_coches] ([id_parenti03])
GO

ALTER TABLE [dbo].[ope.ope_tick_16_det_stock_vehicular] CHECK CONSTRAINT [FK_ope.ope_tick_16_det_stock_vehicular_par.par_enti_03_cab_coches]
GO

CREATE SYNONYM [dbo].[opetick15] FOR [dbo].[ope.ope_tick_15_cab_stock_vehicular]
GO

CREATE SYNONYM [dbo].[opetick16] FOR [dbo].[ope.ope_tick_16_det_stock_vehicular]
GO

