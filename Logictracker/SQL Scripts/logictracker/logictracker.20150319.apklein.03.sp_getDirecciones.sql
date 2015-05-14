CREATE PROCEDURE [dbo].[sp_getDirecciones]
	@ids DBO.IntTable READONLY
AS
BEGIN
	SELECT A.*
		FROM [par.par_geom_01_mov_direcciones] A
			INNER JOIN @ids B
				ON A.id_pargeom01 = B.IntNum
END
