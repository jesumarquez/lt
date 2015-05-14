USE [logictracker]
GO

/****** Object:  StoredProcedure [dbo].[sp_CentroDeCostosDAO_GetCentrosDeCostosPermitidosPorUsuario]    Script Date: 06/05/2015 16:50:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_CentroDeCostosDAO_GetCentrosDeCostosPermitidosPorUsuario]
	@empresasIds DBO.IntTable READONLY,
	@lineasIds DBO.IntTable READONLY,
	@deptosIds DBO.IntTable READONLY,
	@userId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @filtraEmpresa BIT = 1;
	DECLARE @filtraLinea BIT = 1;
	DECLARE @filtraDepto BIT = 1;

	CREATE TABLE #CENTROS(
	[id_parenti37] [int] NOT NULL,
	[parenti37_descri] [varchar](50) NOT NULL,
	[parenti37_codigo] [varchar](50) NOT NULL,
	[rela_parenti01] [int] NULL,
	[rela_parenti02] [int] NULL,
	[parenti37_empresa] [varchar](50) NULL,
	[parenti37_baja] [bit] NOT NULL,
	[rela_parenti04] [int] NULL,
	[rela_parenti09] [int] NULL,
	[parenti37_genera_despachos] [bit] NULL,
	[parenti37_inicio_automatico] [bit] NULL,
	[parenti37_horario_inicio] [datetime] NULL)

	INSERT INTO #CENTROS
	SELECT A.*
	FROM [dbo].[par.par_enti_37_cab_centro_costos] A
	INNER JOIN [dbo].[fn_GetCentrosDeCostosForUser](@userId) B ON B.rela_parenti37 = A.id_parenti37
	WHERE A.parenti37_baja = 0

	IF (SELECT COUNT(*) FROM @empresasIds) = 0 OR (SELECT COUNT(*) FROM @empresasIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraEmpresa = 0;
	IF (SELECT COUNT(*) FROM @lineasIds) = 0 OR (SELECT COUNT(*) FROM @lineasIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraLinea = 0;
	IF (SELECT COUNT(*) FROM @deptosIds) = 0 OR (SELECT COUNT(*) FROM @deptosIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraDepto = 0;

	SELECT A.*
	FROM #CENTROS A
	WHERE (@filtraEmpresa = 0 OR A.rela_parenti01 IS NULL
			OR A.rela_parenti01 IN (SELECT IntNum FROM @empresasIds))
	AND (@filtraLinea = 0 OR A.rela_parenti02 IS NULL
			OR A.rela_parenti02 IN (SELECT IntNum FROM @lineasIds))
	AND (@filtraDepto = 0 OR A.rela_parenti04 IS NULL
			OR A.rela_parenti04 IN (SELECT IntNum FROM @deptosIds))
	AND A.parenti37_baja = 0
		
END
GO


