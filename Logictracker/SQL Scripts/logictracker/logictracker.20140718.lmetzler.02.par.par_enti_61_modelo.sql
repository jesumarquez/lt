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
ALTER TABLE dbo.[par.par_enti_61_modelo] ADD
	rela_parenti58 int NULL
GO
ALTER TABLE dbo.[par.par_enti_61_modelo] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
GO

ALTER TABLE [dbo].[par.par_enti_61_modelo]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_61_modelo_par.par_enti_58_insumos] FOREIGN KEY([rela_parenti58])
REFERENCES [dbo].[par.par_enti_58_insumos] ([id_parenti58])
GO

ALTER TABLE [dbo].[par.par_enti_61_modelo] CHECK CONSTRAINT [FK_par.par_enti_61_modelo_par.par_enti_58_insumos]
GO

