USE [logictracker]
GO

ALTER TABLE dbo.[par.par_tick_08_tbl_estado_logistico] ADD
	partick08_iterativo bit NULL,
	partick08_control_inverso bit NULL
GO

ALTER TABLE dbo.[par.par_tick_08_tbl_estado_logistico] SET (LOCK_ESCALATION = TABLE)
GO
