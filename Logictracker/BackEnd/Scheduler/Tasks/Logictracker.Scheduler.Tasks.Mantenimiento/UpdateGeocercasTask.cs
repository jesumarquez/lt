using System;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Process.Geofences;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Utils;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class UpdateGeocercasTask : BaseTask
    {
        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(GetType().FullName, "Iniciando tarea...");

            var te = new TimeElapsed();

            try
            {
                var empresas = DaoFactory.EmpresaDAO.FindAll().Where(e => e.Baja == false);
                var lineas = DaoFactory.LineaDAO.FindAll().Where(l => l.Baja == false);

                foreach (var empresa in empresas)
                {
                    var lineaEmpresa = lineas.Where(l => l.Empresa.Id == empresa.Id);

                    GeocercaManager.GetQtreeTest(empresa.Id, -1);
                    
                    foreach (var linea in lineaEmpresa)
                    {
                        GeocercaManager.GetQtreeTest(empresa.Id, linea.Id);
                    }
                }

                var ts = te.getTimeElapsed().TotalSeconds;
                STrace.Trace(GetType().FullName, string.Format("Tarea finalizada en {0} segundos", ts));
            }
            catch (Exception ex)
            {
                AddError(ex);
            }
            finally
            {
                ClearData();
            }
        }

        private void ClearData()
        {
            ClearSessions();
            GC.Collect();
        }
    }
}
