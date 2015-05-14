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
ALTER TABLE dbo.[par.par_enti_10_tbl_tiporeferencia] 
ADD	parenti10_controla_permanencia bit NULL,
	parenti10_controla_permanencia_entrega bit NULL,
	parenti10_maxima_permanencia int NULL,
	parenti10_maxima_permanencia_entrega int NULL
GO

ALTER TABLE dbo.[par.par_enti_10_tbl_tiporeferencia] 
ADD CONSTRAINT [DF_par.par_enti_10_tbl_tiporeferencia_parenti10_controla_permanencia] DEFAULT 0 FOR parenti10_controla_permanencia
GO
ALTER TABLE dbo.[par.par_enti_10_tbl_tiporeferencia] 
ADD CONSTRAINT [DF_par.par_enti_10_tbl_tiporeferencia_parenti10_controla_permanencia_entrega] DEFAULT 0 FOR parenti10_controla_permanencia_entrega
GO
ALTER TABLE dbo.[par.par_enti_10_tbl_tiporeferencia] 
ADD CONSTRAINT [DF_par.par_enti_10_tbl_tiporeferencia_parenti10_maxima_permanencia] DEFAULT 0 FOR parenti10_maxima_permanencia
GO
ALTER TABLE dbo.[par.par_enti_10_tbl_tiporeferencia] 
ADD CONSTRAINT [DF_par.par_enti_10_tbl_tiporeferencia_parenti10_maxima_permanencia_entrega] DEFAULT 0 FOR parenti10_maxima_permanencia_entrega
GO
ALTER TABLE dbo.[par.par_enti_10_tbl_tiporeferencia] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT