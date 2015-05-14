USE [logictracker]
GO

/****** Object:  StoredProcedure [dbo].[sp_LogMensajeDAO_GetMensajesPopUpForEmpresa]    Script Date: 4/6/2015 11:36:21 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_LogMensajeDAO_GetMensajesPopUpForEmpresa]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_LogMensajeDAO_GetMensajesPopUpForEmpresa] AS' 
END
GO

ALTER PROCEDURE [dbo].[sp_LogMensajeDAO_GetMensajesPopUpForEmpresa]
@top AS INT,
@empresaId AS INT,
@desde AS DATETIME,
@hasta AS DATETIME = NULL,
@lastId AS INT = NULL
AS
DECLARE @vehiculosTable AS dbo.IntTable;
INSERT INTO @vehiculosTable
SELECT id_parenti03 FROM parenti03 WHERE rela_parenti01 = @empresaId;
EXEC dbo.sp_LogMensajeDAO_GetMensajesPopUpForVehiculos @top = @top, @vehiculosTable = @vehiculosTable, @desde = @desde, @hasta = @hasta, @lastId = @lastId;

GO

