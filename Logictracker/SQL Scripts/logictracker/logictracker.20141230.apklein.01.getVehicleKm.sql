ALTER FUNCTION [dbo].[fn_getVehicleKm]
(
	@parenti03 int,
	@date_from datetime,
	@date_to datetime
)
RETURNS float
AS
BEGIN
	DECLARE @result AS float;
	
	WITH POSI AS (
		SELECT ROW_NUMBER() OVER (ORDER BY opeposi01_fechora ASC, id_OPEPOSI01 ASC) AS [ORD], opeposi01_longitud, opeposi01_latitud
			FROM [ope.ope_posi_01_log_posiciones]
			WHERE RELA_PARENTI03 = @parenti03
				AND OPEPOSI01_FECHORA 
						BETWEEN @date_from AND @date_to
	)
		SELECT @result = SUM(
								geography::STGeomFromText('POINT(' + CAST(P.opeposi01_longitud AS NVARCHAR(MAX)) + ' ' + CAST(P.opeposi01_latitud AS NVARCHAR(MAX)) + ')', 4326).STDistance(
									geography::STGeomFromText('POINT(' + CAST(P2.opeposi01_longitud AS NVARCHAR(MAX)) + ' ' + CAST(P2.opeposi01_latitud AS NVARCHAR(MAX)) + ')', 4326))/1000)
			FROM POSI AS P
			INNER JOIN POSI AS P2
				ON P.[ORD] = P2.[ORD] + 1
	
	RETURN @result;

END