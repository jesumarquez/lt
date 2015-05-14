using System;
using System.Data;
using System.Linq;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class LimpiezaRutasTask : BaseTask
    {
        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(GetType().FullName, "Procesando empresas.");

            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var empresas = DaoFactory.EmpresaDAO.FindAll().Where(e => e.EliminaRutas);

                    foreach (var empresa in empresas)
                    {
                        var hasta = DateTime.UtcNow.AddMonths(-empresa.EliminaAntiguedadMeses);

                        var viajes = DaoFactory.ViajeDistribucionDAO.GetList(new[] {empresa.Id}, new[] {-1}, //LINEAS
                            new[] {-1}, //TRANS
                            new[] {-1}, //DEPTOS
                            new[] {-1}, //CENTROS
                            new[] {-1}, //SUBCENTROS
                            new[] {-1}, //VEHICULOS
                            new[] {-1}, //EMPLEADOS
                            new[] {(int) ViajeDistribucion.Estados.Eliminado, ViajeDistribucion.Estados.Pendiente}, null, hasta);
                        var viajesPendientes = viajes.Count;
                        STrace.Trace(GetType().FullName, string.Format("Viajes a eliminar: {0}", viajesPendientes));

                        foreach (var viaje in viajes)
                        {
                            if (empresa.EliminaPuntosDeEntrega)
                            {
                                STrace.Trace(GetType().FullName, string.Format("Eliminando puntos del viaje: {0}", viaje.Id));
                                EliminarPuntos(viaje);
                            }

                            STrace.Trace(GetType().FullName, string.Format("Eliminando el viaje: {0}", viaje.Id));
                            DaoFactory.ViajeDistribucionDAO.Delete(viaje);
                            STrace.Trace(GetType().FullName, string.Format("Viaje eliminado: {0}", viaje.Id));
                        }
                    }

                    transaction.Commit();
                    STrace.Trace(GetType().FullName, "Tarea finalizada.");
                }
                catch (Exception ex)
                {
                    AddError(ex);
                    transaction.Rollback();
                }
                finally
                {
                    ClearData();
                }
            }
        }

        private void EliminarPuntos(ViajeDistribucion viaje)
        {
            foreach (var entregaDistribucion in viaje.Detalles)
            {
                DaoFactory.PuntoEntregaDAO.Delete(entregaDistribucion.PuntoEntrega);
            }
        }

        private void ClearData()
        {
            ClearSessions();
            GC.Collect();
        }
    }
}
