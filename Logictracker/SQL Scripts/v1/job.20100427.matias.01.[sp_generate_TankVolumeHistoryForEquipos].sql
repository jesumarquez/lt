USE [urbetrack]
GO
/****** Object:  StoredProcedure [dbo].[sp_generate_TankVolumeHistory]    Script Date: 04/20/2010 16:20:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


alter PROCEDURE [dbo].[sp_generate_TankVolumeHistoryForEquipos](@idTanque int, @idEquipo int)
AS
BEGIN
	declare @lastTankVolume float
	declare @lastUpdateDate datetime
	declare @volume float
	declare @date datetime
	
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
	/*Trae todos los consumos a partir de esa fecha ordenados ascendentemente*/
	declare cur_dispatchs cursor for
	select case when (rela_opecomb01 = @idConsumo or rela_opecomb01 = @idConciliacionEgreso) then opecomb02_volumen * -1
									  else opecomb02_volumen end,
		   opecomb02_fecha
	from opecomb02
	join parenti39 on rela_parenti39 = id_parenti39
	where rela_opecomb01 in (@idConciliacionIngreso,@idConciliacionEgreso,@idConsumo,@idIngreso)
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
					@lastTankVolume,	--volumen del tanque a esa fecha 
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
END




