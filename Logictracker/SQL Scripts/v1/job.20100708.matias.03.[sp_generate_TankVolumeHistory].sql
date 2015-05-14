set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[sp_generate_TankVolumeHistory](@idTanque int)
AS
BEGIN
	declare @idRemito int
	declare @idAjuste int
	declare @idDespacho int
	declare @lastTankVolume float
	declare @lastUpdateDate datetime
	declare @volume float
	declare @date datetime
	declare @id int

	/*Finds the Id of the Tipo Remito movement*/
	select top 1 @idRemito = id_opecomb01
	from opecomb01
	where opecomb01_codigo = 'R'
	
	/*Finds the Id of the Tipo Ajuste movement*/
	select top 1 @idAjuste = id_opecomb01
	from opecomb01
	where opecomb01_codigo = 'A'

	/*Finds the Id of the Tipo Despacho movement*/
	select top 1 @idDespacho = id_opecomb01
	from opecomb01
	where opecomb01_codigo = 'D'

/*************************************************************************************************************************/
	/*procesa los remitos, ajustes  y despachos*/
	declare cur_remitos cursor for
	select case when (rela_opecomb01 = @idDespacho) then -1*opecomb02_volumen else opecomb02_volumen end, opecomb02_fecha, id_opecomb02
	from opecomb02
	where rela_parenti36 = @idTanque
	and rela_opecomb01 in (@idRemito,@idAjuste,@idDespacho)
	and opecomb02_processed = 0
	order by opecomb02_fecha
	
	declare @volumenAnterior float

	open cur_remitos
	fetch cur_remitos into @volume,@date,@id

	while @@FETCH_STATUS = 0
	begin
		
		/*obtains the last teoric volume*/
		select top 1 @volumenAnterior = opecomb03_volumen
		from opecomb03
		where rela_parenti36 = @idTanque
		and opecomb03_tipo = 1
		and opecomb03_fecha <= @date
		order by opecomb03_fecha desc, id_opecomb03 desc 
		
		/*Si no existe voluemn teorico usa como inicial el volumen real*/
		if(@volumenAnterior is null)
		begin
		    select top 1 @volumenAnterior = opecomb03_volumen
			from opecomb03
			where rela_parenti36 = @idTanque
			and opecomb03_tipo = 0
			and opecomb03_fecha <= @date
			order by opecomb03_fecha desc, id_opecomb03 desc
			
			/*si tampoco existe volumen real anterior a esa fecha, le asigna 0*/
			if(@volumenAnterior is null) set @volumenAnterior = 0
		end
		/*insertes the new volume*/
		insert into opecomb03 (opecomb03_fecha,
								   opecomb03_volumen,
								   opecomb03_tipo,
								   rela_parenti36)
		values(@date,@volumenAnterior + @volume,1,@idTanque)
		
		/*updates all the next volumes*/
		update opecomb03 set opecomb03_volumen = (opecomb03_volumen + @volume)
		where rela_parenti36 = @idTanque
		and opecomb03_tipo = 1
		and opecomb03_fecha > @date 
		
		/*sets the movemente as processed*/
		update opecomb02 set opecomb02_processed = 1 where id_opecomb02 = @id
		
		fetch cur_remitos into @volume,@date,@id
	end
	
	close cur_remitos
	deallocate cur_remitos
	
	update parenti36
	set parenti36_volTeorico = isnull((select top 1 opecomb03_volumen 
								from opecomb03 where rela_parenti36 = @idTanque and opecomb03_tipo = 1
								order by opecomb03_fecha desc ),0)
	where id_parenti36 = @idTanque
END







