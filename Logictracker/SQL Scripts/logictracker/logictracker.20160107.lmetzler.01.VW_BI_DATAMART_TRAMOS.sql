USE [logictracker]
GO

/****** Object:  View [dbo].[VW_BI_DATAMART_TRAMOS]    Script Date: 07/01/2016 12:29:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[VW_BI_DATAMART_TRAMOS]
AS
SELECT  b.rela_parenti01, 
		b.rela_parenti02, 
		a.id_opeposi10, 
		a.rela_parenti03, 
		CAST(DATEADD(HOUR, - 3, a.opeposi10_inicio) AS DATE) AS opeposi10_inicio_DATE, 
		DATEADD(HOUR, - 3, a.opeposi10_inicio) AS opeposi07_inicio, 
		DATEADD(HOUR, - 3, a.opeposi10_fin) AS opeposi07_fin, 
		a.opeposi10_km, 
		a.opeposi10_horas,
		a.opeposi10_horas_mov, 
		a.opeposi10_horas_det,
		a.opeposi10_horas_det_dentro,
		a.opeposi10_horas_det_fuera, 
		a.opeposi10_detenciones_mayores,
		a.opeposi10_detenciones_menores, 
		a.opeposi10_velocidad_promedio, 
		a.opeposi10_geocercas_base,
		a.opeposi10_geocercas_entregas,
		a.opeposi10_geocercas_otras,
		a.opeposi10_motor_on
FROM dbo.[par.par_enti_03_cab_coches] b
	INNER JOIN dbo.[ope.ope_posi_10_datamart_tramos] a		
	ON a.rela_parenti03 = b.id_parenti03
		AND B.[rela_parenti01] IN (
							SELECT [su10].RELA_PARENTI01
								FROM [logictracker].[dbo].[soc.soc_usua_13_det_paramusuario] [su13]
									INNER JOIN [logictracker].[dbo].[soc.soc_usua_10_mov_empresas] [su10]
										ON (([su10].rela_socusua01 = [su13].rela_socusua01 AND [su13].[socusua13_parametro] = 'bi_login' AND [su13].[socusua13_valor] = SYSTEM_USER) OR SYSTEM_USER = 'sa')
		)
		AND a.opeposi10_inicio >= '20160101'

GO


