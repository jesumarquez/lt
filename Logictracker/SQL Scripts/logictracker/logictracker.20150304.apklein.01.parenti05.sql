USE [logictracker]
GO

/****** Object:  Index [IX_Baja]    Script Date: 3/4/2015 12:36:02 PM ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_05_mov_referencias]') AND name = N'IX_Baja')
DROP INDEX [IX_Baja] ON [dbo].[par.par_enti_05_mov_referencias]
GO

/****** Object:  Index [IX_Baja]    Script Date: 3/4/2015 12:36:02 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[par.par_enti_05_mov_referencias]') AND name = N'IX_Baja')
CREATE NONCLUSTERED INDEX [IX_Baja] ON [dbo].[par.par_enti_05_mov_referencias]
(
	[parenti05_baja] ASC
) INCLUDE (id_parenti05) 
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


