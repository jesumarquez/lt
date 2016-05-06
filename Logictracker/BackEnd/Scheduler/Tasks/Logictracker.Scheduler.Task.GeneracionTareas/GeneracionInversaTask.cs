using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Scheduler.Tasks.GeneracionTareas
{
    public class GeneracionInversaTask : BaseTask
    {
        private const string ComponentName = "Generación de Rutas Task";

        private DateTime Desde
        {
            get
            {
                var startDate = GetDateTime("Desde");
                return startDate.HasValue ? startDate.Value : DateTime.UtcNow.AddHours(-3).Date.AddDays(-1).AddHours(3);
            }
        }
        private DateTime Hasta
        {
            get
            {
                var endDate = GetDateTime("Hasta");
                return endDate.HasValue ? endDate.Value : Desde.AddHours(24);
            }
        }

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");

            var inicio = DateTime.UtcNow;

            var empresas = DaoFactory.EmpresaDAO.GetList().Where(e => e.GeneraRutaInversa);

            STrace.Trace(GetType().FullName, string.Format("Procesando empresas. Cantidad: {0}", empresas.Count()));

            try
            {
                foreach (var empresa in empresas)
	            {
		            var vehiculos = DaoFactory.CocheDAO.FindList(new[]{empresa.Id}, new[] { -1 })
                                                       .Where(c => c.Dispositivo != null);
                    var clientes = DaoFactory.ClienteDAO.GetList(new[]{empresa.Id}, new[] { -1 }).Select(c => c.Id).ToList();

                    var vehiculosPendientes = vehiculos.Count();
                    STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", vehiculosPendientes));

                    var eventosEntradas = DaoFactory.LogMensajeDAO.GetByVehiclesAndCode(vehiculos.Select(v => v.Id).ToArray(), MessageCode.InsideGeoRefference.GetMessageCode(), Desde, Hasta, 1);
                    var eventosSalidas = DaoFactory.LogMensajeDAO.GetByVehiclesAndCode(vehiculos.Select(v => v.Id).ToArray(), MessageCode.OutsideGeoRefference.GetMessageCode(), Desde.AddDays(-7), Hasta, 1);

                    foreach (var vehiculo in vehiculos)
                    {
                        STrace.Trace(GetType().FullName, string.Format("Procesando vehículo: {0}", vehiculo.Id));

                        var entradas = eventosEntradas.Where(ev => ev.Coche.Id == vehiculo.Id
                                                                && ev.Fecha > Desde
                                                                && ev.Fecha < Hasta
                                                                && ev.IdPuntoDeInteres.HasValue);
                
                        STrace.Trace(GetType().FullName, string.Format("Entradas a procesar: {0}", entradas.Count()));

                        var i = 1;
                        foreach (var entrada in entradas)
                        {
                            STrace.Trace(GetType().FullName, string.Format("Procesando entrada: {0}/{1}", i, entradas.Count()));
                            
                            var ptoFin = DaoFactory.PuntoEntregaDAO.FindByClientesAndGeoreferencia(clientes, entrada.IdPuntoDeInteres.Value);
                            if (ptoFin == null) continue;
                                              
                            var salidas = eventosSalidas.Where(ev => ev.Coche.Id == vehiculo.Id
                                                                  && ev.Fecha > entrada.Fecha.AddDays(-7)
                                                                  && ev.Fecha < entrada.Fecha.AddSeconds(-1)
                                                                  && ev.IdPuntoDeInteres.HasValue)
                                                        .OrderByDescending(s => s.Fecha);

                            foreach (var salida in salidas)
                            {   
                                var ptoInicio = DaoFactory.PuntoEntregaDAO.FindByClientesAndGeoreferencia(clientes, salida.IdPuntoDeInteres.Value);
                                if (ptoInicio != null)
                                {
                                    if (ptoInicio == ptoFin) break;

                                    var codigo = salida.Fecha.AddHours(-3).ToString("yyyyMMdd") + "|" + vehiculo.Interno + "|" + ptoInicio.Codigo + "-" + ptoFin.Codigo;
                                    var viaje = new ViajeDistribucion
                                    {
                                        Alta = DateTime.UtcNow,
                                        Codigo = codigo.Length <= 32 ? codigo : codigo.Substring(0, 32),
                                        Empresa = empresa,
                                        Estado = ViajeDistribucion.Estados.Cerrado,
                                        Fin = entrada.Fecha,
                                        Inicio = salida.Fecha,
                                        InicioReal = salida.Fecha,
                                        Linea = vehiculo.Linea,
                                        NumeroViaje = 1,
                                        RegresoABase = false,
                                        Tipo = ViajeDistribucion.Tipos.Desordenado,
                                        Vehiculo = vehiculo
                                    };

                                    var entrega1 = new EntregaDistribucion
                                                    {
                                                        Cliente = ptoInicio.Cliente,
                                                        PuntoEntrega = ptoInicio,
                                                        Descripcion = ptoInicio.Descripcion,
                                                        Estado = EntregaDistribucion.Estados.Visitado,
                                                        Orden = 1,
                                                        Programado = salida.Fecha,
                                                        ProgramadoHasta = salida.Fecha,
                                                        Viaje = viaje,
                                                        Salida = salida.Fecha
                                                    };

                                    viaje.Detalles.Add(entrega1);

                                    var fechaFin = entrada.Fecha;
                                    var kms = 0.0;
                                    var origen = new LatLon(ptoInicio.ReferenciaGeografica.Latitude, ptoInicio.ReferenciaGeografica.Longitude);
                                    var destino = new LatLon(ptoFin.ReferenciaGeografica.Latitude, ptoFin.ReferenciaGeografica.Longitude);
                                    var directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving, string.Empty, null);

                                    if (directions != null)
                                    {
                                        var duracion = directions.Duration;
                                        kms = directions.Distance / 1000.0;
                                        fechaFin = salida.Fecha.Add(duracion);
                                    }

                                    var entrega2 = new EntregaDistribucion
                                                {
                                                    Cliente = ptoFin.Cliente,
                                                    PuntoEntrega = ptoFin,
                                                    Descripcion = ptoFin.Descripcion,
                                                    Estado = EntregaDistribucion.Estados.Visitado,
                                                    Orden = 2,
                                                    Programado = fechaFin,
                                                    ProgramadoHasta = fechaFin,
                                                    Viaje = viaje,
                                                    Entrada = entrada.Fecha,
                                                    KmCalculado = kms
                                                };

                                    viaje.Detalles.Add(entrega2);                                        

                                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);
                                    break;
                                }
                            }
                            i++;
                        }
                    
                        STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", --vehiculosPendientes));                    
                    }
	            }

                STrace.Trace(GetType().FullName, "Tarea finalizada.");

                var fin = DateTime.UtcNow;
                var duration = fin.Subtract(inicio).TotalMinutes;

                DaoFactory.DataMartsLogDAO.SaveNewLog(inicio, fin, duration, DataMartsLog.Moludos.GeneracionInversa, "Generación inversa finalizada exitosamente");
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
