USE [addresser]
GO

/****** Object:  Index [IX_Cruce_Posicion]    Script Date: 12/30/2010 14:11:38 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[cruce]') AND name = N'IX_Cruce_Posicion')
DROP INDEX [IX_Cruce_Posicion] ON [dbo].[cruce] WITH ( ONLINE = OFF )
GO

USE [addresser]
GO

/****** Object:  Index [IX_Cruce_Posicion]    Script Date: 12/30/2010 14:11:38 ******/
CREATE NONCLUSTERED INDEX [IX_Cruce_Posicion] ON [dbo].[cruce] 
(
	[cruc_latitud] ASC,
	[cruc_longitud] ASC
)
INCLUDE ( [id_cruce],
[fk_poligonal],
[fk_poligonal_esquina]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


