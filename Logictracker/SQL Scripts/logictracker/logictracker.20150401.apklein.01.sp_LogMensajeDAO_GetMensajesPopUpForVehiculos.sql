USE [logictracker]
GO

/****** Object:  StoredProcedure [dbo].[sp_LogMensajeDAO_GetMensajesPopUpForVehiculos]    Script Date: 4/6/2015 11:37:09 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_LogMensajeDAO_GetMensajesPopUpForVehiculos]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_LogMensajeDAO_GetMensajesPopUpForVehiculos] AS' 
END
GO


ALTER PROCEDURE [dbo].[sp_LogMensajeDAO_GetMensajesPopUpForVehiculos]
@top AS INT,
@vehiculosTable AS dbo.IntTable READONLY,
@desde AS DATETIME,
@hasta AS DATETIME = NULL,
@lastId AS INT = NULL
AS

IF (@hasta IS NULL)
	SET @hasta = GETUTCDATE();

CREATE TABLE #POPUPS (ID_OPEEVEN01 INT NOT NULL);

IF (@lastId IS NULL OR @lastId = 0)
INSERT INTO #POPUPS
SELECT	
	TOP (@top) A.id_opeeven01
	FROM OPEEVEN01 A
		INNER JOIN @vehiculosTable B
			ON B.IntNum = A.rela_parenti03
		INNER JOIN PAREVEN02 C
			ON C.id_pareven02 = A.rela_pareven02
				AND C.pareve02_es_popup = 1			
				AND A.opeeven01_estado = 0
		WHERE 
			A.opeeven01_datetime BETWEEN @desde AND @hasta
ELSE
INSERT INTO #POPUPS
SELECT	
	TOP (@top) A.id_opeeven01	
	FROM OPEEVEN01 A
		INNER JOIN @vehiculosTable B
			ON B.IntNum = A.rela_parenti03
		INNER JOIN PAREVEN02 C
			ON C.id_pareven02 = A.rela_pareven02
				AND C.pareve02_es_popup = 1			
				AND A.opeeven01_estado = 0
		WHERE 
			A.opeeven01_datetime BETWEEN @desde AND @hasta
			AND A.id_opeeven01 > @lastId

INSERT INTO #POPUPS
SELECT
	A.id_opeeven01
	FROM OPEEVEN01 A
		INNER JOIN @vehiculosTable B
			ON B.IntNum = A.rela_parenti03
		INNER JOIN PAREVEN02 C
			ON C.id_pareven02 = A.rela_pareven02
				AND C.pareve02_es_popup = 1
				AND C.pareven02_requiere_atencion = 1
				AND A.opeeven01_estado = 0
		LEFT OUTER JOIN #POPUPS D
			ON D.id_opeeven01 = A.id_opeeven01
		WHERE 
			A.opeeven01_datetime BETWEEN @desde AND @hasta			
			AND D.id_opeeven01 IS NULL

SELECT A.*
	FROM [ope.ope_even_01_log_eventos] A
		INNER JOIN #POPUPS B
			ON B.id_opeeven01 = A.id_opeeven01
	ORDER BY A.opeeven01_datetime DESC


GO

