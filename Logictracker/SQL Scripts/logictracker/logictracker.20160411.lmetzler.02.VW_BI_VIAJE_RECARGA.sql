USE [logictracker]
GO

/****** Object:  View [dbo].[VW_BI_VIAJE_RECARGA]    Script Date: 12/04/2016 16:01:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[VW_BI_VIAJE_RECARGA]
AS
SELECT	 [id_opetick17]
		,[rela_parenti01]
		,[rela_parenti02]
		,[rela_parenti03]
		,[opetick17_interno]
		,[opetick17_patente]
		,CAST([opetick17_fecha] AS DATE) AS 'opetick17_fecha'
		,[opetick17_accion]
		,CAST([opetick17_inicio] AS TIME) AS 'opetick17_inicio'
		,CAST([opetick17_fin] AS TIME) AS 'opetick17_fin'
		,[opetick17_duracion]
FROM    dbo.[ope.ope_tick_17_viaje_recarga]
WHERE [rela_parenti01] IN (	SELECT [su10].RELA_PARENTI01
							FROM [logictracker].[dbo].[soc.soc_usua_13_det_paramusuario] [su13] WITH (NOLOCK)
							INNER JOIN [logictracker].[dbo].[soc.soc_usua_10_mov_empresas] [su10] WITH (NOLOCK)
									ON (([su10].rela_socusua01 = [su13].rela_socusua01 AND [su13].[socusua13_parametro] = 'bi_login' AND [su13].[socusua13_valor] = SYSTEM_USER) OR SYSTEM_USER = 'sa')	)


GO


