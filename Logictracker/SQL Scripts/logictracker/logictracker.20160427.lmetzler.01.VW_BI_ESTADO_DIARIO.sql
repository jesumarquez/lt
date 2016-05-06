USE [logictracker]
GO

/****** Object:  View [dbo].[VW_BI_ESTADO_DIARIO]    Script Date: 28/04/2016 12:25:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[VW_BI_ESTADO_DIARIO]
AS
SELECT	B.parenti03_patente													AS 'Patente',
		A.rela_parenti03,
		B.rela_parenti01,
		SUM(CASE	WHEN (A.opeposi07_id_geocerca IS NOT NULL 
					AND D.parenti10_es_taller = 1)
					THEN A.opeposi07_horasdet 
					ELSE 0 END)												AS 'Horas Taller',
		SUM(CASE	WHEN (A.opeposi07_id_geocerca IS NOT NULL 
					AND A.opeposi07_id_geocerca = E.rela_parenti05)
					THEN A.opeposi07_horasdet 
					ELSE 0 END)												AS 'Horas Guarda',
		SUM(A.opeposi07_horasmov)											AS 'Horas Desplazamiento',		
		--SUM(A.opeposi07_horasdet) AS 'Detenido',
		SUM(A.opeposi07_horassinrep)										AS 'Sin Reportar',
		SUM(CASE	WHEN A.opeposi07_id_geocerca IS NULL 
					THEN A.opeposi07_horasdet 
					ELSE 0 END)												AS 'Detenido S/Trabajo',
		SUM(CASE	WHEN A.opeposi07_id_geocerca IS NOT NULL 
					AND D.parenti10_es_taller = 0 
					AND A.opeposi07_id_geocerca <> E.rela_parenti05
					THEN A.opeposi07_horasdet 
					ELSE 0 END)												AS 'Detenido C/Trabajo',
		SUM(A.opeposi07_km)													AS 'Kms',
		SUM(A.opeposi07_horasmarcha)										AS 'Horas de Marcha',
		CONVERT(varchar(10), DATEADD(HOUR, -3, A.opeposi07_inicio), 102)	AS 'Fecha'
FROM opeposi07 A WITH (NOLOCK)
INNER JOIN parenti03 B WITH (NOLOCK)
		ON B.id_parenti03 = A.rela_parenti03		
LEFT JOIN parenti05 C WITH (NOLOCK)
		ON C.id_parenti05 = A.opeposi07_id_geocerca
LEFT JOIN parenti10 D WITH (NOLOCK)
		ON D.id_parenti10 = C.rela_parenti10
LEFT JOIN parenti02 E WITH (NOLOCK)
		ON E.id_parenti02 = B.rela_parenti02
WHERE	B.rela_parenti01 IN (SELECT [su10].RELA_PARENTI01
							FROM [logictracker].[dbo].[soc.soc_usua_13_det_paramusuario] [su13] WITH (NOLOCK)
							INNER JOIN [logictracker].[dbo].[soc.soc_usua_10_mov_empresas] [su10] WITH (NOLOCK)
									ON (([su10].rela_socusua01 = [su13].rela_socusua01 
									AND [su13].[socusua13_parametro] = 'bi_login' 
									AND [su13].[socusua13_valor] = SYSTEM_USER) 
										OR SYSTEM_USER = 'sa')	)
AND		A.opeposi07_inicio >= '20160425 03:00'
GROUP BY	B.parenti03_patente,
			A.rela_parenti03,
			B.rela_parenti01,
			CONVERT(varchar(10), DATEADD(HOUR, -3, A.opeposi07_inicio), 102)



GO


