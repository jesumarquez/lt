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

ALTER TABLE dbo.[ope.ope_tick_03_cab_viajedistri] ADD
	opetick03_recepcion datetime NULL
GO
ALTER TABLE dbo.[ope.ope_tick_03_cab_viajedistri] SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE dbo.[ope.ope_tick_04_det_entregadistri] ADD
	opetick04_recepcion_confirmacion datetime NULL,
	opetick04_lectura_confirmacion datetime NULL,
	rela_opeeven01 int NULL
GO
ALTER TABLE dbo.[ope.ope_tick_04_det_entregadistri] SET (LOCK_ESCALATION = TABLE)
GO

COMMIT
