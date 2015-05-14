USE [logictracker]
GO
CREATE NONCLUSTERED INDEX [IX_VIGDESDE_VIGHASTA]
ON [dbo].[par.par_geom_02_mov_historia] ([pargeom02_vigencia_desde], [pargeom02_vigencia_hasta])
INCLUDE ([id_pargeom02], [rela_parenti05], [rela_pargeom01], [rela_pargeom03])
GO