using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
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

        public ReportService(DAOFactory daoFactory, ReportFactory reportFactory)
        {
            DaoFactory = daoFactory;
            ReportFactory = reportFactory;
        }

        public ReportService()
        {}

        #region IoC Objects
        public MessageQueueTemplate MessageQueueTemplate { get; set; }
        public ReportFactory ReportFactory { get; set; }
        public DAOFactory DaoFactory { get; set; }
        
        //datos servidor correo
        public string MailFrom { get; set; }
        public int SmtpPort { get; set; }
        public string  SmtpAddress { get; set; }
        public string Passwd { get; set; }
        public string SupportMail { get; set; }

        #endregion

        #region command generation

        public void GenerateFinalReportAndSendMail(DateTime dateTime, string mail)
        {
            MessageQueueTemplate.ConvertAndSend(new FinalExecutionCommand()
            {
                Email = mail,
                FinalDate = dateTime.Date,
                InitialDate = dateTime.AddDays(1).Date
            });
        }

        public void GenerateEventReportAndSendMail(int customerId, string email, List<int> vehiclesId, List<int> messagesId, List<int> driversId,
           DateTime initialDate, DateTime finalDate)
        {
            MessageQueueTemplate.ConvertAndSend(new EventReportCommand()
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

        public static IReportCommand CreateOdometerReportCommand(int reportId, int customerId, int linea, string mail, DateTime finalDate, DateTime initialDate, List<int> odometersId, List<int> vehiclesId, ProgramacionReporte.FormatoReporte format)
        {
            return new OdometersReportCommand
            {
                ReportId = reportId,
                CustomerId = customerId,
                BaseId = linea,
                Email = mail,
                FinalDate = finalDate,
                InitialDate = initialDate,
                Odometers = odometersId,
                VehiclesId = vehiclesId,
                ReportFormat = format,
                ReportName = "Odometros " + initialDate.ToShortDateString() + " - " + finalDate.ToShortDateString()
            };
        }

        public static IReportCommand CreateExecutionReportCommand()
        {
            return new FinalExecutionCommand
            {
                InitialDate = DateTime.Now,
                ReportFormat = ProgramacionReporte.FormatoReporte.Html
            };
        }

        public static IReportCommand CreateVehicleVerifierCommand(int reportId, int customerId, int baseId, string mail, List<int> carriers, List<int> costCenters, List<int> vehicleTypes)
        {
            return new VehicleVerifierCommand
            {
                ReportId = reportId,
                CustomerId = customerId,
                BaseId = baseId,
                Email = mail,
                VehicleType = vehicleTypes.FirstOrDefault(),
                Carrier = carriers.FirstOrDefault(),
                CostCenter = costCenters.FirstOrDefault(),
                ReportFormat = ProgramacionReporte.FormatoReporte.Html,
                ReportName = "Verificador de Vehiculos " + DateTime.Now.ToShortDateString()
            };
        }

        public static IReportCommand CreateEventReportCommand(int reportId, int customerId, int baseId, string mail, List<int> drivers, DateTime finalDateTime, DateTime initialDateTime, List<int> messageTypes, List<int> vehicles)
        {
            return new EventReportCommand()
            {
                ReportId = reportId, //Id de reporte manual inactivo
                CustomerId = customerId,
                BaseId = baseId,
                Email = mail,
                DriversId = drivers,// GetSelectedListByField("drivers"),
                FinalDate = finalDateTime,
                InitialDate = initialDateTime,
                MessagesId = messageTypes,
                VehiclesId = vehicles,
                ReportFormat = ProgramacionReporte.FormatoReporte.Excel,
                ReportName = "Reporte de Eventos " + initialDateTime.ToShortDateString() + " - " + finalDateTime.ToShortDateString()
            };
        }

        public static IReportCommand CreateVehicleActivityReportCommand(int reportId, int customerId, int baseId, string mail, DateTime finalDateTime, DateTime initialDateTime, List<int> vehicles)
        {
            return new VehicleActivityReportCommand
            {
                ReportId = reportId,
                CustomerId = customerId,
                BaseId = baseId,
                Email = mail,
                FinalDate = finalDateTime,
                InitialDate = initialDateTime,
                VehiclesId = vehicles,
                ReportFormat = ProgramacionReporte.FormatoReporte.Excel,
                ReportName = "Actividad Vehicular" + initialDateTime.ToShortDateString() + " - " + finalDateTime.ToShortDateString()
            };
        }

        public static IReportCommand CreateVehicleInfractionsReportCommand(int reportId, int customerId, int baseId, string mail, DateTime finalDateTime, DateTime initialDateTime, List<int> vehicles)
        {
            return new VehicleInfractionsReportCommand
            {
                ReportId = reportId,
                CustomerId = customerId,
                BaseId = baseId,
                Email = mail,
                FinalDate = finalDateTime,
                InitialDate = initialDateTime,
                VehiclesId = vehicles,
                ReportFormat = ProgramacionReporte.FormatoReporte.Excel,
                ReportName = "Infracciones por Vehiculo " + initialDateTime.ToShortDateString() + " - " + finalDateTime.ToShortDateString()
            };
        }

        public static IReportCommand CreateDriversInfractionsReportCommand(int reportId, int customerId, int baseId, string mail, DateTime finalDateTime, DateTime initialDateTime, List<int> drivers, ProgramacionReporte.FormatoReporte reportFormat)
        {
            return new DriversInfractionsReportCommand
            {
                ReportId = reportId,
                CustomerId = customerId,
                BaseId = baseId,
                Email = mail,
                FinalDate = finalDateTime,
                InitialDate = initialDateTime,
                DriversId = drivers,
                ReportFormat = reportFormat,
                ReportName = "Infracciones por Conductor " + initialDateTime.ToShortDateString() + " - " + finalDateTime.ToShortDateString()
            };
        }

        public static IReportCommand CreateGeofenceEventsReportCommand(int reportId, int customerId, int baseId, string mail, DateTime finalDateTime, DateTime initialDateTime, List<int> vehicles, List<int> geofences, ProgramacionReporte.FormatoReporte reportFormat)
        {
            return new GeofenceEventsReportCommand
            {
                ReportId = reportId,
                CustomerId = customerId,
                BaseId = baseId,
                Email = mail,
                FinalDate = finalDateTime,
                InitialDate = initialDateTime,
                VehiclesId = vehicles,
                Geofences = geofences,
                ReportFormat = reportFormat,
                ReportName = "Eventos de Geocercas " + initialDateTime.ToShortDateString() + " - " + finalDateTime.ToShortDateString()
            };
        }

        public static IReportCommand CreateDocumentExpirationReportCommand(int reportId, int customerId, int baseId, string mail, DateTime finalDate, DateTime initDate, List<int> documents, ProgramacionReporte.FormatoReporte reportFormat)
        {
            return new DocumentsExpirationReportCommand
            {
                ReportId = reportId,
                CustomerId = customerId,
                BaseId = baseId,
                Email = mail,
                FinalDate = finalDate,
                InitialDate = initDate,
                Documents = documents,
                ReportFormat = reportFormat,
                ReportName = "Vencimiento de Documentos " + initDate.ToShortDateString() + " - " + finalDate.ToShortDateString()
            };
        }

        public static IReportCommand CreateDeliverStatusReportCommand(int reportId, int customerId, int baseId, string mail, DateTime finalDate, DateTime initDate, List<int> vehicles)
        {
            return new DeliverStatusReportCommand
            {
                ReportId = reportId,
                CustomerId = customerId,
                BaseId = baseId,
                Email = mail,
                FinalDate = finalDate,
                InitialDate = initDate,
                VehiclesId = vehicles,
                ReportFormat = ProgramacionReporte.FormatoReporte.Excel,
                ReportName = "Estado de Entregas " + initDate.ToShortDateString() + " - " + finalDate.ToShortDateString()
            };
        }

        public static IReportCommand CreateTransfersPerTripReportCommand(int reportId, int customerId, int baseId, string mail, DateTime finalDate, DateTime initDate, List<int> vehicles)
        {
            return new TransfersPerTripReportCommand
            {
                ReportId = reportId,
                CustomerId = customerId,
                BaseId = baseId,
                Email = mail,
                FinalDate = finalDate,
                InitialDate = initDate,
                VehiclesId = vehicles,
                ReportFormat = ProgramacionReporte.FormatoReporte.Excel,
                ReportName = "Traslados por Viaje " + initDate.ToShortDateString() + " - " + finalDate.ToShortDateString()
            };
        }

        public static IReportCommand CreateSummaryRoutesReportCommand(int reportId, int customerId, int baseId, string mail, DateTime finalDate, DateTime initDate, List<int> vehicles)
        {
            return new SummaryRoutesReportCommand
            {
                ReportId = reportId,
                CustomerId = customerId,
                Email = mail,
                FinalDate = finalDate,
                InitialDate = initDate,
                VehiclesId = vehicles,
                ReportFormat = ProgramacionReporte.FormatoReporte.Excel,
                ReportName = "Resumen de Rutas " + initDate.ToShortDateString() + " - " + finalDate.ToShortDateString()
            };
        }

        public static IReportCommand CreateAccumulatedKilometersReportCommand(int reportId, int customerId, int baseId, string mail, DateTime finalDate, DateTime initDate, List<int> vehicles)
        {
            return new AccumulatedKilometersReportCommand
            {
                ReportId = reportId, //Id de reporte manual inactivo
                CustomerId = customerId,
                BaseId = baseId,
                Email = mail,
                FinalDate = finalDate,
                InitialDate = initDate,
                VehiclesId = vehicles,
                ReportFormat = ProgramacionReporte.FormatoReporte.Excel,
                ReportName = "Kilometros Acumulados " + initDate.ToShortDateString() + " - " + finalDate.ToShortDateString()
            };
        }

        public static IReportCommand CreateMobilesTimeReportCommand(int reportId, int customerId, int baseId, string mail, DateTime finalDate, DateTime initDate, List<int> vehicles)
        {
            return new MobilesTimeReportCommand
            {
                ReportId = reportId,
                CustomerId = customerId,
                Email = mail,
                FinalDate = finalDate,
                InitialDate = initDate,
                VehiclesId = vehicles,
                ReportFormat = ProgramacionReporte.FormatoReporte.Excel,
                ReportName = "Tiempo Acumulado " + initDate.ToShortDateString() + " - " + finalDate.ToShortDateString()
            };
        }

        #endregion

        #region report generation
       
        public  Stream GenerateEventReport(EventReportCommand command, IReportStatus reportStatus)
        {
            var results = ReportFactory.MobileEventDAO.GetMobilesEvents(command.VehiclesId,
                command.MessagesId,
                command.DriversId,
                command.InitialDate,
                command.FinalDate,
                1);

            reportStatus.RowCount = results.Count;

            if (results.Count < 1) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(command.CustomerId);
            var baseName = GetLinea(command.BaseId); 
            
            return EventReportGenerator.GenerateReport(results, customer, command.InitialDate, command.FinalDate, baseName);
        }

        private string GetLinea(int baseId)
        {
            var baseName = "Todos";
            if ((baseId != -1) && (baseId != 0))
                baseName = DaoFactory.LineaDAO.FindById(baseId).Descripcion;

            return baseName;
        }

        public string GenerateFinalExcecutionReport(FinalExecutionCommand command, IReportStatus statusReport)
        {
            var execReport = new StringBuilder();
            var reports = DaoFactory.ProgramacionReporteDAO.FindAll();        
            //var reportLogs = DaoFactory.LogProgramacionReporteDAO.FindAll();
            
            foreach (var report in reports)
            {
                var logs = report.ReportLogs.Where(l => l.Inicio.Date == DateTime.Now.Date);
                foreach (var log in logs)
                {
                    //var r = DaoFactory.ProgramacionReporteDAO.FindById(report.Id);
                    execReport.AppendLine(string.Format("Nombre : {0} ", report.ReportName));
                    execReport.AppendLine(string.Format("Tipo : {0} ", report.ReportName));
                    execReport.AppendLine(string.Format("Empresa : {0} ", report.Empresa.RazonSocial));
                    execReport.AppendLine(string.Format("Inicio : {0} ", log.Inicio));
                    execReport.AppendLine(string.Format("Fin : {0} ", log.Fin));
                    execReport.AppendLine(string.Format("Filas : {0} ", log.Filas));
                    execReport.AppendLine(string.Format("Errores : {0} ", log.Error? "Si" : "No"));
                    execReport.AppendLine();
                }
            }
            return execReport.ToString();
        }

        public string GenerateVehicleVerifierReport(VehicleVerifierCommand cmd, IReportStatus statusReport)
        {
            var mobiles = DaoFactory.CocheDAO.GetList(new[] { cmd.CustomerId },
                                          new[] { cmd.BaseId != -1 ? cmd.BaseId : 0 },
                                          new[] { -1 }, // TipoVehiculo
                                          new[] { -1 }, // Transportista
                                          new[] { -1 }, // DEPARTAMENTOS
                                          new[] { -1 }, // CostCenter
                                          new[] { -1 }, // SUB CENTROS DE COSTO
                                          true,         // DispositivosAsignados,
                                          false         // SoloConGarmin
                                          );

            var lastPositions = ReportFactory.MobilePositionDAO.GetMobilesLastPosition(mobiles);

            var activos = lastPositions.Count(p => p.EstadoReporte <= 2);
            var inactivos = lastPositions.Count(p => p.EstadoReporte > 2);

            return ConvertReportToHtml(cmd, activos, inactivos);
        }

        public Stream GenerateAccumulatedKilometersReport(AccumulatedKilometersReportCommand command, IReportStatus reportStatus)
        {
            var results = ReportFactory.MobilesKilometersDAO.GetMobilesKilometers(command.InitialDate, command.FinalDate, command.VehiclesId, true);
            reportStatus.RowCount = results.Count;
            
            if (results.Count < 1) return null;
            
            var customer = DaoFactory.EmpresaDAO.FindById(command.CustomerId);
            var baseName = GetLinea(command.BaseId); 

            //if (command.ReportId != 0)
            //    reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(command.ReportId);
            
            return AccumulatedKilometersReportGenerator.GenerateReport(results, customer, command.InitialDate.ToLocalTime(), command.FinalDate.ToLocalTime(), baseName);
        }

        public Stream GenerateVehicleActivityReport(VehicleActivityReportCommand cmd, IReportStatus reportStatus)
        {
            var results = ReportFactory.MobileActivityDAO.GetMobileActivitys(cmd.InitialDate, cmd.FinalDate, cmd.CustomerId, -1, cmd.VehiclesId, 0);

            reportStatus.RowCount = results.Count;

            if (results.Count < 1) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = GetLinea(cmd.BaseId); 
            
            //var results = (from activity in activities select new MobileActivityVo(activity, desde, hasta, chkDetalleInfracciones.Checked)).ToList();
            reportStatus.RowCount = results.Count;

            return VehicleActivityReportGenerator.GenerateReport(results, customer, cmd.InitialDate, cmd.FinalDate, baseName);
        }

        public Stream GenerateVehicleInfractionsReport(VehicleInfractionsReportCommand cmd, IReportStatus reportStatus)
        {
            //var results = ReportFactory.InfractionDetailDAO.GetInfractionsDetailsByVehicles(cmd.VehiclesId, cmd.InitialDate, cmd.FinalDate).ToList();
            var results = VehicleInfractionsReport(cmd.VehiclesId, cmd.InitialDate, cmd.FinalDate, true);

            reportStatus.RowCount = results.Count;

            if (results.Count < 1) return null;            
            
            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = GetLinea(cmd.BaseId); 

            return VehicleInfractionsReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);

        }

        public Stream GenerateDriversInfractionReport(DriversInfractionsReportCommand cmd, IReportStatus reportStatus)
        {
            if (cmd.CustomerId == 0) return null;
            
            var empresas = new[] {cmd.CustomerId};
            var lineas = new[] {cmd.BaseId};
            var transportadores = new int[] {};

            //var results = ReportFactory.InfractionDetailDAO.GetInfractionsDetails(empresas, lineas, transportadores, 
            //    cmd.DriversId, cmd.InitialDate, cmd.FinalDate).ToList();
            //
            var desde = cmd.InitialDate.ToDataBaseDateTime();
            var hasta = cmd.FinalDate.ToDataBaseDateTime();
            
            var results = ReportFactory.InfractionDetailDAO.GetInfractionsDetails(empresas,lineas, transportadores,  cmd.DriversId, desde, hasta)
                        .Select(o => new InfractionDetailVo(o) { HideCornerNearest = false }).ToList();
            //
            reportStatus.RowCount = results.Count;
            if (results.Count < 1) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = GetLinea(cmd.BaseId); 
            
            return DriversInfractionsReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);
        }

        public Stream GenerateGeofenceEventsReport(GeofenceEventsReportCommand cmd, IReportStatus reportStatus)
        {
            if (cmd.CustomerId == 0) return null;

            var results = ReportFactory.MobileGeocercaDAO.GetGeocercasEvent(cmd.VehiclesId, cmd.Geofences, cmd.InitialDate, cmd.FinalDate, 1);
            //CalculateDurations(geocercas, chkCalcularKmRecorridos.Checked, DAOFactory);
            //FilterGeocercas(geocercas);

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = GetLinea(cmd.BaseId);

            reportStatus.RowCount = results.Count;
            if (results.Count < 1) return null;

            return GeofenceEventsReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);
        }

        public Stream GenerateMobilesTimeReport(MobilesTimeReportCommand cmd, IReportStatus reportStatus)
        {
            if (cmd.CustomerId == 0) return null;

            var results = ReportFactory.MobilesTimeDAO.GetMobilesTime(cmd.InitialDate, cmd.FinalDate, cmd.VehiclesId);

            reportStatus.RowCount = results.Count;
            if (results.Count < 1) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = GetLinea(cmd.BaseId);
            
            return MobilesTimeReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);
        }

        public Stream GenerateDocumentExpirationReport(DocumentsExpirationReportCommand cmd, IReportStatus reportStatus)
        {
            if (cmd.CustomerId == 0) return null;

            var results = DaoFactory.DocumentoDAO.FindByTipo(cmd.Documents.ToArray(), new List<int> { cmd.CustomerId }, new List<int> { -1 });

            reportStatus.RowCount = results.Count;
            if (results.Count < 1) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = GetLinea(cmd.BaseId);

            return DocumentsExpirationReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);
        }

        public Stream GenerateOdometersReport(OdometersReportCommand cmd, IReportStatus reportStatus)
        {
            if (cmd.CustomerId == 0) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);

            var baseName = GetLinea(cmd.BaseId);

            var vehiculos = (from v in cmd.VehiclesId
                             let ve = DaoFactory.CocheDAO.FindById(v)
                             where ve.Dispositivo != null
                             select v).ToList();

            var results = (IEnumerable<OdometroStatus>) ReportFactory.OdometroStatusDAO.FindByVehiculosAndOdometros(cmd.VehiclesId, cmd.Odometers, false);
            var ret = (from OdometroStatus m in results select new OdometroStatusVo(m)).ToList();
            if (cmd.Odometers.Contains(-10))
            {
                var coches = DaoFactory.CocheDAO.GetList(new[] { cmd.CustomerId }, new[] { cmd.BaseId})
                                                .Where(c => vehiculos.Contains(c.Id));

                ret.AddRange(coches.Select(coche => new OdometroStatusVo(coche)));
            }

            reportStatus.RowCount = ret.Count;

            return reportStatus.RowCount>0 ? OdometersReportGenerator.GenerateReport(ret, customer, cmd.InitialDate.ToDataBaseDateTime(), baseName) : null;
        }

        public Stream GenerateTransfersPerTripReport(TransfersPerTripReportCommand command, IReportStatus statusReport)
        {
            if (command.CustomerId == 0) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(command.CustomerId);
            var baseName = GetLinea(command.BaseId);

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

            return statusReport.RowCount>0 ? TransfersPerTripReportGenerator.GenerateReport(results, customer, command.InitialDate.ToLocalTime(), command.FinalDate.ToLocalTime(), baseName) : null;
        }

        public Stream GenerateDeliverStatusReport(DeliverStatusReportCommand cmd, IReportStatus reportStatus)
        {
            if (cmd.CustomerId == 0) return null;
            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = GetLinea(cmd.BaseId);
            
            var ruta = 0;
            var confirmation = true;
            var orden = true;
            var empresa = cmd.CustomerId;
            var linea = cmd.BaseId;
            var selectedVehicles = cmd.VehiclesId;
            var puntoEntrega = 0;
            var estadosEntrega = EntregaDistribucion.Estados.TodosEstados;
            var transportista = new List<int> { -1 };

            var desde = cmd.InitialDate.ToDataBaseDateTime();
            var hasta = cmd.FinalDate.ToDataBaseDateTime();

            var results = DeliverStatusRepor(empresa, linea, selectedVehicles, puntoEntrega,
                estadosEntrega, transportista, desde, hasta, ruta, confirmation, orden);
          
            reportStatus.RowCount = results.Count;
            //if (cmd.ReportId != 0)
            //    reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            return reportStatus.RowCount > 0
                ? DeliverStatusReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(),
                    cmd.FinalDate.ToLocalTime(), baseName)
                : null;
        }

        public Stream GenerateSummaryRoutesReport(SummaryRoutesReportCommand cmd, IReportStatus reportStatus)
        {
            //if (cmd.ReportId != 0)
            //    reportStatus.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            if (cmd.CustomerId == 0) return null;

            var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);
            var baseName = GetLinea(cmd.BaseId);

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
            if (reportStatus.RowCount < 1) return null;

            return SummaryRoutesReportGenerator.GenerateReport(results, customer, cmd.InitialDate.ToLocalTime(), cmd.FinalDate.ToLocalTime(), baseName);
        }
   
        public string GenerateSummarizedDriversInfractionReport(DriversInfractionsReportCommand cmd, IReportStatus status)
        {
            if (cmd.CustomerId == 0) return null;
            
            var desde = cmd.InitialDate.ToDataBaseDateTime();
            var hasta = cmd.FinalDate.ToDataBaseDateTime();
            var empresas = new[] { cmd.CustomerId };
            var lineas = new[] { cmd.BaseId };
            var transportadores = new int[] { };

            var resultsDt = ReportFactory.InfractionDetailDAO.GetInfractionsSummary(empresas, lineas, transportadores, 
                cmd.DriversId, desde, hasta);

            //if (cmd.ReportId != 0)
            //    status.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);
            
            status.RowCount = resultsDt.Rows.Count;

            return status.RowCount > 0 ? ConvertDtDriverInfractionsToString(resultsDt) : null;
        }

        public string GenerateSummarizedDocumentExpirationReport(DocumentsExpirationReportCommand cmd, IReportStatus status)
        {
            if (cmd.CustomerId == 0) return null;

            var hasta = cmd.FinalDate.ToDataBaseDateTime().AddDays(7);
            //var empresas = new[] { cmd.CustomerId };

            //if (cmd.ReportId != 0)
            //    status.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            //var customer = DaoFactory.EmpresaDAO.FindById(cmd.CustomerId);            

            var results = DaoFactory.DocumentoDAO.GetDocumentExpirationSummary(cmd.Documents.ToArray(), new List<int> { cmd.CustomerId }, new List<int> { -1 }, hasta);

            return results != null  ? ConvertDtDocumentExpirationToString(results) : null;
        }

        public string GenerateSummarizedOdometersReport(OdometersReportCommand cmd, IReportStatus status)
        {
            if (cmd.CustomerId == 0) return null;

            //var hasta = cmd.FinalDate.ToDataBaseDateTime().AddDays(7);
            //var empresas = new[] { cmd.CustomerId };

            //if (cmd.ReportId != 0)
            //    status.ReportProg = DaoFactory.ProgramacionReporteDAO.FindById(cmd.ReportId);

            if (cmd.CustomerId == 0) return null;

            var results = ReportFactory.OdometroStatusDAO.GetOdometersSummary(cmd.VehiclesId, cmd.Odometers);

            return results != null ? ConvertDtOdometersToString(results) : null;
        }

        #endregion

        #region Sending and Logging

        public void SendEmptyReport(IReportCommand command, string report, bool isError)
        {
            var subject = report;

            string body;
            if (isError)
                body = "Se ha producido un error al generar el reporte solicitado, por favor pongase en contacto con Logictracker";
            else
                body = string.Format("Usted ha solicitado un {0} a través de la plataforma Logictracker, pero esta consulta no arrojo resultados, modifique los filtros y vuelva a intentarlo.", report);

            body += "\n\n Este mensaje se ha generado automaticamente, No responda este correo.";

            var emailList = ValidateAddress(command.Email);

            Notifier.SmtpMail(MailFrom, emailList, subject, body, null, null, SmtpPort, SmtpAddress, Passwd, false);
        }
        public void SendHtmlReport(string reportString, string email, string report)
        {
            var subject = report + " Logictracker";
            var body = reportString;

            var emailList = ValidateAddress(email);

            Notifier.SmtpMail(MailFrom, emailList, subject, body, null, null, SmtpPort, SmtpAddress, Passwd, true);
        }
        public void SendReport(Stream reportStream, IReportCommand command, string reportName)
        {
            var subject = reportName + " Logictracker";
            var body =
                string.Format("Usted ha solicitado un {0} a través de la plataforma Logictracker. Se ha adjuntado un archivo de Excel.", reportName);
            body += "\n\n Este mensaje se ha generado automaticamente, No responda este correo.";

            var filename = reportName + ".xls";
            //if (reportName != null)
            //    filename = reportName.Trim() + " " + command.InitialDate.ToString("yyyy-MM-dd") + " a " + command.FinalDate.ToString("yyyy-MM-dd") + ".xls";

            var emailList = ValidateAddress(command.Email);

            Notifier.SmtpMail(MailFrom, emailList, subject, body, reportStream, filename, SmtpPort, SmtpAddress, Passwd, false);
        }
        public void LogReportExecution(int reportId, IReportStatus reportStatus)
        {
            try
            {
                var report = DaoFactory.ProgramacionReporteDAO.FindById(reportId);

                var log = new LogProgramacionReporte
                {
                    Inicio = reportStatus.StartReport,
                    Fin = DateTime.Now,
                    Filas = reportStatus.RowCount,
                    Error = reportStatus.Error,
                    ProgramacionReporte = report
                };

                report.ReportLogs.Add(log);

                DaoFactory.ProgramacionReporteDAO.SaveOrUpdate(report);
            }
            catch (Exception ex)
            {
                Logger.WarnFormat("No se pudo guardar la informacion de log del reporte {0}  ", ex.Message);
            }

        }
        public void NotifyError(IReportCommand command, string errorMessage)
        {
            try
            {
                SendEmptyReport(command, "Reporte Logictracker", true);
                var emailList = ValidateAddress(SupportMail);
                Notifier.SmtpMail(MailFrom, emailList, "Error en Reporte " + command.ReportName, errorMessage, null, null, SmtpPort, SmtpAddress, Passwd, false);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("No se pudo notificar a todos los correos {0} ", ex.Message);
            }
        }
        public void SendReport(string reportExecution, string reporteDeEjecucion)
        {
            var subject = "Reporte de ejecucion de Reportes Programados";
            var body = reportExecution;

            var emailList = ValidateAddress(SupportMail);

            Notifier.SmtpMail(MailFrom, emailList, subject, body, null, null, SmtpPort, SmtpAddress, Passwd, false);

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

        #endregion

        #region html templates
        private string ConvertDtDriverInfractionsToString(DataTable dtInfractions)
        {
            var report = new StringBuilder(@"
                <table style='border: solid 1px #3A81B1; border-spacing: 0px; width: 90%; margin: auto;'>
                <tr><td colspan='5' style='background-color:#3A81B1;'><img src='http://web.logictracker.com/App_Themes/Marinero/img/logo-logic-azul.png' /></td>
                </tr><tr style='background-color:#e7e7e7;'>
                <td style='padding: 10px;'><b>Conductor</b></td>
                <td style='padding: 10px;'><b>Vehiculo</b></td>
                <td style='padding: 10px;'><b>Graves</b></td>
                <td style='padding: 10px;'><b>Medias</b></td>
                <td style='padding: 10px;'><b>Leves</b></td>
                </tr>");

            var index = 0;
            foreach (DataRow infractionRow in dtInfractions.Rows)
            {
                report.AppendFormat(index % 2 == 0 ? "<tr>" : "<tr style='background-color:#e7e7e7;'>");

                report.AppendFormat("<td>{0}</td>", infractionRow.ItemArray.GetValue(1));
                report.AppendFormat("<td>{0}</td>", infractionRow.ItemArray.GetValue(2));
                report.AppendFormat("<td>{0}</td>", infractionRow.ItemArray.GetValue(3));
                report.AppendFormat("<td>{0}</td>", infractionRow.ItemArray.GetValue(4));
                report.AppendFormat("<td>{0}</td>", infractionRow.ItemArray.GetValue(5));
                report.Append("</tr>");
                index++;
            }
            report.Append("</table>");
            return report.ToString();
        }
        private string ConvertDtDocumentExpirationToString(DataRow row)
        {
            var report = new StringBuilder(@"
                <table style='border: solid 1px #3A81B1; border-spacing: 0px; width: 90%; margin: auto;'>
                    <tr>
                        <td colspan='5' style='background-color:#3A81B1;'>
                            <img src='http://web.logictracker.com/App_Themes/Marinero/img/logo-logic-azul.png' />
                        </td>
                    </tr>
                    <tr style='background-color:#e7e7e7;'>
                        <td style='padding: 10px;'>
                            <b>1er Aviso:</b>
                        </td>
                        <td style='padding: 10px;'>" +
                            row.ItemArray.GetValue(0) +
                        @"</td>
                    </tr>
                    <tr style='background-color:#e7e7e7;'>
                        <td style='padding: 10px;'>
                            <b>2do Aviso:</b>
                        </td>
                        <td style='padding: 10px;'>" +
                            row.ItemArray.GetValue(1) +
                        @"</td>
                    </tr>
                    <tr style='background-color:#e7e7e7;'>
                        <td style='padding: 10px;'>
                            <b>Vencidos:</b>
                        </td>
                        <td style='padding: 10px;'>" +
                            row.ItemArray.GetValue(2) + 
                        @"</td>
                    </tr>
                    <tr style='background-color:#e7e7e7;'>
                        <td style='padding: 10px;'>
                            <b>A vencer:</b>
                        </td>
                        <td style='padding: 10px;'>" +
                            row.ItemArray.GetValue(3) + 
                        @"</td>
                    </tr>
                </table>");

            return report.ToString();
        } 
        private string ConvertDtOdometersToString(DataRow row)
        {
            var report = new StringBuilder(@"
                <table style='border: solid 1px #3A81B1; border-spacing: 0px; width: 90%; margin: auto;'>
                    <tr>
                        <td colspan='5' style='background-color:#3A81B1;'>
                            <img src='http://web.logictracker.com/App_Themes/Marinero/img/logo-logic-azul.png' />
                        </td>
                    </tr>
                    <tr style='background-color:#e7e7e7;'>
                        <td style='padding: 10px;'>
                            <b>1er Aviso:</b>
                        </td>
                        <td style='padding: 10px;'>" +
                            row.ItemArray.GetValue(0) +
                        @"</td>
                    </tr>
                    <tr style='background-color:#e7e7e7;'>
                        <td style='padding: 10px;'>
                            <b>2do Aviso:</b>
                        </td>
                        <td style='padding: 10px;'>" +
                            row.ItemArray.GetValue(1) +
                        @"</td>
                    </tr>
                    <tr style='background-color:#e7e7e7;'>
                        <td style='padding: 10px;'>
                            <b>Vencidos:</b>
                        </td>
                        <td style='padding: 10px;'>" +
                            row.ItemArray.GetValue(2) +
                        @"</td>
                    </tr>
                </table>");

            return report.ToString();
        }
        private string ConvertReportToHtml(VehicleVerifierCommand cmd, int activos, int inactivos)
        {
            var report = new StringBuilder(@"
                <table style='border: solid 1px #3A81B1; border-spacing: 0px; width: 90%; margin: auto;'>
                    <tr>
                        <td colspan='5' style='background-color:#3A81B1;'>
                            <img src='http://web.logictracker.com/App_Themes/Marinero/img/logo-logic-azul.png' />
                        </td>
                    </tr>
                    <tr style='background-color:#e7e7e7;'>
                        <td style='padding: 10px;'>
                            <b>Activos</b>
                        </td>
                        <td style='padding: 10px;'>");
            report.Append(activos);
            report.Append(@"</td>
                    </tr>
                    <tr style='background-color:#e7e7e7;'>
                        <td style='padding: 10px;'>
                            <b>Inactivos:</b>
                        </td>
                        <td style='padding: 10px;'>");
            report.Append(inactivos);
            report.Append(@"</td>
                    </tr></table>");

            return report.ToString();
        }

        #endregion 

        #region reports
        public List<ReporteDistribucionVo> DeliverStatusRepor(int empresa, int linea, List<int> selectedVehicles, int puntoEntrega, List<int> estadosEntrega, List<int> transportista, DateTime desde, DateTime hasta, int ruta, bool confirmation, bool verOrden)
        {
            var inicio = DateTime.UtcNow;

            try
            {
                var results = new List<ReporteDistribucionVo>();

                if (ruta > 0)
                {
                    var dms = DaoFactory.DatamartDistribucionDAO.GetRecords(ruta);
                    foreach (var dm in dms)
                    {
                        results.Add(new ReporteDistribucionVo(dm, confirmation));
                    }

                    return results;
                }

                var rutas = DaoFactory.ViajeDistribucionDAO.GetList(new[] { empresa }, new[] { linea }, new[] { -1 }, new[] { -1 }, new[] { -1 }, new[] { -1 }, selectedVehicles, desde, hasta);
                if (rutas.Any())
                {
                    foreach (var route in rutas)
                    {
                        var dms = DaoFactory.DatamartDistribucionDAO.GetRecords(route.Id);
                        foreach (var dm in dms)
                        {
                            results.Add(new ReporteDistribucionVo(dm, confirmation));
                        }

                        return results;
                    }
                }

                var sql = DaoFactory.DatamartDistribucionDAO.GetReporteDistribucion(empresa, 
                                                                                       linea,
                                                                                       selectedVehicles,
                                                                                       puntoEntrega,
                                                                                       estadosEntrega,
                                                                                       desde,
                                                                                       hasta);

                sql.SetResultTransformer(Transformers.AliasToBean(typeof(ReporteDistribucionVo)));
                var report = sql.List<ReporteDistribucionVo>();
                results = report.Select(r => new ReporteDistribucionVo(r)).ToList();

                if (hasta > DateTime.Today.ToDataBaseDateTime())
                {
                    var viajesDeHoy = DaoFactory.ViajeDistribucionDAO.GetList( new[] {empresa},
                                                                               new[] {linea},
                                                                              transportista,
                                                                              new[] { -1 }, // DEPARTAMENTOS
                                                                              new[] { -1 }, // CENTROS DE COSTO
                                                                              new[] { -1 }, // SUB CENTROS DE COSTO
                                                                              selectedVehicles,
                                                                              new[] { -1 }, // EMPLEADOS
                                                                              new[] { -1 }, // ESTADOS
                                                                              DateTime.Today.ToDataBaseDateTime(),
                                                                              hasta)
                                                                     .Where(e => e.Id == ruta || ruta == 0);

                    foreach (var viaje in viajesDeHoy)
                    {
                        EntregaDistribucion anterior = null;

                        var estados = estadosEntrega;
                        var detalles = viaje.Detalles;

                        if (verOrden)
                            detalles = viaje.GetEntregasPorOrdenManual();
                        else if (viaje.Tipo == ViajeDistribucion.Tipos.Desordenado)
                            detalles = viaje.GetEntregasPorOrdenReal();

                        detalles = detalles.Where(e => puntoEntrega == 0 ||
                                                       (e.PuntoEntrega != null && e.PuntoEntrega.Id == puntoEntrega))
                                           .Where(e => estados.Contains(e.Estado))
                                           .ToList();

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

                            results.Add(new ReporteDistribucionVo(entrega, anterior, orden, kms, confirmation));
                            orden++;
                            if (!entrega.Estado.Equals(EntregaDistribucion.Estados.Cancelado)
                             && !entrega.Estado.Equals(EntregaDistribucion.Estados.NoCompletado)
                             && !entrega.Estado.Equals(EntregaDistribucion.Estados.SinVisitar))
                                anterior = entrega;
                        }
                    }
                }

                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

                STrace.Trace("Estado de Entregas", String.Format("Duración de la consulta: {0} segundos", duracion));
                return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Estado de Entregas", e);
                throw;
            }
        }

        public List<VehicleInfractionDetailVo> VehicleInfractionsReport(List<int> selectedVehicles,DateTime desde, DateTime hasta, bool verEsquinas)
        {
            desde = desde.ToDataBaseDateTime();
            hasta = hasta.ToDataBaseDateTime();

            var inicio = DateTime.UtcNow;
            try
            {
                var results = ReportFactory.InfractionDetailDAO.GetInfractionsDetailsByVehicles(selectedVehicles, desde, hasta)
                                                               .Select(o => new VehicleInfractionDetailVo(o) { HideCornerNearest = !verEsquinas})
                                                               .ToList();
                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

                STrace.Trace("Detalle de Infracciones por Vehículo", String.Format("Duración de la consulta: {0} segundos", duracion));
                return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Detalle de Infracciones por Vehículo", e, String.Format("Reporte: Detalle de Infracciones por Vehículo. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));
                throw;
            }
        }
        #endregion
    }
}
