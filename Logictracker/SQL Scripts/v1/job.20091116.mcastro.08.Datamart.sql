USE [msdb]
GO
/****** Object:  Job [Datamart]    Script Date: 11/16/2009 12:33:37 ******/
EXEC msdb.dbo.sp_delete_job @job_id=N'a57e3217-c5ac-4066-b3af-5f97920274b9', @delete_unused_schedule=1
GO