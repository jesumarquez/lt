USE [logictracker]
GO
/****** Object:  StoredProcedure [dbo].[sp_LogMensajeDAO_GetByVehiclesAndMessages]    Script Date: 08/05/2015 11:12:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_LogMensajeDAO_GetByVehiclesAndMessages]
	@vehiculosIds DBO.IntTable READONLY,
	@mensajesIds DBO.IntTable READONLY,
	@desde DATETIME,
	@hasta DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	
	CREATE TABLE #eventos ( id_opeeven01 INT NOT NULL, fecha DATETIME NOT NULL)

	IF (SELECT COUNT(*) FROM @mensajesIds) = 0 OR (SELECT COUNT(*) FROM @mensajesIds WHERE IntNum IN (0,-1)) > 0
	BEGIN
		INSERT INTO #eventos
		SELECT A.id_opeeven01, A.opeeven01_datetime
		FROM [dbo].[ope.ope_even_01_log_eventos] A
		INNER JOIN @vehiculosIds B
				ON B.IntNum = A.rela_parenti03
		WHERE A.opeeven01_datetime >= @desde
		AND A.opeeven01_datetime <= @hasta
	END
	ELSE
	BEGIN
		INSERT INTO #eventos
		SELECT A.id_opeeven01, A.opeeven01_datetime
		FROM [dbo].[ope.ope_even_01_log_eventos] A
		INNER JOIN @vehiculosIds B
				ON B.IntNum = A.rela_parenti03
		INNER JOIN @mensajesIds C
				ON C.IntNum = A.rela_pareven01
		WHERE A.opeeven01_datetime >= @desde
		AND A.opeeven01_datetime <= @hasta
	END

	SELECT A.*
	FROM [dbo].[ope.ope_even_01_log_eventos] A
	INNER JOIN #eventos B 
			ON A.id_opeeven01 = B.id_opeeven01 
			AND A.opeeven01_datetime = B.fecha
END

