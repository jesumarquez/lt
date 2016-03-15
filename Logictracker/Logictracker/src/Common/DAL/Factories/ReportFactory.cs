#region Usings

using System;
using System.Collections.Generic;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.ReportObjects;
using Logictracker.DAL.DAO.ReportObjects.ControlDeCombustible;
using Logictracker.DAL.DAO.ReportObjects.RankingDeOperadores;

#endregion

namespace Logictracker.DAL.Factories
{
    public class ReportFactory : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report factory using the specified bussines object access class.
        /// </summary>
        /// <param name="daoFactory"></param>
        public ReportFactory(DAOFactory daoFactory) { _daoFactory = daoFactory; }

        #endregion

        #region Private Properties

        /// <summary>
        /// Bussiness object data access class.
        /// </summary>
        private readonly DAOFactory _daoFactory;

        /// <summary>
        /// Dictionary for holding current instances of report access classes.
        /// </summary>
        private Dictionary<Type, ReportDAO> _daos = new Dictionary<Type,ReportDAO>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Route event data access.
        /// </summary>
        public RouteEventDAO RouteEventDAO { get { return GetDao<RouteEventDAO>(); } }

        /// <summary>
        /// Operator Stadistics data access.
        /// </summary>
        public OperatorStadisticsDAO OperatorStadisticsDAO { get { return GetDao<OperatorStadisticsDAO>(); } }

        /// <summary>
        /// Mobile events data access.
        /// </summary>
        public MobileEventDAO MobileEventDAO { get { return GetDao<MobileEventDAO>(); } }
        
        /// <summary>
        /// Mobile kilometers data access.
        /// </summary>
        public MonthlyKilometersDAO MonthlyKilometersDAO { get { return GetDao<MonthlyKilometersDAO>(); } }

        /// <summary>
        /// Mobile Utilization DAO
        /// </summary>
        public MobileUtilizationDAO MobileUtilizationDAO { get { return GetDao<MobileUtilizationDAO>(); } }

        /// <summary>
        /// Vehicle Utilization DAO
        /// </summary>
        public VehicleUtilizationDAO VehicleUtilizationDAO { get { return GetDao<VehicleUtilizationDAO>(); } }

        /// <summary>
        /// Mobile times data access.
        /// </summary>
        public MonthlyTimesDAO MonthlyTimesDAO { get { return GetDao<MonthlyTimesDAO>(); } }

        /// <summary>
        /// Mobile activity data access.
        /// </summary>
        public MobileActivityDAO MobileActivityDAO { get { return GetDao<MobileActivityDAO>(); } }

        /// <summary>
        /// Operator activity data access.
        /// </summary>
        public OperatorActivityDAO OperatorActivityDAO { get { return GetDao<OperatorActivityDAO>(); } }

        /// <summary>
        /// Infraction details data access.
        /// </summary>
        public InfractionDetailDAO InfractionDetailDAO { get { return GetDao<InfractionDetailDAO>(); } }

        /// <summary>
        /// Operator ranking data access.
        /// </summary>
        public OperatorRankingDAO OperatorRankingDAO { get { return GetDao<OperatorRankingDAO>(); } }

        /// <summary>
        /// Mobile position data access.
        /// </summary>
        public MobilePositionDAO MobilePositionDAO { get { return GetDao<MobilePositionDAO>(); } }

        /// <summary>
        /// Ticket report data access.
        /// </summary>
        public TicketReportDAO TicketReportDAO { get { return GetDao<TicketReportDAO>(); } }

        /// <summary>
        /// Geocercas events data access.
        /// </summary>
        public MobileGeocercasDAO MobileGeocercaDAO { get { return GetDao<MobileGeocercasDAO>(); } }

        /// <summary>
        /// Transport Activity data access.
        /// </summary>
        public TransportActivityDAO TransportActivityDAO { get { return GetDao<TransportActivityDAO>(); } }

        /// <summary>
        /// Mobiles points of interest data access.
        /// </summary>
        public MobilePoiDAO MobilePoiDAO { get { return GetDao<MobilePoiDAO>(); } }

