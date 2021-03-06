CREATE PROCEDURE [dbo].[sp_DatamartDAO_GetKilometrosDiarios]
	@ids DBO.IntTable READONLY,
	@dateFrom DATETIME,
	@dateTo DATETIME,
	@GMT INT = -3
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
		
		SELECT A.id_parenti03 AS Id,
				A.parenti03_interno AS Vehiculo
			INTO #vehiculos
			FROM parenti03 A
				INNER JOIN @ids t 
					ON A.id_parenti03 = t.IntNum

		-- CREO TABLA VACIA #RESULT
		SELECT	v.Vehiculo,
				dm.rela_parenti03 AS Id,
				CAST(DATEADD(HOUR, @GMT, dm.opeposi07_inicio) AS DATE) AS 'Fecha',
				SUM(dm.opeposi07_km) AS 'KmTotales',
				SUM(CASE WHEN dm.opeposi07_id_ciclo > 0 THEN dm.opeposi07_km ELSE 0 END) AS 'KmEnRuta'
		INTO #RESULT
		FROM opeposi07 dm 
			INNER JOIN #vehiculos v
				ON v.Id = dm.rela_parenti03
			WHERE 0=1
			GROUP BY	v.Vehiculo, dm.rela_parenti03, CAST(DATEADD(HOUR, @GMT, dm.opeposi07_inicio) AS DATE)
		DECLARE @Id AS INT;
		DECLARE @Vehiculo AS NVARCHAR(MAX);

		DECLARE vehiclesCursor CURSOR FOR
			SELECT A.Id, A.Vehiculo
				FROM #vehiculos A

		OPEN vehiclesCursor;
		FETCH NEXT FROM vehiclesCursor INTO @Id, @Vehiculo;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			INSERT INTO #RESULT
			SELECT	@Vehiculo AS Vehiculo,
					dm.rela_parenti03 AS Id,
					CAST(DATEADD(HOUR, @GMT, dm.opeposi07_inicio) AS DATE) AS 'Fecha',
					SUM(dm.opeposi07_km) AS 'KmTotales',
					SUM(CASE WHEN dm.opeposi07_id_ciclo > 0 THEN dm.opeposi07_km ELSE 0 END) AS 'KmEnRuta'
			FROM opeposi07 dm				
			WHERE	dm.opeposi07_inicio >= @dateFrom
			AND		dm.opeposi07_fin < @dateTo
			AND		dm.rela_parenti03 = @Id
			GROUP BY	dm.rela_parenti03, CAST(DATEADD(HOUR, @GMT, dm.opeposi07_inicio) AS DATE)

		FETCH NEXT FROM vehiclesCursor INTO @Id, @Vehiculo;
	END
	CLOSE vehiclesCursor
	DEALLOCATE vehiclesCursor
	SELECT Vehiculo, Fecha, KmTotales, KmEnRuta FROM #RESULT;
END
