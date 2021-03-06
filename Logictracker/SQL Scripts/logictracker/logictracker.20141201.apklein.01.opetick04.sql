/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
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
ALTER TABLE dbo.[ope.ope_tick_04_det_entregadistri] ADD
	opetick04_garmin_unread_inactive datetime NULL,
	opetick04_garmin_read_inactive datetime NULL
GO
ALTER TABLE dbo.[ope.ope_tick_04_det_entregadistri] SET (LOCK_ESCALATION = TABLE)
GO