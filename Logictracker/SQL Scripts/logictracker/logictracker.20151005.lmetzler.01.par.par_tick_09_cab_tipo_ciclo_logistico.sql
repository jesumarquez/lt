USE [logictracker]
GO

/****** Object:  Table [dbo].[par.par_tick_09_cab_tipo_ciclo_logistico]    Script Date: 05/10/2015 17:10:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[par.par_tick_09_cab_tipo_ciclo_logistico](
	[id_partick09] [int] IDENTITY(1,1) NOT NULL,
	[partick09_codigo] [varchar](255) NOT NULL,
	[partick09_descripcion] [varchar](255) NOT NULL,
	[rela_parenti01] [int] NOT NULL,
	[partick09_baja] [bit] NOT NULL,
 CONSTRAINT [PK_par.par_tick_09_cab_tipo_ciclo_logistico] PRIMARY KEY CLUSTERED 
(
	[id_partick09] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[par.par_tick_09_cab_tipo_ciclo_logistico]  WITH CHECK ADD  CONSTRAINT [FK_par.par_tick_09_cab_tipo_ciclo_logistico_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[par.par_tick_09_cab_tipo_ciclo_logistico] CHECK CONSTRAINT [FK_par.par_tick_09_cab_tipo_ciclo_logistico_par.par_enti_01_cab_empresas]
GO

CREATE SYNONYM [dbo].[partick09] FOR [dbo].[par.par_tick_09_cab_tipo_ciclo_logistico]
GO


