--exec [dbo].[sp_getReferenciasGeoVigentes] @company = 44
CREATE PROCEDURE [dbo].[sp_getReferenciasGeoVigentes]
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

	SELECT	
			A.id_parenti05 AS Id,
			A.parenti05_descri AS Descripcion,
			A.parent05_vigencia_desde AS Inicio,
			A.parent05_vigencia_hasta AS Fin,
			A.parenti05_timetrack_inicio AS EsInicio,
			A.parenti05_timetrack_intermedio AS EsIntermedio,
			A.parenti05_timetrack_fin AS EsFin,
			B.id_parenti10 AS TipoReferenciaGeograficaId ,
			B.parenti10_controla_entrada_salida AS ControlaEntradaSalida,
			B.parenti10_controla_velocidad AS ControlaVelocidad,
			B.parenti10_controla_permanencia AS ControlaPermanencia,
			B.parenti10_controla_permanencia_entrega AS ControlaPermanenciaEntrega,
			B.parenti10_maxima_permanencia AS MaximaPermanencia,
			B.parenti10_maxima_permanencia_entrega AS MaximaPermanenciaEntrega,
			B.parenti10_es_taller AS EsTaller,

			C.id_pargeom02 AS HistoriaGeoId,
			D.id_pargeom03 AS PoligonoId,
			E.id_pargeom01 As DireccionId

		FROM [REFGEO] A													-- REFERENCIA GEOGRAFICA
			INNER JOIN  [par.par_enti_10_tbl_tiporeferencia]	B		-- TIPO DE REFERENCIA GEOGRAFICA
				ON A.rela_parenti10 = B.id_parenti10
			LEFT OUTER JOIN [GEOREF] C									-- HISTORIA GEOREF
				ON C.rela_parenti05 = A.id_parenti05
				AND C.RN = 1
			LEFT OUTER JOIN [par.par_geom_03_cab_poligono] D			-- POLIGONOS
				ON D.id_pargeom03 = C.rela_pargeom03
					AND (D.pargeom03_vigencia_desde <= @today OR D.pargeom03_vigencia_desde IS NULL)
					AND (D.pargeom03_vigencia_hasta >= @today OR D.pargeom03_vigencia_hasta IS NULL)
			LEFT OUTER JOIN [par.par_geom_01_mov_direcciones] E			-- DIRECCIONES
				ON E.id_pargeom01 = C.rela_pargeom01
		ORDER BY Id
END