set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO


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

	if(@lastTankVolume is null) /*buca el 1º volumen real del tanque*/
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
/*************************************************************************************************************************/
	/*Trae todos los consumos e ingresos a partir de esa fecha ordenados ascendentemente*/
	declare cur_dispatchs cursor for
	select case when (rela_opecomb01 = @idConsumo or rela_opecomb01 = @idConciliacionEgreso) then opecomb02_volumen * -1
									  else opecomb02_volumen end,
		   opecomb02_fecha
	from opecomb02
	join parenti39 on rela_parenti39 = id_parenti39
	where rela_opecomb01 in (@idConsumo,@idIngreso)
	and rela_parenti19 = @idEquipo
	and opecomb02_fecha > @lastUpdateDate
	and opecomb02_automatico = 1
	order by opecomb02_fecha

	open cur_dispatchs
	fetch cur_dispatchs into @volume, @date
	
	while @@FETCH_STATUS = 0
	begin
			/*calcula el volumen teorico y lo inserta */
			set @lastTankVolume = @lastTankVolume + @volume
			
			insert into opecomb03
			values (@date,						--fecha
					@lastTankVolume,	        --volumen del tanque a esa fecha 
					1,							--Tipo = 1 porque es un volumen teorico
					@idTanque)					--Tanque
			
		    fetch cur_dispatchs into @volume, @date
	end

	close cur_dispatchs
	deallocate cur_dispatchs

	/*Actualiza el valor del último volumen teórico*/
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
		and opecomb03_fecha < @date
		order by opecomb03_fecha desc 
		
		/*insertes the new volume*/
		insert into opecomb03 values(@date,@volumenAnterior +
									 case when @idTipoMov = @idConciliacionEgreso
											  then -1*@volume
											  else @volume end,1,@idTanque)
		
		/*updates all the next volumes*/
		update opecomb03 set opecomb03_volumen = opecomb03_volumen + 
										case when @idTipoMov = @idConciliacionEgreso
											  then -1*@volume
											  else @volume end
		where rela_parenti36 = @idTanque
		and opecomb03_tipo = 1
		and opecomb03_fecha > @date 
		
		/*sets the conciliacion as processed*/
		update opecomb02 set opecomb02_processed = 1
		where id_opecomb02 = @id
		
		fetch cur_conciliaciones into @volume,@date,@id,@idTipoMov
	end
	
	close cur_conciliaciones
	deallocate cur_conciliaciones
END






