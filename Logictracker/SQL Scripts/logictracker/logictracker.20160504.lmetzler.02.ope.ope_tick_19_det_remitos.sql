USE [logictracker]
GO

/****** Object:  Table [dbo].[ope.ope_tick_19_det_remitos]    Script Date: 04/05/2016 17:23:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ope.ope_tick_19_det_remitos](
	[id_opetick19] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti58] [int] NOT NULL,
	[opetick19_cantidad] [int] NOT NULL,
	[rela_opetick18] [int] NOT NULL,
 CONSTRAINT [PK_ope.ope_tick_19_det_remitos] PRIMARY KEY CLUSTERED 
(
	[id_opetick19] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ope.ope_tick_19_det_remitos]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_19_det_remitos_ope.ope_tick_18_cab_remitos] FOREIGN KEY([rela_opetick18])
REFERENCES [dbo].[ope.ope_tick_18_cab_remitos] ([id_opetick18])
GO

ALTER TABLE [dbo].[ope.ope_tick_19_det_remitos] CHECK CONSTRAINT [FK_ope.ope_tick_19_det_remitos_ope.ope_tick_18_cab_remitos]
GO

ALTER TABLE [dbo].[ope.ope_tick_19_det_remitos]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_19_det_remitos_par.par_enti_58_insumos] FOREIGN KEY([rela_parenti58])
REFERENCES [dbo].[par.par_enti_58_insumos] ([id_parenti58])
GO

ALTER TABLE [dbo].[ope.ope_tick_19_det_remitos] CHECK CONSTRAINT [FK_ope.ope_tick_19_det_remitos_par.par_enti_58_insumos]
GO

CREATE SYNONYM [dbo].[opetick19] FOR [dbo].[ope.ope_tick_19_det_remitos]
GO


