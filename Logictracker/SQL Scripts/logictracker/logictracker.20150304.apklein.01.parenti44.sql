USE [logictracker]
GO

/****** Object:  Index [IX_PARENTI05]    Script Date: 3/4/2015 12:29:52 PM ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_44_pto_entrega]') AND name = N'IX_PARENTI05')
DROP INDEX [IX_PARENTI05] ON [dbo].[par.par_enti_44_pto_entrega]
GO

/****** Object:  Index [IX_PARENTI05]    Script Date: 3/4/2015 12:29:52 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_44_pto_entrega]') AND name = N'IX_PARENTI05')
CREATE NONCLUSTERED INDEX [IX_PARENTI05] ON [dbo].[par.par_enti_44_pto_entrega]
(
	[rela_parenti05] ASC
)
INCLUDE (ID_PARENTI44)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


