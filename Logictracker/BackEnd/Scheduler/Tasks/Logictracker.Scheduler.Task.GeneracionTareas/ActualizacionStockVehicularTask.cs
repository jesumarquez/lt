using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using System;
using System.Linq;

namespace Logictracker.Scheduler.Tasks.GeneracionTareas
{
    public class ActualizacíonStockVehicularTask : BaseTask
    {
        private const string ComponentName = "Actualización de Stock Vehicular Task";

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");

            var empresas = DaoFactory.EmpresaDAO.GetList().Where(e => e.ActualizaStockVehicular);

            STrace.Trace(GetType().FullName, string.Format("Procesando empresas. Cantidad: {0}", empresas.Count()));

            try
            {
                foreach (var empresa in empresas)
	            {
                    STrace.Trace(GetType().FullName, string.Format("Procesando Empresa: {0}({1})", empresa.RazonSocial, empresa.Id));

		            var vehiculos = DaoFactory.CocheDAO.FindList(new[]{empresa.Id}, new[] { -1 })
                                                       .Where(c => c.Dispositivo != null);

                    var vehiculosPendientes = vehiculos.Count();
                    STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", vehiculosPendientes));

                    foreach (var vehiculo in vehiculos)
                    {
                        STrace.Trace(GetType().FullName, string.Format("Procesando vehículo: {0}({1})", vehiculo.Interno, vehiculo.Id));

                        var lastDm = DaoFactory.DatamartDAO.GetLastDatamart(vehiculo.Id, DateTime.UtcNow);
                        if (lastDm != null && lastDm.GeograficRefference != null)
                        {
                            if (lastDm.GeograficRefference.Zonas.Any())
                            {
                                var zona = lastDm.GeograficRefference.Zonas.OrderBy(z => z.Zona.Prioridad).Select(z => z.Zona).FirstOrDefault();
                                var tipoVehiculo = vehiculo.TipoCoche;

                                var stock = DaoFactory.StockVehicularDAO.GetByZonaAndTipoCoche(zona.Id, tipoVehiculo.Id);
                                if (stock != null && !stock.Detalles.Select(d => d.Vehiculo).Contains(vehiculo))
                                {
                                    var detalle = new DetalleStockVehicular();
                                    detalle.StockVehicular = stock;
                                    detalle.Vehiculo = vehiculo;
                                    stock.Detalles.Add(detalle);
                                    EliminarAsignaciones(detalle);

                                    DaoFactory.StockVehicularDAO.SaveOrUpdate(stock);
                                }
                            }
                        }                
                    
                        STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", --vehiculosPendientes));
                    }
	            }

                STrace.Trace(GetType().FullName, "Tarea finalizada.");
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

        private void EliminarAsignaciones(DetalleStockVehicular detalle)
        {
            var idStockVehicular = detalle.StockVehicular.Id;
            var idCoche = detalle.Vehiculo.Id;

            var asignaciones = DaoFactory.DetalleStockVehicularDAO.GetByStockAndCoche(idStockVehicular, idCoche);

            foreach (var asignacion in asignaciones)
            {
                asignacion.StockVehicular.Detalles.Remove(asignacion);
                DaoFactory.StockVehicularDAO.SaveOrUpdate(asignacion.StockVehicular);
            }
        }

        private void ClearData()
        {
            ClearSessions();
            GC.Collect();
        }
    }
}
