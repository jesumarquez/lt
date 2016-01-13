using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Scheduler.Tasks.Mantenimiento.Util;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects.Datamart;
using Logictracker.Utils;
using Logictracker.DAL.Factories;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Messages;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class DatamartTramosTask : BaseVehicleTask
    {
        #region Private Properties

        private DateTime StartDate
        {
            get
            {
                var startDate = GetDateTime("Desde");
                return startDate != null ? startDate.Value : DateTime.Today.AddDays(-1);
            }
        }
        private DateTime EndDate
        {
            get
            {
                var endDate = GetDateTime("Hasta");
                return endDate.HasValue ? endDate.Value : StartDate.AddHours(24);
            }
        }        

        #endregion

        private void SetVehicles()
        {
            var ids = GetListOfInt("Ids");
            var empresas = GetListOfInt("Empresas");

            if (empresas != null && empresas.Count > 0)
                Vehicles = DaoFactory.CocheDAO.GetList(empresas, new[] { -1 }).Select(v => v.Id).ToList();
            else if (ids != null && ids.Count > 0)
                Vehicles = ids;

            VehiclesToProcess = Vehicles.Count;
        }

        protected override void OnExecute(Timer timer)
        {
			STrace.Trace(GetType().FullName, "Processing vehicles.");
            var start = DateTime.UtcNow;

            SetVehicles();

            STrace.Debug(GetType().FullName, "Vehicle to process " + Vehicles.Count);
            var i = 0;
            foreach (var vehicleId in Vehicles)
            {
                i++;
                STrace.Debug(GetType().FullName, string.Format("Processing {0}/{1}", i, Vehicles.Count));
                var vehicle = DaoFactory.CocheDAO.FindById(vehicleId);
                
                try
                {
                    ProcessPeriod(vehicle, StartDate, EndDate);
                }
                catch (Exception ex)
                {
                    STrace.Exception(GetType().FullName, ex);

                    var procesado = false;
                    var retry = 0;
                    while (!procesado && retry < 5)
                    {
                        retry++;
                        try
                        {
                            ProcessPeriod(vehicle, StartDate, EndDate);
                            procesado = true;
                            var parametros = new[] { "Vehículo " + vehicle.Id + " procesado exitosamente luego de " + retry + " intentos.", vehicle.Id.ToString("#0"), DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") };
                        }
                        catch (Exception e)
                        {
                            STrace.Exception(GetType().FullName, e);
                        }
                    }
                    if (retry == 5 && !procesado)
                    {
                        var parametros = new[] { "No se pudieron generar registros de Datamart Tramos para el vehículo: " + vehicle.Id, vehicle.Id.ToString("#0"), DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") };
                        SendMail(parametros);
                        STrace.Error(GetType().FullName, "No se pudieron generar registros de Datamart Tramos para el vehículo: " + vehicle.Id);
                    }
                }
            }

            var fin = DateTime.UtcNow;
            var duracion = fin.Subtract(start).TotalMinutes;

            var param = new[] { "Datamart Tramos", start.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss"), fin.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss"), duracion + " minutos" };
            SendSuccessMail(param);

            DaoFactory.DataMartsLogDAO.SaveNewLog(start, fin, duracion, DataMartsLog.Moludos.DatamartTramos, "Datamart finalizado exitosamente");
        }
        
        private void ProcessPeriod(Coche vehicle, DateTime from, DateTime to)
        {
            using (var transaction = SmartTransaction.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    // Set environment data
                    var inicio = new DateTime(from.Year, from.Month, from.Day, from.Hour, 0, 0);
                    var fin = new DateTime(to.Year, to.Month, to.Day, to.Hour, 0, 0);

                    // Deletes all previously generated datamart tramo records for the current vehicle and time span.
                    var t = new TimeElapsed();
                    DaoFactory.DatamartTramoDAO.DeleteRecords(vehicle.Id, inicio, fin);
                    var ts = t.getTimeElapsed().TotalSeconds;
                    if (ts > 1) STrace.Trace(GetType().FullName, string.Format("DeleteRecords en {0} segundos", ts));

                    var records = new List<DatamartTramo>();
                    var procesado = false;
                    var retry = 0;

                    while (!procesado && retry < 5)
                    {
                        retry++;
                        records = GenerateRecords(vehicle, inicio, fin);

                        t.Restart();
                        foreach (var record in records) DaoFactory.DatamartTramoDAO.Save(record);
                        ts = t.getTimeElapsed().TotalSeconds;
                        if (ts > 1) STrace.Trace(GetType().FullName, string.Format("Save en {0} segundos", ts));

                        transaction.Commit();
                        procesado = true;                        
                    }                    

                    if (retry == 5 && !procesado)
                    {
                        var parametros = new[] { "No se generaron registros de Datamart Tramos para el vehículo: " + vehicle.Id, vehicle.Id.ToString("#0"), DateTime.Today.ToString("dd/MM/yyyy HH:mm") };
                        SendMail(parametros);
                        STrace.Error(GetType().FullName, "No se generaron registros de Datamart Tramos para el vehículo: " + vehicle.Id);
                    }
                }
                catch (Exception ex)
                {
                    AddError(ex);
                    transaction.Rollback();
                }
                finally
                {
                    ClearData();
                    DoSleep();
                }
            }
        }

        private List<DatamartTramo> GenerateRecords(Coche vehiculo, DateTime inicio, DateTime fin)
        {
            var listado = new List<DatamartTramo>();

            var eventos = DaoFactory.LogMensajeDAO.GetEventos(vehiculo.Id, inicio, fin, 3, MessageCode.EngineOn.GetMessageCode(), MessageCode.EngineOff.GetMessageCode())
                                    .OrderBy(e => e.Fecha).ToArray();
            eventos = FiltrarRepetidos(eventos);
            var entradas = DaoFactory.LogMensajeDAO.GetEventos(vehiculo.Id, inicio, fin, 3, MessageCode.InsideGeoRefference.GetMessageCode());
            var dets = DaoFactory.LogMensajeDAO.GetEventos(vehiculo.Id, inicio, fin, 3, MessageCode.StoppedEvent.GetMessageCode());

            var bases = DaoFactory.LineaDAO.GetList(new[] { vehiculo.Empresa.Id });
            var rutas = DaoFactory.ViajeDistribucionDAO.FindList(vehiculo.Id, inicio, fin);
            var entregas = new List<EntregaDistribucion>();
            foreach (var ruta in rutas)
            {
                entregas.AddRange(ruta.Detalles);
            }

            if (eventos.Any())
            {
                var primerEvento = eventos.First();
                var ultimoEvento = eventos.Last();

                // PRIMER TRAMO                
                if (primerEvento.Fecha > inicio)
                {
                    var detenciones = dets.Where(d => d.Fecha >= inicio && d.Fecha < primerEvento.Fecha);

                    var motorOn = primerEvento.Mensaje.Codigo == MessageCode.EngineOff.GetMessageCode();
                    var datamartTramo = CalcularTramo(vehiculo, inicio, primerEvento.Fecha, motorOn, entradas, detenciones, bases, entregas);
                    listado.Add(datamartTramo);
                }

                for (var i = 0; i < eventos.Count() - 1; i++)
                {
                    var evento = eventos[i];
                    var siguiente = eventos[i + 1];

                    var detenciones = dets.Where(d => d.Fecha >= evento.Fecha && d.Fecha < siguiente.Fecha);

                    var motorOn = evento.Mensaje.Codigo == MessageCode.EngineOn.GetMessageCode();
                    var datamartTramo = CalcularTramo(vehiculo, evento.Fecha, siguiente.Fecha, motorOn, entradas, detenciones, bases, entregas);
                    listado.Add(datamartTramo);
                }

                // ULTIMO TRAMO
                if (ultimoEvento.Fecha < fin)
                {
                    var detenciones = dets.Where(d => d.Fecha >= ultimoEvento.Fecha && d.Fecha < fin);

                    var motorOn = ultimoEvento.Mensaje.Codigo == MessageCode.EngineOn.GetMessageCode();
                    var datamartTramo = CalcularTramo(vehiculo, ultimoEvento.Fecha, fin, motorOn, entradas, detenciones, bases, entregas);
                    listado.Add(datamartTramo);
                }
            }

            return listado;
        }

        private LogMensaje[] FiltrarRepetidos(LogMensaje[] eventos)
        {
            var list = new List<LogMensaje>();

            foreach (var evento in eventos)
            {
                if (!eventos.Any(e => e.Id != evento.Id && Math.Abs(e.Fecha.Subtract(evento.Fecha).TotalSeconds) < 5))
                    list.Add(evento);
            }

            return list.ToArray();
        }

        private DatamartTramo CalcularTramo(Coche vehiculo, DateTime fechaInicio, DateTime fechaFin, bool motorOn, IEnumerable<LogMensaje> entradas, IEnumerable<LogMensaje> detenciones, IEnumerable<Linea> bases, IEnumerable<EntregaDistribucion> entregas)
        {
            var datamartTramo = new DatamartTramo();
            datamartTramo.Vehicle = vehiculo;
            datamartTramo.Inicio = fechaInicio;
            datamartTramo.Fin = fechaFin;
            datamartTramo.MotorOn = motorOn;

            var dm = DaoFactory.DatamartDAO.GetSummarizedDatamart(fechaInicio, fechaFin, vehiculo.Id);
            datamartTramo.Kilometros = dm.Kilometros;
            datamartTramo.VelocidadPromedio = (int)dm.AverageSpeed;
            
            #region Geocercas

            var geocercas = entradas.Where(e => e.Fecha >= fechaInicio && e.Fecha < fechaFin);

            var cantBases = 0;
            var cantEntregas = 0;
            var cantOtras = 0;

            var ids = new List<int>();
            foreach (var geocerca in geocercas)
            {
                if (geocerca.IdPuntoDeInteres.HasValue && !ids.Contains(geocerca.IdPuntoDeInteres.Value))
                {
                    ids.Add(geocerca.IdPuntoDeInteres.Value);
                    var esBase = bases.Any(l => l.ReferenciaGeografica != null && l.ReferenciaGeografica.Id == geocerca.IdPuntoDeInteres.Value);
                    if (esBase)
                        cantBases++;
                    else
                    {
                        var esEntrega = entregas.Any(e => e.PuntoEntrega != null && e.PuntoEntrega.ReferenciaGeografica != null
                                                       && e.PuntoEntrega.ReferenciaGeografica.Id == geocerca.IdPuntoDeInteres.Value);
                        if (esEntrega)
                            cantEntregas++;
                        else
                            cantOtras++;
                    }
                }
            }

            #endregion

            datamartTramo.GeocercasBase = cantBases;
            datamartTramo.GeocercasEntregas = cantEntregas;
            datamartTramo.GeocercasOtras = cantOtras;

            #region Detenciones

            var horasDentro = 0.0;
            var horasFuera = 0.0;
            var mayores = 0;
            var menores = 0;

            foreach (var detencion in detenciones)
            {
                var inicioDetencion = detencion.Fecha >= fechaInicio ? detencion.Fecha : fechaInicio;
                var finDetencion = detencion.FechaFin <= fechaFin ? detencion.FechaFin : fechaFin;
                var duracion = finDetencion.Value.Subtract(inicioDetencion);

                if (duracion.TotalMinutes > 2) mayores++;
                else menores++;

                if (detencion.IdPuntoDeInteres.HasValue)
                    horasDentro += duracion.TotalHours;
                else
                    horasFuera += duracion.TotalHours;
            }

            #endregion

            var horasTotales = fechaFin.Subtract(fechaInicio).TotalHours;
            datamartTramo.Horas = horasTotales;
            datamartTramo.HorasDetenidoDentro = horasDentro;
            datamartTramo.HorasDetenidoFuera = horasFuera;            
            datamartTramo.HorasDetenido = horasDentro + horasFuera;
            datamartTramo.HorasMovimiento = horasTotales - horasDentro - horasFuera;            
            datamartTramo.DetencionesMayores = mayores;
            datamartTramo.DetencionesMenores = menores;

            return datamartTramo;
        }

        private void ClearData()
        {
            ClearSessions();

            GC.Collect();
        }

        private void SendMail(string[] parametros)
        {
            var configFile = Config.Mailing.DatamartErrorMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);

            sender.Config.Subject = "Datamart Tramos Error: Id=" + parametros[1] + " Fecha=" + parametros[2];
            
            SendMailToAllDestinations(sender, parametros.ToList());
        }

        private void SendSuccessMail(IEnumerable<string> parametros)
        {
            var configFile = Config.Mailing.DatamartSuccessMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);

            sender.Config.Subject = "Datamart Tramos finalizado exitosamente";
            
            SendMailToAllDestinations(sender, parametros.ToList());
        }
    }
}
