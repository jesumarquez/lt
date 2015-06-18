USE [logictracker]
GO

/****** Object:  Table [dbo].[aux.aux_04_tbl_mensajes_ignorados]    Script Date: 15/06/2015 17:31:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[aux.aux_04_tbl_mensajes_ignorados](
	[id_aux04] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti08] [int] NOT NULL,
	[aux04_codigo] [varchar](10) NOT NULL,
 CONSTRAINT [PK_aux.aux_04_tbl_mensajes_ignorados] PRIMARY KEY CLUSTERED 
(
	[id_aux04] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[aux.aux_04_tbl_mensajes_ignorados]  WITH CHECK ADD  CONSTRAINT [FK_aux.aux_04_tbl_mensajes_ignorados_par.par_enti_08_cab_dispositivos] FOREIGN KEY([rela_parenti08])
REFERENCES [dbo].[par.par_enti_08_cab_dispositivos] ([id_parenti08])
GO

ALTER TABLE [dbo].[aux.aux_04_tbl_mensajes_ignorados] CHECK CONSTRAINT [FK_aux.aux_04_tbl_mensajes_ignorados_par.par_enti_08_cab_dispositivos]
GO

CREATE SYNONYM [dbo].[aux04] FOR [dbo].[aux.aux_04_tbl_mensajes_ignorados]
GO


