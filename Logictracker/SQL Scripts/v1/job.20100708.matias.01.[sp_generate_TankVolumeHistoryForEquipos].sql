set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go



ALTER PROCEDURE [dbo].[sp_generate_TankVolumeHistoryForEquipos](@idTanque int, @idEquipo int)
AS
BEGIN
	declare @lastTankVolume float
	declare @lastUpdateDate datetime
	declare @volume float
	declare @date datetime
	declare @id int
	declare @idTipoMov int
	
	declare @idConciliacionIngreso int
	declare @idConciliacionEgreso int
	declare @idConsumo int
	declare @idIngreso int

	/*Finds the Id of the Tipo Conciliacion Ingreso movement*/
	select top 1 @idConciliacionIngreso = id_opecomb01
	from opecomb01
	where opecomb01_codigo = 'C'
	
	/*Finds the Id of the Tipo Conciliacion Egreso movement*/
	select top 1 @idConciliacionEgreso = id_opecomb01
	from opecomb01
	where opecomb01_codigo = 'E'

	/*Finds the Id of the Tipo Consumo movement*/
	select top 1 @idConsumo = id_opecomb01
	from opecomb01
	where opecomb01_codigo = 'M'
	
	/*Finds the Id of the Tipo Ingreso al Tanque movement*/
	select top 1 @idIngreso = id_opecomb01
	from opecomb01
	where opecomb01_codigo = 'I'

	/*Sets the last Tank Update date and volume*/
    select top 1 @lastUpdateDate = opecomb03_fecha,
				@lastTankVolume = opecomb03_volumen
	from opecomb03
	where rela_parenti36 = @idTanque
		  and opecomb03_tipo = 1 
	order by opecomb03_fecha desc

	if(@lastTankVolume is null) /*buca el 1� volumen real del tanque*/
	begin
		/*Sets the last Tank Update date and volume*/
		select top 1 @lastUpdateDate = opecomb03_fecha,
					@lastTankVolume = opecomb03_volumen
		from opecomb03
		where rela_parenti36 = @idTanque
			  and opecomb03_tipo = 0
		order by opecomb03_fecha asc
	end
	/*si no se pudo ubicar una fecha y vol, los setea con valores minimos*/
	if(@lastTankVolume is null) set @lastTankVolume = 0
	if(@lastUpdateDate is null) set @lastUpdateDate = '1900-01-01'
/***********************************************************************************************************/
/*controla los vol�menes 0 para los tanques*/
	declare @lastVolume float
	declare @lastDate datetime
	declare @waterVolume float 
	
    select top 1 @lastDate = opecomb03_fecha,
				@lastVolume = opecomb03_volumen
	from opecomb03
	where rela_parenti36 = @idTanque
		  and opecomb03_tipo = 0
		  and opecomb03_fecha <= @lastUpdateDate 
	order by opecomb03_fecha desc

	declare cur_invalid_tank_volumes cursor for
	select opecomb03_volumen, opecomb03_fecha, id_opecomb03, opecomb03_volumen_agua
	from opecomb03
	where rela_parenti36 = @idTanque
	and opecomb03_tipo = 0
	and opecomb03_fecha >= @lastUpdateDate
	order by opecomb03_fecha asc

	open cur_invalid_tank_volumes
	fetch cur_invalid_tank_volumes into @volume,@date, @id, @waterVolume

	while @@FETCH_STATUS = 0
	begin
		
		if(((@lastVolume - @volume) >= 4000 or (@volume - @lastVolume) >= 5000) 
			and (DATEDIFF(mm,@date,@lastDate) <= 5))
		begin
			insert into opecomb05 values(@date,@volume,0,@idTanque,@waterVolume,GETDATE(),0)
			
			delete from opecomb03 where id_opecomb03 = @id
		end
		set @lastVolume = @volume
		set @lastDate = @date 
		fetch cur_invalid_tank_volumes into @volume,@date,@id, @waterVolume	
	end

	close cur_invalid_tank_volumes
	deallocate cur_invalid_tank_volumes

