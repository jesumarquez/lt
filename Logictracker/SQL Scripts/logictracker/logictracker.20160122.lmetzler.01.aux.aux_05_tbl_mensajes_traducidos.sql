USE [logictracker]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[aux.aux_05_tbl_mensajes_traducidos](
	[id_aux05] [int] IDENTITY(1,1) NOT NULL,
	[aux05_codigo_original] [varchar](10) NOT NULL,
	[aux05_codigo_final] [varchar](10) NOT NULL,
	[rela_parenti01] [int] NOT NULL,
 CONSTRAINT [PK_aux.aux_05_tbl_mensajes_traducidos] PRIMARY KEY CLUSTERED 
(
	[id_aux05] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[aux.aux_05_tbl_mensajes_traducidos]  WITH CHECK ADD  CONSTRAINT [FK_aux.aux_05_tbl_mensajes_traducidos_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[aux.aux_05_tbl_mensajes_traducidos] CHECK CONSTRAINT [FK_aux.aux_05_tbl_mensajes_traducidos_par.par_enti_01_cab_empresas]
GO

CREATE SYNONYM [dbo].[aux05] FOR [dbo].[aux.aux_05_tbl_mensajes_traducidos]
GO

