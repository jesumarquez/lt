CREATE PROCEDURE [dbo].[sp_getPoligonos]
	@ids DBO.IntTable READONLY
AS
BEGIN
	SELECT A.*
		FROM [par.par_geom_03_cab_poligono] A
			INNER JOIN @ids B
				ON A.id_pargeom03 = B.IntNum
END
