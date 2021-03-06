using System;
using System.Data;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BaseClasses.Interfaces;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.DAL.DAO.BusinessObjects.Auditoria;
using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico;
using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.TimeTracking;
using Logictracker.DAL.DAO.BusinessObjects.ControlAcceso;
using Logictracker.DAL.DAO.BusinessObjects.ControlDeCombustible;
using Logictracker.DAL.DAO.BusinessObjects.Dispositivos;
using Logictracker.DAL.DAO.BusinessObjects.Documentos;
using Logictracker.DAL.DAO.BusinessObjects.Entidades;
using Logictracker.DAL.DAO.BusinessObjects.Mantenimiento;
using Logictracker.DAL.DAO.BusinessObjects.Messages;
using Logictracker.DAL.DAO.BusinessObjects.Ordenes;
using Logictracker.DAL.DAO.BusinessObjects.Organizacion;
using Logictracker.DAL.DAO.BusinessObjects.Positions;
using Logictracker.DAL.DAO.BusinessObjects.Postal;
using Logictracker.DAL.DAO.BusinessObjects.Rechazos;
using Logictracker.DAL.DAO.BusinessObjects.ReferenciasGeograficas;
using Logictracker.DAL.DAO.BusinessObjects.Support;
using Logictracker.DAL.DAO.BusinessObjects.Sync;
using Logictracker.DAL.DAO.BusinessObjects.Tickets;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.DAL.DAO.ReportObjects;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;

namespace Logictracker.DAL.Factories
{
    public interface IDAOFactory
    {
        /// <summary>
        /// NHibernate data access session.
        /// </summary>
        ISession Session { get; }

