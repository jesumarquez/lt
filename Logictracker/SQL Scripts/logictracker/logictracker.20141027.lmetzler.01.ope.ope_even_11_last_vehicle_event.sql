USE [logictracker]
GO

/****** Object:  Table [dbo].[ope.ope_even_11_last_vehicle_event]    Script Date: 10/27/2014 12:08:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ope.ope_even_11_last_vehicle_event](
	[id_opeeven11] [int] IDENTITY(1,1) NOT NULL,
	[opeeven11_tipo_evento] [int] NOT NULL,
	[rela_parenti03] [int] NOT NULL,
	[rela_opeeven01] [int] NOT NULL,
 CONSTRAINT [PK_ope.ope_even_11_last_vehicle_event] PRIMARY KEY CLUSTERED 
(
	[id_opeeven11] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ope.ope_even_11_last_vehicle_event]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_even_11_last_vehicle_event_par.par_enti_03_cab_coches] FOREIGN KEY([rela_parenti03])
REFERENCES [dbo].[par.par_enti_03_cab_coches] ([id_parenti03])
GO

ALTER TABLE [dbo].[ope.ope_even_11_last_vehicle_event] CHECK CONSTRAINT [FK_ope.ope_even_11_last_vehicle_event_par.par_enti_03_cab_coches]
GO

CREATE SYNONYM [dbo].[opeeven11] FOR [logictracker].[dbo].[ope.ope_even_11_last_vehicle_event]
GO

