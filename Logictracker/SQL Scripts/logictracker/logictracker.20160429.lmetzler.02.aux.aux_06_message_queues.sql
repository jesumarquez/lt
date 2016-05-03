USE [logictracker]
GO

/****** Object:  Table [dbo].[aux.aux_06_message_queues]    Script Date: 29/04/2016 17:27:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[aux.aux_06_message_queues](
	[id_aux06] [int] IDENTITY(1,1) NOT NULL,
	[aux06_nombre] [varchar](64) NOT NULL,
	[aux06_cantidad] [int] NOT NULL,
	[aux06_last_update] [datetime] NOT NULL,
 CONSTRAINT [PK_aux.aux_06_message_queues] PRIMARY KEY CLUSTERED 
(
	[id_aux06] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE SYNONYM [dbo].[aux06] FOR [dbo].[aux.aux_06_message_queues]
GO
