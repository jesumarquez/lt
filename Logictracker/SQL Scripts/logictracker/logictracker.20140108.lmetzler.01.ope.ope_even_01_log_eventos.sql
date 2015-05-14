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
ALTER TABLE dbo.[ope.ope_even_01_log_eventos] ADD
	rela_opetick03 int NULL,
	rela_opetick04 int NULL
GO
ALTER TABLE dbo.[ope.ope_even_01_log_eventos] SET (LOCK_ESCALATION = TABLE)

GO
ALTER TABLE dbo.[ope.ope_tick_04_det_entregadistri] SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.[ope.ope_tick_03_cab_viajedistri] SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.[ope.ope_even_01_log_eventos] ADD CONSTRAINT
	[FK_ope.ope_even_01_log_eventos_ope.ope_tick_03_cab_viajedistri] FOREIGN KEY
	(
	rela_opetick03
	) REFERENCES dbo.[ope.ope_tick_03_cab_viajedistri]
	(
	id_opetick03
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[ope.ope_even_01_log_eventos] ADD CONSTRAINT
	[FK_ope.ope_even_01_log_eventos_ope.ope_tick_04_det_entregadistri] FOREIGN KEY
	(
	rela_opetick04
	) REFERENCES dbo.[ope.ope_tick_04_det_entregadistri]
	(
	id_opetick04
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[ope.ope_even_01_log_eventos] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
