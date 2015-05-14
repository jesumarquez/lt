USE [logictracker]
GO

/****** Object:  Table [dbo].[par.par_enti_102_agenda_vehicular]    Script Date: 18/02/2015 10:47:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[par.par_enti_102_agenda_vehicular](
	[id_parenti102] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti01] [int] NOT NULL,
	[rela_parenti02] [int] NOT NULL,
	[rela_parenti03] [int] NOT NULL,
	[rela_parenti09] [int] NOT NULL,
	[rela_parenti46] [int] NULL,
	[parenti102_fecha_desde] [datetime] NOT NULL,
	[parenti102_fecha_hasta] [datetime] NOT NULL,
	[parenti102_estado] [int] NOT NULL,
 CONSTRAINT [PK_par.par_enti_102_agenda_vehicular] PRIMARY KEY CLUSTERED 
(
	[id_parenti102] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[par.par_enti_102_agenda_vehicular]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_102_agenda_vehicular_par.par_enti_01_cab_empresas] FOREIGN KEY([rela_parenti01])
REFERENCES [dbo].[par.par_enti_01_cab_empresas] ([id_parenti01])
GO

ALTER TABLE [dbo].[par.par_enti_102_agenda_vehicular] CHECK CONSTRAINT [FK_par.par_enti_102_agenda_vehicular_par.par_enti_01_cab_empresas]
GO

ALTER TABLE [dbo].[par.par_enti_102_agenda_vehicular]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_102_agenda_vehicular_par.par_enti_02_det_lineas] FOREIGN KEY([rela_parenti02])
REFERENCES [dbo].[par.par_enti_02_det_lineas] ([id_parenti02])
GO

ALTER TABLE [dbo].[par.par_enti_102_agenda_vehicular] CHECK CONSTRAINT [FK_par.par_enti_102_agenda_vehicular_par.par_enti_02_det_lineas]
GO

ALTER TABLE [dbo].[par.par_enti_102_agenda_vehicular]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_102_agenda_vehicular_par.par_enti_03_cab_coches] FOREIGN KEY([rela_parenti03])
REFERENCES [dbo].[par.par_enti_03_cab_coches] ([id_parenti03])
GO

ALTER TABLE [dbo].[par.par_enti_102_agenda_vehicular] CHECK CONSTRAINT [FK_par.par_enti_102_agenda_vehicular_par.par_enti_03_cab_coches]
GO

ALTER TABLE [dbo].[par.par_enti_102_agenda_vehicular]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_102_agenda_vehicular_par.par_enti_09_cab_empleados] FOREIGN KEY([rela_parenti09])
REFERENCES [dbo].[par.par_enti_09_cab_empleados] ([id_parenti09])
GO

ALTER TABLE [dbo].[par.par_enti_102_agenda_vehicular] CHECK CONSTRAINT [FK_par.par_enti_102_agenda_vehicular_par.par_enti_09_cab_empleados]
GO

ALTER TABLE [dbo].[par.par_enti_102_agenda_vehicular]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_102_agenda_vehicular_par.par_enti_46_turnos] FOREIGN KEY([rela_parenti46])
REFERENCES [dbo].[par.par_enti_46_turnos] ([id_parenti46])
GO

ALTER TABLE [dbo].[par.par_enti_102_agenda_vehicular] CHECK CONSTRAINT [FK_par.par_enti_102_agenda_vehicular_par.par_enti_46_turnos]
GO

CREATE SYNONYM [dbo].[parenti102] FOR [dbo].[par.par_enti_102_agenda_vehicular]
GO




