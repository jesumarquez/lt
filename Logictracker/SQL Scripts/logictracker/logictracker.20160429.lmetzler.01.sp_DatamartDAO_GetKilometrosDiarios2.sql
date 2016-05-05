USE [logictracker]
GO

/****** Object:  StoredProcedure [dbo].[sp_DatamartDAO_GetKilometrosDiarios2]    Script Date: 29/04/2016 13:14:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_DatamartDAO_GetKilometrosDiarios2]
	@ids DBO.IntTable READONLY,
	@dateFrom DATETIME,
	@dateTo DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#vehiculos') IS NOT NULL 
	BEGIN 
		DROP TABLE #vehiculos;
	END

	IF OBJECT_ID('tempdb..#RESULT') IS NOT NULL 
	BEGIN 
		DROP TABLE #RESULT;
	END

	IF CURSOR_STATUS('global','vehiclesCursor')>=-1
	BEGIN
		DEALLOCATE vehiclesCursor
	END
		
	SELECT	A.id_parenti03 AS 'Id',
			A.parenti03_patente AS 'Patente',
			A.rela_parenti02 AS 'rela_parenti02'
	INTO #vehiculos
	FROM parenti03 A
	INNER JOIN @ids t 
			ON A.id_parenti03 = t.IntNum

	-- CREO TABLA VACIA #RESULT
	SELECT	v.Patente AS 'Patente',
			dm.rela_parenti03 AS 'IdVehiculo',
			SUM(dm.opeposi07_km) AS 'KmTotales',
			SUM(CASE WHEN dm.opeposi07_id_ciclo > 0 THEN dm.opeposi07_km ELSE 0 END) AS 'KmEnRuta',
			SUM(dm.opeposi07_horasmarcha) AS 'HorasDeMarcha',
			SUM(dm.opeposi07_horasmov) AS 'HorasMovimiento',
			SUM(dm.opeposi07_horasdet) AS 'HorasDetenido',
			SUM(dm.opeposi07_horassinrep) AS 'HorasSinReportar',
			SUM(CASE WHEN dm.opeposi07_id_geocerca IS NOT NULL AND tr.parenti10_es_taller = 1
						THEN dm.opeposi07_horasdet ELSE 0 END) AS 'HorasDetenidoEnTaller',
			SUM(CASE WHEN dm.opeposi07_id_geocerca IS NOT NULL AND dm.opeposi07_id_geocerca = b.rela_parenti05
						THEN dm.opeposi07_horasdet ELSE 0 END) AS 'HorasDetenidoEnBase',
			SUM(CASE WHEN dm.opeposi07_id_geocerca IS NOT NULL 
						AND dm.opeposi07_id_geocerca <> b.rela_parenti05
						AND tr.parenti10_es_taller = 0
						THEN dm.opeposi07_horasdet ELSE 0 END) AS 'HorasDetenidoConTarea',
			SUM(CASE WHEN dm.opeposi07_id_geocerca = 0 OR dm.opeposi07_id_geocerca IS NULL
						THEN dm.opeposi07_horasdet ELSE 0 END) AS 'HorasDetenidoSinTarea'
	INTO #RESULT
	FROM opeposi07 dm 
	INNER JOIN #vehiculos v
			ON v.Id = dm.rela_parenti03
	LEFT JOIN parenti05 r
			ON r.id_parenti05 = dm.opeposi07_id_geocerca
	LEFT JOIN parenti10 tr
			ON tr.id_parenti10 = r.rela_parenti10
	LEFT JOIN parenti02 b
			ON b.id_parenti02 = v.rela_parenti02
	WHERE 0=1
	GROUP BY	v.Patente, dm.rela_parenti03
		
	DECLARE @Id AS INT;
	DECLARE @Patente AS NVARCHAR(MAX);
	DECLARE @rela_parenti02 AS INT;

	DECLARE vehiclesCursor CURSOR FOR
	SELECT	A.Id, 
			A.Patente,
			A.rela_parenti02
	FROM #vehiculos A

	OPEN vehiclesCursor;
	FETCH NEXT FROM vehiclesCursor
	INTO @Id, @Patente, @rela_parenti02;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO #RESULT
		SELECT	@Patente AS 'Patente',
				dm.rela_parenti03 AS 'IdVehiculo',
				SUM(dm.opeposi07_km) AS 'KmTotales',
				SUM(CASE WHEN dm.opeposi07_id_ciclo > 0 THEN dm.opeposi07_km ELSE 0 END) AS 'KmEnRuta',
				SUM(dm.opeposi07_horasmarcha) AS 'HorasDeMarcha',
				SUM(dm.opeposi07_horasmov) AS 'HorasMovimiento',
				SUM(dm.opeposi07_horasdet) AS 'HorasDetenido',
				SUM(dm.opeposi07_horassinrep) AS 'HorasSinReportar',
				SUM(CASE WHEN dm.opeposi07_id_geocerca IS NOT NULL AND tr.parenti10_es_taller = 1
							THEN dm.opeposi07_horasdet ELSE 0 END) AS 'HorasDetenidoEnTaller',
				SUM(CASE WHEN dm.opeposi07_id_geocerca IS NOT NULL AND dm.opeposi07_id_geocerca = b.rela_parenti05
							THEN dm.opeposi07_horasdet ELSE 0 END) AS 'HorasDetenidoEnBase',
				SUM(CASE WHEN dm.opeposi07_id_geocerca IS NOT NULL 
							AND dm.opeposi07_id_geocerca <> b.rela_parenti05
							AND tr.parenti10_es_taller = 0
							THEN dm.opeposi07_horasdet ELSE 0 END) AS 'HorasDetenidoConTarea',
				SUM(CASE WHEN dm.opeposi07_id_geocerca = 0 OR dm.opeposi07_id_geocerca IS NULL
							THEN dm.opeposi07_horasdet ELSE 0 END) AS 'HorasDetenidoSinTarea'
		FROM opeposi07 dm		
		LEFT JOIN parenti05 r
				ON r.id_parenti05 = dm.opeposi07_id_geocerca
		LEFT JOIN parenti10 tr
				ON tr.id_parenti10 = r.rela_parenti10
		LEFT JOIN parenti02 b
				ON b.id_parenti02 = @rela_parenti02
		WHERE	dm.opeposi07_inicio >= @dateFrom
		AND		dm.opeposi07_fin < @dateTo
		AND		dm.rela_parenti03 = @Id
		GROUP BY	dm.rela_parenti03

		FETCH NEXT FROM vehiclesCursor INTO @Id, @Patente, @rela_parenti02;
	END
	CLOSE vehiclesCursor
	DEALLOCATE vehiclesCursor
	
	SELECT	Patente, 
			IdVehiculo,
			KmTotales, 
			KmEnRuta, 
			HorasDeMarcha, 
			HorasMovimiento, 
			HorasDetenido, 
			HorasSinReportar,
			HorasDetenidoEnTaller,
			HorasDetenidoEnBase,
			HorasDetenidoConTarea,
			HorasDetenidoSinTarea
	FROM #RESULT;
END


GO