        TipoDocumentoDAO TipoDocumentoDAO { get; }
        DocumentoDAO DocumentoDAO { get; }
        TipoCocheDAO TipoCocheDAO { get; }
        CocheDAO CocheDAO { get; }
        MarcaDAO MarcaDAO { get; }
        ModeloDAO ModeloDAO { get; }
        OdometroDAO OdometroDAO { get; }
        MovOdometroVehiculoDAO MovOdometroVehiculoDAO { get; }
        CocheOperacionDAO CocheOperacionDAO { get; }
        DispositivoDAO DispositivoDAO { get; }
        ConfiguracionDispositivoDAO ConfiguracionDispositivoDAO { get; }
        FirmwareDAO FirmwareDAO { get; }
        TipoDispositivoDAO TipoDispositivoDAO { get; }
        TipoParametroDispositivoDAO TipoParametroDispositivoDAO { get; }
        DetalleDispositivoDAO DetalleDispositivoDAO { get; }
        PlanDAO PlanDAO { get; }
        LineaTelefonicaDAO LineaTelefonicaDAO { get; }
        TipoEmpleadoDAO TipoEmpleadoDAO { get; }
        EmpleadoDAO EmpleadoDAO { get; }
        TipoServicioDAO TipoServicioDAO { get; }
        MotivoDAO MotivoDAO { get; }
        DistribuidorDAO DistribuidorDAO { get; }
        UsuarioDAO UsuarioDAO { get; }
        SistemaDAO SistemaDAO { get; }
        FuncionDAO FuncionDAO { get; }
        PerfilDAO PerfilDAO { get; }
        TipoMensajeDAO TipoMensajeDAO { get; }
        MensajeDAO MensajeDAO { get; }
        AccionDAO AccionDAO { get; }
        LogMensajeDAO LogMensajeDAO { get; }
        LogMensajeAdminDAO LogMensajeAdminDAO { get; }
        AtencionEventoDAO AtencionEventoDAO { get; }
        LogUltimoLoginDAO LogUltimoLoginDAO { get; }
        InfraccionDAO InfraccionDAO { get; }
        EvenDistriDAO EvenDistriDAO { get; }
        MensajeIgnoradoDAO MensajeIgnoradoDAO { get; }
        MensajeTraducidoDAO MensajeTraducidoDAO { get; }
        OrderDAO OrderDAO { get; }
        EmpresaDAO EmpresaDAO { get; }
        LineaDAO LineaDAO { get; }
        TransportistaDAO TransportistaDAO { get; }
        TipoReferenciaGeograficaDAO TipoReferenciaGeograficaDAO { get; }
        ReferenciaGeograficaDAO ReferenciaGeograficaDAO { get; }
        TallerDAO TallerDAO { get; }
        ZonaDAO ZonaDAO { get; }
        TipoZonaAccesoDAO TipoZonaAccesoDAO { get; }
        ZonaAccesoDAO ZonaAccesoDAO { get; }
        TipoZonaDAO TipoZonaDAO { get; }
        TicketDAO TicketDAO { get; }
        DetalleTicketDAO DetalleTicketDAO { get; }
        CicloLogisticoDAO CicloLogisticoDAO { get; }
        ServicioDAO ServicioDAO { get; }
        DetalleCicloDAO DetalleCicloDAO { get; }
        TipoServicioCicloDAO TipoServicioCicloDAO { get; }
        BocaDeCargaDAO BocaDeCargaDAO { get; }
        PedidoDAO PedidoDAO { get; }
        PreasignacionViajeVehiculoDAO PreasignacionViajeVehiculoDAO { get; }
        ViajeDistribucionDAO ViajeDistribucionDAO { get; }
        EntregaDistribucionDAO EntregaDistribucionDAO { get; }
        EstadoDistribucionDAO EstadoDistribucionDAO { get; }
        RecorridoDistribucionDAO RecorridoDistribucionDAO { get; }
        RecorridoDAO RecorridoDAO { get; }
        DetalleRecorridoDAO DetalleRecorridoDAO { get; }
        LastVehicleEventDAO LastVehicleEventDAO { get; }
        TicketRechazoDAO TicketRechazoDAO { get; }
        SosTicketDAO SosTicketDAO { get; }
        ViajeProgramadoDAO ViajeProgramadoDAO { get; }
        EstadoLogisticoDAO EstadoLogisticoDAO { get; }
        TipoCicloLogisticoDAO TipoCicloLogisticoDAO { get; }
        MovimientoDAO MovimientoDAO { get; }
        TanqueDAO TanqueDAO { get; }
        VolumenHistoricoDAO VolumenHistoricoDAO { get; }
        TipoMovimientoDAO TipoMovimientoDAO { get; }
        CaudalimetroDAO CaudalimetroDAO { get; }
        MotivoConciliacionDAO MotivoConciliacionDAO { get; }
        VolumenHistoricoInvalidoDAO VolumenHistoricoInvalidoDAO { get; }
        OutQueueDAO OutQueueDAO { get; }
        OutStateDAO OutStateDAO { get; }
        TarjetaDAO TarjetaDAO { get; }
        CategoriaAccesoDAO CategoriaAccesoDAO { get; }
        SupportTicketDAO SupportTicketDAO { get; }
        CategoriaDAO CategoriaDAO { get; }
        SubCategoriaDAO SubCategoriaDAO { get; }
        NivelDAO NivelDAO { get; }
        EstadoDAO EstadoDAO { get; }
        EquipoDAO EquipoDAO { get; }
        ProductoDAO ProductoDAO { get; }
        EventoAccesoDAO EventoAccesoDAO { get; }
        IconoDAO IconoDAO { get; }
        SonidoDAO SonidoDAO { get; }
        LogPosicionDAO LogPosicionDAO { get; }
        LogPosicionHistoricaDAO LogPosicionHistoricaDAO { get; }
        ClienteDAO ClienteDAO { get; }
        PuertaAccesoDAO PuertaAccesoDAO { get; }
        FeriadoDAO FeriadoDAO { get; }
        PuntoEntregaDAO PuntoEntregaDAO { get; }
        RoutePositionsDAO RoutePositionsDAO { get; }
        LogPosicionDescartadaDAO LogPosicionDescartadaDAO { get; }
        PeriodoDAO PeriodoDAO { get; }
        CentroDeCostosDAO CentroDeCostosDAO { get; }
        TarifaTransportistaDAO TarifaTransportistaDAO { get; }
        ShiftDAO ShiftDAO { get; }
        LoginAuditDAO LoginAuditDAO { get; }
        EntityAuditDAO EntityAuditDAO { get; }
        LogMensajeDescartadoDAO LogMensajeDescartadoDAO { get; }
        DatamartDAO DatamartDAO { get; }
        DatamartTramoDAO DatamartTramoDAO { get; }
        DatamartDistribucionDAO DatamartDistribucionDAO { get; }
        DatamartViajeDAO DatamartViajeDAO { get; }
        PuntajeExcesoVelocidadDAO PuntajeExcesoVelocidadDAO { get; }
        PuntajeExcesoTiempoDAO PuntajeExcesoTiempoDAO { get; }
        EventoCombustibleDAO EventoCombustibleDAO { get; }
        RutaDAO RutaDAO { get; }
        AsegurableDAO AsegurableDAO { get; }
        ProgramacionReporteDAO ProgramacionReporteDAO { get; }
        LogProgramacionReporteDAO LogProgramacionReporteDAO { get; }
        ConsumoCabeceraDAO ConsumoCabeceraDAO { get; }
        ConsumoDetalleDAO ConsumoDetalleDAO { get; }
        InsumoDAO InsumoDAO { get; }
        TipoInsumoDAO TipoInsumoDAO { get; }
        DepositoDAO DepositoDAO { get; }
        StockDAO StockDAO { get; }
        StockVehicularDAO StockVehicularDAO { get; }
        DetalleStockVehicularDAO DetalleStockVehicularDAO { get; }
        ProveedorDAO ProveedorDAO { get; }
        ReportsCacheDAO ReportsCacheDAO { get; }
        TipoEntidadDAO TipoEntidadDAO { get; }
        TipoMedicionDAO TipoMedicionDAO { get; }
        EntidadDAO EntidadDAO { get; }
        SubEntidadDAO SubEntidadDAO { get; }
        SensorDAO SensorDAO { get; }
        MedicionDAO MedicionDAO { get; }
        PrecintoDAO PrecintoDAO { get; }
        DetalleDAO DetalleDAO { get; }
        LogEventoDAO LogEventoDAO { get; }
        UnidadMedidaDAO UnidadMedidaDAO { get; }
        TipoProveedorDAO TipoProveedorDAO { get; }
        DepartamentoDAO DepartamentoDAO { get; }
        EventoViajeDAO EventoViajeDAO { get; }
        SubCentroDeCostosDAO SubCentroDeCostosDAO { get; }
        TicketMantenimientoDAO TicketMantenimientoDAO { get; }
        LogicLinkFileDAO LogicLinkFileDAO { get; }
        DataMartsLogDAO DataMartsLogDAO { get; }
        AgendaVehicularDAO AgendaVehicularDAO { get; }
        InformeViajeRecargaDAO InformeViajeRecargaDAO { get; }

        /// <summary>
        /// Gets a generic DAO for the givenn type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        GenericDAO<T> GetGenericDAO<T>() where T : IAuditable;

        /// <summary>
        /// Creates anew database command using the current stateless session.
        /// </summary>
        /// <returns></returns>
        IDbCommand CreateCommand();

        /// <summary>
        /// Creates a new sql query using the prived sql text within the current stateless session.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        ISQLQuery CreateSqlQuery(String sql);

        /// <summary>
        /// Removes the specified object from te current session.
        /// </summary>
        /// <param name="obj"></param>
        void RemoveFromSession(Object obj);

        void SessionClear();
        void Dispose();
    }

    public class DAOFactory : IDisposable, IDAOFactory
    {
        #region Private Properties

        /// <summary>
        /// NHibernate data access session.
        /// </summary>
        public ISession Session {
            get { return SessionHelper.Current; }
        }
        
        #endregion

        #region Public Properties

        #region Documentos

        private TipoDocumentoDAO _tipoDocumentoDao;
        public TipoDocumentoDAO TipoDocumentoDAO { get { return _tipoDocumentoDao ?? (_tipoDocumentoDao = GetDao<TipoDocumentoDAO>()); } }

