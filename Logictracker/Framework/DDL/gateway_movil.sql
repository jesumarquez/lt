SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_01_cab_empresas]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[par.par_enti_01_cab_empresas](
	[id_parenti01] [int] IDENTITY(1,1) NOT NULL,
	[parenti01_razsoc] [varchar](255) NULL,
	[parenti01_fantasia] [varchar](255) NULL,
	[parenti01_cuit] [varchar](13) NULL,
	[parenti01_ncalle] [int] NULL,
	[parenti01_ncalle1] [int] NULL,
	[parenti01_ncalle2] [int] NULL,
	[parenti01_altura] [int] NULL,
	[parenti01_idmapa] [int] NULL,
	[parenti01_localidad] [varchar](64) NULL,
	[parenti01_codpostal] [varchar](8) NULL,
	[parenti01_logo] [varbinary](max) NULL,
	[parenti01_icono] [int] NULL,
	[parenti01_latitud] [float] NULL,
	[parenti01_longitud] [float] NULL,
	[parenti01_pais] [varchar](64) NULL,
	[parenti01_provincia] [varchar](64) NULL,
	[parenti01_baja] [bit] NULL CONSTRAINT [DF_par.par_enti_01_cab_empresas_parenti01_baja]  DEFAULT ((0)),
	[parenti01_telefono] [varchar](32) NULL,
	[parenti01_email] [varchar](255) NULL,
	[parenti01_codigo] [varchar](50) NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_08_cab_dispositivos]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[par.par_enti_08_cab_dispositivos](
	[id_parenti08] [int] IDENTITY(1,1) NOT NULL,
	[parenti08_min] [varchar](32) NOT NULL,
	[parenti08_codigo] [varchar](64) NULL,
	[parenti08_coddisplay] [varchar](64) NOT NULL,
	[parenti08_marca] [varchar](64) NOT NULL,
	[parenti08_modelo] [varchar](64) NULL,
	[parenti08_pollinterval] [smallint] NOT NULL,
	[parenti08_ipadress] [varchar](16) NOT NULL,
	[parenti08_port] [int] NOT NULL,
	[parenti08_mdn] [char](10) NULL,
	[parenti08_imei] [varchar](32) NOT NULL,
	[parenti08_firmware] [varchar](64) NULL,
	[parenti08_tablas] [nchar](2) NOT NULL,
	[parenti08_clave] [varchar](10) NOT NULL,
	[parenti08_minutos] [smallint] NOT NULL,
	[parenti08_mensajes] [smallint] NOT NULL,
	[parenti08_camaras] [smallint] NOT NULL CONSTRAINT [DF_par.par_enti_08_cab_dispositivos_parenti08_camaras]  DEFAULT ((0)),
	[rela_parenti24] [int] NULL,
	[parenti08_estado] [smallint] NULL,
	[parenti08_dtCambioEstado] [datetime] NULL,
	[parenti08_revision_conf] [int] NULL,
	[parenti08_revision_qtree] [int] NULL,
	[rela_parenti32] [int] NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_24_tbl_firmware]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[par.par_enti_24_tbl_firmware](
	[id_parenti24] [int] IDENTITY(1,1) NOT NULL,
	[parenti24_nombre] [varchar](32) NOT NULL,
	[parenti24_descripcion] [varchar](128) NOT NULL,
	[parenti24_fecha] [datetime] NOT NULL,
	[parenti24_firma] [varchar](32) NOT NULL,
	[parenti24_binario] [varbinary](max) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_32_tipo_dispositivo]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[par.par_enti_32_tipo_dispositivo](
	[id_parenti32] [int] IDENTITY(1,1) NOT NULL,
	[parenti32_modelo] [varchar](50) NOT NULL,
	[parenti32_fabricante] [varchar](50) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_02_det_lineas]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[par.par_enti_02_det_lineas](
	[id_parenti02] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti01] [int] NOT NULL,
	[parenti02_descri] [varchar](255) NOT NULL,
	[parenti02_descricorta] [varchar](8) NOT NULL,
	[parenti02_baja] [bit] NOT NULL CONSTRAINT [DF_par.par_enti_02_det_lineas_parenti02_baja]  DEFAULT ((0)),
	[parenti02_wap] [bit] NOT NULL CONSTRAINT [DF_par.par_enti_02_det_lineas_parenti02_wap]  DEFAULT ((0)),
	[parenti02_larga] [bit] NOT NULL CONSTRAINT [DF_par.par_enti_02_det_lineas_parenti02_larga]  DEFAULT ((0)),
	[parenti02_verhorarios] [bit] NOT NULL CONSTRAINT [DF_par.par_enti_02_det_lineas_parenti02_verhorarios]  DEFAULT ((0)),
	[parenti02_rangohoras] [smallint] NOT NULL CONSTRAINT [DF_par.par_enti_02_det_lineas_parenti02_rangohoras]  DEFAULT ((6)),
	[rela_parenti05] [int] NULL,
	[pe02_latitud] [float] NOT NULL,
	[pe02_longitud] [float] NOT NULL,
	[pe02_radio] [int] NOT NULL,
	[pe02_telefono] [varchar](32) NOT NULL,
	[pe02_mail] [varchar](64) NOT NULL,
	[pe02_time_zone_id] [varchar](50) NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_03_cab_coches]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[par.par_enti_03_cab_coches](
	[id_parenti03] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti02] [int] NULL,
	[parenti03_interno] [varchar](32) NOT NULL,
	[rela_parenti06] [int] NULL,
	[parenti03_modelo] [varchar](32) NOT NULL,
	[parenti03_patente] [varchar](10) NOT NULL,
	[parenti03_anopat] [smallint] NOT NULL,
	[rela_parenti07] [int] NULL,
	[parenti03_poliza] [varchar](16) NOT NULL,
	[parenti03_fvto] [datetime] NULL,
	[parenti03_nrochasis] [varchar](64) NULL,
	[parenti03_nromotor] [varchar](64) NULL,
	[rela_parenti08] [int] NULL,
	[parenti03_estado] [smallint] NOT NULL,
	[rela_parenti17] [int] NULL,
	[rela_parenti09] [int] NULL,
	[parenti03_dtCambioEstado] [datetime] NULL,
	[paren03_referencia] [varchar](max) NULL,
	[paren03_odometro_aplicacion] [float] NOT NULL CONSTRAINT [DF_par.par_enti_03_cab_coches_paren03_odometro_aplicacion]  DEFAULT ((0)),
	[paren03_odometro_inicial] [float] NOT NULL CONSTRAINT [DF_par.par_enti_03_cab_coches_paren03_odometro_inicial]  DEFAULT ((0)),
	[paren03_odometro_parcial] [float] NOT NULL CONSTRAINT [DF_par.par_enti_03_cab_coches_paren03_odometro_parcial]  DEFAULT ((0)),
	[paren03_reset_odometro] [datetime] NULL,
	[paren03_kilometros_diarios] [float] NOT NULL CONSTRAINT [DF_par.par_enti_03_cab_coches_paren03_kilometros_diarios]  DEFAULT ((0)),
	[paren03_inicio_turno] [float] NULL,
	[paren03_fin_turno] [float] NULL,
	[rela_parenti37] [int] NULL,
	[rela_parenti01] [int] NULL,
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_30_det_dispositivos]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[par.par_enti_30_det_dispositivos](
	[id_parenti30] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti31] [int] NOT NULL,
	[rela_parenti08] [int] NOT NULL,
	[parenti30_valor] [varchar](96) NOT NULL,
	[parenti30_revision] [int] NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_31_tipo_parametro]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[par.par_enti_31_tipo_parametro](
	[id_parenti31] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti32] [int] NOT NULL,
	[parenti31_nombre] [varchar](32) NOT NULL,
	[parenti31_descripcion] [varchar](255) NOT NULL,
	[parenti31_tipo_dato] [varchar](16) NOT NULL,
	[parenti31_consumidor] [nchar](1) NOT NULL,
	[parenti31_valor_inicial] [varchar](96) NOT NULL,
	[parenti31_editable] [bit] NOT NULL
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.synonyms WHERE name = N'parenti01')
CREATE SYNONYM [dbo].[parenti01] FOR [dbo].[par.par_enti_01_cab_empresas]
GO
IF NOT EXISTS (SELECT * FROM sys.synonyms WHERE name = N'parenti02')
CREATE SYNONYM [dbo].[parenti02] FOR [dbo].[par.par_enti_02_det_lineas]
GO
IF NOT EXISTS (SELECT * FROM sys.synonyms WHERE name = N'parenti03')
CREATE SYNONYM [dbo].[parenti03] FOR [dbo].[par.par_enti_03_cab_coches]
GO
IF NOT EXISTS (SELECT * FROM sys.synonyms WHERE name = N'parenti08')
CREATE SYNONYM [dbo].[parenti08] FOR [dbo].[par.par_enti_08_cab_dispositivos]
GO
IF NOT EXISTS (SELECT * FROM sys.synonyms WHERE name = N'parenti30')
CREATE SYNONYM [dbo].[parenti30] FOR [dbo].[par.par_enti_30_det_dispositivos]
GO
IF NOT EXISTS (SELECT * FROM sys.synonyms WHERE name = N'parenti31')
CREATE SYNONYM [dbo].[parenti31] FOR [dbo].[par.par_enti_31_tipo_parametro]
GO
IF NOT EXISTS (SELECT * FROM sys.synonyms WHERE name = N'parenti32')
CREATE SYNONYM [dbo].[parenti32] FOR [dbo].[par.par_enti_32_tipo_dispositivo]
GO
IF NOT EXISTS (SELECT * FROM sys.synonyms WHERE name = N'parenti24')
CREATE SYNONYM [dbo].[parenti24] FOR [dbo].[par.par_enti_24_tbl_firmware]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_repair_device_parameters]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_repair_device_parameters]
@p1_device_id int

