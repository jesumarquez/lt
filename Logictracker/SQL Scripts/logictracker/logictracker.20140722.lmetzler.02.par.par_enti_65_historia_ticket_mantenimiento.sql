USE [logictracker]
GO

/****** Object:  Table [dbo].[par.par_enti_65_historia_ticket_mantenimiento]    Script Date: 07/22/2014 15:21:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento](
	[id_parenti65] [int] IDENTITY(1,1) NOT NULL,
	[parenti65_codigo] [varchar](16) NULL,
	[parenti65_descripcion] [varchar](max) NOT NULL,
	[parenti65_nro_primer_presupuesto] [varchar](16) NULL,
	[parenti65_nro_presupuesto] [varchar](16) NULL,
	[parenti65_monto] [float] NULL,
	[parenti65_fecha_solicitud] [datetime] NOT NULL,
	[parenti65_fecha_turno] [datetime] NULL,
	[parenti65_fecha_recepcion] [datetime] NULL,
	[rela_parenti01] [int] NOT NULL,
	[rela_parenti03] [int] NULL,
	[rela_parenti09] [int] NULL,
	[rela_parenti35] [int] NOT NULL,
	[parenti65_fecha_presupuesto_original] [datetime] NULL,
	[parenti65_fecha_presupuestada] [datetime] NULL,
	[parenti65_fecha_recotizacion] [datetime] NULL,
	[parenti65_fecha_aprobacion] [datetime] NULL,
	[parenti65_fecha_verificacion] [datetime] NULL,
	[parenti65_fecha_trabajo_terminado] [datetime] NULL,
	[parenti65_fecha_entrega] [datetime] NULL,
	[parenti65_fecha_trabajo_aceptado] [datetime] NULL,
	[parenti65_estado] [smallint] NOT NULL,
	[parenti65_estado_presupuesto] [smallint] NULL,
	[parenti65_nivel_complejidad] [smallint] NULL,
	[rela_parenti64] [int] NOT NULL,
	[parenti65_datetime] [datetime] NOT NULL,
	[rela_socusua01] [int] NOT NULL,
 CONSTRAINT [PK_par.par_enti_65_historia_ticket_mantenimiento] PRIMARY KEY CLUSTERED 
(
	[id_parenti65] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_65_historia_ticket_mantenimiento_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento] CHECK CONSTRAINT [FK_par.par_enti_65_historia_ticket_mantenimiento_par.par_enti_01_cab_empresas]
GO

ALTER TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_65_historia_ticket_mantenimiento_par.par_enti_03_cab_coches] FOREIGN KEY([rela_parenti03])
REFERENCES [dbo].[par.par_enti_03_cab_coches] ([id_parenti03])
GO

ALTER TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento] CHECK CONSTRAINT [FK_par.par_enti_65_historia_ticket_mantenimiento_par.par_enti_03_cab_coches]
GO

ALTER TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_65_historia_ticket_mantenimiento_par.par_enti_09_cab_empleados] FOREIGN KEY([rela_parenti09])
REFERENCES [dbo].[par.par_enti_09_cab_empleados] ([id_parenti09])
GO

ALTER TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento] CHECK CONSTRAINT [FK_par.par_enti_65_historia_ticket_mantenimiento_par.par_enti_09_cab_empleados]
GO

ALTER TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_65_historia_ticket_mantenimiento_par.par_enti_35_tbl_taller] FOREIGN KEY([rela_parenti35])
REFERENCES [dbo].[par.par_enti_35_tbl_taller] ([id_parenti35])
GO

ALTER TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento] CHECK CONSTRAINT [FK_par.par_enti_65_historia_ticket_mantenimiento_par.par_enti_35_tbl_taller]
GO

ALTER TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_65_historia_ticket_mantenimiento_par.par_enti_64_ticket_mantenimiento] FOREIGN KEY([rela_parenti64])
REFERENCES [dbo].[par.par_enti_64_ticket_mantenimiento] ([id_parenti64])
GO

ALTER TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento] CHECK CONSTRAINT [FK_par.par_enti_65_historia_ticket_mantenimiento_par.par_enti_64_ticket_mantenimiento]
GO

ALTER TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_65_historia_ticket_mantenimiento_soc.soc_usua_01_cab_usuarios] FOREIGN KEY([rela_socusua01])
REFERENCES [dbo].[soc.soc_usua_01_cab_usuarios] ([id_socusua01])
GO

ALTER TABLE [dbo].[par.par_enti_65_historia_ticket_mantenimiento] CHECK CONSTRAINT [FK_par.par_enti_65_historia_ticket_mantenimiento_soc.soc_usua_01_cab_usuarios]
GO

CREATE SYNONYM [dbo].[parenti65] FOR [dbo].[par.par_enti_65_historia_ticket_mantenimiento]
GO

