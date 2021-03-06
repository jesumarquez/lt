/*
   Lunes, 31 de Octubre de 201103:03:40 p.m.
   User: 
   Server: RAMIRO
   Database: remitos_cache
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
CREATE TABLE dbo.config
	(
	parameter varchar(50) NOT NULL,
	value varchar(128) NOT NULL
	)  ON [PRIMARY]
GO
COMMIT
select Has_Perms_By_Name(N'dbo.config', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.config', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.config', 'Object', 'CONTROL') as Contr_Per 