        private DocumentoDAO _documentoDao;
        public DocumentoDAO DocumentoDAO { get { return _documentoDao ?? (_documentoDao = GetDao<DocumentoDAO>()); } }

        #endregion

        #region Vehiculos

        private TipoCocheDAO _tipoCoche;
        public TipoCocheDAO TipoCocheDAO { get { return _tipoCoche ?? (_tipoCoche = GetDao<TipoCocheDAO>()); } }

        private CocheDAO _cocheDao;
        public CocheDAO CocheDAO { get { return _cocheDao ?? (_cocheDao = GetDao<CocheDAO>()); } }

        private MarcaDAO _marcaDao;
        public MarcaDAO MarcaDAO { get { return _marcaDao ?? (_marcaDao = GetDao<MarcaDAO>()); } }

        private ModeloDAO _modeloDao;
        public ModeloDAO ModeloDAO { get { return _modeloDao ?? (_modeloDao = GetDao<ModeloDAO>()); } }

        private OdometroDAO _odometroDao;
        public OdometroDAO OdometroDAO { get { return _odometroDao ?? (_odometroDao = GetDao<OdometroDAO>()); } }

        private MovOdometroVehiculoDAO _movOdometroVehiculoDao;
        public MovOdometroVehiculoDAO MovOdometroVehiculoDAO { get { return _movOdometroVehiculoDao ?? (_movOdometroVehiculoDao = GetDao<MovOdometroVehiculoDAO>()); } }

        private CocheOperacionDAO _cocheOperacionDao;
        public CocheOperacionDAO CocheOperacionDAO { get { return _cocheOperacionDao ?? (_cocheOperacionDao = GetDao<CocheOperacionDAO>()); } }

        #endregion

        #region Dispositivos

        private DispositivoDAO _dispositivoDao;

        public virtual DispositivoDAO DispositivoDAO
        {
            get { return _dispositivoDao ?? (_dispositivoDao = GetDao<DispositivoDAO>()); }
        }

        private ConfiguracionDispositivoDAO _configuracionDispositivoDao;
        public ConfiguracionDispositivoDAO ConfiguracionDispositivoDAO { get { return _configuracionDispositivoDao ?? (_configuracionDispositivoDao = GetDao<ConfiguracionDispositivoDAO>()); } }

        private FirmwareDAO _firmwareDao;
        public FirmwareDAO FirmwareDAO { get { return _firmwareDao ?? (_firmwareDao = GetDao<FirmwareDAO>()); } }

        private TipoDispositivoDAO _tipoDispositivoDao;
        public TipoDispositivoDAO TipoDispositivoDAO { get { return _tipoDispositivoDao ?? (_tipoDispositivoDao = GetDao<TipoDispositivoDAO>()); } }

        private TipoParametroDispositivoDAO _tipoParametroDispositivoDao;
        public TipoParametroDispositivoDAO TipoParametroDispositivoDAO { get { return _tipoParametroDispositivoDao ?? (_tipoParametroDispositivoDao = GetDao<TipoParametroDispositivoDAO>()); } }

        private DetalleDispositivoDAO _detalleDispositivoDao;
        public DetalleDispositivoDAO DetalleDispositivoDAO { get { return _detalleDispositivoDao ?? (_detalleDispositivoDao = GetDao<DetalleDispositivoDAO>()); } }

        private PlanDAO _planDao;
        public PlanDAO PlanDAO { get { return _planDao ?? (_planDao = GetDao<PlanDAO>()); } }

        private LineaTelefonicaDAO _lineaTelefonicaDao;
        public LineaTelefonicaDAO LineaTelefonicaDAO { get { return _lineaTelefonicaDao ?? (_lineaTelefonicaDao = GetDao<LineaTelefonicaDAO>()); } }

        #endregion

        #region Empleados

        private TipoEmpleadoDAO _tipoEmpleadoDao;
        public TipoEmpleadoDAO TipoEmpleadoDAO { get { return _tipoEmpleadoDao ?? (_tipoEmpleadoDao = GetDao<TipoEmpleadoDAO>()); } }

        private EmpleadoDAO _empleadoDao;
        public virtual EmpleadoDAO EmpleadoDAO { get { return _empleadoDao ?? (_empleadoDao = GetDao<EmpleadoDAO>()); } }

        

        #endregion

        #region La Postal

        private TipoServicioDAO _tipoServicio;
        public TipoServicioDAO TipoServicioDAO { get { return _tipoServicio ?? (_tipoServicio = GetDao<TipoServicioDAO>()); } }

        private MotivoDAO _motivoDao;
        public MotivoDAO MotivoDAO { get { return _motivoDao ?? (_motivoDao = GetDao<MotivoDAO>()); } }

        private DistribuidorDAO _distribuidorDao;
        public DistribuidorDAO DistribuidorDAO { get { return _distribuidorDao ?? (_distribuidorDao = GetDao<DistribuidorDAO>()); } }

        #endregion

        #region Seguridad

        private UsuarioDAO _usuarioDao;
        public UsuarioDAO UsuarioDAO { get { return _usuarioDao ?? (_usuarioDao = GetDao<UsuarioDAO>()); } }

        private SistemaDAO _sistemaDao;
        public SistemaDAO SistemaDAO { get { return _sistemaDao ?? (_sistemaDao = GetDao<SistemaDAO>()); } }

        private FuncionDAO _funcionDao;
        public FuncionDAO FuncionDAO { get { return _funcionDao ?? (_funcionDao = GetDao<FuncionDAO>()); } }

        private PerfilDAO _perfilDao;
        public PerfilDAO PerfilDAO { get { return _perfilDao ?? (_perfilDao = GetDao<PerfilDAO>()); } }

        #endregion

        #region Mensajes

        private TipoMensajeDAO _tipoMensajeDao;
        public TipoMensajeDAO TipoMensajeDAO { get { return _tipoMensajeDao ?? (_tipoMensajeDao = GetDao<TipoMensajeDAO>()); } }

