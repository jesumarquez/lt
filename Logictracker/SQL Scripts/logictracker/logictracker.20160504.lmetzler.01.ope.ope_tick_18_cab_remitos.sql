USE [logictracker]
GO

/****** Object:  Table [dbo].[ope.ope_tick_18_cab_remitos]    Script Date: 04/05/2016 17:22:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ope.ope_tick_18_cab_remitos](
	[id_opetick18] [int] IDENTITY(1,1) NOT NULL,
	[opetick18_codigo] [varchar](64) NOT NULL,
	[opetick18_fecha] [datetime] NOT NULL,
	[rela_parenti18] [int] NULL,
	[rela_parenti44] [int] NULL,
 CONSTRAINT [PK_ope.ope_tick_18_cab_remitos] PRIMARY KEY CLUSTERED 
(
	[id_opetick18] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ope.ope_tick_18_cab_remitos]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_18_cab_remitos_par.par_enti_18_cab_clientes] FOREIGN KEY([rela_parenti18])
REFERENCES [dbo].[par.par_enti_18_cab_clientes] ([id_parenti18])
GO

ALTER TABLE [dbo].[ope.ope_tick_18_cab_remitos] CHECK CONSTRAINT [FK_ope.ope_tick_18_cab_remitos_par.par_enti_18_cab_clientes]
GO

ALTER TABLE [dbo].[ope.ope_tick_18_cab_remitos]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_18_cab_remitos_par.par_enti_44_pto_entrega] FOREIGN KEY([rela_parenti44])
REFERENCES [dbo].[par.par_enti_44_pto_entrega] ([id_parenti44])
GO

ALTER TABLE [dbo].[ope.ope_tick_18_cab_remitos] CHECK CONSTRAINT [FK_ope.ope_tick_18_cab_remitos_par.par_enti_44_pto_entrega]
GO

CREATE SYNONYM [dbo].[opetick18] FOR [dbo].[ope.ope_tick_18_cab_remitos]
GO

