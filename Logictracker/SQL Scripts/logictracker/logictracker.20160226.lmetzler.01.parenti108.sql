USE [logictracker]
GO

/****** Object:  Table [dbo].[par.par_enti_108_det_contenedor]    Script Date: 26/02/2016 15:29:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[par.par_enti_108_det_contenedor](
	[id_parenti108] [int] IDENTITY(1,1) NOT NULL,
	[parenti108_descripcion] [varchar](32) NOT NULL,
	[parenti108_capacidad] [float] NOT NULL,
	[rela_parenti17] [int] NOT NULL,
 CONSTRAINT [PK_par.par_enti_108_det_contenedor] PRIMARY KEY CLUSTERED 
(
	[id_parenti108] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[par.par_enti_108_det_contenedor]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_108_det_contenedor_par.par_enti_17_tbl_tipocoche] FOREIGN KEY([rela_parenti17])
REFERENCES [dbo].[par.par_enti_17_tbl_tipocoche] ([id_parenti17])
GO

ALTER TABLE [dbo].[par.par_enti_108_det_contenedor] CHECK CONSTRAINT [FK_par.par_enti_108_det_contenedor_par.par_enti_17_tbl_tipocoche]
GO

CREATE SYNONYM [dbo].[parenti108] FOR [dbo].[par.par_enti_108_det_contenedor]
GO

