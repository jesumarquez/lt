USE [logictracker]
GO

/****** Object:  UserDefinedFunction [dbo].[fn_GetEmpresasForUser]    Script Date: 06/05/2015 16:36:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fn_GetEmpresasForUser]
(
	@userId INT
)
RETURNS @returnTable TABLE 
(
	[rela_parenti01] INT NOT NULL
)
AS
BEGIN
	DECLARE @xEmpresa BIT = 0;
	SELECT @xEmpresa = A.socus01_porempresa
	FROM [DBO].[soc.soc_usua_01_cab_usuarios] A
	WHERE A.id_socusua01 = @userId;

	IF (@xEmpresa = 1)
		INSERT INTO @returnTable (rela_parenti01)
		SELECT A.rela_parenti01			
		FROM [dbo].[soc.soc_usua_10_mov_empresas] A
		INNER JOIN [dbo].[par.par_enti_01_cab_empresas] B
				ON B.id_parenti01 = A.rela_parenti01
				AND B.parenti01_baja = 0
		WHERE A.rela_socusua01 = @userId					
	ELSE
		INSERT INTO @returnTable (rela_parenti01)
		SELECT A.id_parenti01
		FROM [dbo].[par.par_enti_01_cab_empresas] A
		WHERE A.parenti01_baja = 0
	RETURN 
END

GO


CREATE FUNCTION [dbo].[fn_GetLineasForUser]
(
	@userId INT
)
RETURNS @returnTable TABLE 
(
	[rela_parenti02] INT NOT NULL
)
AS
BEGIN
	DECLARE @xLinea BIT = 0;
	SELECT @xLinea = A.socus01_porlinea
	FROM [DBO].[soc.soc_usua_01_cab_usuarios] A
	WHERE A.id_socusua01 = @userId

	IF (@xLinea = 1)
		INSERT INTO @returnTable (rela_parenti02)
		SELECT A.rela_parenti02			
		FROM [dbo].[soc.soc_usua_11_mov_lineas] A
		INNER JOIN [dbo].[par.par_enti_02_det_lineas] B
				ON B.id_parenti02 = A.rela_parenti02
				AND B.parenti02_baja = 0
		WHERE A.rela_socusua01 = @userId					
	ELSE
		INSERT INTO @returnTable (rela_parenti02)
		SELECT A.id_parenti02
		FROM [dbo].[par.par_enti_02_det_lineas] A
		WHERE (A.rela_parenti01 IN (SELECT rela_parenti01 FROM [dbo].[fn_GetEmpresasForUser](@userId))
			OR A.rela_parenti01 IS NULL)
		AND A.parenti02_baja = 0
	RETURN 
END

GO

CREATE FUNCTION [dbo].[fn_GetCentrosDeCostosForUser]
(
	@userId INT
)
RETURNS @returnTable TABLE 
(
	[rela_parenti37] INT NOT NULL
)
AS
BEGIN
	DECLARE @xCentroDeCostos BIT = 0;
	SELECT @xCentroDeCostos = A.socus01_porcentrocostos
	FROM [DBO].[soc.soc_usua_01_cab_usuarios] A
	WHERE A.id_socusua01 = @userId

	IF (@xCentroDeCostos = 1)
	BEGIN
		INSERT INTO @returnTable (rela_parenti37)
		SELECT A.rela_parenti37
		FROM [dbo].[soc.soc_usua_12_mov_centros_costos] A
		INNER JOIN [dbo].[par.par_enti_37_cab_centro_costos] B
				ON	B.id_parenti37 = A.rela_parenti37
				AND B.parenti37_baja = 0
		WHERE A.rela_socusua01 = @userId
	END
	ELSE
	BEGIN
		INSERT INTO @returnTable (rela_parenti37)
		SELECT A.id_parenti37
		FROM [dbo].[par.par_enti_37_cab_centro_costos] A
		WHERE (A.rela_parenti01 IN (SELECT rela_parenti01 FROM [dbo].[fn_GetEmpresasForUser](@userId))
			OR A.rela_parenti01 IS NULL)
		AND	(A.rela_parenti02 IN (SELECT rela_parenti02 FROM [dbo].[fn_GetLineasForUser](@userId))
			OR A.rela_parenti02 IS NULL)
		AND A.parenti37_baja = 0		
	END	
	
	RETURN 
END

GO

CREATE FUNCTION [dbo].[fn_GetTransportistasForUser]
(
	@userId INT
)
RETURNS @returnTable TABLE 
(
	[rela_parenti07] INT NOT NULL
)
AS
BEGIN
	DECLARE @xTransportista BIT = 0;
	SELECT @xTransportista = A.socus01_portransportista
	FROM [DBO].[soc.soc_usua_01_cab_usuarios] A
	WHERE A.id_socusua01 = @userId

	IF (@xTransportista = 1)
	BEGIN
		INSERT INTO @returnTable (rela_parenti07)
		SELECT A.rela_parenti07
		FROM [dbo].[soc.soc_usua_09_mov_transportistas] A
		INNER JOIN [dbo].[par.par_enti_07_tbl_transportistas] B
				ON	B.id_parenti07 = A.rela_parenti07
				AND B.parenti07_baja = 0
		WHERE A.rela_socusua01 = @userId
	END
	ELSE
	BEGIN
		INSERT INTO @returnTable (rela_parenti07)
		SELECT A.id_parenti07
		FROM [dbo].[par.par_enti_07_tbl_transportistas] A
		WHERE (A.rela_parenti01 IN (SELECT rela_parenti01 FROM [dbo].[fn_GetEmpresasForUser](@userId))
			OR A.rela_parenti01 IS NULL)
		AND	(A.rela_parenti02 IN (SELECT rela_parenti02 FROM [dbo].[fn_GetLineasForUser](@userId))
			OR A.rela_parenti02 IS NULL)
		AND A.parenti07_baja = 0
	END	

	RETURN
END

GO

CREATE FUNCTION [dbo].[fn_GetCochesForUser]
(
	@userId INT
)
RETURNS @returnTable TABLE 
(
	[rela_parenti03] INT NOT NULL
)
AS
BEGIN
	DECLARE @xCoche BIT = 0;
	SELECT @xCoche = A.socus01_porcoche
	FROM [DBO].[soc.soc_usua_01_cab_usuarios] A
	WHERE A.id_socusua01 = @userId

	IF (@xCoche = 1)
	BEGIN
		INSERT INTO @returnTable (rela_parenti03)
		SELECT A.rela_parenti03
		FROM [dbo].[soc.soc_usua_04_mov_coches] A
		INNER JOIN [dbo].[par.par_enti_03_cab_coches] B
				ON	B.id_parenti03 = A.rela_parenti03
		WHERE A.rela_socusua01 = @userId
	END
	ELSE
	BEGIN
		INSERT INTO @returnTable (rela_parenti03)
		SELECT A.id_parenti03
		FROM [dbo].[par.par_enti_03_cab_coches] A
		WHERE (A.rela_parenti01 IN (SELECT rela_parenti01 FROM [dbo].[fn_GetEmpresasForUser] (@userId))
			OR A.rela_parenti01 IS NULL)
		AND (A.rela_parenti02 IN (SELECT rela_parenti02 FROM [dbo].[fn_GetLineasForUser] (@userId))
			OR A.rela_parenti02 IS NULL)
		AND (A.rela_parenti07 IN (SELECT rela_parenti07 FROM [dbo].[fn_GetTransportistasForUser] (@userId))
			OR A.rela_parenti07 IS NULL)
		AND (A.rela_parenti37 IN (SELECT rela_parenti37 FROM [dbo].[fn_GetCentrosDeCostosForUser] (@userId))
			OR A.rela_parenti37 IS NULL)		
	END
	
	RETURN
END


GO


