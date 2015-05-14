USE [logictracker]
GO

/****** Object:  Table [dbo].[par.par_enti_64_ticket_mantenimiento]    Script Date: 07/22/2014 15:19:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[par.par_enti_64_ticket_mantenimiento](
	[id_parenti64] [int] IDENTITY(1,1) NOT NULL,
	[parenti64_codigo] [varchar](16) NULL,
	[parenti64_descripcion] [varchar](max) NOT NULL,
	[parenti64_nro_primer_presupuesto] [varchar](16) NULL,
	[parenti64_nro_presupuesto] [varchar](16) NULL,
	[parenti64_monto] [float] NULL,
	[parenti64_fecha_solicitud] [datetime] NOT NULL,
	[parenti64_fecha_turno] [datetime] NULL,
	[parenti64_fecha_recepcion] [datetime] NULL,
	[rela_parenti01] [int] NOT NULL,
	[rela_parenti03] [int] NULL,
	[rela_parenti09] [int] NULL,
	[rela_parenti35] [int] NOT NULL,
	[parenti64_fecha_presupuesto_original] [datetime] NULL,
	[parenti64_fecha_presupuestada] [datetime] NULL,
	[parenti64_fecha_recotizacion] [datetime] NULL,
	[parenti64_fecha_aprobacion] [datetime] NULL,
	[parenti64_fecha_verificacion] [datetime] NULL,
	[parenti64_fecha_trabajo_terminado] [datetime] NULL,
	[parenti64_fecha_entrega] [datetime] NULL,
	[parenti64_fecha_trabajo_aceptado] [datetime] NULL,
	[parenti64_estado] [smallint] NOT NULL,
	[parenti64_estado_presupuesto] [smallint] NULL,
	[parenti64_nivel_complejidad] [smallint] NULL,
	[parenti64_entrada] [datetime] NULL,
	[parenti64_salida] [datetime] NULL,
 CONSTRAINT [PK_par.par_enti_64_ticket_mantenimiento] PRIMARY KEY CLUSTERED 
(
	[id_parenti64] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[par.par_enti_64_ticket_mantenimiento]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_64_ticket_mantenimiento_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[par.par_enti_64_ticket_mantenimiento] CHECK CONSTRAINT [FK_par.par_enti_64_ticket_mantenimiento_par.par_enti_01_cab_empresas]
GO

ALTER TABLE [dbo].[par.par_enti_64_ticket_mantenimiento]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_64_ticket_mantenimiento_par.par_enti_03_cab_coches] FOREIGN KEY([rela_parenti03])
REFERENCES [dbo].[par.par_enti_03_cab_coches] ([id_parenti03])
GO

ALTER TABLE [dbo].[par.par_enti_64_ticket_mantenimiento] CHECK CONSTRAINT [FK_par.par_enti_64_ticket_mantenimiento_par.par_enti_03_cab_coches]
GO

ALTER TABLE [dbo].[par.par_enti_64_ticket_mantenimiento]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_64_ticket_mantenimiento_par.par_enti_09_cab_empleados] FOREIGN KEY([rela_parenti09])
REFERENCES [dbo].[par.par_enti_09_cab_empleados] ([id_parenti09])
GO

ALTER TABLE [dbo].[par.par_enti_64_ticket_mantenimiento] CHECK CONSTRAINT [FK_par.par_enti_64_ticket_mantenimiento_par.par_enti_09_cab_empleados]
GO

ALTER TABLE [dbo].[par.par_enti_64_ticket_mantenimiento]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_64_ticket_mantenimiento_par.par_enti_35_tbl_taller] FOREIGN KEY([rela_parenti35])
REFERENCES [dbo].[par.par_enti_35_tbl_taller] ([id_parenti35])
GO

ALTER TABLE [dbo].[par.par_enti_64_ticket_mantenimiento] CHECK CONSTRAINT [FK_par.par_enti_64_ticket_mantenimiento_par.par_enti_35_tbl_taller]
GO

CREATE SYNONYM [dbo].[parenti64] FOR [dbo].[par.par_enti_64_ticket_mantenimiento]
GO

