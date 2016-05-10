USE [logictracker]
GO

/****** Object:  Table [dbo].[ope.ope_tick_21_remito_x_entrega]    Script Date: 04/05/2016 17:25:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ope.ope_tick_21_remito_x_entrega](
	[id_opetick21] [int] IDENTITY(1,1) NOT NULL,
	[rela_opetick04] [int] NOT NULL,
	[rela_opetick18] [int] NOT NULL,
 CONSTRAINT [PK_ope.ope_tick_21_remito_x_entrega] PRIMARY KEY CLUSTERED 
(
	[id_opetick21] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ope.ope_tick_21_remito_x_entrega]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_21_remito_x_entrega_ope.ope_tick_04_det_entregadistri] FOREIGN KEY([rela_opetick04])
REFERENCES [dbo].[ope.ope_tick_04_det_entregadistri] ([id_opetick04])
GO

ALTER TABLE [dbo].[ope.ope_tick_21_remito_x_entrega] CHECK CONSTRAINT [FK_ope.ope_tick_21_remito_x_entrega_ope.ope_tick_04_det_entregadistri]
GO

ALTER TABLE [dbo].[ope.ope_tick_21_remito_x_entrega]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_21_remito_x_entrega_ope.ope_tick_18_cab_remitos] FOREIGN KEY([rela_opetick18])
REFERENCES [dbo].[ope.ope_tick_18_cab_remitos] ([id_opetick18])
GO

ALTER TABLE [dbo].[ope.ope_tick_21_remito_x_entrega] CHECK CONSTRAINT [FK_ope.ope_tick_21_remito_x_entrega_ope.ope_tick_18_cab_remitos]
GO

CREATE SYNONYM [dbo].[opetick21] FOR [dbo].[ope.ope_tick_21_remito_x_entrega]
GO


