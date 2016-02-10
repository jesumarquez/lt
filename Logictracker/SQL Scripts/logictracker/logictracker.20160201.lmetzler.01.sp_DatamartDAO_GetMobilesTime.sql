USE [logictracker]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_DatamartDAO_GetMobilesTimes]
	@ids DBO.IntTable READONLY,
	@dateFrom DATETIME,
	@dateTo DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#RESULT') IS NOT NULL 
	BEGIN 
		DROP TABLE #RESULT;
	END

	IF CURSOR_STATUS('global','vehiclesCursor')>=-1
	BEGIN
	 DEALLOCATE vehiclesCursor
	END

	CREATE TABLE #RESULT (
		ID_PARENTI03 INT NOT NULL,
		PARENTI03_INTERNO NVARCHAR(32),
		HORAS FLOAT NOT NULL
	)

	DECLARE @id_parenti03 AS INT;
	DECLARE @parenti03_interno AS NVARCHAR(32);


	DECLARE vehiclesCursor CURSOR FOR
	SELECT ID_PARENTI03, parenti03_interno
		FROM PARENTI03 A
			INNER JOIN @ids B
				ON A.id_parenti03 = B.[IntNum]

	OPEN vehiclesCursor;
	FETCH NEXT FROM vehiclesCursor INTO @id_parenti03, @parenti03_interno;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		
		INSERT INTO #RESULT ([ID_PARENTI03], [PARENTI03_INTERNO], [HORAS])
		SELECT	rela_parenti03, @parenti03_interno, sum(opeposi07_horasmov)
		FROM	opeposi07 A 
		WHERE	A.rela_parenti03 = @id_parenti03
		and		A.opeposi07_inicio >= @dateFrom 
		and		A.opeposi07_inicio < @dateTo
		GROUP BY A.rela_parenti03
			

		IF (@@ROWCOUNT = 0)
		BEGIN
			INSERT INTO #RESULT ([ID_PARENTI03], [PARENTI03_INTERNO], [HORAS]) VALUES (@id_parenti03, @parenti03_interno, 0)
		END

		FETCH NEXT FROM vehiclesCursor INTO @id_parenti03, @parenti03_interno;
	END
	CLOSE vehiclesCursor
	DEALLOCATE vehiclesCursor

	SELECT ID_PARENTI03 AS Movil, PARENTI03_INTERNO AS Intern, HORAS AS ElapsedTime FROM #RESULT;
END


GO