        private MensajeDAO _mensajeDao;
        public virtual MensajeDAO MensajeDAO { get { return _mensajeDao ?? (_mensajeDao = GetDao<MensajeDAO>()); } }

        private AccionDAO _accionDao;
        public AccionDAO AccionDAO { get { return _accionDao ?? (_accionDao = GetDao<AccionDAO>()); } }

        private LogMensajeDAO _logMensajeDao;
        public LogMensajeDAO LogMensajeDAO { get { return _logMensajeDao ?? (_logMensajeDao = GetDao<LogMensajeDAO>()); } }

        private LogMensajeAdminDAO _logMensajeAdminDao;
        public LogMensajeAdminDAO LogMensajeAdminDAO { get { return _logMensajeAdminDao ?? (_logMensajeAdminDao = GetDao<LogMensajeAdminDAO>()); } }

        private AtencionEventoDAO _atencionEventoDAO;
        public AtencionEventoDAO AtencionEventoDAO { get { return _atencionEventoDAO ?? (_atencionEventoDAO = GetDao<AtencionEventoDAO>()); } }

        private LogUltimoLoginDAO _logUltimoLoginDao;
        public LogUltimoLoginDAO LogUltimoLoginDAO { get { return _logUltimoLoginDao ?? (_logUltimoLoginDao = GetDao<LogUltimoLoginDAO>()); } }

        private InfraccionDAO _infraccionDAO;
        public InfraccionDAO InfraccionDAO { get { return _infraccionDAO ?? (_infraccionDAO = GetDao<InfraccionDAO>()); } }

        private EvenDistriDAO _evenDistriDAO;
        public EvenDistriDAO EvenDistriDAO { get { return _evenDistriDAO ?? (_evenDistriDAO = GetDao<EvenDistriDAO>()); } }

        private MensajeIgnoradoDAO _mensajeIgnoradoDAO;
        public MensajeIgnoradoDAO MensajeIgnoradoDAO { get { return _mensajeIgnoradoDAO ?? (_mensajeIgnoradoDAO = GetDao<MensajeIgnoradoDAO>()); } }

        private MensajeTraducidoDAO _mensajeTraducidoDAO;
        public MensajeTraducidoDAO MensajeTraducidoDAO { get { return _mensajeTraducidoDAO ?? (_mensajeTraducidoDAO = GetDao<MensajeTraducidoDAO>()); } }

        #endregion

        #region Ordenes

        private OrderDAO _orderDao;
        public OrderDAO OrderDAO { get { return _orderDao ?? (_orderDao = GetDao<OrderDAO>()); } }

        #endregion

        #region Entidades

        private EmpresaDAO _empresa;
        public EmpresaDAO EmpresaDAO { get { return _empresa ?? (_empresa = GetDao<EmpresaDAO>()); } }

        private LineaDAO _lineaDao;
        public LineaDAO LineaDAO { get { return _lineaDao ?? (_lineaDao = GetDao<LineaDAO>()); } }

        private TransportistaDAO _transportistaDao;
        public TransportistaDAO TransportistaDAO { get { return _transportistaDao ?? (_transportistaDao = GetDao<TransportistaDAO>()); } }

        #endregion

        #region Referencias Geograficas

        private TipoReferenciaGeograficaDAO _tipoReferenciaGeograficaDao;
        public TipoReferenciaGeograficaDAO TipoReferenciaGeograficaDAO { get { return _tipoReferenciaGeograficaDao ?? (_tipoReferenciaGeograficaDao = GetDao<TipoReferenciaGeograficaDAO>()); } }

        private ReferenciaGeograficaDAO _referenciaGeograficaDao;
        public ReferenciaGeograficaDAO ReferenciaGeograficaDAO { get { return _referenciaGeograficaDao ?? (_referenciaGeograficaDao = GetDao<ReferenciaGeograficaDAO>()); } }

        private TallerDAO _tallerDao;
        public TallerDAO TallerDAO { get { return _tallerDao ?? (_tallerDao = GetDao<TallerDAO>()); } }

        private ZonaDAO _zonaDao;
        public ZonaDAO ZonaDAO { get { return _zonaDao ?? (_zonaDao = GetDao<ZonaDAO>()); } }

        private TipoZonaAccesoDAO _tipoZonaAccesoDao;
        public TipoZonaAccesoDAO TipoZonaAccesoDAO { get { return _tipoZonaAccesoDao ?? (_tipoZonaAccesoDao = GetDao<TipoZonaAccesoDAO>()); } }

        private ZonaAccesoDAO _zonaAccesoDao;
        public ZonaAccesoDAO ZonaAccesoDAO { get { return _zonaAccesoDao ?? (_zonaAccesoDao = GetDao<ZonaAccesoDAO>()); } }

        private TipoZonaDAO _tipoZonaDao;
        public TipoZonaDAO TipoZonaDAO { get { return _tipoZonaDao ?? (_tipoZonaDao = GetDao<TipoZonaDAO>()); } }

        #endregion

        #region Ciclo Logístico Hormigon

        private TicketDAO _ticketDao;
        public TicketDAO TicketDAO { get { return _ticketDao ?? (_ticketDao = GetDao<TicketDAO>()); } }

        private DetalleTicketDAO _detalleTicketDao;
        public DetalleTicketDAO DetalleTicketDAO { get { return _detalleTicketDao ?? (_detalleTicketDao = GetDao<DetalleTicketDAO>()); } }

        private CicloLogisticoDAO _cicloLogisticoDao;
        public CicloLogisticoDAO CicloLogisticoDAO { get { return _cicloLogisticoDao ?? (_cicloLogisticoDao = GetDao<CicloLogisticoDAO>()); } }

        private ServicioDAO _servicioDao;
        public ServicioDAO ServicioDAO { get { return _servicioDao ?? (_servicioDao = GetDao<ServicioDAO>()); } }

        private DetalleCicloDAO _detalleCicloDao;
        public DetalleCicloDAO DetalleCicloDAO { get { return _detalleCicloDao ?? (_detalleCicloDao = GetDao<DetalleCicloDAO>()); } }

