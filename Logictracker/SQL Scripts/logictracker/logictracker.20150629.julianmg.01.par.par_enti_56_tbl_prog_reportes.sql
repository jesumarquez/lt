﻿/*
Deployment script for logictracker

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "logictracker"
:setvar DefaultFilePrefix "logictracker"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL10_50.SA\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL10_50.SA\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
/*
The column [dbo].[par.par_enti_56_tbl_prog_reportes].[parenti56_baja] is being dropped, data loss could occur.

The column [dbo].[par.par_enti_56_tbl_prog_reportes].[parenti56_parametros] is being dropped, data loss could occur.

The column [dbo].[par.par_enti_56_tbl_prog_reportes].[parenti56_parametrosCSV] is being dropped, data loss could occur.

The column [dbo].[par.par_enti_56_tbl_prog_reportes].[parenti56_periodicidad] is being dropped, data loss could occur.

The column [dbo].[par.par_enti_56_tbl_prog_reportes].[parenti56_reporte] is being dropped, data loss could occur.

The column [dbo].[par.par_enti_56_tbl_prog_reportes].[rela_parenti02] is being dropped, data loss could occur.

The column [dbo].[par.par_enti_56_tbl_prog_reportes].[parenti56_active] on table [dbo].[par.par_enti_56_tbl_prog_reportes] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column [dbo].[par.par_enti_56_tbl_prog_reportes].[parenti56_periodicity] on table [dbo].[par.par_enti_56_tbl_prog_reportes] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column [dbo].[par.par_enti_56_tbl_prog_reportes].[parenti56_report] on table [dbo].[par.par_enti_56_tbl_prog_reportes] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column [dbo].[par.par_enti_56_tbl_prog_reportes].[parenti56_vehicles] on table [dbo].[par.par_enti_56_tbl_prog_reportes] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.
*/

--IF EXISTS (select top 1 1 from [dbo].[par.par_enti_56_tbl_prog_reportes])
--    RAISERROR (N'Rows were detected. The schema update is terminating because data loss might occur.', 16, 127) WITH NOWAIT

GO
PRINT N'Dropping [dbo].[FK_par.par_enti_62_log_reportes_programados_par.par_enti_56_tbl_prog_reportes]...';


GO
ALTER TABLE [dbo].[par.par_enti_62_log_reportes_programados] DROP CONSTRAINT [FK_par.par_enti_62_log_reportes_programados_par.par_enti_56_tbl_prog_reportes];


GO
PRINT N'Dropping <unnamed>...';


GO
EXECUTE sp_droprolemember @rolename = N'db_accessadmin', @membername = N'Administrador';


GO
PRINT N'Dropping <unnamed>...';


GO
EXECUTE sp_droprolemember @rolename = N'db_backupoperator', @membername = N'Administrador';


GO
PRINT N'Dropping <unnamed>...';


GO
EXECUTE sp_droprolemember @rolename = N'db_datareader', @membername = N'Administrador';


GO
PRINT N'Dropping <unnamed>...';


GO
EXECUTE sp_droprolemember @rolename = N'db_datawriter', @membername = N'Administrador';


GO
PRINT N'Dropping <unnamed>...';


GO
EXECUTE sp_droprolemember @rolename = N'db_ddladmin', @membername = N'Administrador';


GO
PRINT N'Dropping <unnamed>...';


GO
EXECUTE sp_droprolemember @rolename = N'db_denydatareader', @membername = N'Administrador';


GO
PRINT N'Dropping <unnamed>...';


GO
EXECUTE sp_droprolemember @rolename = N'db_denydatawriter', @membername = N'Administrador';


GO
PRINT N'Dropping <unnamed>...';


GO
EXECUTE sp_droprolemember @rolename = N'db_owner', @membername = N'logictracker_web';


GO
PRINT N'Dropping <unnamed>...';


GO
EXECUTE sp_droprolemember @rolename = N'db_owner', @membername = N'Administrador';


GO
PRINT N'Dropping <unnamed>...';


GO
EXECUTE sp_droprolemember @rolename = N'db_securityadmin', @membername = N'Administrador';


GO
PRINT N'Starting rebuilding table [dbo].[par.par_enti_56_tbl_prog_reportes]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_par.par_enti_56_tbl_prog_reportes] (
    [id_parenti56]           INT           IDENTITY (1, 1) NOT NULL,
    [parenti56_report]       VARCHAR (100) NOT NULL,
    [parenti56_vehicles]     VARCHAR (500) NOT NULL,
    [parenti56_periodicity]  CHAR (1)      NOT NULL,
    [parenti56_active]       BIT           NOT NULL,
    [parenti56_mail]         VARCHAR (500) NOT NULL,
    [rela_parenti01]         INT           NOT NULL,
    [parenti56_drivers]      VARCHAR (500) NULL,
    [parenti56_created]      DATE          NULL,
    [parenti56_messageTypes] VARCHAR (500) NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_par.par_enti_56_tbl_prog_reportes] PRIMARY KEY CLUSTERED ([id_parenti56] ASC)
);

--IF EXISTS (SELECT TOP 1 1 
--           FROM   [dbo].[par.par_enti_56_tbl_prog_reportes])
--    BEGIN
--        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_par.par_enti_56_tbl_prog_reportes] ON;
--        INSERT INTO [dbo].[tmp_ms_xx_par.par_enti_56_tbl_prog_reportes] ([id_parenti56], [parenti56_mail], [rela_parenti01])
--        SELECT   [id_parenti56],
--                 [parenti56_mail],
--                 [rela_parenti01]
--        FROM     [dbo].[par.par_enti_56_tbl_prog_reportes]
--        ORDER BY [id_parenti56] ASC;
--        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_par.par_enti_56_tbl_prog_reportes] OFF;
--    END

DROP TABLE [dbo].[par.par_enti_56_tbl_prog_reportes];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_par.par_enti_56_tbl_prog_reportes]', N'par.par_enti_56_tbl_prog_reportes';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_par.par_enti_56_tbl_prog_reportes]', N'PK_par.par_enti_56_tbl_prog_reportes', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[FK_par.par_enti_62_log_reportes_programados_par.par_enti_56_tbl_prog_reportes]...';


GO
ALTER TABLE [dbo].[par.par_enti_62_log_reportes_programados] WITH NOCHECK
    ADD CONSTRAINT [FK_par.par_enti_62_log_reportes_programados_par.par_enti_56_tbl_prog_reportes] FOREIGN KEY ([rela_parenti56]) REFERENCES [dbo].[par.par_enti_56_tbl_prog_reportes] ([id_parenti56]);


GO
PRINT N'Creating Permission...';


GO
GRANT EXECUTE
    ON SCHEMA::[dbo] TO [logictracker_services];


GO
PRINT N'Creating Permission...';


GO
GRANT EXECUTE
    ON SCHEMA::[dbo] TO [logictracker_web];


GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[par.par_enti_62_log_reportes_programados] WITH CHECK CHECK CONSTRAINT [FK_par.par_enti_62_log_reportes_programados_par.par_enti_56_tbl_prog_reportes];


GO
PRINT N'Update complete.';


GO
