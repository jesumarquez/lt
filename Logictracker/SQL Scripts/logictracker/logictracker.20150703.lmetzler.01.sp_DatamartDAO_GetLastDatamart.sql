USE [logictracker]
GO
/****** Object:  StoredProcedure [dbo].[sp_DatamartDAO_GetLastDatamart]    Script Date: 03/07/2015 10:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_DatamartDAO_GetLastDatamart]
	@vehicleId INT,
	@fecha DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @id AS INT

	SELECT TOP 1 @id = id_opeposi07
	FROM opeposi07 WITH (NOLOCK)
	WHERE rela_parenti03 = @vehicleId
	AND opeposi07_inicio <= @fecha
	ORDER BY opeposi07_inicio DESC

	SELECT A.* 
	FROM opeposi07 A WITH (NOLOCK)
	WHERE id_opeposi07 = @id

END

