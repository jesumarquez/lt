GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[par.par_enti_99_cab_subcentro_costos](
	[id_parenti99] [int] IDENTITY(1,1) NOT NULL,
	[parenti99_codigo] [varchar](50) NOT NULL,
	[parenti99_descripcion] [varchar](50) NOT NULL,
	[parenti99_objetivo] [int] NOT NULL,
	[rela_parenti01] [int] NOT NULL,
	[rela_parenti02] [int] NULL,
	[rela_parenti37] [int] NOT NULL,
	[parenti99_baja] [bit] NOT NULL,
 CONSTRAINT [PK_par.par_enti_99_cab_subcentro_costos] PRIMARY KEY CLUSTERED 
(
	[id_parenti99] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[par.par_enti_99_cab_subcentro_costos]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_99_cab_subcentro_costos_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[par.par_enti_99_cab_subcentro_costos] CHECK CONSTRAINT [FK_par.par_enti_99_cab_subcentro_costos_par.par_enti_01_cab_empresas]
GO

ALTER TABLE [dbo].[par.par_enti_99_cab_subcentro_costos]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_99_cab_subcentro_costos_par.par_enti_02_det_lineas] FOREIGN KEY([rela_parenti02])
REFERENCES [dbo].[par.par_enti_02_det_lineas] ([id_parenti02])
GO

ALTER TABLE [dbo].[par.par_enti_99_cab_subcentro_costos] CHECK CONSTRAINT [FK_par.par_enti_99_cab_subcentro_costos_par.par_enti_02_det_lineas]
GO

ALTER TABLE [dbo].[par.par_enti_99_cab_subcentro_costos]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_99_cab_subcentro_costos_par.par_enti_37_cab_centro_costos] FOREIGN KEY([rela_parenti37])
REFERENCES [dbo].[par.par_enti_37_cab_centro_costos] ([id_parenti37])
GO

ALTER TABLE [dbo].[par.par_enti_99_cab_subcentro_costos] CHECK CONSTRAINT [FK_par.par_enti_99_cab_subcentro_costos_par.par_enti_37_cab_centro_costos]
GO

CREATE SYNONYM [dbo].[parenti99] FOR [dbo].[par.par_enti_99_cab_subcentro_costos]
GO