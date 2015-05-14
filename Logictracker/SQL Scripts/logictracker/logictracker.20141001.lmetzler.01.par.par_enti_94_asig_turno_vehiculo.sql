USE [logictracker]
GO

/****** Object:  Table [dbo].[par.par_enti_94_asig_turno_vehiculo]    Script Date: 10/01/2014 11:49:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[par.par_enti_94_asig_turno_vehiculo](
	[id_parenti94] [int] IDENTITY(1,1) NOT NULL,
	[rela_parenti46] [int] NOT NULL,
	[rela_parenti03] [int] NOT NULL,
 CONSTRAINT [PK_par.par_enti_94_asig_turno_vehiculo] PRIMARY KEY CLUSTERED 
(
	[id_parenti94] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[par.par_enti_94_asig_turno_vehiculo]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_94_asig_turno_vehiculo_par.par_enti_03_cab_coches] FOREIGN KEY([rela_parenti03])
REFERENCES [dbo].[par.par_enti_03_cab_coches] ([id_parenti03])
GO

ALTER TABLE [dbo].[par.par_enti_94_asig_turno_vehiculo] CHECK CONSTRAINT [FK_par.par_enti_94_asig_turno_vehiculo_par.par_enti_03_cab_coches]
GO

ALTER TABLE [dbo].[par.par_enti_94_asig_turno_vehiculo]  WITH CHECK ADD  CONSTRAINT [FK_par.par_enti_94_asig_turno_vehiculo_par.par_enti_46_turnos] FOREIGN KEY([rela_parenti46])
REFERENCES [dbo].[par.par_enti_46_turnos] ([id_parenti46])
GO

ALTER TABLE [dbo].[par.par_enti_94_asig_turno_vehiculo] CHECK CONSTRAINT [FK_par.par_enti_94_asig_turno_vehiculo_par.par_enti_46_turnos]
GO


CREATE SYNONYM [dbo].[parenti94] FOR [dbo].[par.par_enti_94_asig_turno_vehiculo]
GO