        /// <summary>
        /// Mobiles points of interest data access.
        /// </summary>
        public MobilePoiHistoricDAO MobilePoiHistoricDAO { get { return GetDao<MobilePoiHistoricDAO>(); } }

        /// <summary>
        /// Mobile Messages in a date range.
        /// </summary>
        public MobileMessageDAO MobileMessageDAO { get { return GetDao<MobileMessageDAO>(); } }

        /// <summary>
        /// Mobile dialy max speeds.
        /// </summary>
        public MaxSpeedsDAO MaxSpeedDAO { get { return GetDao<MaxSpeedsDAO>(); } }

        /// <summary>
        /// Mobile drivers data access.
        /// </summary>
        public MobileDriversDAO MobileDriversDAO { get { return GetDao<MobileDriversDAO>(); } }

        /// <summary>
        /// Operator mobiles data access.
        /// </summary>
        public OperatorMobilesDAO OperatorMobilesDAO { get { return GetDao<OperatorMobilesDAO>(); } }

        /// <summary>
        /// Mobile Drivers By Day data access.
        /// </summary>
        public MobileDriversByDayDAO MobileDriversByDayDAO { get { return GetDao<MobileDriversByDayDAO>(); } }

        /// <summary>
        /// Mobile Drivers By Day data access.
        /// </summary>
        public OperatorMobilesByDayDAO OperatorMobilesByDayDAO { get { return GetDao<OperatorMobilesByDayDAO>(); } }

        /// <summary>
        /// Mobile stadistics data access class.
        /// </summary>
        public MobileStadisticsDAO MobileStadisticsDAO { get { return GetDao<MobileStadisticsDAO>(); } }

        /// <summary>
        /// Mobile routes data access class.
        /// </summary>
        public MobileRoutesDAO MobileRoutesDAO { get { return GetDao<MobileRoutesDAO>(); } }

        /// <summary>
        /// Despachos data access class.
        /// </summary>
        public CentroDeCostosDespachosDAO CentroDeCostosDespachosDAO { get { return GetDao<CentroDeCostosDespachosDAO>(); } }

        /// <summary>
        /// Remitos data access class.
        /// </summary>
        public CentroDeCargaRemitosDAO CentroDeCargaRemitosDAO { get { return GetDao<CentroDeCargaRemitosDAO>(); } }

        /// <summary>
        /// Stock Consistence data access class.
        /// </summary>
        public ConsistenciaStockDAO ConsistenciaStockDAO { get { return GetDao<ConsistenciaStockDAO>(); } }

        /// <summary>
        /// Mobile maintenance data access class.
        /// </summary>
        public MobileMaintenanceDAO MobileMaintenanceDAO { get { return GetDao<MobileMaintenanceDAO>(); } }

        /// <summary>
        /// Mobiles Kilometers data access class.
        /// </summary>
        public MobilesKilometersDAO MobilesKilometersDAO { get { return GetDao<MobilesKilometersDAO>(); } }

        /// <summary>
        /// Mobiles time data access class.
        /// </summary>
        public MobilesTimeDAO MobilesTimeDAO { get { return GetDao<MobilesTimeDAO>(); } }

        /// <summary>
        /// Acumulated Worked Hours for a employee.
        /// </summary>
        public AcumulatedHoursDAO AcumulatedHoursDAO { get { return GetDao<AcumulatedHoursDAO>(); } }

        /// <summary>
        /// Mobiles Extra Hours data access class.
        /// </summary>
        public MobileExtraHoursDAO MobileExtraHoursDAO { get { return GetDao<MobileExtraHoursDAO>(); } }

        /// <summary>
        /// Consumos por motor Data access class.
        /// </summary>
        public ConsumosPorMotorDAO ConsumosPorMotorDAO { get { return GetDao<ConsumosPorMotorDAO>(); } }

        /// <summary>
        /// Consumos Diarios data access class.
        /// </summary>
        public ConsumoDiarioDAO ConsumoDiarioDAO { get { return GetDao<ConsumoDiarioDAO>(); } }

        /// <summary>
        /// Consumo Caudal Motor data access class.
        /// </summary>
        public ConsumoCaudalMotorDAO ConsumoCaudalMotorDAO { get { return GetDao<ConsumoCaudalMotorDAO>(); } }

