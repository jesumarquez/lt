USE [msdb]
GO
/****** Object:  Job [syspolicy_purge_history]    Script Date: 11/16/2009 12:33:58 ******/
EXEC msdb.dbo.sp_delete_job @job_id=N'24874388-1302-4e2d-8fc0-0635ddb2373c', @delete_unused_schedule=1
GO
