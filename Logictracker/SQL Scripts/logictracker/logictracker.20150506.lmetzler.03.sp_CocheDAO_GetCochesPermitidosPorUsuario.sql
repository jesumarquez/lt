USE [logictracker]
GO

/****** Object:  StoredProcedure [dbo].[sp_CocheDAO_GetCochesPermitidosPorUsuario]    Script Date: 06/05/2015 16:51:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_CocheDAO_GetCochesPermitidosPorUsuario]
	@empresasIds DBO.IntTable READONLY,
	@lineasIds DBO.IntTable READONLY,
	@tiposVehiculoIds DBO.IntTable READONLY,
	@transportistasIds DBO.IntTable READONLY,
	@departamentosIds DBO.IntTable READONLY,
	@centrosDeCostosIds DBO.IntTable READONLY,
	@subCentrosDeCostosIds DBO.IntTable READONLY,
	@marcasIds DBO.IntTable READONLY,
	@modelosIds DBO.IntTable READONLY,
	@tiposEmpleadoIds DBO.IntTable READONLY,
	@empleadoIds DBO.IntTable READONLY,
	@userId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @filtraEmpresa BIT = 1;
	DECLARE @filtraLinea BIT = 1;
	DECLARE @filtraTipoVehiculo BIT = 1;
	DECLARE @filtraTransportista BIT = 1;
	DECLARE @filtraDepartamento BIT = 1;
	DECLARE @filtraCentroDeCosto BIT = 1;
	DECLARE @filtraSubCentroDeCosto BIT = 1;
	DECLARE @filtraMarca BIT = 1;
	DECLARE @filtraModelo BIT = 1;
	DECLARE @filtraTipoEmpleado BIT = 1;
	DECLARE @filtraEmpleado BIT = 1;

	DECLARE @COCHES DBO.IntTable
	
	IF (SELECT COUNT(*) FROM @empresasIds) = 0 OR (SELECT COUNT(*) FROM @empresasIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraEmpresa = 0;
	IF (SELECT COUNT(*) FROM @lineasIds) = 0 OR (SELECT COUNT(*) FROM @lineasIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraLinea = 0;
	IF (SELECT COUNT(*) FROM @tiposVehiculoIds) = 0 OR (SELECT COUNT(*) FROM @tiposVehiculoIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraTipoVehiculo = 0;
	IF (SELECT COUNT(*) FROM @transportistasIds) = 0 OR (SELECT COUNT(*) FROM @transportistasIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraTransportista = 0;
	IF (SELECT COUNT(*) FROM @departamentosIds) = 0 OR (SELECT COUNT(*) FROM @departamentosIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraDepartamento = 0;
	IF (SELECT COUNT(*) FROM @centrosDeCostosIds) = 0 OR (SELECT COUNT(*) FROM @centrosDeCostosIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraCentroDeCosto = 0;
	IF (SELECT COUNT(*) FROM @subCentrosDeCostosIds) = 0 OR (SELECT COUNT(*) FROM @subCentrosDeCostosIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraSubCentroDeCosto = 0;
	IF (SELECT COUNT(*) FROM @marcasIds) = 0 OR (SELECT COUNT(*) FROM @marcasIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraMarca = 0;
	IF (SELECT COUNT(*) FROM @modelosIds) = 0 OR (SELECT COUNT(*) FROM @modelosIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraModelo = 0;
	IF (SELECT COUNT(*) FROM @tiposEmpleadoIds) = 0 OR (SELECT COUNT(*) FROM @tiposEmpleadoIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraTipoEmpleado = 0;
	IF (SELECT COUNT(*) FROM @empleadoIds) = 0 OR (SELECT COUNT(*) FROM @empleadoIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraEmpleado = 0;

	INSERT INTO @COCHES
	SELECT A.id_parenti03
	FROM [dbo].[par.par_enti_03_cab_coches] A
	INNER JOIN [dbo].[fn_GetCochesForUser](@userId) B ON B.rela_parenti03 = A.id_parenti03
	WHERE (@filtraEmpresa = 0 OR A.rela_parenti01 IS NULL
			OR A.rela_parenti01 IN (SELECT IntNum FROM @empresasIds))
	AND (@filtraLinea = 0 OR A.rela_parenti02 IS NULL
			OR A.rela_parenti02 IN (SELECT IntNum FROM @lineasIds))
	AND (@filtraDepartamento = 0 OR A.rela_parenti04 IS NULL
			OR A.rela_parenti04 IN (SELECT IntNum FROM @departamentosIds))
	AND (@filtraTransportista = 0 OR A.rela_parenti07 IS NULL
			OR A.rela_parenti07 IN (SELECT IntNum FROM @transportistasIds))
	AND (@filtraEmpleado = 0 OR A.rela_parenti09 IS NULL
			OR A.rela_parenti09 IN (SELECT IntNum FROM @empleadoIds))
	AND (@filtraTipoVehiculo = 0 OR A.rela_parenti17 IS NULL
			OR A.rela_parenti17 IN (SELECT IntNum FROM @tiposVehiculoIds))
	AND (@filtraModelo = 0 OR A.rela_parenti61 IS NULL
			OR A.rela_parenti61 IN (SELECT IntNum FROM @modelosIds))
	AND (@filtraCentroDeCosto = 0 OR A.rela_parenti37 IS NULL
			OR A.rela_parenti37 IN (SELECT IntNum FROM @centrosDeCostosIds))
	AND (@filtraSubCentroDeCosto = 0 OR A.rela_parenti99 IS NULL
			OR A.rela_parenti99 IN (SELECT IntNum FROM @subCentrosDeCostosIds))
	AND (@filtraMarca = 0 OR A.rela_parenti61 IS NULL
			OR A.rela_parenti61 IN (SELECT id_parenti61 FROM parenti61 
									INNER JOIN @marcasIds ON IntNum = rela_parenti06))
	AND (@filtraTipoEmpleado = 0 OR A.rela_parenti09 IS NULL
			OR A.rela_parenti09 IN (SELECT id_parenti09 FROM parenti09
									INNER JOIN @tiposEmpleadoIds ON IntNum = rela_parenti43))
		
	SELECT A.*
	FROM [dbo].[par.par_enti_03_cab_coches] A
	INNER JOIN @COCHES B ON B.IntNum = A.id_parenti03

END
GO


