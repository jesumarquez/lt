CREATE PROCEDURE [dbo].[sp_CocheDAO_GetByIds]
@vehiculosIds AS dbo.IntTable READONLY,
@estadoInactivo AS INT
AS

	SELECT A.*
		FROM [dbo].[par.par_enti_03_cab_coches] A
			INNER JOIN @vehiculosIds B
				ON B.IntNum = A.id_parenti03
	WHERE A.parenti03_estado != @estadoInactivo

GO