AS
BEGIN
	-- COMPLETA LA POBLACION DE PARENTI30 EN BASE AL PARAMETRO ID PARAMETER
	INSERT INTO parenti30 (rela_parenti31, rela_parenti08, parenti30_valor, parenti30_revision)
	SELECT id_parenti31, id_parenti08, parenti31_valor_inicial, 0 as revision
		FROM parenti08 pe08
		JOIN parenti31 pe31 ON pe08.rela_parenti32 = pe31.rela_parenti32
		WHERE pe08.id_parenti08 = @p1_device_id			
	EXCEPT
	SELECT id_parenti31, id_parenti08, parenti31_valor_inicial, revision FROM
	(SELECT id_parenti31, id_parenti08, parenti31_valor_inicial, 0 as revision
		FROM parenti08 pe08
		JOIN parenti31 pe31 ON pe08.rela_parenti32 = pe31.rela_parenti32
		WHERE pe08.id_parenti08 = @p1_device_id) as vista
	JOIN parenti30 pe30 ON (pe30.rela_parenti31 = id_parenti31 AND pe30.rela_parenti08 = id_parenti08)
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_set_device_parameter]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[sp_set_device_parameter]
@p1_device_id int,
@p2_parameter varchar(32),
@p3_value varchar(96),
@p4_consumer nchar(1),
@p5_data_type varchar(16),
@p6_valor_inicial varchar(96),
@p7_editable bit

