using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects;
using Logictracker.Types.ReportObjects.ControlDeCombustible;
using Logictracker.Types.ValueObjects.Combustible;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using Logictracker.Web.Helpers.ExportHelpers;

namespace Logictracker.Scheduler.Tasks.ReportsScheduler
{
    public class Task : BaseTask
    {
        private int _results;

        protected override void OnExecute(Timer timer)
        {
            var mail = new MailSender(Config.Mailing.ReportSchedulerMailingConfiguration);
            
            // BUSCO TODOS LAS PROGRAMACIONES DIARIAS
            var reportesProgramados = DaoFactory.ProgramacionReporteDAO.FindByPeriodicidad('D');
            
            // SI ES LUNES AGREGO LAS PROGRAMACIONES SEMANALES
            if (DateTime.UtcNow.ToDisplayDateTime().DayOfWeek == DayOfWeek.Monday)
                reportesProgramados.AddRange(DaoFactory.ProgramacionReporteDAO.FindByPeriodicidad('S'));

            // SI ES 1° AGREGO LAS PROGRAMACIONES MENSUALES
            if (DateTime.UtcNow.ToDisplayDateTime().Day == 1)
                reportesProgramados.AddRange(DaoFactory.ProgramacionReporteDAO.FindByPeriodicidad('M'));

            foreach (var programacionReporte in reportesProgramados)
            {
                var reporte = programacionReporte.Reporte;
				var parametros = new Dictionary<String, String>();
				var parametrosCsv = new Dictionary<String, String>();

                if (programacionReporte.Parametros.Contains('&'))
                {
                    foreach (var parametro in programacionReporte.Parametros.Split('&'))
                    {
                        if (parametro.Contains('='))
                        {
                            var key = parametro.Split('=')[0];
                            var value = parametro.Split('=')[1];
                            parametros.Add(key, value);
                        }
                    }
                }
                if (programacionReporte.ParametrosCsv.Contains('&'))
                {
                    foreach (var parametroCsv in programacionReporte.ParametrosCsv.Split('&'))
                    {
                        if (parametroCsv.Contains('='))
                        {
                            var key = parametroCsv.Split('=')[0];
                            var value = parametroCsv.Split('=')[1];
                            parametrosCsv.Add(key, value);
                        }
                    }
                }

                var archivo = GenerarArchivo(programacionReporte.Empresa, programacionReporte.Linea, reporte, parametros, parametrosCsv, programacionReporte.Periodicidad, programacionReporte.Id);
                
                var mails = programacionReporte.Mail.Replace(',', ';').Split(';');
                
                var att = new Attachment(archivo, "Reporte Programado");
                var disposition = att.ContentDisposition;
                disposition.FileName = "ReporteProgramadoLogictracker.xlsx";

                mail.SendMail(att, new[] { reporte });

                foreach (var address in mails)
                {
                    if (mail.Config != null)
                    {
                        mail.Config.ToAddress = address.Trim();
                        mail.SendMail(att, new[] {reporte});
                    }
                }
            }
        }

        private MemoryStream GenerarArchivo(Empresa empresa, Linea linea, String reporte, Dictionary<String, String> parametros, Dictionary<String, String> parametrosCsv, char periodicidad, int idProgramacionReporte)
        {
            var inicio = DateTime.UtcNow.ToDataBaseDateTime();
            
            try
            {
                var timeZoneId = (linea != null) ? linea.TimeZoneId : empresa.TimeZoneId;
                var culture = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                var hasta = DateTime.UtcNow.ToDataBaseDateTime();
                var desde = hasta.AddDays(-1);

                switch (periodicidad)
                {
                    case 'S':
                        desde = hasta.AddDays(-7);
                        break;
                    case 'M':
                        desde = hasta.AddMonths(-1);
                        break;
                }

                _results = 0;
                inicio = DateTime.UtcNow.ToDataBaseDateTime();
                var stream = GetStream(empresa, linea, reporte, parametros, parametrosCsv, desde, hasta, culture);
                var fin = DateTime.UtcNow.ToDataBaseDateTime();

                using (var transaction = SmartTransaction.BeginTransaction())
                {
                    try
                    {
                        var log = new LogProgramacionReporte
                                  {
                                      Inicio = inicio,
                                      Fin = fin,
                                      Filas = _results,
                                      Error = false,
                                      ProgramacionReporte = DaoFactory.ProgramacionReporteDAO.FindById(idProgramacionReporte)
                                  };

                        DaoFactory.LogProgramacionReporteDAO.SaveOrUpdate(log);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(GetType().FullName, ex, "GenerarArchivo(Empresa, Linea, String, Dictionary<String, String>, Dictionary<String, String>, ....)");
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            STrace.Exception(GetType().FullName, ex2, "GenerarArchivo(Empresa, Linea, String, Dictionary<String, String>, Dictionary<String, String>, ....); doing Rollback()");
                        }
                        throw ex;
                    }
                }
                return stream;
            }
            catch (Exception e)
            {

                var log = new LogProgramacionReporte
                          {
                              Inicio = inicio,
                              Fin = DateTime.UtcNow.ToDataBaseDateTime(),
                              Filas = 0,
                              Error = true,
                              ProgramacionReporte = DaoFactory.ProgramacionReporteDAO.FindById(idProgramacionReporte)
                          };

                DaoFactory.LogProgramacionReporteDAO.SaveOrUpdate(log);

                STrace.Exception("Programación de Reportes", e);
            }

