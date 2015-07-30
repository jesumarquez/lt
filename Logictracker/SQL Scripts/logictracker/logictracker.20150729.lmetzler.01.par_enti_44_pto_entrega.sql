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
ALTER TABLE dbo.[par.par_enti_44_pto_entrega] ADD
	rela_parenti09 int NULL
GO
ALTER TABLE dbo.[par.par_enti_44_pto_entrega] SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE [dbo].[par.par_enti_44_pto_entrega]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_44_pto_entrega_par.par_enti_09_cab_empleados] FOREIGN KEY([rela_parenti09])
REFERENCES [dbo].[par.par_enti_09_cab_empleados] ([id_parenti09])
GO

ALTER TABLE [dbo].[par.par_enti_44_pto_entrega] CHECK CONSTRAINT [FK_par.par_enti_44_pto_entrega_par.par_enti_09_cab_empleados]
GO

COMMIT
