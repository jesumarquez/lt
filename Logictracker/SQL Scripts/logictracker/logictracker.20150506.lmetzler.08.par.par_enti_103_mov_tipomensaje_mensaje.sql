USE [logictracker]
GO

/****** Object:  Table [dbo].[par.par_enti_103_mov_tipomensaje_mensaje]    Script Date: 06/05/2015 17:41:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[par.par_enti_103_mov_tipomensaje_mensaje](
	[id_parenti103] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti16] [int] NOT NULL,
	[rela_pareven01] [int] NOT NULL,
 CONSTRAINT [PK_par.par_enti_103_mov_tipomensaje_mensaje] PRIMARY KEY CLUSTERED 
(
	[id_parenti103] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[par.par_enti_103_mov_tipomensaje_mensaje]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_103_mov_tipomensaje_mensaje_par.par_enti_16_tbl_tipomensaje] FOREIGN KEY([rela_parenti16])
REFERENCES [dbo].[par.par_enti_16_tbl_tipomensaje] ([id_parenti16])
GO

ALTER TABLE [dbo].[par.par_enti_103_mov_tipomensaje_mensaje] CHECK CONSTRAINT [FK_par.par_enti_103_mov_tipomensaje_mensaje_par.par_enti_16_tbl_tipomensaje]
GO

ALTER TABLE [dbo].[par.par_enti_103_mov_tipomensaje_mensaje]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_103_mov_tipomensaje_mensaje_par.par_even_01_tbl_mensajes] FOREIGN KEY([rela_pareven01])
REFERENCES [dbo].[par.par_even_01_tbl_mensajes] ([id_pareven01])
GO

ALTER TABLE [dbo].[par.par_enti_103_mov_tipomensaje_mensaje] CHECK CONSTRAINT [FK_par.par_enti_103_mov_tipomensaje_mensaje_par.par_even_01_tbl_mensajes]
GO


CREATE SYNONYM [dbo].[parenti103] FOR [dbo].[par.par_enti_103_mov_tipomensaje_mensaje]
GO