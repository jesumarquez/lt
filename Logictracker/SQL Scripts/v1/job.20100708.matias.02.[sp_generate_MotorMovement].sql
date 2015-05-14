set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_generate_MotorMovement] (@motorID int)
AS
BEGIN
	declare @fecha datetime
	declare @volumen float
	declare @caudal float
	declare @horometro float
	declare @temperatura float
	declare @rpm float
	declare @tipoMov int
	declare @movementID int
	declare @nroRemito varchar(50)
	declare @idTipoIngreso int
	
	declare @lastVolume float
	declare @lastUpdateDate datetime

	select top 1 @idTipoIngreso = id_opecomb01
	from opecomb01
	where opecomb01_codigo = 'I'
	
	/* Obtains the Date and Volume of the last consumo that has been processed*/
	select top 1 @lastUpdateDate = opecomb02_fecha,
				 @lastVolume = opecomb02_volumen
	from opecomb02
	where rela_parenti39 = @motorID
	and opecomb02_automatico = 0
	and opecomb02_processed = 1
	order by opecomb02_fecha desc
	
	/*obtiene todos los consumos/ingresos para ese motor*/
	declare cur_consumos cursor for 
	select id_opecomb02,
		   opecomb02_fecha,
		   opecomb02_volumen,
		   opecomb02_caudal,
		   opecomb02_horometro,
		   opecomb02_temperatura,
		   opecomb02_rpm,
		   rela_opecomb01,
		   opecomb02_observacion
	from opecomb02
	where rela_parenti39 = @motorID
	and opecomb02_automatico = 0
	and opecomb02_processed = 0
	and (opecomb02_fecha > @lastUpdateDate or @lastUpdateDate is null)
	
	open cur_consumos
	fetch cur_consumos into @movementID,@fecha,@volumen,@caudal,@horometro,@temperatura,@rpm,@tipoMov,@nroRemito
	
	set @lastVolume = ISNULL(@lastVolume,0)
	
	while @@FETCH_STATUS = 0
	begin
		/*evita que los volumenes decrezcan (siempre DEBEN aumentar en el tiempo)*/
		if((@tipoMov <> @idTipoIngreso and @volumen >= @lastVolume) or (@tipoMov = @idTipoIngreso and @volumen > @lastVolume ) )
		begin
			insert into opecomb02 
			values(	@fecha,								--fecha
					GETDATE(),							--fecha ingreso
					@volumen - ISNULL(@lastVolume,0),	--volumen consumido
					@nroRemito,								--nro remito
					1,									--Tipo : Automatico
					NULL,								--Tanque
					@motorID,							--Motor
					@tipoMov,							--Tipo de Movimiento
					NULL,								--Coche
					@caudal,
					@horometro,
					@temperatura,
					@rpm,
					0,
					null)									--Procesado = true
			
			/*Marca el consumo como ya procesado*/
			update opecomb02 set opecomb02_processed = 1 where id_opecomb02 = @movementID
			
			set @lastVolume = @volumen
		end
		
		fetch cur_consumos into @movementID,@fecha,@volumen,@caudal,@horometro,@temperatura,@rpm,@tipoMov,@nroRemito
	end
	
	close cur_consumos
	deallocate cur_consumos
END

