USE [logictracker]
GO

/****** Object:  StoredProcedure [dbo].[sp_LogMensajeDAO_GetEventos]    Script Date: 19/03/2015 17:44:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_LogMensajeDAO_GetEventos]
	@ids DBO.IntTable READONLY,
	@codigo1 NVARCHAR(MAX),
	@codigo2 NVARCHAR(MAX),
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

	SELECT id_pareven01, pareven01_codigo
	INTO #PAREVEN01
	FROM PAREVEN01
	WHERE pareven01_codigo in (@codigo1, @codigo2)

	CREATE TABLE #RESULT 
	(
		ID_OPEEVEN01 INT NOT NULL,
		CODIGO NVARCHAR(MAX) NOT NULL
	)

	DECLARE @id_parenti03 AS INT;

	DECLARE vehiclesCursor CURSOR FOR
	SELECT A.IntNum
	FROM @ids A

	OPEN vehiclesCursor;
	FETCH NEXT FROM vehiclesCursor INTO @id_parenti03;

	WHILE @@FETCH_STATUS = 0
	BEGIN		
		PRINT @id_parenti03
		PRINT @dateFrom
		PRINT @dateTo
		PRINT @codigo1
		PRINT @codigo2
		PRINT '----------------------'
		INSERT INTO #RESULT ([ID_OPEEVEN01], [CODIGO])
		SELECT A.id_opeeven01, B.pareven01_codigo
		FROM   OPEEVEN01 A 
		INNER JOIN #PAREVEN01 B 
				ON A.rela_pareven01 = B.ID_PAREVEN01
		WHERE  A.rela_parenti03 = @id_parenti03
		AND A.opeeven01_datetime BETWEEN @dateFrom AND @dateTo
		FETCH NEXT FROM vehiclesCursor INTO @id_parenti03;
	END
	CLOSE vehiclesCursor
	DEALLOCATE vehiclesCursor

	SELECT	B.id_opeeven01 as 'Id',
			B.opeeven01_datetime as 'Fecha',
			B.rela_parenti03 as 'IdCoche',
			A.CODIGO as 'CodigoMensaje'
	FROM OPEEVEN01 B
	INNER JOIN #RESULT A 
			ON B.id_opeeven01  = A.ID_OPEEVEN01
	ORDER BY B.rela_parenti03, B.opeeven01_datetime
END


GO


