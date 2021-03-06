USE [logictracker]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetCochesForUser]    Script Date: 18/02/2016 13:06:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[fn_GetCochesForUser]
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
	DECLARE @xTransportista BIT = 0;

	SELECT	@xCoche = A.socus01_porcoche,
			@xTransportista = A.socus01_portransportista
	FROM	[DBO].[soc.soc_usua_01_cab_usuarios] A
	WHERE	A.id_socusua01 = @userId

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
			OR (A.rela_parenti07 IS NULL AND @xTransportista = 0))
		AND (A.rela_parenti37 IN (SELECT rela_parenti37 FROM [dbo].[fn_GetCentrosDeCostosForUser] (@userId))
			OR A.rela_parenti37 IS NULL)		
	END
	
	RETURN
END

