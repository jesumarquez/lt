USE [logictracker]
GO

/****** Object:  Table [dbo].[ope.ope_even_10_atencion_evento]    Script Date: 10/10/2014 17:13:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ope.ope_even_10_atencion_evento](
	[id_opeeven10] [int] IDENTITY(1,1) NOT NULL,
	[rela_opeeven01] [int] NOT NULL,
	[rela_socusua01] [int] NOT NULL,
	[opeeven10_datetime] [datetime] NOT NULL,
	[opeeven10_observacion] [varchar](255) NULL,
	[rela_pareven01] [int] NULL,
 CONSTRAINT [PK_ope.ope_even_10_atencion_evento] PRIMARY KEY CLUSTERED 
(
	[id_opeeven10] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ope.ope_even_10_atencion_evento]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_even_10_atencion_evento_ope.ope_even_10_atencion_evento] FOREIGN KEY([id_opeeven10])
REFERENCES [dbo].[ope.ope_even_10_atencion_evento] ([id_opeeven10])
GO

ALTER TABLE [dbo].[ope.ope_even_10_atencion_evento] CHECK CONSTRAINT [FK_ope.ope_even_10_atencion_evento_ope.ope_even_10_atencion_evento]
GO

ALTER TABLE [dbo].[ope.ope_even_10_atencion_evento]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_even_10_atencion_evento_par.par_even_01_tbl_mensajes] FOREIGN KEY([rela_pareven01])
REFERENCES [dbo].[par.par_even_01_tbl_mensajes] ([id_pareven01])
GO

ALTER TABLE [dbo].[ope.ope_even_10_atencion_evento] CHECK CONSTRAINT [FK_ope.ope_even_10_atencion_evento_par.par_even_01_tbl_mensajes]
GO

ALTER TABLE [dbo].[ope.ope_even_10_atencion_evento]  WITH CHECK ADD  CONSTRAINT [FK_ope.ope_even_10_atencion_evento_soc.soc_usua_01_cab_usuarios] FOREIGN KEY([rela_socusua01])
REFERENCES [dbo].[soc.soc_usua_01_cab_usuarios] ([id_socusua01])
GO

ALTER TABLE [dbo].[ope.ope_even_10_atencion_evento] CHECK CONSTRAINT [FK_ope.ope_even_10_atencion_evento_soc.soc_usua_01_cab_usuarios]
GO

CREATE SYNONYM [dbo].[opeeven10] FOR [logictracker].[dbo].[ope.ope_even_10_atencion_evento]
GO
