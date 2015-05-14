USE [logictracker]
GO

/****** Object:  Table [dbo].[soc.soc_usua_14_mov_tipo_mensaje]    Script Date: 06/05/2015 17:07:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[soc.soc_usua_14_mov_tipo_mensaje](
	[id_socusua14] [int] IDENTITY(1,1) NOT NULL,
	[rela_socusua01] [int] NOT NULL,
	[rela_parenti16] [int] NOT NULL,
 CONSTRAINT [PK_soc.soc_usua_14_mov_tipo_mensaje] PRIMARY KEY CLUSTERED 
(
	[id_socusua14] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[soc.soc_usua_14_mov_tipo_mensaje]  WITH CHECK ADD  CONSTRAINT [FK_soc.soc_usua_14_mov_tipo_mensaje_par.par_enti_16_tbl_tipomensaje] FOREIGN KEY([rela_parenti16])
REFERENCES [dbo].[par.par_enti_16_tbl_tipomensaje] ([id_parenti16])
GO

ALTER TABLE [dbo].[soc.soc_usua_14_mov_tipo_mensaje] CHECK CONSTRAINT [FK_soc.soc_usua_14_mov_tipo_mensaje_par.par_enti_16_tbl_tipomensaje]
GO

ALTER TABLE [dbo].[soc.soc_usua_14_mov_tipo_mensaje]  WITH CHECK ADD  CONSTRAINT [FK_soc.soc_usua_14_mov_tipo_mensaje_soc.soc_usua_01_cab_usuarios] FOREIGN KEY([rela_socusua01])
REFERENCES [dbo].[soc.soc_usua_01_cab_usuarios] ([id_socusua01])
GO

ALTER TABLE [dbo].[soc.soc_usua_14_mov_tipo_mensaje] CHECK CONSTRAINT [FK_soc.soc_usua_14_mov_tipo_mensaje_soc.soc_usua_01_cab_usuarios]
GO


CREATE SYNONYM [dbo].[socusua14] FOR [dbo].[soc.soc_usua_14_mov_tipo_mensaje]
GO