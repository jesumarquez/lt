USE [logictracker]
GO

/****** Object:  View [dbo].[VW_BI_CENTROS_DE_COSTOS]    Script Date: 27/04/2016 17:40:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[VW_BI_SUB_CENTROS_DE_COSTOS] AS
SELECT [A].[id_parenti99],
      [A].[parenti99_descripcion],
      [A].[parenti99_codigo],
      [A].[rela_parenti01],
      [A].[rela_parenti02],
      [A].[parenti99_objetivo],
      [A].[parenti99_baja],
      [A].[rela_parenti37]
  FROM [logictracker].[dbo].[par.par_enti_99_cab_subcentro_costos] A WITH (NOLOCK)
		WHERE A.[rela_parenti01] IN (
							SELECT [su10].RELA_PARENTI01
								FROM [logictracker].[dbo].[soc.soc_usua_13_det_paramusuario] [su13] WITH (NOLOCK)
									INNER JOIN [logictracker].[dbo].[soc.soc_usua_10_mov_empresas] [su10] WITH (NOLOCK)
										ON (([su10].rela_socusua01 = [su13].rela_socusua01 AND [su13].[socusua13_parametro] = 'bi_login' AND [su13].[socusua13_valor] = SYSTEM_USER) OR SYSTEM_USER = 'sa')
		)

GO