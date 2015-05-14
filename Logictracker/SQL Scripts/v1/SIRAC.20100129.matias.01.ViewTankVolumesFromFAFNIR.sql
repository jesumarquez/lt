CREATE VIEW dbo.TankVolumes
AS
SELECT     LogDate,Volume,WaterLevel
FROM         OPENDATASOURCE ('Microsoft.Jet.OLEDB.4.0', 
                      'Data Source="C:\Documents and Settings\pump\Escritorio\MonitorData.mdb";User ID=;Password=' )...tblInventoryLog
WHERE TankNo <> 0