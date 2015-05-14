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
ALTER TABLE dbo.[par.par_enti_37_cab_centro_costos] ADD
	rela_parenti04 int NULL,
	rela_parenti09 int NULL,
	parenti37_genera_despachos bit NULL,
	parenti37_inicio_automatico bit NULL,
	parenti37_horario_inicio datetime NULL
GO
ALTER TABLE dbo.[par.par_enti_37_cab_centro_costos] SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE [dbo].[par.par_enti_37_cab_centro_costos]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_37_cab_centro_costos_par.par_enti_04_cab_departamento] FOREIGN KEY([rela_parenti04])
REFERENCES [dbo].[par.par_enti_04_cab_departamento] ([id_parenti04])
GO

ALTER TABLE [dbo].[par.par_enti_37_cab_centro_costos] CHECK CONSTRAINT [FK_par.par_enti_37_cab_centro_costos_par.par_enti_04_cab_departamento]
GO

ALTER TABLE [dbo].[par.par_enti_37_cab_centro_costos]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_37_cab_centro_costos_par.par_enti_09_cab_empleados] FOREIGN KEY([rela_parenti09])
REFERENCES [dbo].[par.par_enti_09_cab_empleados] ([id_parenti09])
GO

ALTER TABLE [dbo].[par.par_enti_37_cab_centro_costos] CHECK CONSTRAINT [FK_par.par_enti_37_cab_centro_costos_par.par_enti_09_cab_empleados]
GO

COMMIT
