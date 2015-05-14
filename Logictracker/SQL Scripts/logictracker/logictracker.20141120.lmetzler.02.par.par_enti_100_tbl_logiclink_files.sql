USE [logictracker]
GO

/****** Object:  Table [dbo].[par.par_enti_100_tbl_logiclink_files]    Script Date: 11/20/2014 12:46:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[par.par_enti_100_tbl_logiclink_files](
	[id_parenti100] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti01] [int] NOT NULL,
	[rela_parenti02] [int] NULL,
	[parenti100_server_name] [varchar](255) NOT NULL,
	[parenti100_file_path] [varchar](1024) NULL,
	[parenti100_file_source] [varchar](255) NULL,
	[parenti100_strategy] [varchar](255) NOT NULL,
	[parenti100_date_added] [datetime] NOT NULL,
	[parenti100_date_processed] [datetime] NULL,
	[parenti100_status] [int] NOT NULL,
	[parenti100_result] [varchar](255) NULL,
 CONSTRAINT [PK_par.par_enti_100_tbl_logiclink_files] PRIMARY KEY CLUSTERED 
(
	[id_parenti100] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[par.par_enti_100_tbl_logiclink_files]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_100_tbl_logiclink_files_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[par.par_enti_100_tbl_logiclink_files] CHECK CONSTRAINT [FK_par.par_enti_100_tbl_logiclink_files_par.par_enti_01_cab_empresas]
GO

ALTER TABLE [dbo].[par.par_enti_100_tbl_logiclink_files]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_100_tbl_logiclink_files_par.par_enti_02_det_lineas] FOREIGN KEY([rela_parenti02])
REFERENCES [dbo].[par.par_enti_02_det_lineas] ([id_parenti02])
GO

ALTER TABLE [dbo].[par.par_enti_100_tbl_logiclink_files] CHECK CONSTRAINT [FK_par.par_enti_100_tbl_logiclink_files_par.par_enti_02_det_lineas]
GO


CREATE SYNONYM [dbo].[parenti100] FOR [dbo].[par.par_enti_100_tbl_logiclink_files]
GO

