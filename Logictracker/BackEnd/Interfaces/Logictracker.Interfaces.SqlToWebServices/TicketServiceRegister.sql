-- Enable CLR Integration
sp_configure 'clr enable', 1
GO
RECONFIGURE
GO

USE Logictracker
GO

-- Set TRUSTWORTHY database's option ON
ALTER DATABASE Logictracker SET TRUSTWORTHY ON
GO

--exec sp_changedbowner 'MICROSTAR\rbugallo'
CREATE ASSEMBLY SMDiagnostics
AUTHORIZATION dbo
FROM 'C:\WINDOWS\Microsoft.NET\Framework\v3.0\Windows Communication Foundation\SMDiagnostics.dll'
WITH PERMISSION_SET = UNSAFE
GO


CREATE ASSEMBLY Serialization
AUTHORIZATION dbo
FROM 'C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0\System.Runtime.Serialization.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY Web
AUTHORIZATION dbo
FROM 'C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Web.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY IdentityModel
AUTHORIZATION dbo
FROM 'C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0\system.identitymodel.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY IdentityModelSelectors
AUTHORIZATION dbo
FROM 'C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0\system.identitymodel.selectors.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY TransactionsBridge
AUTHORIZATION dbo
FROM 'C:\WINDOWS\Microsoft.NET\Framework\v3.0\Windows Communication Foundation\microsoft.transactions.bridge.dll'
WITH PERMISSION_SET = UNSAFE
GO


-- Add SqlToWebServices assembly
-- remember to set the path to Logictracker.Interfaces.SqlToWebServices.dll correctly
CREATE ASSEMBLY SqlToWebServices
AUTHORIZATION dbo
FROM 'D:\Proyectos\Logictracker\Logictracker\BackEnd\Interfaces\Logictracker.Interfaces.SqlToWebServices\bin\Debug\Logictracker.Interfaces.SqlToWebServices.dll'--CAMBIAR
WITH PERMISSION_SET = UNSAFE
GO

-- Create procedures
CREATE PROCEDURE TicketService_Login
@username  nvarchar(200),
@password  nvarchar(200),
@sessionId  nvarchar(MAX) OUTPUT,
@errorMessage  nvarchar(MAX) OUTPUT
AS EXTERNAL NAME SqlToWebServices.[Logictracker.Interfaces.SqlToWebServices.TicketService].Login
GO

CREATE PROCEDURE TicketService_IsActive
@sessionId  nvarchar(200),
@active  bit OUTPUT,
@errorMessage  nvarchar(MAX) OUTPUT
AS EXTERNAL NAME SqlToWebServices.[Logictracker.Interfaces.SqlToWebServices.TicketService].IsActive
GO

CREATE PROCEDURE TicketService_Logout
@sessionId  nvarchar(200),
@done  bit OUTPUT,
@errorMessage  nvarchar(MAX) OUTPUT
AS EXTERNAL NAME SqlToWebServices.[Logictracker.Interfaces.SqlToWebServices.TicketService].Logout
GO

CREATE PROCEDURE TicketService_AssignAndInit
@sessionId  nvarchar(200),
@company nvarchar(200),
@branch nvarchar(200),
@utcDate datetime,
@clientCode  nvarchar(200),
@deliveryPointCode nvarchar(200),
@tripNo int,
@vehicleDomain  nvarchar(200),
@driver  nvarchar(200),
@load  nvarchar(200),
@done  bit OUTPUT,
@errorMessage  nvarchar(MAX) OUTPUT
AS EXTERNAL NAME SqlToWebServices.[Logictracker.Interfaces.SqlToWebServices.TicketService].AssignAndInit
GO

/*
-- Run this after rebuilding assembly 
ALTER ASSEMBLY SqlToWebServices
FROM 'C:\Logictracker.Interfaces.SqlToWebServices.dll'
WITH PERMISSION_SET = UNSAFE
*/


/*
-- para probarlo

declare @sess nvarchar(200)
declare @msg nvarchar(MAX)
EXEC TicketService_Login 'admin', 'admin', @sessionId = @sess OUTPUT , @errorMessage=@msg OUTPUT
print @sess
print @msg

declare @loggedin bit
EXEC TicketService_IsActive @sess, @active = @loggedin OUTPUT , @errorMessage=@msg OUTPUT
print @loggedin
print @msg

declare @loggedout bit
EXEC TicketService_Logout @sess, @done = @loggedout OUTPUT , @errorMessage=@msg OUTPUT
print @loggedout
print @msg

declare @done bit
EXEC TicketService_AssignAndInit @sess, 'COMPANY', 'BRANCH','2011-10-24 10:00','CLI_CODE', 'POI_CODE', 1, 'ABC 123', '005', '8.5' @done = @done OUTPUT , @errorMessage=@msg OUTPUT
print @loggedout
print @msg

*/


/*
-- Remove procedures and SqlToWebServices from database

DROP PROCEDURE TicketService_Login
GO
DROP PROCEDURE TicketService_IsActive
GO
DROP PROCEDURE TicketService_Logout
GO
DROP PROCEDURE TicketService_AssignAndInit
GO
DROP ASSEMBLY SqlToWebServices
GO
*/
