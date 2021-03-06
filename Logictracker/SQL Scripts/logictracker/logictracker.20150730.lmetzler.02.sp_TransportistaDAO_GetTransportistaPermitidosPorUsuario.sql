USE [logictracker]
GO
/****** Object:  StoredProcedure [dbo].[sp_TransportistaDAO_GetTransportistasPermitidosPorUsuario]    Script Date: 30/07/2015 16:12:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_TransportistaDAO_GetTransportistasPermitidosPorUsuario]
	@empresasIds DBO.IntTable READONLY,
	@lineasIds DBO.IntTable READONLY,
	@userId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @filtraEmpresa BIT = 1;
	DECLARE @filtraLinea BIT = 1;

	CREATE TABLE #TRANSPORTISTAS (	
	[id_parenti07] [int] NOT NULL,
	[parenti07_descri] [varchar](64) NOT NULL,
	[parenti07_tarifa_tramo_corto] [float] NULL,
	[parenti07_tarifa_tramo_largo] [float] NULL,
	[parenti07_contacto] [varchar](128) NULL,
	[parenti07_telefono] [varchar](20) NULL,
	[parenti07_mail] [varchar](128) NULL,
	[rela_parenti01] [int] NULL,
	[rela_parenti02] [int] NULL,
	[rela_parenti05] [int] NULL,
	[parenti07_baja] [bit] NOT NULL,
	[parenti07_identifica_choferes] [bit] NOT NULL,
	[parenti07_costo_por_bulto] [float] NULL,
	[parenti07_costo_por_hora] [float] NULL,
	[parenti07_costo_por_km] [float])

	INSERT INTO #TRANSPORTISTAS
	SELECT	A.id_parenti07,
			A.parenti07_descri,
			A.parenti07_tarifa_tramo_corto,
			A.parenti07_tarifa_tramo_largo,
			A.parenti07_contacto,
			A.parenti07_telefono,
			A.parenti07_mail,
			A.rela_parenti01,
			A.rela_parenti02,
			A.rela_parenti05,
			A.parenti07_baja,
			A.parenti07_identifica_choferes,
			A.parenti07_costo_por_bulto,
			A.parenti07_costo_por_hora,
			A.parenti07_costo_por_km
	FROM [dbo].[par.par_enti_07_tbl_transportistas] A
	INNER JOIN [dbo].[fn_GetTransportistasForUser](@userId) B ON B.rela_parenti07 = A.id_parenti07
	WHERE A.parenti07_baja = 0	

	IF (SELECT COUNT(*) FROM @empresasIds) = 0 OR (SELECT COUNT(*) FROM @empresasIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraEmpresa = 0;
	IF (SELECT COUNT(*) FROM @lineasIds) = 0 OR (SELECT COUNT(*) FROM @lineasIds WHERE IntNum IN (0,-1)) > 0
		SET @filtraLinea = 0;
	
	SELECT	A.id_parenti07,
			A.parenti07_descri,
			A.parenti07_tarifa_tramo_corto,
			A.parenti07_tarifa_tramo_largo,
			A.parenti07_contacto,
			A.parenti07_telefono,
			A.parenti07_mail,
			A.rela_parenti01,
			A.rela_parenti02,
			A.rela_parenti05,
			A.parenti07_baja,
			A.parenti07_identifica_choferes,
			A.parenti07_costo_por_bulto,
			A.parenti07_costo_por_hora,
			A.parenti07_costo_por_km
	FROM #TRANSPORTISTAS A
	WHERE (@filtraEmpresa = 0 OR A.rela_parenti01 IS NULL
			OR A.rela_parenti01 IN (SELECT IntNum FROM @empresasIds))
	AND (@filtraLinea = 0 OR A.rela_parenti02 IS NULL
			OR A.rela_parenti02 IN (SELECT IntNum FROM @lineasIds))
	ORDER BY A.parenti07_descri ASC	
END