USE [logictracker]
GO

ALTER TABLE dbo.[ope.ope_tick_04_det_entregadistri] ADD
	opetick04_volumen float(53) NULL,
	opetick04_peso float(53) NULL,
	opetick04_valor float(53) NULL
GO