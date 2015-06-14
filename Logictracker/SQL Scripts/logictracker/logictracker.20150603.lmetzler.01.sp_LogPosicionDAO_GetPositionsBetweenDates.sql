USE [logictracker]
GO

/****** Object:  StoredProcedure [dbo].[sp_LogPosicionDAO_GetPositionsBetweenDates]    Script Date: 03/06/2015 15:05:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_LogPosicionDAO_GetPositionsBetweenDates]
	@vehicleId INT,
	@desde DATETIME,
	@hasta DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @posiciones AS dbo.IntTable

	INSERT INTO @posiciones
	SELECT id_opeposi01
	FROM opeposi01
	WHERE rela_parenti03 = @vehicleId
	AND opeposi01_fechora BETWEEN @desde AND @hasta
	ORDER BY opeposi01_fechora ASC

	SELECT A.*
	FROM OPEPOSI01 A
	INNER JOIN @posiciones B ON A.id_OPEPOSI01 = B.IntNum
	WHERE a.opeposi01_fechora BETWEEN @desde AND @hasta

END


GO


