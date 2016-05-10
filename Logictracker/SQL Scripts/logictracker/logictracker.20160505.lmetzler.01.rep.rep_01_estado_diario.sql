USE [logictracker]
GO

/****** Object:  Table [dbo].[rep.rep_01_estado_diario]    Script Date: 05/05/2016 13:04:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[rep.rep_01_estado_diario](
	[id_rep01] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti01] [int] NOT NULL,
	[rela_parenti03] [int] NOT NULL,
	[rep01_horas_en_taller] [float] NOT NULL,
	[rep01_horas_en_base] [float] NOT NULL,
	[rep01_horas_movimiento] [float] NOT NULL,
	[rep01_horas_sin_reportar] [float] NOT NULL,
	[rep01_horas_detenido] [float] NOT NULL,
	[rep01_horas_det_en_geocerca] [float] NOT NULL,
	[rep01_horas_det_sin_geocerca] [float] NOT NULL,
	[rep01_km] [float] NOT NULL,
	[rep01_horas_en_marcha] [float] NOT NULL,
	[rep01_fecha] [datetime] NOT NULL,
 CONSTRAINT [PK_rep.rep_01_estado_diario] PRIMARY KEY CLUSTERED 
(
	[id_rep01] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[rep.rep_01_estado_diario]  WITH CHECK ADD  CONSTRAINT [FK_rep.rep_01_estado_diario_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[rep.rep_01_estado_diario] CHECK CONSTRAINT [FK_rep.rep_01_estado_diario_par.par_enti_01_cab_empresas]
GO

ALTER TABLE [dbo].[rep.rep_01_estado_diario]  WITH CHECK ADD  CONSTRAINT [FK_rep.rep_01_estado_diario_par.par_enti_03_cab_coches] FOREIGN KEY([rela_parenti03])
REFERENCES [dbo].[par.par_enti_03_cab_coches] ([id_parenti03])
GO

ALTER TABLE [dbo].[rep.rep_01_estado_diario] CHECK CONSTRAINT [FK_rep.rep_01_estado_diario_par.par_enti_03_cab_coches]
GO

CREATE SYNONYM [dbo].[rep01] FOR [dbo].[rep.rep_01_estado_diario]
GO