            return new MemoryStream();
        }

        private MemoryStream GetStream(Empresa empresa, Linea linea, string reporte, Dictionary<string, string> parametros, Dictionary<string, string> parametrosCsv, DateTime desde, DateTime hasta, TimeZoneInfo culture)
        {
            var path = Config.Scheduler.SchedulerTemplateDirectory + GetTemplateName(reporte);
            //var path = "C:\\GIT\\Logictracker\\Logictracker\\Logictracker\\src\\Web\\Logictracker\\Logictracker.Web\\ExcelTemplate\\Logictracker\\" + GetTemplateName(reporte);
            var builder = new GridToExcelBuilder(path);
            var filters = GetFilters(reporte, parametrosCsv, desde.AddHours(culture.BaseUtcOffset.TotalHours), hasta.AddHours(culture.BaseUtcOffset.TotalHours));
            builder.GenerateHeader(CultureManager.GetMenu(GetVariableName(reporte)), filters);

            switch (reporte)
            {
                case "Mensajes Vehículo":
                    var mensajes = ReportFactory.MobileMessageDAO.GetMobileMessages(int.Parse(parametros["PARENTI03"]), desde, hasta).Select(m => new MobileMessageVo(m, false)).ToList();
                    _results = mensajes.Count;
                    builder.GenerateColumns(mensajes);
                    builder.GenerateFields(mensajes);
                    break;
                case "Detalle de Infracciones":
                    var operadores = parametros.Values.Select(parametro => int.Parse(parametro)).ToList();
                    //var infracciones = ReportFactory.InfractionDetailDAO.GetInfractionsDetails(operadores, desde, hasta).Select(o => new InfractionDetailVo(o)).ToList();
                    //_results = infracciones.Count;
                    //builder.GenerateColumns(infracciones);
                    //builder.GenerateFields(infracciones);
                    break;
                case "Ranking de Choferes":
                    var transportistas = parametros["TRANS"].Split(',').Select(transportista => int.Parse(transportista)).ToList();
                    var tipos = parametros["TIPOS"].Split(',').Select(tipo => int.Parse(tipo)).ToList();
                    var bases = parametros["BASES"].Split(',').Select(bas => int.Parse(bas)).ToList();
                    var centros = new List<Int32> { -1 };
                    var deptos = new List<Int32> { -1 };
                    var distr = new List<Int32> {-1};
                    var ranking = ReportFactory.OperatorRankingDAO.GetRanking(distr, bases, transportistas, tipos, centros, deptos, desde, hasta).Select(o => new OperatorRankingVo(o)).ToList();
                    _results = ranking.Count;
                    builder.GenerateColumns(ranking);
                    builder.GenerateFields(ranking);
                    break;
                case "Consistencia de Pozos":
                    var result = (int.Parse(parametros["TANQUE"]) > 0) ? ReportFactory.ConsistenciaStockPozoDAO.FindConsistenciaBetweenDates(int.Parse(parametros["TANQUE"]), desde, hasta): new List<ConsistenciaStockPozoVo>();
                    var pozos = (from ConsistenciaStockPozo consistencia in result select new ConsistenciaStockPozoVo(consistencia)).ToList();
                    _results = pozos.Count;
                    builder.GenerateColumns(pozos);
                    builder.GenerateFields(pozos);
                    break;
                case "Eventos Combustible":
                    var motores = new List<int>();
                    var tanques = new List<int>();
                    var msjs = parametros["MENSAJES"].Split(',').ToList();
                    var aMotores = parametros["MOTORES"].Split(',');
                    var aTanques = parametros["TANQUES"].Split(',');

                    if (!aMotores[0].Equals(""))
                        motores = aMotores.Select(motor => int.Parse(motor)).ToList();
                    if (!aTanques[0].Equals(""))
                        tanques = aTanques.Select(tanque => int.Parse(tanque)).ToList();

                    var res = ReportFactory.CombustibleEventsDAO.FindByMotoresMensajesAndFecha(motores, tanques, msjs, desde, hasta);
                    var events = (from CombustibleEvent m in res select new CombustibleEventVo(m)).ToList();
                    _results = events.Count;
                    builder.GenerateColumns(events);
                    builder.GenerateFields(events);
                    break;
                case "Cercanía a Puntos de Interés":
                    var distritos = new List<int>();
                    var basess = new List<int>();
                    var user = DaoFactory.UsuarioDAO.FindById(int.Parse(parametros["USER"]));
                    var georef = int.Parse(parametros["GEOREF"]);
                    var distancia = int.Parse(parametros["DISTANCIA"]);
                    var aDistritos = parametros["DISTRITOS"].Split(',');
                    var aBases = parametros["BASES"].Split(',');

                    if (!aDistritos[0].Equals(""))
                        distritos = aDistritos.Select(distrito => int.Parse(distrito)).ToList();
                    if (!aBases[0].Equals(""))
                        basess = aBases.Select(bas => int.Parse(bas)).ToList();

                    var coches = DaoFactory.CocheDAO.FindList(distritos, basess, new[] { -1 }, user)
                        .Where(c => c.Estado < Coche.Estados.Inactivo || c.DtCambioEstado > desde)
                        .ToList();
                    var cercanias = ReportFactory.MobilePoiHistoricDAO.GetMobileNearPois(coches, georef, distancia, desde, hasta).Select(r => new MobilePoiHistoricVo(r)).ToList();
                    _results = cercanias.Count;
                    builder.GenerateColumns(cercanias);
                    builder.GenerateFields(cercanias);
                    break;
                case "Reporte de Eventos":
                    IEnumerable<int> vehiculos = new[] { 0 };
                    IEnumerable<int> msj = new[] { 0 };
                    IEnumerable<int> choferes = new[] { 0 };

                    var aVehiculos = parametros["VEHICULOS"].Split(',');
                    var aMensajes = parametros["MENSAJES"].Split(',');
                    var aChoferes = parametros["CHOFERES"].Split(',');

                    if (!aVehiculos[0].Equals(""))
                        vehiculos = aVehiculos.Select(v => int.Parse(v));
                    if (!aMensajes[0].Equals(""))
                        msj = aMensajes.Select(m => int.Parse(m));
                    if (!aChoferes[0].Equals(""))
                        choferes = aChoferes.Select(c => int.Parse(c));

                    if (vehiculos.Count() == 1 && vehiculos.Contains(0))
                        vehiculos = DaoFactory.CocheDAO.GetList(new[] { empresa != null ? empresa.Id : -1 },
                                                                new[] { linea != null ? linea.Id : -1 })
                                                       .Select(c => c.Id);

                    const int maxMonths = 3;
                    var eventos = ReportFactory.MobileEventDAO.GetMobilesEvents(vehiculos.ToList(), msj, choferes.ToList(), desde, hasta, maxMonths).Select(o => new MobileEventVo(o)).ToList();
                    _results = eventos.Count;
                    builder.GenerateColumns(eventos);
                    builder.GenerateFields(eventos);
                    break;
                case "Reporte de Geocercas":
                    var moviles = new List<int>();
                    var cercas = new List<int>();

                    var aMoviles = parametros["MOVILES"].Split(',');
                    var aGeocercas = parametros["GEOCERCAS"].Split(',');
                    var enCerca = Convert.ToDouble(parametros["ENCERCA"]);
                    var enMarcha = TimeSpan.Parse(parametros["ENMARCHA"]);

                    if (!aMoviles[0].Equals(""))
                        moviles = aMoviles.Select(m => int.Parse(m)).ToList();
                    if (!aGeocercas[0].Equals(""))
                        cercas = aGeocercas.Select(g => int.Parse(g)).ToList();

                    var geocercas = ReportFactory.MobileGeocercaDAO.GetGeocercasEvent(moviles, cercas, desde, hasta, enCerca);

                    for (var i = 0; i < geocercas.Count - 1; i++)
                    {
                        if (geocercas[i].Interno.Equals(geocercas[i + 1].Interno))
                        {
                            var salida = geocercas[i].Salida;
                            var entrada = geocercas[i + 1].Entrada;
                            geocercas[i].ProximaGeocerca = salida.Equals(DateTime.MinValue) || entrada.Equals(DateTime.MinValue) ? TimeSpan.MinValue : entrada.Subtract(salida);
                        }
                    }
                    var resu = new List<MobileGeocerca>(geocercas.Count);

                    for (var i = 0; i < geocercas.Count; i++)
                    {
                        if (geocercas[i].ProximaGeocerca.Equals(TimeSpan.MinValue) || geocercas[i].ProximaGeocerca >= enMarcha)
                        {
                            resu.Add(geocercas[i]);

                            if ((i < geocercas.Count - 1) && (geocercas[i].Interno.Equals(geocercas[i + 1].Interno)))
                            {
                                resu.Add(geocercas[i + 1]);
                                i++;
                            }
                        }
                    }
                    
                    var geofences = resu.Select(geo => new MobileGeocercaVo(geo)).ToList();
                    _results = geofences.Count;
                    builder.GenerateColumns(geofences);
                    builder.GenerateFields(geofences);
                    break;
                case "Reporte de Odometros":
                    var iCoches = new List<int>();
                    var iOdometros = new List<int>();
                    var sCoches = parametros["MOVILES"].Split(',');
                    var sOdometros = parametros["ODOMETROS"].Split(',');
                    var vencimiento = Convert.ToBoolean(parametros["VENCIMIENTO"]);

                    if (!sCoches[0].Equals(""))
                        iCoches = sCoches.Select(c => int.Parse(c)).ToList();
                    if (!sOdometros[0].Equals(""))
                        iOdometros = sOdometros.Select(o => int.Parse(o)).ToList();

                    var p = ReportFactory.OdometroStatusDAO.FindByVehiculosAndOdometros(iCoches, iOdometros, vencimiento);
                    var odometros = (from OdometroStatus m in p select new OdometroStatusVo(m)).ToList();
                    _results = odometros.Count;
                    builder.GenerateColumns(odometros);
                    builder.GenerateFields(odometros);
                    break;
                case "Reporte de Actividad Vehicular":
                    var sBase = parametros["BASE"];
                    var iVehiculos = new List<int>();
                    var sVehiculos = parametros["VEHICULOS"].Split(',');
                    var km = Convert.ToInt32(parametros["KM"]);

                    if (!sVehiculos[0].Equals(""))
                        iVehiculos = sVehiculos.Select(v => int.Parse(v)).ToList();

                    var lin = sBase != "" ? DaoFactory.LineaDAO.FindById(Convert.ToInt32(sBase)) : null;
                    var idEmpresa = lin != null ? lin.Empresa.Id : -1;
                    var idLinea = lin != null ? lin.Id : -1;

                    var activities = ReportFactory.MobileActivityDAO.GetMobileActivitys(desde, hasta, idEmpresa, idLinea, iVehiculos, km).Select(a => new MobileActivityVo(a)).ToList();
                    _results = activities.Count;
                    builder.GenerateColumns(activities);
                    builder.GenerateFields(activities);
                    break;
                case "Resumen Vehicular":
                    var vehiculo = Convert.ToInt32(parametros["VEHICULO"]);
                    var speeds = ReportFactory.MaxSpeedDAO.GetMobileMaxSpeeds(vehiculo, desde, hasta);
                    _results = speeds.Count;
                    builder.GenerateColumns(speeds);
                    builder.GenerateFields(speeds);
                    break;
                case "Control de Ciclo":
                    var iEmpresa = Convert.ToInt32(parametros["EMPRESA"]);
                    var iLinea = Convert.ToInt32(parametros["LINEA"]);
                    var iCliente = Convert.ToInt32(parametros["CLIENTE"]);
                    var iPunto = Convert.ToInt32(parametros["PUNTO"]);
                    var ciclo0 = Convert.ToBoolean(parametros["CICLO_0"]);
                    var obra0 = Convert.ToBoolean(parametros["OBRA_0"]);
                    var sinVehiculo = Convert.ToBoolean(parametros["SIN_VEHICULO"]);

                    var tickets = DaoFactory.TicketDAO.GetList(new[] { iEmpresa }, 
                                                               new[] { iLinea }, 
                                                               new[] { -1 }, // TRANSPORTISTAS
                                                               new[] { -1 }, // DEPARTAMENTOS
                                                               new[] { -1 }, // CENTROS DE COSTO
                                                               new[] { -1 }, // TIPOS DE VEHICULO
                                                               new[] { -1 }, // VEHICULOS
                                                               new[] { -1 }, // ESTADOS
                                                               new[] { iCliente },
                                                               new[] { iPunto }, 
                                                               new[] { -1 },
                                                               desde, 
                                                               hasta)
                                                      .Select(t => new DuracionEstadosVo(t))
                                                      .Where(t => (ciclo0 || t.TiempoCiclo != TimeSpan.Zero) &&
                                                                  (obra0 || t.TiempoEnObra != TimeSpan.Zero) &&
                                                                  (sinVehiculo || t.Vehiculo != String.Empty))
                                                      .ToList();
                    _results = tickets.Count;
                    builder.GenerateColumns(tickets);
                    builder.GenerateFields(tickets);
                    break;
            }

            return builder.Close();
        }

