USE [logictracker]
GO

/****** Object:  StoredProcedure [dbo].[sp_LogMensajeDAO_GetEventos]    Script Date: 18/05/2015 10:39:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_LogMensajeDAO_GetEventos]
	@ids DBO.IntTable READONLY,
	@codigo1 NVARCHAR(MAX),
	@codigo2 NVARCHAR(MAX),
	@dateFrom DATETIME,
	@dateTo DATETIME
AS
BEGIN
	IF OBJECT_ID('tempdb..#RESULT') IS NOT NULL 
	BEGIN 
		DROP TABLE #RESULT;
	END
	
	CREATE TABLE #RESULT 
	(
		id_opeeven01 INT NOT NULL,
		opeeven01_datetime DATETIME NOT NULL,
		codigo NVARCHAR(MAX)
	)

	INSERT INTO #RESULT ([id_opeeven01],[opeeven01_datetime],[codigo])
	SELECT A.id_opeeven01, A.opeeven01_datetime, B.pareven01_codigo
	FROM OPEEVEN01 A 
	INNER JOIN @ids C ON C.IntNum = A.rela_parenti03
	INNER JOIN pareven01 B 
				ON A.rela_pareven01 = B.id_pareven01
				AND B.pareven01_codigo in (@codigo1, @codigo2)
	AND A.opeeven01_datetime BETWEEN @dateFrom AND @dateTo
	
	SELECT	B.id_opeeven01 as 'Id',
			B.opeeven01_datetime as 'Fecha',
			B.rela_parenti03 as 'IdCoche',
			A.codigo as 'CodigoMensaje'
	FROM OPEEVEN01 B
	INNER JOIN #RESULT A 
			ON B.id_opeeven01 = A.id_opeeven01
			AND B.opeeven01_datetime = A.opeeven01_datetime
	ORDER BY B.rela_parenti03, B.opeeven01_datetime
END




GO


