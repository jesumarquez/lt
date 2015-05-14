GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ope.ope_tick_06_tick_datamart](
	[id_opetick06] [int] IDENTITY(1,1) NOT NULL,
	[opetick06_fecha] [datetime] NOT NULL,
	[opetick06_ruta] [varchar](50) NOT NULL,
	[opetick06_orden] [int] NOT NULL,
	[opetick06_entrega] [varchar](50) NOT NULL,
	[rela_parenti03] [int] NOT NULL,
	[rela_parenti44] [int] NOT NULL,
	[opetick06_estado] [varchar](50) NOT NULL,
	[opetick06_km] [float] NOT NULL,
	[opetick06_recorrido] [time](7) NOT NULL,
	[opetick06_tiempo_entrega] [time](7) NOT NULL,
	[opetick06_entrada] [datetime] NULL,
	[opetick06_salida] [datetime] NULL,
	[opetick06_manual] [datetime] NULL,
 CONSTRAINT [PK_ope.ope_tick_06_tick_datamart] PRIMARY KEY CLUSTERED 
(
	[id_opetick06] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ope.ope_tick_06_tick_datamart]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_06_tick_datamart_par.par_enti_03_cab_coches] FOREIGN KEY([rela_parenti03])
REFERENCES [dbo].[par.par_enti_03_cab_coches] ([id_parenti03])
GO

ALTER TABLE [dbo].[ope.ope_tick_06_tick_datamart] CHECK CONSTRAINT [FK_ope.ope_tick_06_tick_datamart_par.par_enti_03_cab_coches]
GO

ALTER TABLE [dbo].[ope.ope_tick_06_tick_datamart]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_06_tick_datamart_par.par_enti_44_pto_entrega] FOREIGN KEY([rela_parenti44])
REFERENCES [dbo].[par.par_enti_44_pto_entrega] ([id_parenti44])
GO

ALTER TABLE [dbo].[ope.ope_tick_06_tick_datamart] CHECK CONSTRAINT [FK_ope.ope_tick_06_tick_datamart_par.par_enti_44_pto_entrega]
GO

CREATE SYNONYM [dbo].[opetick06] FOR [dbo].[ope.ope_tick_06_tick_datamart]
GO

