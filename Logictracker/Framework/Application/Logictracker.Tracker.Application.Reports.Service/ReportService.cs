using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Reports.Messaging;
using Logictracker.Security;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ReportObjects;
using Logictracker.Types.ReportObjects.Datamart;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using Logictracker.Utils;
using NHibernate.Transform;
using Spring.Messaging.Core;
using System.Text.RegularExpressions;

namespace Logictracker.Tracker.Application.Reports
{
    public class ReportService : IReportService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReportService));
        public MessageQueueTemplate MessageQueueTemplate { get; set; }
        public ReportFactory ReportFactory { get; set; }
        public DAOFactory DaoFactory { get; set; }
        
        //datos servidor correo
        public string MailFrom { get; set; }
        public int SmtpPort { get; set; }
        public string  SmtpAddress { get; set; }
        public string Passwd { get; set; }
        public string SupportMail { get; set; }

        //public ReportService()
        //{
        //    DaoFactory = new DAOFactory();
        //    ReportFactory = new ReportFactory(DaoFactory);    
        //}

        public void GenerateDailyEventReportAndSendMail(int customerId, string email, List<int> vehiclesId, List<int> messagesId, List<int> driversId,
            DateTime initialDate, DateTime finalDate)
        {
            MessageQueueTemplate.ConvertAndSend(new DailyEventReportCommand()
            {
                CustomerId = customerId,
                Email = email,
                DriversId = driversId,
                FinalDate = finalDate,
                InitialDate = initialDate,
                MessagesId = messagesId,
                VehiclesId = vehiclesId
            });
        }

        public  Stream GenerateDailyEventReport(EventReportCommand reportGenerationCommand, IReportStatus reportStatus)
        {
            var command = reportGenerationCommand;

            if (reportGenerationCommand.ReportId!=0)
                reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(reportGenerationCommand.ReportId);

            var customer = DaoFactory.EmpresaDAO.FindById(command.CustomerId);
            var baseName = command.BaseId == 0 ? "Todos" : "Ninguno";

            var results = ReportFactory.MobileEventDAO.GetMobilesEvents(command.VehiclesId,
                command.MessagesId,
                command.DriversId,
                command.InitialDate,
                command.FinalDate,
                1);
            reportStatus.RowCount = results.Count;

            return DailyEventReportGenerator.GenerateReport(results, customer, command.InitialDate, command.FinalDate, baseName);
        }

        public void LogReportExecution(ReportStatus reportStatus)
        {
            var log = new LogProgramacionReporte
            {
                Inicio = reportStatus.StartReport,
                Fin = DateTime.Now,
                Filas = reportStatus.RowCount,
                Error = reportStatus.Error
            };

            if (reportStatus.ReportProg != null)
                log.ProgramacionReporte = reportStatus.ReportProg;

            try
            {
                DaoFactory.LogProgramacionReporteDAO.SaveOrUpdate(log);
            }
            catch (Exception ex)
            {
                if (reportStatus.ReportProg != null)
                    Logger.WarnFormat("No se pudo guardar la informacion de log del reporte {0} debido a {1} ", reportStatus.ReportProg.ReportName, ex.Message);
                else
                    Logger.WarnFormat("No se pudo guardar la informacion de log del reporte {0}  ", ex.Message);
            }
        }

        public void SendReport(Stream reportStream, IReportCommand command, string report)
        {
            var subject = report + " Logictracker";
            var body =
                string.Format("Usted ha solicitado un {0} a través de la plataforma Logictracker. Se ha adjuntado un archivo de Excel.", report);
            body += "\n\n Este mensaje se ha generado automaticamente, No responda este correo.";

            var filename = "Reporte Logictracker " + DateTime.Now.ToString("G") + ".xls";
            if (report != null)
            filename = report.Trim() + " " + command.InitialDate.ToString("yyyy-MM-dd") + " a " + command.FinalDate.ToString("yyyy-MM-dd") + ".xls";

            var emailList = ValidateAddress(command.Email);

            Notifier.SmtpMail(MailFrom, emailList, subject, body, reportStream, filename,SmtpPort, SmtpAddress, Passwd);
        }

        private static List<string> ValidateAddress(string email)
        {
            var emailAddresses = email.Replace(',', ';').Split(';');

            var emails = new List<string>();
            foreach (string emailAddress in emailAddresses)
            {
                if (IsValidEmail(emailAddress.Trim()))
                    emails.Add(emailAddress.Trim());
                else
                    Logger.WarnFormat("Direccion de Correo Invalida: {0}", emailAddress);
            }
            return emails;
        }

        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                    @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        public void SendReport(string reportExecution, string reporteDeEjecucion)
        {
            var subject = "Reporte de ejecucion de Reportes Programados";
            var body = reportExecution;

            var emailList = ValidateAddress(SupportMail);

            Notifier.SmtpMail(MailFrom, emailList, subject, body, null, null, SmtpPort, SmtpAddress, Passwd);            
       
        }

        public string GenerateFinalExcecutionReport(FinalExecutionCommand command, ReportStatus statusReport)
        {
            var execReport = new StringBuilder();
            var reportLogs = DaoFactory.LogProgramacionReporteDAO.FindAll();
            foreach (var reportlog in reportLogs)
            {
                if (reportlog.Inicio.Date == command.InitialDate)
                {
                    var report = DaoFactory.ProgramacionReporteDAO.FindById(reportlog.ProgramacionReporte.Id);
                    execReport.AppendLine(string.Format("Nombre : {0} ", report.ReportName));
                    execReport.AppendLine(string.Format("Tipo : {0} ", reportlog.ProgramacionReporte.Report));
                    execReport.AppendLine(string.Format("Empresa : {0} ", reportlog.ProgramacionReporte.Empresa.RazonSocial));
                    execReport.AppendLine(string.Format("Inicio : {0} ", reportlog.Inicio));
                    execReport.AppendLine(string.Format("Fin : {0} ", reportlog.Fin));
                    execReport.AppendLine(string.Format("Filas : {0} ", reportlog.Filas));
                    execReport.AppendLine(string.Format("Errores : {0} ", reportlog.Error? "Si" : "No"));
                }
                execReport.AppendLine();
            }
            return execReport.ToString();
        }

        public Stream GenerateAccumulatedKilometersReport(AccumulatedKilometersReportCommand reportGenerationCommand, ReportStatus reportStatus)
        {
            var command = reportGenerationCommand;

            if (reportGenerationCommand.ReportId != 0)
                reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(reportGenerationCommand.ReportId);

            var customer = DaoFactory.EmpresaDAO.FindById(command.CustomerId);
            var baseName = command.BaseId == 0 ? "Todos" : "Ninguno";

            var results = ReportFactory.MobilesKilometersDAO.GetMobilesKilometers(command.InitialDate, command.FinalDate, command.VehiclesId, command.InCicle);
            
            reportStatus.RowCount = results.Count;

            return AccumulatedKilometersReportGenerator.GenerateReport(results, customer, command.InitialDate.ToLocalTime(), command.FinalDate.ToLocalTime(), baseName);
        }

        public Stream GenerateVehicleActivityReport(VehicleActivityReportCommand cmd, ReportStatus reportStatus)
        {
            var command = cmd;

            if (cmd.ReportId != 0)
                reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            var customer = DaoFactory.EmpresaDAO.FindById(command.CustomerId);
            var baseName = "Ninguno"; 

            var results = ReportFactory.MobileActivityDAO.GetMobileActivitys(cmd.InitialDate, cmd.FinalDate, cmd.CustomerId, -1, cmd.VehiclesId, cmd.OvercomeKilometers);
            
            //var results = (from activity in activities select new MobileActivityVo(activity, desde, hasta, chkDetalleInfracciones.Checked)).ToList();
            reportStatus.RowCount = results.Count;

            return VehicleActivityReportGenerator.GenerateReport(results, customer, command.InitialDate, command.FinalDate, baseName);
        }

        public Stream GenerateVehicleInfractionsReport(VehicleInfractionsReportCommand cmd, ReportStatus reportStatus)
        {
            if (cmd.ReportId != 0)
                reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = cmd.BaseId == 0 ? "Todos" : "Ninguno";

            var results = ReportFactory.InfractionDetailDAO.GetInfractionsDetailsByVehicles(cmd.VehiclesId, cmd.InitialDate, cmd.FinalDate).ToList();

            reportStatus.RowCount = results.Count;

            return VehicleInfractionsReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);

        }

        public Stream GenerateDriversInfractionReport(DriversInfractionsReportCommand cmd, ReportStatus reportStatus)
        {
            if (cmd.ReportId != 0)
                reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            if (cmd.CustomerId == 0) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = cmd.BaseId == 0 ? "Todos" : "Ninguno";
            
            var empresas = new[] {cmd.CustomerId};
            var lineas = new[] {cmd.BaseId};
            var transportadores = new int[] {};

            var results = ReportFactory.InfractionDetailDAO.GetInfractionsDetails(empresas, lineas, transportadores, cmd.DriversId, cmd.InitialDate, cmd.FinalDate).ToList();

            reportStatus.RowCount = results.Count;

            return DriversInfractionsReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);
        }

        public Stream GenerateGeofenceEventsReport(GeofenceEventsReportCommand cmd, ReportStatus reportStatus)
        {
            if (cmd.ReportId != 0)
                reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            if (cmd.CustomerId == 0) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = cmd.BaseId == 0 ? "Todos" : "Ninguno";

            var results = ReportFactory.MobileGeocercaDAO.GetGeocercasEvent(cmd.VehiclesId, cmd.Geofences, cmd.InitialDate, cmd.FinalDate, cmd.InGeofenceTime);
            //CalculateDurations(geocercas, chkCalcularKmRecorridos.Checked, DAOFactory);
            //FilterGeocercas(geocercas);

            reportStatus.RowCount = results.Count;

            return GeofenceEventsReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);
        }

        public Stream GenerateMobilesTimeReport(MobilesTimeReportCommand cmd, ReportStatus reportStatus)
        {
            if (cmd.ReportId != 0)
                reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            if (cmd.CustomerId == 0) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = "Todos";

            var results = ReportFactory.MobilesTimeDAO.GetMobilesTime(cmd.InitialDate, cmd.FinalDate, cmd.VehiclesId);

            reportStatus.RowCount = results.Count;

            return MobilesTimeReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);
        }

        public Stream GenerateDocumentExpirationReport(DocumentsExpirationReportCommand cmd, ReportStatus reportStatus)
        {
            if (cmd.ReportId != 0)
                reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            if (cmd.CustomerId == 0) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = "Todos";

            var results = DaoFactory.DocumentoDAO.FindByTipo(cmd.Documents.ToArray(), new List<int> { cmd.CustomerId }, new List<int> { -1 });

            reportStatus.RowCount = results.Count;

            return DocumentsExpirationReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);
        }

        public Stream GenerateOdometersReport(OdometersReportCommand cmd, ReportStatus reportStatus)
        {
            if (cmd.ReportId != 0)
                reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            if (cmd.CustomerId == 0) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = "Todos";

            var results = (IEnumerable<OdometroStatus>) ReportFactory.OdometroStatusDAO.FindByVehiculosAndOdometros(cmd.VehiclesId, cmd.Odometers, false);

            reportStatus.RowCount = 0;

            return OdometersReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);
        }

        public Stream GenerateTransfersPerTripReport(TransfersPerTripReportCommand command, ReportStatus statusReport)
        {
            if (command.ReportId != 0)
                statusReport.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(command.ReportId);

            if (command.CustomerId == 0) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(command.CustomerId);
            var baseName = "Todos";

            var desde = command.InitialDate;
            var hasta = command.FinalDate;

            if (hasta > DateTime.Today) hasta = DateTime.Today;

            var viajes = DaoFactory.ViajeDistribucionDAO.GetList(new[] { command.CustomerId }, //empresas
                                                                    new[] { -1 }, //plantas
                                                                    new[] { 0 }, //transportistas
                                                                    new[] { -1 }, // DEPTOS
                                                                    new[] { -1 }, // CENTROS
                                                                    new[] { -1 }, // SUBCC
                                                                    command.VehiclesId,
                                                                    desde,
                                                                    hasta)
                                                        .Where(v => v.Vehiculo != null && v.InicioReal.HasValue);

            var idsViajes = viajes.Select(v => v.Id);
            var dms = new List<DatamartViaje>();
            foreach (var ids in idsViajes.InSetsOf(1500))
            {
                var datamarts = DaoFactory.DatamartViajeDAO.GetList(ids.ToArray());
                if (datamarts != null && datamarts.Any())
                    dms.AddRange(datamarts);
            }

            var results = dms.Select(datamartViaje => new ReporteTrasladoVo(datamartViaje.Viaje, datamartViaje.KmTotales, datamartViaje.KmImproductivos, datamartViaje.KmProductivos, datamartViaje.KmProgramados)).ToList();

        
            statusReport.RowCount = viajes.Count();

            return TransfersPerTripReportGenerator.GenerateReport(results, customer, command.InitialDate.ToLocalTime(), command.FinalDate.ToLocalTime(), baseName);
        }

        public Stream GenerateDeliverStatusReport(DeliverStatusReportCommand cmd, ReportStatus reportStatus)
        {
            if (cmd.ReportId != 0)
                reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            if (cmd.CustomerId == 0) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = "Todos";

            var results = new List<ReporteDistribucionVo>();

            //if (ddlRuta.Selected > 0)
            //{
            //    var dms = DAOFactory.DatamartDistribucionDAO.GetRecords(ddlRuta.Selected);
            //    foreach (var dm in dms)
            //    {
            //        results.Add(new ReporteDistribucionVo(dm, chkVerConfirmacion.Checked));
            //    }

            //    return results;
            //}

            //if (QueryExtensions.IncludesAll(ddlVehiculo.SelectedValues))
            //    ddlVehiculo.ToogleItems();
            //if (ddlEstados.SelectedStringValues.Count == 0)
            //    ddlEstados.ToogleItems();

            //var desde = dpDesde.SelectedDate.Value.ToDataBaseDateTime();
            //var hasta = dpHasta.SelectedDate.Value.ToDataBaseDateTime();
            var sql = DaoFactory.DatamartDistribucionDAO.GetReporteDistribucion(cmd.CustomerId,
                                                                                   0,
                                                                                   cmd.VehiclesId,
                                                                                   0,
                                                                                   new List<int> { -1 },
                                                                                   cmd.InitialDate,
                                                                                   cmd.FinalDate);

            sql.SetResultTransformer(Transformers.AliasToBean(typeof(ReporteDistribucionVo)));
            var report = sql.List<ReporteDistribucionVo>();
            results = report.Select(r => new ReporteDistribucionVo(r)).ToList();

            if (cmd.FinalDate > DateTime.Today.ToDataBaseDateTime())
            {
                var viajesDeHoy = DaoFactory.ViajeDistribucionDAO.GetList( new[] { cmd.CustomerId },
                                                                          new[] { -1 },
                                                                          new[] { -1 },
                                                                          new[] { -1 }, // DEPARTAMENTOS
                                                                          new[] { -1 }, // CENTROS DE COSTO
                                                                          new[] { -1 }, // SUB CENTROS DE COSTO
                                                                          cmd.VehiclesId,
                                                                          new[] { -1 }, // EMPLEADOS
                                                                          new[] { -1 }, // ESTADOS
                                                                          DateTime.Today.ToDataBaseDateTime(),
                                                                          cmd.FinalDate);

                foreach (var viaje in viajesDeHoy)
                {
                    EntregaDistribucion anterior = null;

                    //var estados = ddlEstados.SelectedValues;
                    var detalles = viaje.Detalles;

                    //if (chkVerOrdenManual.Checked)
                        //detalles = viaje.GetEntregasPorOrdenManual();
                    //else 
                    if (viaje.Tipo == ViajeDistribucion.Tipos.Desordenado)
                        detalles = viaje.GetEntregasPorOrdenReal();

                    //detalles = detalles.Where(e => ddlPuntoEntrega.Selected == 0 ||
                    //                               (e.PuntoEntrega != null && e.PuntoEntrega.Id == ddlPuntoEntrega.Selected))
                    //                   .Where(e => estados.Contains(e.Estado))
                    //                   .ToList();

                    var orden = 0;
                    foreach (var entrega in detalles)
                    {
                        var kms = 0.0;

                        if (anterior != null && !entrega.Estado.Equals(EntregaDistribucion.Estados.Cancelado)
                         && !entrega.Estado.Equals(EntregaDistribucion.Estados.NoCompletado)
                         && !entrega.Estado.Equals(EntregaDistribucion.Estados.SinVisitar)
                         && entrega.Viaje.Vehiculo != null
                         && anterior.FechaMin < entrega.FechaMin
                         && entrega.FechaMin < DateTime.MaxValue)
                            kms = DaoFactory.CocheDAO.GetDistance(entrega.Viaje.Vehiculo.Id, anterior.FechaMin, entrega.FechaMin);

                        results.Add(new ReporteDistribucionVo(entrega, anterior, orden, kms, false));
                        orden++;
                        if (!entrega.Estado.Equals(EntregaDistribucion.Estados.Cancelado)
                         && !entrega.Estado.Equals(EntregaDistribucion.Estados.NoCompletado)
                         && !entrega.Estado.Equals(EntregaDistribucion.Estados.SinVisitar))
                            anterior = entrega;
                    }
                }
            }


            ////
            //var results = ReportFactory.OdometroStatusDAO.FindByVehiculosAndOdometros(cmd.VehiclesId, cmd.Odometers, false);

            reportStatus.RowCount = 0;

            return DeliverStatusReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);
        
        }

        public Stream GenerateSummaryRoutesReport(SummaryRoutesReportCommand cmd, ReportStatus reportStatus)
        {
            if (cmd.ReportId != 0)
                reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            if (cmd.CustomerId == 0) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = "Todos";

            var results = new List<ResumenDeRutasVo>();
            var viajes = new List<ViajeDistribucion>();
            //tbl_totales.Visible = false;



                viajes = DaoFactory.ViajeDistribucionDAO.GetList(new[] { cmd.CustomerId },
                                                                 new[] { -1 },
                                                                 new[] { -1 },//lbTransportista.SelectedValues,}
                                                                 new[] { -1 },//lbDepartamento.SelectedValues,
                                                                 new[] { -1 },//lbCentroDeCostos.SelectedValues,
                                                                 new[] { -1 },//lbSubCentroDeCostos.SelectedValues,
                                                                 cmd.VehiclesId,
                                                                 cmd.InitialDate,
                                                                 cmd.FinalDate);
            
                results = viajes.Select(v => new ResumenDeRutasVo(v, true)).ToList();

            reportStatus.RowCount = viajes.Count();

            return SummaryRoutesReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);
       
        }
    }
}
