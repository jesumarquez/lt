USE [logictracker]
GO

/****** Object:  Table [dbo].[ope.ope_tick_14_det_viaje_prog]    Script Date: 28/01/2016 13:04:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ope.ope_tick_14_det_viaje_prog](
	[id_opetick14] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti44] [int] NOT NULL,
	[rela_opetick13] [int] NOT NULL,
	[opetick14_orden] [int] NOT NULL
 CONSTRAINT [PK_ope.ope_tick_14_det_viaje_prog] PRIMARY KEY CLUSTERED 
(
	[id_opetick14] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ope.ope_tick_14_det_viaje_prog]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_14_det_viaje_prog_ope.ope_tick_13_cab_viaje_prog] FOREIGN KEY([rela_opetick13])
REFERENCES [dbo].[ope.ope_tick_13_cab_viaje_prog] ([id_opetick13])
GO

ALTER TABLE [dbo].[ope.ope_tick_14_det_viaje_prog] CHECK CONSTRAINT [FK_ope.ope_tick_14_det_viaje_prog_ope.ope_tick_13_cab_viaje_prog]
GO

ALTER TABLE [dbo].[ope.ope_tick_14_det_viaje_prog]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_14_det_viaje_prog_par.par_enti_44_pto_entrega] FOREIGN KEY([rela_parenti44])
REFERENCES [dbo].[par.par_enti_44_pto_entrega] ([id_parenti44])
GO

ALTER TABLE [dbo].[ope.ope_tick_14_det_viaje_prog] CHECK CONSTRAINT [FK_ope.ope_tick_14_det_viaje_prog_par.par_enti_44_pto_entrega]
GO

CREATE SYNONYM [dbo].[opetick14] FOR [dbo].[ope.ope_tick_14_det_viaje_prog]
GO


