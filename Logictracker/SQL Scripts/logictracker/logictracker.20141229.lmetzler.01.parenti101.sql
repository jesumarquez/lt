USE [logictracker]
GO

/****** Object:  Table [dbo].[par.par_enti_101_dataMarts]    Script Date: 29/12/2014 15:19:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[par.par_enti_101_dataMarts](
	[id_parenti101] [int] IDENTITY(1,1) NOT NULL,
	[parenti101_fecha_inicio] [datetime] NULL,
	[parenti101_fecha_fin] [datetime] NULL,
	[parenti101_modulo] [smallint] NULL,
	[parenti101_mensaje] [varchar](128) NULL,
	[parenti101_duracion] [float] NULL,
 CONSTRAINT [PK_par.par_enti_101_dataMarts] PRIMARY KEY CLUSTERED 
(
	[id_parenti101] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

CREATE SYNONYM [dbo].[parenti101] FOR [dbo].[par.par_enti_101_dataMarts]
GO

