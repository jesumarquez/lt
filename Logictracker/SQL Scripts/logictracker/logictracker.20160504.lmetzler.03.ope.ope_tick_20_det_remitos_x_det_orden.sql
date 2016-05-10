USE [logictracker]
GO

/****** Object:  Table [dbo].[ope.ope_tick_20_det_remitos_x_det_orden]    Script Date: 04/05/2016 17:24:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ope.ope_tick_20_det_remitos_x_det_orden](
	[id_opetick20] [int] IDENTITY(1,1) NOT NULL,
	[rela_opetick19] [int] NOT NULL,
	[rela_parenti105] [int] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ope.ope_tick_20_det_remitos_x_det_orden]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_20_det_remitos_x_det_orden_ope.ope_tick_19_det_remitos] FOREIGN KEY([rela_opetick19])
REFERENCES [dbo].[ope.ope_tick_19_det_remitos] ([id_opetick19])
GO

ALTER TABLE [dbo].[ope.ope_tick_20_det_remitos_x_det_orden] CHECK CONSTRAINT [FK_ope.ope_tick_20_det_remitos_x_det_orden_ope.ope_tick_19_det_remitos]
GO

ALTER TABLE [dbo].[ope.ope_tick_20_det_remitos_x_det_orden]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_20_det_remitos_x_det_orden_par.par_enti_105_det_orden] FOREIGN KEY([rela_parenti105])
REFERENCES [dbo].[par.par_enti_105_det_orden] ([id_parenti105])
GO

ALTER TABLE [dbo].[ope.ope_tick_20_det_remitos_x_det_orden] CHECK CONSTRAINT [FK_ope.ope_tick_20_det_remitos_x_det_orden_par.par_enti_105_det_orden]
GO

CREATE SYNONYM [dbo].[opetick20] FOR [dbo].[ope.ope_tick_20_det_remitos_x_det_orden]
GO