        private TipoServicioCicloDAO _tipoServicioCicloDao;
        public TipoServicioCicloDAO TipoServicioCicloDAO { get { return _tipoServicioCicloDao ?? (_tipoServicioCicloDao = GetDao<TipoServicioCicloDAO>()); } }

        private BocaDeCargaDAO _bocaDeCargaDao;
        public BocaDeCargaDAO BocaDeCargaDAO { get { return _bocaDeCargaDao ?? (_bocaDeCargaDao = GetDao<BocaDeCargaDAO>()); } }

        private PedidoDAO _pedidoDao;
        public PedidoDAO PedidoDAO { get { return _pedidoDao ?? (_pedidoDao = GetDao<PedidoDAO>()); } }

        #endregion

        #region Ciclo Logístico Distribución

        private PreasignacionViajeVehiculoDAO _preasignacionViajeVehiculoDAO;
        public PreasignacionViajeVehiculoDAO PreasignacionViajeVehiculoDAO { get { return _preasignacionViajeVehiculoDAO ?? (_preasignacionViajeVehiculoDAO = GetDao<PreasignacionViajeVehiculoDAO>()); } }

        private ViajeDistribucionDAO _viajeDistribucionDao;
        public ViajeDistribucionDAO ViajeDistribucionDAO { get { return _viajeDistribucionDao ?? (_viajeDistribucionDao = GetDao<ViajeDistribucionDAO>()); } }

        private EntregaDistribucionDAO _entregaDistribucionDao;
        public EntregaDistribucionDAO EntregaDistribucionDAO { get { return _entregaDistribucionDao ?? (_entregaDistribucionDao = GetDao<EntregaDistribucionDAO>()); } }

        private EstadoDistribucionDAO _estadoDistribucionDao;
        public EstadoDistribucionDAO EstadoDistribucionDAO { get { return _estadoDistribucionDao ?? (_estadoDistribucionDao = GetDao<EstadoDistribucionDAO>()); } }

        private RecorridoDistribucionDAO _recorridoDistribucionDao;
        public RecorridoDistribucionDAO RecorridoDistribucionDAO { get { return _recorridoDistribucionDao ?? (_recorridoDistribucionDao = GetDao<RecorridoDistribucionDAO>()); } }

        private RecorridoDAO _recorridoDao;
        public RecorridoDAO RecorridoDAO { get { return _recorridoDao ?? (_recorridoDao = GetDao<RecorridoDAO>()); } }

        private DetalleRecorridoDAO _detalleRecorridoDao;
        public DetalleRecorridoDAO DetalleRecorridoDAO { get { return _detalleRecorridoDao ?? (_detalleRecorridoDao = GetDao<DetalleRecorridoDAO>()); } }

        private LastVehicleEventDAO _lastVehicleEventDao;
        public LastVehicleEventDAO LastVehicleEventDAO { get { return _lastVehicleEventDao ?? (_lastVehicleEventDao = GetDao<LastVehicleEventDAO>()); } }

        private TicketRechazoDAO _ticketRechazoDao;
        public TicketRechazoDAO TicketRechazoDAO { get { return _ticketRechazoDao ?? (_ticketRechazoDao = GetDao<TicketRechazoDAO>()); } }

        private SosTicketDAO _sosTicketDAO;
        public SosTicketDAO SosTicketDAO { get { return _sosTicketDAO ?? (_sosTicketDAO = GetDao<SosTicketDAO>()); } }

        private ViajeProgramadoDAO _viajeProgramadoDAO;
        public ViajeProgramadoDAO ViajeProgramadoDAO { get { return _viajeProgramadoDAO ?? (_viajeProgramadoDAO = GetDao<ViajeProgramadoDAO>()); } }

        #endregion

        #region Ciclo Logístico

        private EstadoLogisticoDAO _estadoLogisticoDao;
        public EstadoLogisticoDAO EstadoLogisticoDAO { get { return _estadoLogisticoDao ?? (_estadoLogisticoDao = GetDao<EstadoLogisticoDAO>()); } }

        private TipoCicloLogisticoDAO _tipoCicloLogisticoDao;
        public TipoCicloLogisticoDAO TipoCicloLogisticoDAO { get { return _tipoCicloLogisticoDao ?? (_tipoCicloLogisticoDao = GetDao<TipoCicloLogisticoDAO>()); } }


        #endregion

        #region Combustible SAI

        private MovimientoDAO _movimientoDao;
        public MovimientoDAO MovimientoDAO { get { return _movimientoDao ?? (_movimientoDao = GetDao<MovimientoDAO>()); } }

        private TanqueDAO _tanqueDao;
        public TanqueDAO TanqueDAO { get { return _tanqueDao ?? (_tanqueDao = GetDao<TanqueDAO>()); } }

        private VolumenHistoricoDAO _volumenHistoricoDao;
        public VolumenHistoricoDAO VolumenHistoricoDAO { get { return _volumenHistoricoDao ?? (_volumenHistoricoDao = GetDao<VolumenHistoricoDAO>()); } }

        private TipoMovimientoDAO _tipoMovimientoDao;
        public TipoMovimientoDAO TipoMovimientoDAO { get { return _tipoMovimientoDao ?? (_tipoMovimientoDao = GetDao<TipoMovimientoDAO>()); } }

        private CaudalimetroDAO _caudalimetroDao;
        public CaudalimetroDAO CaudalimetroDAO { get { return _caudalimetroDao ?? (_caudalimetroDao = GetDao<CaudalimetroDAO>()); } }

        private MotivoConciliacionDAO _motivoConciliacionDao;
        public MotivoConciliacionDAO MotivoConciliacionDAO { get { return _motivoConciliacionDao ?? (_motivoConciliacionDao = GetDao<MotivoConciliacionDAO>()); } }

        private VolumenHistoricoInvalidoDAO _volumenHistoricoInvalidoDao;
        public VolumenHistoricoInvalidoDAO VolumenHistoricoInvalidoDAO { get { return _volumenHistoricoInvalidoDao ?? (_volumenHistoricoInvalidoDao = GetDao<VolumenHistoricoInvalidoDAO>()); } }

