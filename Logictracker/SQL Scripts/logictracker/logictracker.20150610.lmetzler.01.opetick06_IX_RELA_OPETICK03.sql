USE [logictracker]
GO

/****** Object:  Index [IX_RELA_OPETICK03]    Script Date: 10/06/2015 11:35:10 ******/
CREATE NONCLUSTERED INDEX [IX_RELA_OPETICK03] ON [dbo].[ope.ope_tick_06_tick_datamart]
(
	[rela_opetick03] ASC
)
INCLUDE ( 	[id_opetick06],
	[opetick06_fecha],
	[opetick06_ruta],
	[opetick06_orden],
	[opetick06_entrega],
	[opetick06_estado],
	[opetick06_km],
	[opetick06_recorrido],
	[opetick06_tiempo_entrega],
	[opetick06_entrada],
	[opetick06_salida],
	[opetick06_manual],
	[opetick06_programado],
	[opetick06_desvio],
	[opetick06_importe],
	[opetick06_cliente],
	[rela_parenti01],
	[rela_parenti02],
	[rela_parenti03],
	[rela_parenti37],
	[rela_opetick04],
	[rela_parenti44],
	[opetick06_confirmacion]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


