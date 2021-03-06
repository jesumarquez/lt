/*
   lunes, 07 de enero de 201303:10:38 p.m.
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
	print_legajo bit NOT NULL CONSTRAINT DF_back_print_legajo DEFAULT 1,
	print_documento bit NOT NULL CONSTRAINT DF_back_print_documento DEFAULT 1,
	print_upcode bit NOT NULL CONSTRAINT DF_back_print_upcode DEFAULT 1,
	print_nombre bit NOT NULL CONSTRAINT DF_back_print_nombre DEFAULT 1,
	print_apellido bit NOT NULL CONSTRAINT DF_back_print_apellido DEFAULT 1,
	print_foto bit NOT NULL CONSTRAINT DF_back_print_foto DEFAULT 1
GO
ALTER TABLE dbo.back SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.back', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.back', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.back', 'Object', 'CONTROL') as Contr_Per 