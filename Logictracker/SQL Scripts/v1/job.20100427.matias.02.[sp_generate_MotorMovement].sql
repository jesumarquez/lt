CREATE PROCEDURE [dbo].[sp_generate_MotorMovement] (@motorID int)
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
	
	declare @lastVolume float
	declare @lastUpdateDate datetime
	
	/* Obtains the Date adn Volume of the last consumo that has been processed*/
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
		   rela_opecomb01
	from opecomb02
	where rela_parenti39 = @motorID
	and opecomb02_automatico = 0
	and opecomb02_processed = 0
	and (opecomb02_fecha > @lastUpdateDate or @lastUpdateDate is null)
	
	open cur_consumos
	fetch cur_consumos into @movementID,@fecha,@volumen,@caudal,@horometro,@temperatura,@rpm,@tipoMov
	
	set @lastVolume = ISNULL(@lastVolume,0)
	
	while @@FETCH_STATUS = 0
	begin
		/*evita que los volumenes decrezcan (siempre DEBEN aumentar en el tiempo)*/
		if(@volumen >= @lastVolume)
		begin
			insert into opecomb02 
			values(	@fecha,								--fecha
					GETDATE(),							--fecha ingreso
					@volumen - ISNULL(@lastVolume,0),	--volumen consumido
					NULL,								--Tanque
					1,									--Tipo : Automatico
					NULL,								--Tanque
					@motorID,							--Motor
					@tipoMov,							--Tipo de Movimiento
					NULL,								--Coche
					@caudal,
					@horometro,
					@temperatura,
					@rpm,
					1)									--Procesado = true
			
			/*Marca el consumo como ya procesado*/
			update opecomb02 set opecomb02_processed = 1 where id_opecomb02 = @movementID
			
			set @lastVolume = @volumen
		end
		
		fetch cur_consumos into @movementID,@fecha,@volumen,@caudal,@horometro,@temperatura,@rpm,@tipoMov
	end
	
	close cur_consumos
	deallocate cur_consumos
END