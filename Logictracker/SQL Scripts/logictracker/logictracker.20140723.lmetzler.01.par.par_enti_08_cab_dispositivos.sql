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
ALTER TABLE dbo.[par.par_enti_08_cab_dispositivos] ADD
	parenti08_verificado bit NULL
GO
ALTER TABLE dbo.[par.par_enti_08_cab_dispositivos] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
