USE [logictracker]
GO

/****** Object:  StoredProcedure [dbo].[sp_EmpresaDAO_GetEmpresasPermitidasPorUsuario]    Script Date: 06/05/2015 16:52:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_EmpresaDAO_GetEmpresasPermitidasPorUsuario]
	@userId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT rela_parenti01 INTO #IDS FROM [dbo].[fn_GetEmpresasForUser](@userId);

	SELECT A.*
	FROM [dbo].[par.par_enti_01_cab_empresas] A
	INNER JOIN #IDS B
			ON B.rela_parenti01 = A.id_parenti01
	WHERE A.parenti01_baja = 0
	ORDER BY A.parenti01_razsoc
END

GO


