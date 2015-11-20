USE [logictracker]
GO

ALTER TABLE dbo.[par.par_enti_102_agenda_vehicular] ADD
	rela_parenti04 int NULL
GO
ALTER TABLE dbo.[par.par_enti_102_agenda_vehicular] SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE [dbo].[par.par_enti_102_agenda_vehicular]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_102_agenda_vehicular_par.par_enti_04_cab_departamento] FOREIGN KEY([rela_parenti04])
REFERENCES [dbo].[par.par_enti_04_cab_departamento] ([id_parenti04])
GO

ALTER TABLE [dbo].[par.par_enti_102_agenda_vehicular] CHECK CONSTRAINT [FK_par.par_enti_102_agenda_vehicular_par.par_enti_04_cab_departamento]
GO