AS
BEGIN
	DECLARE @id_parenti30 INT
	DECLARE @id_parenti31 INT
	DECLARE @id_parenti32 INT
	DECLARE @pe30_revision INT
		
	-- tipo de dispositivo
	SELECT @id_parenti32 = rela_parenti32 
	FROM parenti08 
	WHERE id_parenti08 = @p1_device_id AND rela_parenti32 IS NOT NULL;
	
	-- si no hay tipo de dispositivo, no hago nada.
	IF @id_parenti32 IS NULL 
		RETURN -1;
	
	-- el tipo de parametro 
	SELECT @id_parenti31 = id_parenti31 
	FROM parenti31 
	WHERE parenti31_nombre = @p2_parameter 
		AND rela_parenti32 = @id_parenti32;
	
	-- si no existe lo creo.
	IF @id_parenti31 IS NULL 
	BEGIN
		INSERT INTO parenti31
			(rela_parenti32, parenti31_nombre, parenti31_descripcion,
			 parenti31_tipo_dato, parenti31_consumidor, parenti31_valor_inicial,
			 parenti31_editable) 
			VALUES
			(@id_parenti32, @p2_parameter, @p2_parameter,  
			 @p5_data_type, @p4_consumer, @p6_valor_inicial, 
			 @p7_editable);
		SET @id_parenti31 = @@IDENTITY;
	END
	
	-- el parametro 
	SELECT @id_parenti30 = id_parenti30, @pe30_revision = parenti30_revision
	FROM parenti30
	WHERE rela_parenti31 = @id_parenti31;

	-- si no existe, lo creo.
	IF @id_parenti30 IS NULL
	BEGIN
		INSERT INTO parenti30 
			(rela_parenti08, rela_parenti31, parenti30_valor, parenti30_revision)
		VALUES
			(@p1_device_id, @id_parenti31, @p3_value, 0);
		RETURN 0;
	END
	
	-- si existe lo actualizo.
	UPDATE parenti30 
	SET parenti30_valor = @p3_value, parenti30_revision = @pe30_revision + 1 
	WHERE rela_parenti31 = @id_parenti31 AND rela_parenti08 = @p1_device_id;

	RETURN @pe30_revision+1;
	
END
' 
END