/*************************************************************************************************************************/
	/*Trae todos los consumos e ingresos a partir de esa fecha ordenados ascendentemente*/
	declare cur_dispatchs cursor for
	select case when (rela_opecomb01 = @idConsumo) then opecomb02_volumen * -1
									  else opecomb02_volumen end,
		   opecomb02_fecha,
		   id_opecomb02
	from opecomb02
	join parenti39 on rela_parenti39 = id_parenti39
	where rela_opecomb01 in (@idConsumo,@idIngreso)
	and rela_parenti19 = @idEquipo
	and opecomb02_processed = 0
	and opecomb02_automatico = 1
	order by opecomb02_fecha

	open cur_dispatchs
	fetch cur_dispatchs into @volume, @date, @id
	
	while @@FETCH_STATUS = 0
	begin
	    /*obtains the last teoric volume*/
		select top 1 @lastTankVolume = opecomb03_volumen
		from opecomb03
		where rela_parenti36 = @idTanque
		and opecomb03_tipo = 1
		and opecomb03_fecha <= @date
		order by opecomb03_fecha desc,id_opecomb03 desc
		
		/*Si no existe volumen teorico usa como inicial el volumen real*/
		if(@lastTankVolume is null)
		begin
		    select top 1 @lastTankVolume = opecomb03_volumen
			from opecomb03
			where rela_parenti36 = @idTanque
			and opecomb03_tipo = 0
			and opecomb03_fecha <= @date
			order by opecomb03_fecha desc, id_opecomb03 desc
			
			/*si tampoco existe volumen real anterior a esa fecha, le asigna 0*/
			if(@lastTankVolume is null) set @lastTankVolume = 0
		end
	
		insert into opecomb03
		values (@date,						--fecha
				@lastTankVolume + @volume,  --volumen del tanque a esa fecha 
				1,							--Tipo = 1 porque es un volumen teorico
				@idTanque,
				0)					--Tanque
			
		/*updates all the next volumes*/
		update opecomb03 set opecomb03_volumen = (opecomb03_volumen + @volume)
		where rela_parenti36 = @idTanque
		and opecomb03_tipo = 1
		and opecomb03_fecha > @date 
		
		/*sets the conciliacion as processed*/
		update opecomb02 set opecomb02_processed = 1 where id_opecomb02 = @id
		
		fetch cur_dispatchs into @volume, @date, @id
	end

	close cur_dispatchs
	deallocate cur_dispatchs

	/*Actualiza el valor del �ltimo volumen te�rico*/
	update parenti36
	set parenti36_volTeorico = @lastTankVolume
	where id_parenti36 = @idTanque
/*************************************************************************************************************************/
	/*procesa las conciliaciones atrasadas*/
	declare cur_conciliaciones cursor for
	select opecomb02_volumen, opecomb02_fecha, id_opecomb02, rela_opecomb01
	from opecomb02
	where rela_parenti36 = @idTanque
	and rela_opecomb01 in (@idConciliacionIngreso,@idConciliacionEgreso)
	and opecomb02_processed = 0
	
	open cur_conciliaciones
	fetch cur_conciliaciones into @volume,@date,@id,@idTipoMov

	while @@FETCH_STATUS = 0
	begin
		declare @volumenAnterior float
		
		/*obtains the last teoric volume*/
		select top 1 @volumenAnterior = opecomb03_volumen
		from opecomb03
		where rela_parenti36 = @idTanque
		and opecomb03_tipo = 1
		and opecomb03_fecha <= @date
		order by opecomb03_fecha desc, id_opecomb03 desc 
		
		/*insertes the new volume*/
		insert into opecomb03 values(@date,@volumenAnterior +
									 case when @idTipoMov = @idConciliacionEgreso
											  then -1*@volume
											  else @volume end,1,@idTanque,
											  0)
		
		/*updates all the next volumes*/
		update opecomb03 set opecomb03_volumen = opecomb03_volumen + 
										case when @idTipoMov = @idConciliacionEgreso
											  then -1*@volume
											  else @volume end
		where rela_parenti36 = @idTanque
		and opecomb03_tipo = 1
		and opecomb03_fecha > @date 
		
		/*sets the conciliacion as processed*/
		update opecomb02 set opecomb02_processed = 1 where id_opecomb02 = @id
		
		fetch cur_conciliaciones into @volume,@date,@id,@idTipoMov
	end
	
	close cur_conciliaciones
	deallocate cur_conciliaciones
END








