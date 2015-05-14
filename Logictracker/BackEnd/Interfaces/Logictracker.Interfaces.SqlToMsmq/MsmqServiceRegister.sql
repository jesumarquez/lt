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

-- Add System.Messaging and other assemblies to database
-- remember to check the path to System.Messaging.dll
CREATE ASSEMBLY Messaging
AUTHORIZATION dbo
FROM 'C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Messaging.dll'
WITH PERMISSION_SET = UNSAFE
GO

-- Add SqlMSMQ assembly
-- remember to set the path to Logictracker.Interfaces.SqlToMsmq.dll correctly
CREATE ASSEMBLY SqlToMsmq
AUTHORIZATION dbo
FROM 'C:\Logictracker.Interfaces.SqlToMsmq.dll'--CAMBIAR
WITH PERMISSION_SET = UNSAFE
GO

-- Create procedures
CREATE PROCEDURE MsmqService_Send
@queue  nvarchar(200),
@message   nvarchar(MAX),
@errorMessage    nvarchar(MAX) OUTPUT
AS EXTERNAL NAME SqlToMsmq.[Logictracker.Interfaces.SqlToMsmq.MsmqService].Send
GO

CREATE PROCEDURE MsmqService_Peek
@queue  nvarchar(200),
@message   nvarchar(MAX),
@errorMessage    nvarchar(MAX) OUTPUT
AS EXTERNAL NAME SqlToMsmq.[Logictracker.Interfaces.SqlToMsmq.MsmqService].Peek
GO

CREATE PROCEDURE MsmqService_Receive
@queue  nvarchar(200),
@message   nvarchar(MAX),
@errorMessage    nvarchar(MAX) OUTPUT
AS EXTERNAL NAME SqlToMsmq.[Logictracker.Interfaces.SqlToMsmq.MsmqService].Receive
GO

CREATE PROCEDURE MsmqService_Count
@queue  nvarchar(200),
@count int OUTPUT,
@errorMessage    nvarchar(MAX) OUTPUT
AS EXTERNAL NAME SqlToMsmq.[Logictracker.Interfaces.SqlToMsmq.MsmqService].Count
GO


/*
-- Uncomment this to test SqlToMsmq

DECLARE @returnMsg nvarchar(MAX)
EXEC MsmqService_Send '.\private$\test_queue', '<MESSAGE>Mr. Watson, come here, I need you</MESSAGE>', @errorMessage = @returnMsg OUTPUT
PRINT @returnMsg
GO

DECLARE @text nvarchar(1024)
declare @returnMsg nvarchar(MAX)
EXEC MsmqService_Peek '.\private$\test_queue', @message = @text OUTPUT, @errorMessage = @returnMsg OUTPUT
PRINT @text
PRINT @returnMsg
GO

DECLARE @text nvarchar(1024)
DECLARE @returnMsg nvarchar(MAX)
EXEC MsmqService_Receive '.\private$\test_queue', @message = @text OUTPUT, @errorMessage = @returnMsg OUTPUT
PRINT @text
PRINT @returnMsg
GO

DECLARE @result int
DECLARE @returnMsg nvarchar(MAX)
EXEC MsmqService_Count '.\private$\test_queue', @count = @result OUTPUT, @errorMessage = @returnMsg OUTPUT
PRINT @result
PRINT @returnMsg

*/


/*
-- Run this after rebuilding assembly 
ALTER ASSEMBLY SqlToMsmq
FROM 'C:\DLogictracker.Interfaces.SqlToMsmq.dll'--CAMBIAR
WITH PERMISSION_SET = UNSAFE
*/


/*
-- Remove procedures and SqlToMsmq from database

DROP PROCEDURE MsmqService_Send
GO
DROP PROCEDURE MsmqService_Peek
GO
DROP PROCEDURE MsmqService_Receive
GO
DROP PROCEDURE MsmqService_Count
GO
DROP ASSEMBLY SqlToMsmq
GO
*/
