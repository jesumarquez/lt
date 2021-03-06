USE [logictracker]
GO
/****** Object:  StoredProcedure [dbo].[sp_LogMensajeDAO_GetLastByVehicleAndCodes]    Script Date: 05/11/2015 14:17:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_LogMensajeDAO_GetLastByVehicleAndCodes]
	@id INT,
	@codigo1 NVARCHAR(MAX),
	@codigo2 NVARCHAR(MAX),
	@dateFrom DATETIME,
	@dateTo DATETIME
AS
BEGIN

	SELECT TOP (1) *
	FROM opeeven01
	WHERE id_opeeven01 IN (SELECT id_opeeven01 
						   FROM opeeven01
						   WHERE rela_pareven01 in (SELECT id_pareven01
													FROM pareven01
													WHERE pareven01_codigo in (@codigo1, @codigo2)) 
						   AND (opeeven01_datetime between @dateFrom and @dateTo)
						   AND rela_parenti03 = @id)
	ORDER BY opeeven01_datetime DESC

END

