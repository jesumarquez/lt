USE [logictracker]
GO

/****** Object:  StoredProcedure [dbo].[sp_LastVehicleEventDAO_GetEventsByVehiclesAndType]    Script Date: 4/6/2015 5:20:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_LastVehicleEventDAO_GetEventsByVehiclesAndType]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'ALTER PROCEDURE [dbo].[sp_LastVehicleEventDAO_GetEventsByVehiclesAndType]
@vehiculosIds AS dbo.IntTable READONLY,
@eventType AS INT
AS
	
SELECT A.rela_opeeven01
	INTO #IDs
	FROM [dbo].[ope.ope_even_11_last_vehicle_event] A
			INNER JOIN @vehiculosIds B
				ON B.IntNum = A.rela_parenti03	

	SELECT A.*
		FROM [dbo].[ope.ope_even_01_log_eventos] A
			INNER JOIN #IDs B
				ON A.id_opeeven01 = B.rela_opeeven01
' 
END
GO


