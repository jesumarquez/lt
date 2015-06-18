USE [logictracker]
GO
/****** Object:  StoredProcedure [dbo].[sp_LastVehicleEventDAO_GetEventsByVehiclesAndType]    Script Date: 16/06/2015 12:32:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_LastVehicleEventDAO_GetEventsByVehiclesAndType]
@vehiculosIds AS dbo.IntTable READONLY,
@eventType AS INT
AS
	
SELECT A.rela_opeeven01
	INTO #IDs
	FROM [dbo].[ope.ope_even_11_last_vehicle_event] A
	INNER JOIN @vehiculosIds B ON B.IntNum = A.rela_parenti03	
	WHERE A.opeeven11_tipo_evento = @eventType

	SELECT A.*
	FROM [dbo].[ope.ope_even_01_log_eventos] A
	INNER JOIN #IDs B ON A.id_opeeven01 = B.rela_opeeven01