        #endregion

        #region Sync Out
        private OutQueueDAO _outQueueDao;
        public OutQueueDAO OutQueueDAO { get { return _outQueueDao ?? (_outQueueDao = GetDao<OutQueueDAO>()); } }

        private OutStateDAO _outStateDao;
        public OutStateDAO OutStateDAO { get { return _outStateDao ?? (_outStateDao = GetDao<OutStateDAO>()); } } 
        #endregion

        #region Control De Acceso

        private TarjetaDAO _tarjetaDao;
        public TarjetaDAO TarjetaDAO { get { return _tarjetaDao ?? (_tarjetaDao = GetDao<TarjetaDAO>()); } }

        private CategoriaAccesoDAO _categoriaAccesoDAO;
        public CategoriaAccesoDAO CategoriaAccesoDAO { get { return _categoriaAccesoDAO ?? (_categoriaAccesoDAO = GetDao<CategoriaAccesoDAO>()); } }

        #endregion

        #region Tickets de Soporte

        private SupportTicketDAO _supportTicketDao;
        public SupportTicketDAO SupportTicketDAO { get { return _supportTicketDao ?? (_supportTicketDao = GetDao<SupportTicketDAO>()); } }

        private CategoriaDAO _categoriaDao;
        public CategoriaDAO CategoriaDAO { get { return _categoriaDao ?? (_categoriaDao = GetDao<CategoriaDAO>()); } }

        private SubCategoriaDAO _subcategoriaDao;
        public SubCategoriaDAO SubCategoriaDAO { get { return _subcategoriaDao ?? (_subcategoriaDao = GetDao<SubCategoriaDAO>()); } }

        private NivelDAO _nivelDao;
        public NivelDAO NivelDAO { get { return _nivelDao ?? (_nivelDao = GetDao<NivelDAO>()); } } 

        #endregion

     
        private EstadoDAO _estadoDao;
        public EstadoDAO EstadoDAO { get { return _estadoDao ?? (_estadoDao = GetDao<EstadoDAO>()); } }

        private EquipoDAO _equipoDao;
        public EquipoDAO EquipoDAO { get { return _equipoDao ?? (_equipoDao = GetDao<EquipoDAO>()); } }

        private ProductoDAO _productoDao;
        public ProductoDAO ProductoDAO { get { return _productoDao ?? (_productoDao = GetDao<ProductoDAO>()); } }

        private EventoAccesoDAO _eventoAccesoDao;
        public EventoAccesoDAO EventoAccesoDAO { get { return _eventoAccesoDao ?? (_eventoAccesoDao = GetDao<EventoAccesoDAO>()); } }

        private IconoDAO _iconoDao;
        public IconoDAO IconoDAO { get { return _iconoDao ?? (_iconoDao = GetDao<IconoDAO>()); } }

        private SonidoDAO _sonidoDao;
        public SonidoDAO SonidoDAO { get { return _sonidoDao ?? (_sonidoDao = GetDao<SonidoDAO>()); } }

        private LogPosicionDAO _logPosicionDao;
        public LogPosicionDAO LogPosicionDAO { get { return _logPosicionDao ?? (_logPosicionDao = GetDao<LogPosicionDAO>()); } }

        private LogPosicionHistoricaDAO _logPosicionHistoricaDao;
        public LogPosicionHistoricaDAO LogPosicionHistoricaDAO { get { return _logPosicionHistoricaDao ?? (_logPosicionHistoricaDao = GetDao<LogPosicionHistoricaDAO>()); } }

        private ClienteDAO _clienteDao;
        public ClienteDAO ClienteDAO { get { return _clienteDao ?? (_clienteDao = GetDao<ClienteDAO>()); } }

        private PuertaAccesoDAO _puertaAccesoDao;
        public PuertaAccesoDAO PuertaAccesoDAO { get { return _puertaAccesoDao ?? (_puertaAccesoDao = GetDao<PuertaAccesoDAO>()); } }

        private FeriadoDAO _feriadoDao;
        public FeriadoDAO FeriadoDAO { get { return _feriadoDao ?? (_feriadoDao = GetDao<FeriadoDAO>()); } }

        private PuntoEntregaDAO _puntoEntregaDao;
        public PuntoEntregaDAO PuntoEntregaDAO { get { return _puntoEntregaDao ?? (_puntoEntregaDao = GetDao<PuntoEntregaDAO>()); } }

        private RoutePositionsDAO _routePositionsDao;
        public RoutePositionsDAO RoutePositionsDAO { get { return _routePositionsDao ?? (_routePositionsDao = GetDao<RoutePositionsDAO>()); } }

        private LogPosicionDescartadaDAO _logPosicionDescartadaDao;
        public LogPosicionDescartadaDAO LogPosicionDescartadaDAO { get { return _logPosicionDescartadaDao ?? (_logPosicionDescartadaDao = GetDao<LogPosicionDescartadaDAO>()); } }

        private PeriodoDAO _periodoDao;
        public PeriodoDAO PeriodoDAO { get { return _periodoDao ?? (_periodoDao = GetDao<PeriodoDAO>()); } }

        private CentroDeCostosDAO _centroDeCostosDao;
        public CentroDeCostosDAO CentroDeCostosDAO { get { return _centroDeCostosDao ?? (_centroDeCostosDao = GetDao<CentroDeCostosDAO>()); } }

        private TarifaTransportistaDAO _tarifaTransportistaDao;
        public TarifaTransportistaDAO TarifaTransportistaDAO { get { return _tarifaTransportistaDao ?? (_tarifaTransportistaDao = GetDao<TarifaTransportistaDAO>()); } }

        private ShiftDAO _shiftDao;
        public ShiftDAO ShiftDAO { get { return _shiftDao ?? (_shiftDao = GetDao<ShiftDAO>()); } }
        
        private LoginAuditDAO _loginAuditDao;
        public LoginAuditDAO LoginAuditDAO { get { return _loginAuditDao ?? (_loginAuditDao = GetDao<LoginAuditDAO>()); } }

