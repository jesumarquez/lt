declare @linkedservername nvarchar(MAC), @linkedserverfile nvarchar(MAX)
set @linkedservername = 'remitos'
set @linkedserverfile = 'D:\Proyectos\Urbetrack\Clientes\Fenomix\remitos.mdb'

/****** Object:  LinkedServer @linkedservername    Script Date: 10/05/2011 13:32:40 ******/
EXEC master.dbo.sp_addlinkedserver @server = @linkedservername, @srvproduct=N'OLE DB Provider for Jet', @provider=N'Microsoft.Jet.OLEDB.4.0', @datasrc=@linkedserverfile
EXEC master.dbo.sp_addlinkedsrvlogin @rmtsrvname=@linkedservername,@useself=N'True',@locallogin=NULL,@rmtuser=NULL,@rmtpassword=NULL
EXEC master.dbo.sp_serveroption @server=@linkedservername, @optname=N'collation compatible', @optvalue=N'false'
EXEC master.dbo.sp_serveroption @server=@linkedservername, @optname=N'data access', @optvalue=N'true'
EXEC master.dbo.sp_serveroption @server=@linkedservername, @optname=N'dist', @optvalue=N'false'
EXEC master.dbo.sp_serveroption @server=@linkedservername, @optname=N'pub', @optvalue=N'false'
EXEC master.dbo.sp_serveroption @server=@linkedservername, @optname=N'rpc', @optvalue=N'false'
EXEC master.dbo.sp_serveroption @server=@linkedservername, @optname=N'rpc out', @optvalue=N'false'
EXEC master.dbo.sp_serveroption @server=@linkedservername, @optname=N'sub', @optvalue=N'false'
EXEC master.dbo.sp_serveroption @server=@linkedservername, @optname=N'connect timeout', @optvalue=N'0'
EXEC master.dbo.sp_serveroption @server=@linkedservername, @optname=N'collation name', @optvalue=null
EXEC master.dbo.sp_serveroption @server=@linkedservername, @optname=N'lazy schema validation', @optvalue=N'false'
EXEC master.dbo.sp_serveroption @server=@linkedservername, @optname=N'query timeout', @optvalue=N'0'
EXEC master.dbo.sp_serveroption @server=@linkedservername, @optname=N'use remote collation', @optvalue=N'true'


