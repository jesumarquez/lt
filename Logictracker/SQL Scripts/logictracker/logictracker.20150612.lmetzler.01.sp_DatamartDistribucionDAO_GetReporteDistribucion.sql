USE [logictracker]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_DatamartDistribucionDAO_GetReporteDistribucion]
	@empresaId int,
	@lineaId int,
	@vehiculosIds DBO.IntTable READONLY,
	@puntoEntregaId int,
	@estadosIds DBO.IntTable READONLY,
	@desde DATETIME,
	@hasta DATETIME 
AS
BEGIN
	SET NOCOUNT ON;
	
	CREATE TABLE #entregas (id_opetick04 INT NOT NULL)
	
	INSERT INTO #entregas
	SELECT A.id_opetick04
	FROM [dbo].[ope.ope_tick_04_det_entregadistri] A
	INNER JOIN [dbo].[ope.ope_tick_03_cab_viajedistri] B ON B.id_opetick03 = A.rela_opetick03
	WHERE B.rela_parenti01 = @empresaId
	AND (@lineaId <= 0 OR B.rela_parenti02 = @lineaId)
	AND (@puntoEntregaId <= 0 OR A.rela_parenti44 = @puntoEntregaId)
	AND (B.rela_parenti03 IN (SELECT C.IntNum FROM @vehiculosIds C) OR B.rela_parenti03 IS NULL)
	AND A.opetick04_estado IN (SELECT D.IntNum FROM @estadosIds D)
	AND B.opetick03_inicio BETWEEN @desde AND @hasta
	
	SELECT	opetick06.rela_opetick03															AS 'Id',
			opetick04.opetick04_descripcion														AS 'Descripcion',
			ISNULL(parenti03.rela_parenti08, 0)													AS 'IdDispositivo',
			opetick06.opetick06_ruta															AS 'Ruta',
			ISNULL(parenti17.parenti17_descripcion, '')											AS 'TipoVehiculo',
			ISNULL(parenti03.parenti03_interno, '')												AS 'Vehiculo',
			CASE WHEN socusua05.socusua05_apellido IS NULL 
				 THEN '' ELSE socusua05.socusua05_apellido + ', ' + socusua05.socusua05_nombre 
			END																					AS 'Empleado',
			opetick06.opetick06_fecha															AS 'Date',
			opetick04.opetick04_orden															AS 'Orden',
			opetick06.opetick06_orden															AS 'OrdenReal',
			ISNULL(parenti44.parenti44_descri,parenti02.parenti02_descri)						AS 'PuntoEntrega',
			opetick06.opetick06_manual															AS 'DateManual',
			opetick06.opetick06_entrada															AS 'DateEntrada',
			opetick06.opetick06_salida															AS 'DateSalida',
			opetick06.opetick06_estado															AS 'Estado',
			opetick06.opetick06_km																AS 'Km',
			opetick04.opetick04_garmin_unread_inactive											AS 'DateGarminUnreadInactive',
			opetick04.opetick04_garmin_read_inactive											AS 'DateGarminReadInactive',
			opetick06.opetick06_confirmacion													AS 'Confirmacion',
			opetick04.opetick04_lectura_confirmacion											AS 'DateConfirmacion'
	FROM [dbo].[ope.ope_tick_06_tick_datamart] opetick06
	INNER JOIN #entregas ENT ON opetick06.rela_opetick04 = ENT.id_opetick04
	INNER JOIN [dbo].[ope.ope_tick_04_det_entregadistri] opetick04 ON opetick04.id_opetick04 = ENT.id_opetick04
	LEFT JOIN [dbo].[par.par_enti_03_cab_coches] parenti03 ON parenti03.id_parenti03 = opetick06.rela_parenti03
	LEFT JOIN [dbo].[par.par_enti_17_tbl_tipocoche] parenti17 ON parenti17.id_parenti17 = parenti03.rela_parenti17
	INNER JOIN [dbo].[ope.ope_tick_03_cab_viajedistri] opetick03 ON opetick03.id_opetick03 = opetick06.rela_opetick03
	LEFT JOIN [dbo].[par.par_enti_09_cab_empleados] parenti09 ON parenti09.id_parenti09 = opetick03.rela_parenti09
	LEFT JOIN [dbo].[soc.soc_usua_05_cab_entidades] socusua05 ON socusua05.id_socusua05 = parenti09.rela_socusua05
	LEFT JOIN [dbo].[par.par_enti_44_pto_entrega] parenti44 ON parenti44.id_parenti44 = opetick04.rela_parenti44
	LEFT JOIN [dbo].[par.par_enti_02_det_lineas] parenti02 ON parenti02.id_parenti02 = opetick04.rela_parenti02
	ORDER BY 'Date', 'Ruta', 'OrdenReal'
END

GO