        private EntityAuditDAO _entityAuditDao;
        public EntityAuditDAO EntityAuditDAO { get { return _entityAuditDao ?? (_entityAuditDao = GetDao<EntityAuditDAO>()); } }

        private LogMensajeDescartadoDAO _logMensajeDescartadoDao;
        public LogMensajeDescartadoDAO LogMensajeDescartadoDAO { get { return _logMensajeDescartadoDao ?? (_logMensajeDescartadoDao = GetDao<LogMensajeDescartadoDAO>()); } }

        private DatamartDAO _datamartDao;
        public DatamartDAO DatamartDAO { get { return _datamartDao ?? (_datamartDao = GetDao<DatamartDAO>()); } }

        private DatamartTramoDAO _datamartTramoDao;
        public DatamartTramoDAO DatamartTramoDAO { get { return _datamartTramoDao ?? (_datamartTramoDao = GetDao<DatamartTramoDAO>()); } }

        private DatamartDistribucionDAO _datamartDistribucionDao;
        public DatamartDistribucionDAO DatamartDistribucionDAO { get { return _datamartDistribucionDao ?? (_datamartDistribucionDao = GetDao<DatamartDistribucionDAO>()); } }

        private DatamartViajeDAO _datamartViajeDao;
        public DatamartViajeDAO DatamartViajeDAO { get { return _datamartViajeDao ?? (_datamartViajeDao = GetDao<DatamartViajeDAO>()); } }
        
        private PuntajeExcesoVelocidadDAO _puntajeExcesoVelocidadDao;
        public PuntajeExcesoVelocidadDAO PuntajeExcesoVelocidadDAO { get { return _puntajeExcesoVelocidadDao ?? (_puntajeExcesoVelocidadDao = GetDao<PuntajeExcesoVelocidadDAO>()); } }

        private PuntajeExcesoTiempoDAO _puntajeExcesoTiempoDao;
        public PuntajeExcesoTiempoDAO PuntajeExcesoTiempoDAO { get { return _puntajeExcesoTiempoDao ?? (_puntajeExcesoTiempoDao = GetDao<PuntajeExcesoTiempoDAO>()); } }

        private EventoCombustibleDAO _eventoCombustibleDao;
        public EventoCombustibleDAO EventoCombustibleDAO { get { return _eventoCombustibleDao ?? (_eventoCombustibleDao = GetDao<EventoCombustibleDAO>()); } }

        private RutaDAO _rutaDao;
        public RutaDAO RutaDAO { get { return _rutaDao ?? (_rutaDao = GetDao<RutaDAO>()); } }

        private AsegurableDAO _asegurableDao;
        public AsegurableDAO AsegurableDAO { get { return _asegurableDao ?? (_asegurableDao = GetDao<AsegurableDAO>()); } }

        private ProgramacionReporteDAO _programacionReporteDao;
        public ProgramacionReporteDAO ProgramacionReporteDAO { get { return _programacionReporteDao ?? (_programacionReporteDao = GetDao<ProgramacionReporteDAO>()); } }

        private LogProgramacionReporteDAO _logProgramacionReporteDao;
        public LogProgramacionReporteDAO LogProgramacionReporteDAO { get { return _logProgramacionReporteDao ?? (_logProgramacionReporteDao = GetDao<LogProgramacionReporteDAO>()); } }

        private ConsumoCabeceraDAO _consumoCabeceraDao;
        public ConsumoCabeceraDAO ConsumoCabeceraDAO { get { return _consumoCabeceraDao ?? (_consumoCabeceraDao = GetDao<ConsumoCabeceraDAO>()); } }

        private ConsumoDetalleDAO _consumoDetalleDao;
        public ConsumoDetalleDAO ConsumoDetalleDAO { get { return _consumoDetalleDao ?? (_consumoDetalleDao = GetDao<ConsumoDetalleDAO>()); } }

        private InsumoDAO _insumoDao;
        public InsumoDAO InsumoDAO { get { return _insumoDao ?? (_insumoDao = GetDao<InsumoDAO>()); } }

        private TipoInsumoDAO _tipoInsumoDao;
        public TipoInsumoDAO TipoInsumoDAO { get { return _tipoInsumoDao ?? (_tipoInsumoDao = GetDao<TipoInsumoDAO>()); } }

        private DepositoDAO _depositoDao;
        public DepositoDAO DepositoDAO { get { return _depositoDao ?? ( _depositoDao = GetDao<DepositoDAO>()); } }

        private StockDAO _stockDao;
        public StockDAO StockDAO { get { return _stockDao ?? (_stockDao = GetDao<StockDAO>()); } }

        private StockVehicularDAO _stockVehicularDao;
        public StockVehicularDAO StockVehicularDAO { get { return _stockVehicularDao ?? (_stockVehicularDao = GetDao<StockVehicularDAO>()); } }

        private DetalleStockVehicularDAO _detalleStockVehicularDao;
        public DetalleStockVehicularDAO DetalleStockVehicularDAO { get { return _detalleStockVehicularDao ?? (_detalleStockVehicularDao = GetDao<DetalleStockVehicularDAO>()); } }

        private ProveedorDAO _proveedorDao;
        public ProveedorDAO ProveedorDAO { get { return _proveedorDao ?? (_proveedorDao = GetDao<ProveedorDAO>()); } }

        private ReportsCacheDAO _reportsCacheDao;
        public ReportsCacheDAO ReportsCacheDAO { get { return _reportsCacheDao ?? (_reportsCacheDao = GetDao<ReportsCacheDAO>()); } }

        private TipoEntidadDAO _tipoEntidadDao;
        public TipoEntidadDAO TipoEntidadDAO { get { return _tipoEntidadDao ?? (_tipoEntidadDao = GetDao<TipoEntidadDAO>()); } }

        private TipoMedicionDAO _tipoMedicionDao;
        public TipoMedicionDAO TipoMedicionDAO { get { return _tipoMedicionDao ?? (_tipoMedicionDao = GetDao<TipoMedicionDAO>()); } }

