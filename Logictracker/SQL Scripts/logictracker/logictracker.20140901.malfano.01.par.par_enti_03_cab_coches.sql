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
ALTER TABLE dbo.[par.par_enti_03_cab_coches] ADD
	rela_parenti04 int NULL
GO
ALTER TABLE dbo.[par.par_enti_03_cab_coches] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

ALTER TABLE [dbo].[par.par_enti_03_cab_coches]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_03_cab_coches_par.par_enti_04_cab_departamento] FOREIGN KEY([rela_parenti04])
REFERENCES [dbo].[par.par_enti_04_cab_departamento] ([id_parenti04])
GO

ALTER TABLE [dbo].[par.par_enti_03_cab_coches] CHECK CONSTRAINT [FK_par.par_enti_03_cab_coches_par.par_enti_04_cab_departamento]
GO

