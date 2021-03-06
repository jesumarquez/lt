USE [urbetrack]
GO

/****** Object:  StoredProcedure [dbo].[sp_generateAllTanksTeoricVolume]    Script Date: 04/27/2010 13:05:44 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_generateAllTanksTeoricVolume]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_generateAllTanksTeoricVolume]
GO


USE [urbetrack]
GO
/****** Object:  StoredProcedure [dbo].[sp_generateAllTanksTeoricVolume]    Script Date: 04/20/2010 16:17:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_generate_AllTanksTeoricVolume]
AS
BEGIN

	declare @tankID int
	declare @equipID int
	
	BEGIN TRAN
		/*cursor para procesar los tanques de las Plantas*/
		declare cur_tanks cursor for
		select id_parenti36 from parenti36 where rela_parenti02 is not null
		
		open cur_tanks
		fetch cur_tanks into @tankID

			while @@FETCH_STATUS = 0
			begin
				exec [dbo].[sp_generate_TankVolumeHistory] @tankID
				fetch cur_tanks into @tankID
			end

		close cur_tanks
		deallocate cur_tanks
	COMMIT TRAN
	
	BEGIN TRAN
		/*cursor para procesar los Tanques de los Equipos*/
		declare cur_equip_tanks cursor for
		select id_parenti36,rela_parenti19 from parenti36 where rela_parenti19 is not null
		
		open cur_equip_tanks
		fetch cur_equip_tanks into @tankID, @equipID
		
		while @@FETCH_STATUS = 0
		begin
			exec [dbo].[sp_generate_TankVolumeHistoryForEquipos] @tankID,@equipID
			fetch cur_equip_tanks into @tankID, @equipID	
		end
		
		close cur_equip_tanks
		deallocate cur_equip_tanks
	COMMIT TRAN

END
