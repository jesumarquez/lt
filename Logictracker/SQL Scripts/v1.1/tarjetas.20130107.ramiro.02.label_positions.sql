/*
   lunes, 07 de enero de 201304:08:08 p.m.
   User: 
   Server: RBUGALLO-PC
   Database: tarjetas
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
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
ALTER TABLE dbo.back ADD
	legajo_top int NOT NULL CONSTRAINT DF_back_legajo_top DEFAULT 1760,
	legajo_left int NOT NULL CONSTRAINT DF_back_legajo_left DEFAULT 120,
	documento_top int NOT NULL CONSTRAINT DF_back_documento_top DEFAULT 2160,
	documento_left int NOT NULL CONSTRAINT DF_back_documento_left DEFAULT 120,
	upcode_top int NOT NULL CONSTRAINT DF_back_upcode_top DEFAULT 3460,
	upcode_left int NOT NULL CONSTRAINT DF_back_upcode_left DEFAULT 120,
	nombre_top int NOT NULL CONSTRAINT DF_back_nombre_top DEFAULT 4260,
	nombre_left int NOT NULL CONSTRAINT DF_back_nombre_left DEFAULT 1270,
	apellido_top int NOT NULL CONSTRAINT DF_back_apellido_top DEFAULT 3840,
	apellido_left int NOT NULL CONSTRAINT DF_back_apellido_left DEFAULT 1270,
	foto_top int NOT NULL CONSTRAINT DF_back_foto_top DEFAULT 1540,
	foto_left int NOT NULL CONSTRAINT DF_back_foto_left DEFAULT 1410
GO
ALTER TABLE dbo.back SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.back', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.back', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.back', 'Object', 'CONTROL') as Contr_Per 