        private static string GetTemplateName(string reporte)
        {
            var template = string.Empty;

            switch (reporte)
            {
                case "Mensajes Vehículo":
                    template = "MensajesVehiculo.xlsx";
                    break;
                case "Detalle de Infracciones":
                    template = "infractionsDetails.xlsx";
                    break;
                case "Ranking de Choferes":
                    template = "OperatorRanking.xlsx";
                    break;
                case "Consistencia de Pozos":
                case "Eventos Combustible":
                    break;
                case "Cercanía a Puntos de Interés":
                    template = "MobilePoisHistoric.xlsx";
                    break;
                case "Reporte de Eventos":
                    template = "eventos.xlsx";
                    break;
                case "Reporte de Geocercas":
                    template = "GeocercasEvents.xlsx";
                    break;
                case "Reporte de Odometros":
                    template = "ReporteOdometros.xlsx";
                    break;
                case "Reporte de Actividad Vehicular":
                    template = "ActividadVehicular.xlsx";
                    break;
                case "Resumen Vehicular":
                    template = "ResumenVehicular.xlsx";
                    break;
                case "Control de Ciclo":
                    template = "DuracionEstadosTicket.xlsx";
                    break;
            }

            return template;
        }
        private static string GetVariableName(string reporte)
        {
            var variableName = string.Empty;

            switch (reporte)
            {
                case "Mensajes Vehículo":
                    variableName = "ACC_REP_DETALLE_RECORRIDO";
                    break;
                case "Detalle de Infracciones":
                    variableName = "ACC_DET_INFRACCIONES";
                    break;
                case "Ranking de Choferes":
                    variableName = "ACC_RANKING_CHOFERES";
                    break;
                case "Consistencia de Pozos":
                case "Eventos Combustible":
                    break;
                case "Cercanía a Puntos de Interés":
                    variableName = "DOP_CERCANIA_POI";
                    break;
                case "Reporte de Eventos":
                    variableName = "DOP_REP_EVENTOS";
                    break;
                case "Reporte de Geocercas":
                    variableName = "DOP_REP_GEOCERCAS";
                    break;
                case "Reporte de Odometros":
                    variableName = "REPORTE_ODOMETROS";
                    break;
                case "Reporte de Actividad Vehicular":
                    variableName = "STAT_ACTI_VEHICULAR";
                    break;
                case "Resumen Vehicular":
                    variableName = "STAT_RESUMEN_VEHICULAR";
                    break;
                case "Control de Ciclo":
                    variableName = "CLOG_REP_DURATION_TICKETS";
                    break;
            }

            return variableName;
        }
        private static Dictionary<string, string> GetFilters(string reporte, Dictionary<string, string> filters, DateTime desde, DateTime hasta)
        {
            switch (reporte)
            {
                case "Mensajes Vehículo":
                case "Detalle de Infracciones":
                case "Ranking de Choferes":
                case "Consistencia de Pozos":
                case "Eventos Combustible":
                case "Cercanía a Puntos de Interés":
                case "Reporte de Geocercas":
                case "Reporte de Odometros":
                case "Reporte de Actividad Vehicular":
                case "Resumen Vehicular":
                case "Control de Ciclo":
                case "Reporte de Eventos":
                    filters.Add("Desde", desde.ToString("dd/MM/yyyy HH:mm"));
                    filters.Add("Hasta", hasta.ToString("dd/MM/yyyy HH:mm"));
                    break;
            }

            return filters;
        }       
    }
}
