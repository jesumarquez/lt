USE [logictracker]
GO

/****** Object:  Table [dbo].[ope.ope_tick_10_param_viaje]    Script Date: 10/11/2015 17:42:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ope.ope_tick_10_param_viaje](
	[id_opetick10] [int] IDENTITY(1,1) NOT NULL,
	[rela_opetick03] [int] NOT NULL,
	[opetick10_parametro] [varchar](64) NOT NULL,
	[opetick10_valor] [varchar](256) NOT NULL,
 CONSTRAINT [PK_ope.ope_tick_10_param_viaje] PRIMARY KEY CLUSTERED 
(
	[id_opetick10] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ope.ope_tick_10_param_viaje]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_tick_10_param_viaje_ope.ope_tick_03_cab_viajedistri] FOREIGN KEY([rela_opetick03])
REFERENCES [dbo].[ope.ope_tick_03_cab_viajedistri] ([id_opetick03])
GO

ALTER TABLE [dbo].[ope.ope_tick_10_param_viaje] CHECK CONSTRAINT [FK_ope.ope_tick_10_param_viaje_ope.ope_tick_03_cab_viajedistri]
GO


CREATE SYNONYM [dbo].[opetick10] FOR [dbo].[ope.ope_tick_10_param_viaje]
GO