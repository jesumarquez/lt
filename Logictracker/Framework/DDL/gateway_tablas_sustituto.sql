SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fleet01_devices]') AND type in (N'U'))
	DROP TABLE [fleet01_devices];
GO
BEGIN
CREATE TABLE [fleet01_devices](
	[id_parenti08] [int] NOT NULL,
	[parenti08_codigo] [varchar](64) NULL,
	[parenti08_imei] [varchar](32) NOT NULL,
	[parenti08_clave] [varchar](10) NOT NULL,
	[rela_parenti24] [int] NULL,
	[rela_parenti32] [int] NULL,
	[parenti02_descri] [varchar](255) NULL,
	[parenti03_interno] [varchar](32) NULL,
	[parenti24_firma] [varchar](32) NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fleet02_parameters]') AND type in (N'U'))
	DROP TABLE [fleet02_parameters];
GO
BEGIN
CREATE TABLE [fleet02_parameters](
	[parenti30_valor] [varchar](96) NOT NULL,
	[device_id] [int] NOT NULL,
	[parenti30_revision] [int] NOT NULL,
	[parenti31_nombre] [varchar](32) NOT NULL,
	[parenti31_tipo_dato] [varchar](16) NOT NULL,
	[parenti31_consumidor] [nchar](1) NOT NULL,
	[parenti31_valor_inicial] [varchar](96) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fleet03_firmware]') AND type in (N'U'))
	DROP TABLE [fleet03_firmware]
GO
BEGIN
CREATE TABLE [fleet03_firmware](
	[id_parenti24] [int] IDENTITY(1,1) NOT NULL,
	[parenti24_nombre] [varchar](32) NOT NULL,
	[parenti24_descripcion] [varchar](128) NOT NULL,
	[parenti24_fecha] [datetime] NOT NULL,
	[parenti24_firma] [varchar](32) NOT NULL,
	[parenti24_binario] [varbinary](max) NOT NULL
) ON [PRIMARY]
END
