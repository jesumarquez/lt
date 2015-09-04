USE [logictracker]
GO

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.[par.par_enti_55_puerta_acceso] ADD
	rela_parenti05 int NULL
GO
ALTER TABLE dbo.[par.par_enti_55_puerta_acceso] SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE [dbo].[par.par_enti_55_puerta_acceso]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_55_puerta_acceso_par.par_enti_05_mov_referencias] FOREIGN KEY([rela_parenti05])
REFERENCES [dbo].[par.par_enti_05_mov_referencias] ([id_parenti05])
GO

ALTER TABLE [dbo].[par.par_enti_55_puerta_acceso] CHECK CONSTRAINT [FK_par.par_enti_55_puerta_acceso_par.par_enti_05_mov_referencias]
GO

COMMIT