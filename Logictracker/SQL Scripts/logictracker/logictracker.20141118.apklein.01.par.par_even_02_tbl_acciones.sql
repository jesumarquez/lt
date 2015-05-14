CREATE NONCLUSTERED INDEX [IX_BAJA_PAREVEN01] ON [dbo].[par.par_even_02_tbl_acciones]
(
	[pareve02_baja] ASC,
	[rela_pareven01] ASC
)
INCLUDE (id_pareven02) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
DROP INDEX [IX_Baja_Codigo] ON [dbo].[par.par_even_02_tbl_acciones]
GO

