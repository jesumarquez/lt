USE [logictracker]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[ope.ope_even_12_log_eventos_admin](
	[id_opeeven12] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti08] [int] NOT NULL,
	[rela_pareven01] [int] NOT NULL,
	[rela_pareven02] [int] NULL,
	[rela_parenti03] [int] NULL,
	[opeeven12_datetime] [datetime] NOT NULL,
	[opeeven12_mensaje] [varchar](1024) NOT NULL,
	[opeeven12_expiracion] [datetime] NULL,
	[opeeven12_estado] [tinyint] NOT NULL,
	[rela_parenti09] [int] NULL,
	[rela_opetick01] [int] NULL,
	[rela_opetick02] [int] NULL,
	[rela_socusua01] [int] NULL,
	[opeeven12_latitud] [float] NULL,
	[opeeven12_longitud] [float] NULL,
	[opeeven12_datetime_fin] [datetime] NULL,
	[opeeven12_latitud_fin] [float] NULL,
	[opeeven12_longitud_fin] [float] NULL,
	[opeeven12_velocidad_permitida] [int] NULL,
	[opeeven12_velocidad_alcanzada] [int] NULL,
	[rela_parenti05] [int] NULL,
	[opeeven12_tienefoto] [bit] NOT NULL CONSTRAINT [DF_ope.opeeven_12_log_eventos_admin_opeeven12_tienefoto]  DEFAULT ((0)),
	[opeeven12_datetime_alta] [datetime] NULL CONSTRAINT [DF_ope.opeeven_12_log_eventos_admin_opeeven12_datetime_alta]  DEFAULT (getutcdate()),
	[rela_parenti89] [int] NULL,
 CONSTRAINT [PK_ope.ope_even_12_log_eventos_admin_LT] PRIMARY KEY CLUSTERED 
(
	[opeeven12_datetime] ASC,
	[id_opeeven12] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100)
)

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin]  WITH CHECK ADD  CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_ope.opetick_01_cab_tickets] FOREIGN KEY([rela_opetick01])
REFERENCES [dbo].[ope.ope_tick_01_cab_tickets] ([id_opetick01])
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin] CHECK CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_ope.opetick_01_cab_tickets]
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin]  WITH CHECK ADD  CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_ope.opetick_02_det_tickets] FOREIGN KEY([rela_opetick02])
REFERENCES [dbo].[ope.ope_tick_02_det_tickets] ([id_opetick02])
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin] CHECK CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_ope.opetick_02_det_tickets]
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin]  WITH CHECK ADD  CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_par.parenti_05_mov_referencias] FOREIGN KEY([rela_parenti05])
REFERENCES [dbo].[par.par_enti_05_mov_referencias] ([id_parenti05])
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin] CHECK CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_par.parenti_05_mov_referencias]
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin]  WITH NOCHECK ADD  CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_par.parenti_09_cab_choferes] FOREIGN KEY([rela_parenti09])
REFERENCES [dbo].[par.par_enti_09_cab_empleados] ([id_parenti09])
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin] CHECK CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_par.parenti_09_cab_choferes]
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin]  WITH CHECK ADD  CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_par.pareven_01_tbl_mensajes] FOREIGN KEY([rela_pareven01])
REFERENCES [dbo].[par.par_even_01_tbl_mensajes] ([id_pareven01])
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin] CHECK CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_par.pareven_01_tbl_mensajes]
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin]  WITH NOCHECK ADD  CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_par.pareven_02_tbl_acciones] FOREIGN KEY([rela_pareven02])
REFERENCES [dbo].[par.par_even_02_tbl_acciones] ([id_pareven02])
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin] CHECK CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_par.pareven_02_tbl_acciones]
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin]  WITH CHECK ADD  CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_soc.socusua_01_cab_usuarios] FOREIGN KEY([rela_socusua01])
REFERENCES [dbo].[soc.soc_usua_01_cab_usuarios] ([id_socusua01])
GO

ALTER TABLE [dbo].[ope.ope_even_12_log_eventos_admin] CHECK CONSTRAINT [FK_ope.opeeven_12_log_eventos_admin_soc.socusua_01_cab_usuarios]
GO

CREATE SYNONYM [dbo].[opeeven12] FOR [dbo].[ope.ope_even_12_log_eventos_admin]
GO


