USE [logictracker]
GO

/****** Object:  StoredProcedure [dbo].[sp_LineaDAO_GetLineasPermitidasPorUsuario]    Script Date: 06/05/2015 16:52:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_LineaDAO_GetLineasPermitidasPorUsuario]
	@empresasIds DBO.IntTable READONLY,
	@userId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @filtraEmpresa BIT = 1;

	CREATE TABLE #LINEAS(
	[id_parenti02] [int] NOT NULL,
	[rela_parenti01] [int] NOT NULL,
	[parenti02_descri] [varchar](255) NOT NULL,
	[parenti02_descricorta] [varchar](8) NOT NULL,
	[parenti02_baja] [bit] NOT NULL,
	[rela_parenti05] [int] NULL,
	[pe02_telefono] [varchar](32) NOT NULL,
	[pe02_mail] [varchar](64) NOT NULL,
	[pe02_time_zone_id] [varchar](50) NULL,
	[parenti02_interfaceable] [bit] NOT NULL,
	[parenti02_identifica_choferes] [bit] NOT NULL)

	INSERT INTO #LINEAS
	SELECT A.*
	FROM [dbo].[par.par_enti_02_det_lineas] A
	INNER JOIN [dbo].[fn_GetLineasForUser](@userId) B ON B.rela_parenti02 = A.id_parenti02
	WHERE A.parenti02_baja = 0

	IF (SELECT COUNT(*) FROM @empresasIds) = 0 OR (SELECT COUNT(*) FROM @empresasIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraEmpresa = 0;

	SELECT A.*
	FROM #LINEAS A
	WHERE (@filtraEmpresa = 0 OR A.rela_parenti01 IS NULL
			OR A.rela_parenti01 IN (SELECT IntNum FROM @empresasIds))
	AND A.parenti02_baja = 0
	ORDER BY A.parenti02_descri ASC

END
GO


