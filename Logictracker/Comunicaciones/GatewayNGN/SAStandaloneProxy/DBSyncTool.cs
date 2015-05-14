using System;
using System.Collections.Generic;
using System.Threading;
using Urbetrack.DDL;
using Urbetrack.Toolkit;

namespace Urbetrack.GatewayMovil
{
    internal class DBSyncTool
    {
        private readonly DDL.Toolkit.SqlBatchProgressHandler SqlBatch;
        private readonly Task.TaskResultHandler TaskResult;

        public DBSyncTool(DDL.Toolkit.SqlBatchProgressHandler sqlBatchProgress, Task.TaskResultHandler finish)
        {
            SqlBatch = sqlBatchProgress;
            TaskResult = finish;
            UnderlineThread = new Thread(SyncProc);
            UnderlineThread.Start();
        }

        public Thread UnderlineThread { get; private set; }

        public void SyncProc()
        {
            try
            {
                /*var cortos = new List<DDL.Toolkit.SqlTable>
                                 {
                                     new DDL.Toolkit.SqlTable() { full_name = "par.par_enti_30_det_dispositivos", synonym = "parenti30" ,qualified_name = "[dbo].[par.par_enti_30_det_dispositivos]", source_where = "rela_parenti31 IN (SELECT id_parenti31 FROM parenti31 WHERE parenti31_nombre NOT LIKE 'q%')" },
                                     new DDL.Toolkit.SqlTable() { full_name = "par.par_enti_31_tipo_parametro",   synonym = "parenti31" ,qualified_name = "[dbo].[par.par_enti_31_tipo_parametro]"},
                                     new DDL.Toolkit.SqlTable() { full_name = "par.par_enti_03_cab_coches",       synonym = "parenti03" ,qualified_name = "[dbo].[par.par_enti_03_cab_coches]"},
                                     new DDL.Toolkit.SqlTable() { full_name = "par.par_enti_08_cab_dispositivos", synonym = "parenti08" ,qualified_name = "[dbo].[par.par_enti_08_cab_dispositivos]"},
                                     new DDL.Toolkit.SqlTable() { full_name = "par.par_enti_32_tipo_dispositivo", synonym = "parenti32" ,qualified_name = "[dbo].[par.par_enti_32_tipo_dispositivo]"},
                                     new DDL.Toolkit.SqlTable() { full_name = "par.par_enti_02_det_lineas",       synonym = "parenti02" ,qualified_name = "[dbo].[par.par_enti_02_det_lineas]"},
                                     new DDL.Toolkit.SqlTable() { full_name = "par.par_enti_01_cab_empresas",     synonym = "parenti01" ,qualified_name = "[dbo].[par.par_enti_01_cab_empresas]"}
                                 };*/
                var cortos = new List<DDL.Toolkit.SqlTable>
                                 {
                                     new DDL.Toolkit.SqlTable() { synonym = "fleet01_devices", source_where = "rela_parenti31 IN (SELECT id_parenti31 FROM parenti31 WHERE parenti31_nombre NOT LIKE 'q%')" },
                                     new DDL.Toolkit.SqlTable() { synonym = "fleet02_parameters" },
                                     new DDL.Toolkit.SqlTable() { synonym = "fleet03_firmware" }
                                 };
                var SourceConnectionString = Config.GetConfigurationString("core", "connection_string", "error");
                var DestinationConnectionString = Config.GetConfigurationString("global", "connection_string", "error");
                if (SourceConnectionString == DestinationConnectionString)
                {
                    throw new Exception("Error de configuracion: La base de datos maesta y la base de datos secundaria son la misma base.");
                }
                if (DDL.Toolkit.Syncronize(cortos, SourceConnectionString, DestinationConnectionString, SqlBatch))
                {
                    TaskResult(null, Task.TaskResults.Success);

                } else
                {
                    TaskResult(null, Task.TaskResults.Failure);
                }
            } catch (Exception e)
            {
                T.EXCEPTION(e);
                TaskResult(null, Task.TaskResults.Failure);
            }
        }
    }
}
