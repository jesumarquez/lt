--exec [dbo].[sp_getPoligonosVigentes] @company = 22
CREATE PROCEDURE [dbo].[sp_getPoligonosVigentes]
@today AS DATETIME = NULL,
@company AS INT = NULL,
@branch AS INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (@today IS NULL)
		SET @today = GETUTCDATE();

	IF (@company IS NULL)
		RETURN;

	CREATE TABLE #EMPRESA_LINEA
	(RELA_PARENTI01 INT, RELA_PARENTI02 INT)

	IF (@company IS NOT NULL AND @company != -1)
	IF (@branch IS NULL OR @branch = -1)
	BEGIN
		INSERT INTO #EMPRESA_LINEA
		SELECT A.ID_PARENTI01, B.ID_PARENTI02			
			FROM [par.par_enti_01_cab_empresas] A
				INNER JOIN [par.par_enti_02_det_lineas] B
					ON A.id_parenti01 = B.rela_parenti01
			WHERE A.id_parenti01 = @company;
	END
	ELSE
	BEGIN
		INSERT INTO #EMPRESA_LINEA
		SELECT A.ID_PARENTI01, B.ID_PARENTI02
			FROM [par.par_enti_01_cab_empresas] A
				INNER JOIN [par.par_enti_02_det_lineas] B
					ON A.id_parenti01 = B.rela_parenti01						
			WHERE A.id_parenti01 = @company AND B.id_parenti02 = @branch;
	END;
		
	WITH REFGEO AS (
		SELECT A.*
			FROM [par.par_enti_05_mov_referencias] A
			WHERE (A.parent05_vigencia_desde <= @today OR A.parent05_vigencia_desde IS NULL)
				AND (A.parent05_vigencia_hasta > @today OR A.parent05_vigencia_hasta IS NULL)
				AND A.parenti05_baja = 0
				AND A.rela_parenti01 = @company AND A.rela_parenti02 IS NULL
		UNION
		SELECT A.*
			FROM [par.par_enti_05_mov_referencias] A
				INNER JOIN #EMPRESA_LINEA B
					ON A.rela_parenti01 = B.rela_parenti01 AND (A.rela_parenti02 = B.rela_parenti02)
			WHERE (A.parent05_vigencia_desde <= @today OR A.parent05_vigencia_desde IS NULL)
				AND (A.parent05_vigencia_hasta >= @today OR A.parent05_vigencia_hasta IS NULL)
				AND A.parenti05_baja = 0
	), GEOREF AS (
		SELECT C.*, ROW_NUMBER() OVER (PARTITION BY C.rela_parenti05 ORDER BY C.id_pargeom02 DESC) AS RN
			FROM [par.par_geom_02_mov_historia] C	-- HISTORIA GEOREF
			WHERE	(C.pargeom02_vigencia_desde <= @today OR C.pargeom02_vigencia_desde IS NULL)
					AND (C.pargeom02_vigencia_hasta >= @today OR C.pargeom02_vigencia_hasta IS NULL)
	)

	SELECT P.*, E.*
		FROM [par.par_geom_03_cab_poligono] P
			LEFT OUTER JOIN [par.par_geom_04_det_punto] E
				ON E.rela_pargeom03 = P.id_pargeom03
			INNER JOIN (
				SELECT	D.*
					FROM [REFGEO] A													-- REFERENCIA GEOGRAFICA
						INNER JOIN  [par.par_enti_10_tbl_tiporeferencia]	B		-- TIPO DE REFERENCIA GEOGRAFICA
							ON A.rela_parenti10 = B.id_parenti10
						LEFT OUTER JOIN [GEOREF] C									-- HISTORIA GEOREF
							ON C.rela_parenti05 = A.id_parenti05
							AND C.RN = 1
						INNER JOIN [par.par_geom_03_cab_poligono] D			-- POLIGONOS
							ON D.id_pargeom03 = C.rela_pargeom03
							AND (D.pargeom03_vigencia_desde <= @today OR D.pargeom03_vigencia_desde IS NULL)
							AND (D.pargeom03_vigencia_hasta >= @today OR D.pargeom03_vigencia_hasta IS NULL)
				
			) D
				ON D.ID_PARGEOM03 = P.ID_PARGEOM03
		ORDER BY ID_PARGEOM03, E.ID_PARGEOM04
END