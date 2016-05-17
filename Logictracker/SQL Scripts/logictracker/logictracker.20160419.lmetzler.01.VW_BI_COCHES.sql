USE [logictracker]
GO

/****** Object:  View [dbo].[VW_BI_COCHES]    Script Date: 19/04/2016 14:04:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER VIEW [dbo].[VW_BI_COCHES] AS
SELECT A.[id_parenti03],
      A.[rela_parenti02],
      A.[parenti03_interno],
      A.[rela_parenti06],
      A.[parenti03_modelo],
      A.[parenti03_patente],
      A.[parenti03_anopat],
      A.[rela_parenti07],
      A.[parenti03_poliza],
      A.[parenti03_fvto],
      A.[parenti03_nrochasis],
      A.[parenti03_nromotor],
      A.[rela_parenti08],
      A.[parenti03_estado],
      A.[rela_parenti17],
      A.[rela_parenti09],
      A.[parenti03_dtCambioEstado],
      A.[paren03_referencia],
      A.[paren03_odometro_aplicacion],
      A.[paren03_odometro_inicial],
      A.[paren03_odometro_parcial],
      A.[paren03_reset_odometro],
      A.[paren03_kilometros_diarios],
      A.[rela_parenti37],
      A.[rela_parenti01],
      A.[paren03_last_odometer_update],
      A.[pae03_velocidad_promedio],
      A.[pae03_odometro_diario],
      A.[pae03_disparo_odometro_diario],
      A.[pae03_controla_km],
      A.[pae03_controla_hs],
      A.[pae03_controla_turnos],
      A.[pae03_controla_servicios],
      A.[pae03_porc_productividad],
      A.[parenti03_capacidad],
      A.[parenti03_identifica_choferes],
      A.[parenti03_reporta_ac],
      A.[rela_parenti61],
      A.[rela_opeenti03],
      A.[parenti03_espuerta],
      A.[rela_parenti99],
	  B.[opeenti03_fecha_inicio]
  FROM [logictracker].[dbo].[par.par_enti_03_cab_coches] A WITH (NOLOCK)
  LEFT JOIN [logictracker].[dbo].[ope.ope_enti_03_coches] B WITH (NOLOCK)
		ON B.id_opeenti03 = A.rela_opeenti03
	WHERE [rela_parenti01] IN (
							SELECT [su10].RELA_PARENTI01
								FROM [logictracker].[dbo].[soc.soc_usua_13_det_paramusuario] [su13] WITH (NOLOCK)
									INNER JOIN [logictracker].[dbo].[soc.soc_usua_10_mov_empresas] [su10] WITH (NOLOCK)
										ON (([su10].rela_socusua01 = [su13].rela_socusua01 AND [su13].[socusua13_parametro] = 'bi_login' AND [su13].[socusua13_valor] = SYSTEM_USER) OR SYSTEM_USER = 'sa')
	)

GO


