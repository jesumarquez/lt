USE [logictracker]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_LogMensajeDAO_GetLastByVehicleAndCode]
	@id INT,
	@codigo NVARCHAR(MAX),
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
													WHERE pareven01_codigo = @codigo)
						   AND (opeeven01_datetime between @dateFrom and @dateTo)
						   AND rela_parenti03 = @id)
	ORDER BY opeeven01_datetime DESC
END
GO


