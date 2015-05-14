-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		evecchio
-- Create date: <Create Date,,>
-- Description:	
-- =============================================
ALTER PROCEDURE sp_urb_lomax_insert_ticket
	@sourcehost varchar(50),  -- maquina origen del ticket
	@sourcefile varchar(50),  -- archivo origen del ticket
	@codigo int,  -- codigo legacy del ticket
	@canceled bit,  -- booleano true si esta cancelado
	@Z3 varchar(10), -- rela a parenti01_interno (camion)	
	@Z10 varchar(10), -- rela a parenti18_codigo (planta)
	@Z14 varchar(10), -- rela a parenti06_legajo (chofer)
	@Z19 varchar(10), -- rela a perenti19_codigo (obra - todavia no existe)
	@Z9 varchar(20), -- Fechar del Ticket
	@Z75 varchar(10), -- Hora de Ticket
	@Z152 varchar(10), -- Hora de descarga
	@Z153 varchar(50), -- user data 1
	@Z154 varchar(50), -- user data 2
	@Z157 varchar(50) -- user data 3
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO opetick01 
		(opetick01_sourcestation, opetick01_sourcefile, opetick01_codigo, 
		opetick01_canceled, 
		rela_parenti03, 	
		rela_parenti18, 
		rela_parenti09,
		rela_parenti08,
		opetick01_fecha_ticket,  
		opetick01_fecha_descarga, 
		opetick01_userfield1, 
		opetick01_userfield2,
		opetick01_userfield3) 
		SELECT @sourcehost, @sourcefile, @codigo, @canceled, 
		 (SELECT id_parenti03 FROM parenti03 WHERE parenti03_interno = @Z3), 
		 (SELECT id_parenti18 FROM parenti18 WHERE parenti18_codigo = @Z10),
		 (SELECT id_parenti09 FROM parenti09 WHERE parenti09_legajo = @Z14),
		 (SELECT rela_parenti08 FROM parenti03 WHERE parenti03_interno = @Z3),
		 (SELECT CONVERT(datetime, @Z9 + ' ' + @Z75, 105)),
		 (SELECT CONVERT(datetime, @Z9 + ' ' + @Z152, 105)),
		  @Z153,@Z154,@Z157;
    
END
GO
