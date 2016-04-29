USE [logictracker]
GO

/****** Object:  View [dbo].[VW_BI_BASES]    Script Date: 27/04/2016 17:43:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[VW_BI_DEPARTAMENTOS] AS 
SELECT [id_parenti04]
      ,[rela_parenti01]
	  ,[rela_parenti02]
      ,[parenti04_codigo]
      ,[parenti04_descri]
      ,[parenti04_baja]
      ,[rela_parenti09]      
  FROM [logictracker].[dbo].[par.par_enti_04_cab_departamento] WITH (NOLOCK)
	WHERE [rela_parenti01] IN (
							SELECT [su10].RELA_PARENTI01
								FROM [logictracker].[dbo].[soc.soc_usua_13_det_paramusuario] [su13] WITH (NOLOCK)
									INNER JOIN [logictracker].[dbo].[soc.soc_usua_10_mov_empresas] [su10] WITH (NOLOCK)
										ON (([su10].rela_socusua01 = [su13].rela_socusua01 AND [su13].[socusua13_parametro] = 'bi_login' AND [su13].[socusua13_valor] = SYSTEM_USER) OR SYSTEM_USER = 'sa')
		)

GO