        private EntidadDAO _entidadDao;
        public EntidadDAO EntidadDAO { get { return _entidadDao ?? (_entidadDao = GetDao<EntidadDAO>()); } }

        private SubEntidadDAO _subEntidadDao;
        public SubEntidadDAO SubEntidadDAO { get { return _subEntidadDao ?? (_subEntidadDao = GetDao<SubEntidadDAO>()); } }

        private SensorDAO _sensorDao;
        public SensorDAO SensorDAO { get { return _sensorDao ?? (_sensorDao = GetDao<SensorDAO>()); } }

        private MedicionDAO _medicionDao;
        public MedicionDAO MedicionDAO { get { return _medicionDao ?? (_medicionDao = GetDao<MedicionDAO>()); } }

        private PrecintoDAO _precintoDao;
        public PrecintoDAO PrecintoDAO { get { return _precintoDao ?? (_precintoDao = GetDao<PrecintoDAO>()); } }

        private DetalleDAO _detalleDao;
        public DetalleDAO DetalleDAO { get { return _detalleDao ?? (_detalleDao = GetDao<DetalleDAO>()); } }

        private LogEventoDAO _logEventoDao;
        public LogEventoDAO LogEventoDAO { get { return _logEventoDao ?? (_logEventoDao = GetDao<LogEventoDAO>()); } }

        private UnidadMedidaDAO _unidadMedidaDao;
        public UnidadMedidaDAO UnidadMedidaDAO { get { return _unidadMedidaDao ?? (_unidadMedidaDao = GetDao<UnidadMedidaDAO>()); } }

        private TipoProveedorDAO _tipoProveedorDao;
        public TipoProveedorDAO TipoProveedorDAO { get { return _tipoProveedorDao ?? (_tipoProveedorDao = GetDao<TipoProveedorDAO>()); } }

        private DepartamentoDAO _departamentoDao;
        public DepartamentoDAO DepartamentoDAO { get { return _departamentoDao ?? (_departamentoDao = GetDao<DepartamentoDAO>()); } }

        private EventoViajeDAO _eventoViajeDao;
        public EventoViajeDAO EventoViajeDAO { get { return _eventoViajeDao ?? (_eventoViajeDao = GetDao<EventoViajeDAO>()); } }

        private SubCentroDeCostosDAO _subCentroDeCostosDao;
        public SubCentroDeCostosDAO SubCentroDeCostosDAO { get { return _subCentroDeCostosDao ?? (_subCentroDeCostosDao = GetDao<SubCentroDeCostosDAO>()); } }

        private TicketMantenimientoDAO _ticketMantenimientoDAO;
        public TicketMantenimientoDAO TicketMantenimientoDAO { get { return _ticketMantenimientoDAO ?? (_ticketMantenimientoDAO = GetDao<TicketMantenimientoDAO>()); } }

        private LogicLinkFileDAO _logicLinkFileDAO;
        public LogicLinkFileDAO LogicLinkFileDAO { get { return _logicLinkFileDAO ?? (_logicLinkFileDAO = GetDao<LogicLinkFileDAO>()); } }

        private DataMartsLogDAO _dataMartsLogDao;
        public DataMartsLogDAO DataMartsLogDAO { get { return _dataMartsLogDao ?? (_dataMartsLogDao = GetDao<DataMartsLogDAO>()); } }

        private AgendaVehicularDAO _agendaVehicularDao;
        public AgendaVehicularDAO AgendaVehicularDAO { get { return _agendaVehicularDao ?? (_agendaVehicularDao = GetDao<AgendaVehicularDAO>()); } }

        private InformeViajeRecargaDAO _informeRecargaDao;
        public InformeViajeRecargaDAO InformeViajeRecargaDAO { get { return _informeRecargaDao ?? (_informeRecargaDao = GetDao<InformeViajeRecargaDAO>()); } }

        private EstadoDiarioDAO _estadoDiarioDao;
        public EstadoDiarioDAO EstadoDiarioDAO { get { return _estadoDiarioDao ?? (_estadoDiarioDao = GetDao<EstadoDiarioDAO>()); } }

        private ColaMensajesDAO _colaMensajesDAO;
        public ColaMensajesDAO ColaMensajesDAO { get { return _colaMensajesDAO ?? (_colaMensajesDAO = GetDao<ColaMensajesDAO>()); } }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets an instance of the dao associated to the givenn type.
        /// </summary>
        /// <returns></returns>
        public static T GetDao<T>() where T : IGenericDAO
        {
            var type = typeof(T);

            var constructor = type.GetConstructor(Type.EmptyTypes);

            var dao = constructor.Invoke(new Object[] {}) as IGenericDAO;

            return (T) dao;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a generic DAO for the givenn type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public GenericDAO<T> GetGenericDAO<T>() where T : IAuditable { return new GenericDAO<T>(); }

        /// <summary>
        /// Creates anew database command using the current stateless session.
        /// </summary>
        /// <returns></returns>
        public IDbCommand CreateCommand() { return Session.Connection.CreateCommand(); }

        /// <summary>
        /// Creates a new sql query using the prived sql text within the current stateless session.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ISQLQuery CreateSqlQuery(String sql) { return Session.CreateSQLQuery(sql); }

        /// <summary>
        /// Removes the specified object from te current session.
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveFromSession(Object obj) { Session.Evict(obj); }

        #endregion

        public void SessionClear()
        {
            Session.Clear();
        }

        public void Dispose()
        {
            //Dispose(true);
            //GC.SuppressFinalize(this);
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
                
        //        //// free managed resources
        //        //if (managedResource != null)
        //        //{
        //        //    managedResource.Dispose();
        //        //    managedResource = null;
        //        //}
        //    }
        //    // free native resources if there are any.
        //    //if (nativeResource != IntPtr.Zero)
        //    //{
        //    //    Marshal.FreeHGlobal(nativeResource);
        //    //    nativeResource = IntPtr.Zero;
        //    //}
        //}
    }
}