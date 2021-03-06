USE [urbetrack]
GO
/****** Object:  StoredProcedure [dbo].[sp_generateAllTanksTeoricVolume]    Script Date: 04/20/2010 12:46:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_generate_AllMotorMovements]
AS
BEGIN

	declare @engineID int

	declare cur_motor cursor for
	select id_parenti39 from parenti39

	open cur_motor
	fetch cur_motor into @engineID

	while @@FETCH_STATUS = 0
	begin
		exec [dbo].[sp_generate_MotorMovement] @engineID
		fetch cur_motor into @engineID
	end

	close cur_motor
	deallocate cur_motor
END