        /// <summary>
        /// CombustibleEvents data access class.
        /// </summary>
        public CombustibleEventsDAO CombustibleEventsDAO { get { return GetDao<CombustibleEventsDAO>(); } }

        /// <summary>
        /// CombustibleEvents data access class.
        /// </summary>
        public OdometroStatusDAO OdometroStatusDAO { get { return GetDao<OdometroStatusDAO>(); } }

        /// <summary>
        /// IngresosPorTanque data access class.
        /// </summary>
        public IngresosPorTanqueDAO IngresosPorTanqueDAO { get { return GetDao<IngresosPorTanqueDAO>(); } }

        /// <summary>
        /// Mobile activity data access.
        /// </summary>
        public MarchaPorMotorDAO MarchaPorMotorDAO { get { return GetDao<MarchaPorMotorDAO>(); } }

        /// <summary>
        /// NivelesTanqueDAO data access class.
        /// </summary>
        public NivelesTanqueDAO NivelesTanqueDAO { get { return GetDao<NivelesTanqueDAO>(); } }

        /// <summary>
        /// ConsistenciaStockPozoDAO data access class.
        /// </summary>
        public ConsistenciaStockPozoDAO ConsistenciaStockPozoDAO { get { return GetDao<ConsistenciaStockPozoDAO>(); } }

        /// <summary>
        /// MobileStoppedEvent data access class.
        /// </summary>
        public MobileStoppedEventDAO MobileStoppedEventDAO { get { return GetDao<MobileStoppedEventDAO>(); } }

        /// <summary>
        /// ConsistenciaStockGraphDAO data access class.
        /// </summary>
        public ConsistenciaStockGraphDAO ConsistenciaStockGraphDAO { get { return GetDao<ConsistenciaStockGraphDAO>(); } }

        /// <summary>
        /// ConsistenciaStockGraphDAO data access class.
        /// </summary>
        public DetentionTimesDAO DetentionTimesDAO { get { return GetDao<DetentionTimesDAO>(); } }

        /// <summary>
        /// Infraction duration associated points data access.
        /// </summary>
        public PuntajeExcesoTiempoDAO PuntajeExcesoTiempoDAO { get { return GetDao<PuntajeExcesoTiempoDAO>(); } }

        /// <summary>
        /// Speed ponderation data access.
        /// </summary>
        public PuntajeExcesoVelocidadDAO PuntajeExcesoVelocidadDAO { get { return GetDao<PuntajeExcesoVelocidadDAO>(); } }

        /// <summary>
        /// Stopped Hours DAO.
        /// </summary>
        public StoppedHoursDAO StoppedHoursDAO { get { return GetDao<StoppedHoursDAO>(); } }

        /// <summary>
        /// Horas trabajadas por los empleados para el control de acceso
        /// </summary>
        public WorkedHoursDAO WorkedHoursDAO { get { return GetDao<WorkedHoursDAO>(); } }

        /// <summary>
        /// Idle Time in Plantas DAO.
        /// </summary>
        public IdleTimesDAO IdleTimesDAO { get { return GetDao<IdleTimesDAO>(); } }

        public VerificadorEmpleadoDAO VerificadorEmpleadoDAO { get { return GetDao<VerificadorEmpleadoDAO>(); } }

        public RankingVehiculosDAO RankingVehiculosDAO { get { return GetDao<RankingVehiculosDAO>(); } }

        public RankingTransportistasDAO RankingTransportistasDAO { get { return GetDao<RankingTransportistasDAO>(); } }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a instance of the report dao associated to the givenn type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T GetDao<T>() where T : ReportDAO
        {
            var type = typeof (T);

            if (!_daos.ContainsKey(type))
            {
                var constructor = type.GetConstructor(new[] {typeof (DAOFactory)});

                var dao = constructor.Invoke(new[] {_daoFactory}) as ReportDAO;

                _daos.Add(type, dao);
            }

            return (T) _daos[type];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Dispose all associated resources.
        /// </summary>
        public void Dispose()
        {
            _daos.Clear();

            _daos = null;
        }

        #endregion
    }
}