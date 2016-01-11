USE [logictracker]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ope.ope_posi_10_datamart_tramos](
	[id_opeposi10] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti03] [int] NOT NULL,
	[opeposi10_inicio] [datetime] NOT NULL,
	[opeposi10_fin] [datetime] NOT NULL,
	[opeposi10_km] [float] NOT NULL,
	[opeposi10_horas] [float] NOT NULL,
	[opeposi10_horas_mov] [float] NOT NULL,
	[opeposi10_horas_det] [float] NOT NULL,
	[opeposi10_horas_det_dentro] [float] NOT NULL,
	[opeposi10_horas_det_fuera] [float] NOT NULL,
	[opeposi10_detenciones_mayores] [int] NOT NULL,
	[opeposi10_detenciones_menores] [int] NOT NULL,
	[opeposi10_velocidad_promedio] [int] NOT NULL,
	[opeposi10_geocercas_base] [int] NOT NULL,
	[opeposi10_geocercas_entregas] [int] NOT NULL,
	[opeposi10_geocercas_otras] [int] NOT NULL,
	[opeposi10_motor_on] [bit] NOT NULL
 CONSTRAINT [PK_ope.ope_posi_10_datamart_tramos] PRIMARY KEY CLUSTERED 
(
	[id_opeposi10] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ope.ope_posi_10_datamart_tramos]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_posi_10_datamart_tramos_par.par_enti_03_cab_coches] FOREIGN KEY([rela_parenti03])
REFERENCES [dbo].[par.par_enti_03_cab_coches] ([id_parenti03])
GO

ALTER TABLE [dbo].[ope.ope_posi_10_datamart_tramos] CHECK CONSTRAINT [FK_ope.ope_posi_10_datamart_tramos_par.par_enti_03_cab_coches]
GO

CREATE SYNONYM [dbo].[opeposi10] FOR [dbo].[ope.ope_posi_10_datamart_tramos]
GO


