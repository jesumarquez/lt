using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;
using Logictracker.Web.CustomWebControls.DropDownLists;

namespace Logictracker.Web.CustomWebControls.Binding
{
    /// <summary>
    /// Custom controls bindings helper.
    /// </summary>
    public partial class BindingManager
    {
        #region Private Properties

        /// <summary>
        /// Logic cycle types.
        /// </summary>
        private static IEnumerable<Pair> TiposLogisticos
        {
            get
            {
                return new List<Pair>
                                           {
                                               new Pair((int)Estado.Evento.Normal, CultureManager.GetLabel("EST_LOG_NORMAL")),
                                               new Pair((int)Estado.Evento.Iniciado, CultureManager.GetLabel("EST_LOG_INICIADO")),
                                               new Pair((int)Estado.Evento.GiroTrompoDerecha, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_DERECHA")),
                                               new Pair((int)Estado.Evento.GiroTrompoIzquierda, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_IZQUIERDA")),
                                               new Pair((int)Estado.Evento.TolvaActiva, CultureManager.GetLabel("EST_LOG_TOLVA_ACTIVA")),
                                               new Pair((int)Estado.Evento.TolvaInactiva, CultureManager.GetLabel("EST_LOG_TOLVA_INACTIVA")),
                                               new Pair((int)Estado.Evento.SaleDePlanta, CultureManager.GetLabel("EST_LOG_SALE_PLANTA")),
                                               new Pair((int)Estado.Evento.LlegaAObra, CultureManager.GetLabel("EST_LO_LLEGA_OBRA")),
                                               new Pair((int)Estado.Evento.SaleDeObra, CultureManager.GetLabel("EST_LOG_SALE_OBRA")),
                                               new Pair((int)Estado.Evento.LlegaAPlanta, CultureManager.GetLabel("EST_LO_LLEGA_PLANTA")),
                                               new Pair((int)Estado.Evento.GiroTrompoDetenido, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_DETENIDO")),
                                               new Pair((int)Estado.Evento.GiroTrompoHorarioLento, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_HOR_LENTO")),
                                               new Pair((int)Estado.Evento.GiroTrompoHorarioRapido, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_HOR_RAPIDO")),
                                               new Pair((int)Estado.Evento.GiroTrompoAntihorarioLento, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_AH_LENTO")),
                                               new Pair((int)Estado.Evento.GiroTrompoAntihorarioRapido, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_AH_RAPIDO"))
                                           };
            }
        }

        /// <summary>
        /// Gets all ticket states
        /// </summary>
        private static Dictionary<short, string> EstadosTicket
        {
            get
            {
                return new Dictionary<short, string>
                                         {
                                             {Ticket.Estados.Pendiente, CultureManager.GetLabel("TICKETSTATE_PROGRAMMED")},
                                             {Ticket.Estados.EnCurso, CultureManager.GetLabel("TICKETSTATE_CURRENT")},
                                             {Ticket.Estados.Cerrado, CultureManager.GetLabel("TICKETSTATE_CLOSED")},
                                             {Ticket.Estados.Anulado, CultureManager.GetLabel("TICKETSTATE_ANULADO")}
                                         };
            }
        }

        /// <summary>
        /// A list of DetalleCiclo types.
        /// </summary>
        private static Dictionary<short, string> TiposDetalleCiclo
        {
            get
            {
                return new Dictionary<short, string>
                                        {
                                            {DetalleCiclo.TipoTiempo, "Tiempo"},
                                            {DetalleCiclo.TipoEvento, "Evento"},
                                            {DetalleCiclo.TipoEntradaPoi, "Entrada a Referencia Geografica"},
                                            {DetalleCiclo.TipoSalidaPoi, "Salida de Referencia Geografica"},
                                            {DetalleCiclo.TipoCicloLogistico, "Ciclo Logistico"}
                                        };
            }
        }

        private static Dictionary<int, string> EstadosPedido
        {
            get
            {
                return new Dictionary<int, string>
                           {
                               {Pedido.Estados.Pendiente, CultureManager.GetLabel("ESTADO_PEDIDO_PENDIENTE")},
                               {Pedido.Estados.EnCurso, CultureManager.GetLabel("ESTADO_PEDIDO_ENCURSO")},
                               {Pedido.Estados.Entregado, CultureManager.GetLabel("ESTADO_PEDIDO_ENTREGADO")},
                               {Pedido.Estados.Cancelado, CultureManager.GetLabel("ESTADO_PEDIDO_CANCELADO")}
                           };
            }
        }

        private static Dictionary<int, string> EstadosSoporte
        {
            get
            {
                return new Dictionary<int, string>
                           {
                               { 1, CultureManager.GetLabel("SUPPORT_STATE_1_OPEN") },
                               { 2, CultureManager.GetLabel("SUPPORT_STATE_2_WORKING") },
                               { 3, CultureManager.GetLabel("SUPPORT_STATE_3_WAITING_USER") },
                               { 4, CultureManager.GetLabel("SUPPORT_STATE_4_SOLVED") },
                               { 5, CultureManager.GetLabel("SUPPORT_STATE_5_APPROVED") },
                               { 6, CultureManager.GetLabel("SUPPORT_STATE_6_REJECTED") },
                               { 7, CultureManager.GetLabel("SUPPORT_STATE_7_CLOSED") },
                               { 8, CultureManager.GetLabel("SUPPORT_STATE_8_INVALID") }
                           };
            }
        }

        private static Dictionary<int, string> EstadosEntregaDistribucion
        {
            get
            {
                return new Dictionary<int, string>
                           {
                               {EntregaDistribucion.Estados.EnZona, CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(EntregaDistribucion.Estados.EnZona)) },
                               {EntregaDistribucion.Estados.Pendiente, CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(EntregaDistribucion.Estados.Pendiente)) },
                               {EntregaDistribucion.Estados.EnSitio, CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(EntregaDistribucion.Estados.EnSitio)) },
                               {EntregaDistribucion.Estados.Visitado, CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(EntregaDistribucion.Estados.Visitado)) },
                               {EntregaDistribucion.Estados.SinVisitar, CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(EntregaDistribucion.Estados.SinVisitar)) },
                               {EntregaDistribucion.Estados.NoCompletado, CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(EntregaDistribucion.Estados.NoCompletado)) },
                               {EntregaDistribucion.Estados.Completado, CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(EntregaDistribucion.Estados.Completado)) }
                           };
            }
        }

        private static Dictionary<int, string> EstadosViajeDistribucion
        {
            get
            {
                return new Dictionary<int, string>
                           {
                               {ViajeDistribucion.Estados.Cerrado, CultureManager.GetLabel(ViajeDistribucion.Estados.GetLabelVariableName(ViajeDistribucion.Estados.Cerrado)) },
                               {ViajeDistribucion.Estados.EnCurso, CultureManager.GetLabel(ViajeDistribucion.Estados.GetLabelVariableName(ViajeDistribucion.Estados.EnCurso)) },
                               {ViajeDistribucion.Estados.Pendiente, CultureManager.GetLabel(ViajeDistribucion.Estados.GetLabelVariableName(ViajeDistribucion.Estados.Pendiente)) }
                           };
            }
        }

        private static Dictionary<int, string> ModulosDatamart
        {
            get
            {
                return new Dictionary<int, string>
                           {
                               {DataMartsLog.Moludos.DatamartEntregas, DataMartsLog.Moludos.GetString(DataMartsLog.Moludos.DatamartEntregas) },
                               {DataMartsLog.Moludos.DatamartRecorridos, DataMartsLog.Moludos.GetString(DataMartsLog.Moludos.DatamartRecorridos) },
                               {DataMartsLog.Moludos.DatamartRutas, DataMartsLog.Moludos.GetString(DataMartsLog.Moludos.DatamartRutas)}

                           };
            }
        }

        private static Dictionary<int, string> ReportStatus
        {
            get
            {
                return new Dictionary<int, string>
                           {
                               {0, "Correctos"},
                               {1, "Erroneos"}
                           };
            }
        }

        private static Dictionary<int, string> EstadosArchivosLogicLink
        {
            get
            {
                return new Dictionary<int, string>
                           {   {LogicLinkFile.Estados.Pendiente, LogicLinkFile.Estados.GetString(LogicLinkFile.Estados.Pendiente)},
                               {LogicLinkFile.Estados.Procesado, LogicLinkFile.Estados.GetString(LogicLinkFile.Estados.Procesado)},
                               {LogicLinkFile.Estados.Procesando, LogicLinkFile.Estados.GetString(LogicLinkFile.Estados.Procesando)},
                               {LogicLinkFile.Estados.Cancelado, LogicLinkFile.Estados.GetString(LogicLinkFile.Estados.Cancelado)},
                           };
            }
        }

        #endregion


        #region Public Method
              
        /// <summary>
        /// Messages binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindMensajesEstadosLogisticos(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();

            var linea = idLinea > 0 ? DaoFactory.LineaDAO.FindById(idLinea) : null;
            var empresa = linea != null ? linea.Empresa : idEmpresa > 0 ? DaoFactory.EmpresaDAO.FindById(idEmpresa) : null;

            foreach (var mensaje in DaoFactory.MensajeDAO.FindCicloLogistico(empresa, linea)) autoBindeable.AddItem(mensaje.Descripcion, mensaje.Id);
        }

    

        /// <summary>
        /// Logic types binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindTiposLogisticos(TipoLogisticoDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (var pair in TiposLogisticos) autoBindeable.Items.Add(new ListItem(pair.Second.ToString(), pair.First.ToString()));
        }

        /// <summary>
        /// Ticket status data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindEstadoTicket(EstadoTicketDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            AddDefaultItems(autoBindeable);

            foreach (var pair in EstadosTicket) autoBindeable.Items.Add(new ListItem(pair.Value, pair.Key.ToString("#0")));
        }

        public void BindTipoDetalleCiclo(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            foreach (var tipo in TiposDetalleCiclo) autoBindeable.AddItem(tipo.Value, tipo.Key);
        }

        public void BindCiclosLogisticos(ICicloLogisticoAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();

            var ciclos = DaoFactory.CicloLogisticoDAO.GetByType(idEmpresa, idLinea, autoBindeable.AddCiclos, autoBindeable.AddEstados);

            foreach (var ciclo in ciclos) autoBindeable.AddItem(ciclo.Descripcion, ciclo.Id);
        }

        public void BindEstadosPedido(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            AddDefaultItems(autoBindeable);

            foreach (var tipo in EstadosPedido) autoBindeable.AddItem(tipo.Value, tipo.Key);
        }

        public void BindBocaDeCarga(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            AddDefaultItems(autoBindeable);
            
            if (autoBindeable.GetParent<Empresa>() == null && autoBindeable.GetParent<Linea>() == null) return;

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();

            if (idEmpresa <= 0 && idLinea <= 0) return;

            var bocas = DaoFactory.BocaDeCargaDAO.GetList(new[] { idEmpresa }, new[] { idLinea });

            foreach (var boca in bocas) autoBindeable.AddItem(boca.Descripcion, boca.Id);
        }

        public void BindEstadoTicketSoporte(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            AddDefaultItems(autoBindeable);

            foreach (var estado in EstadosSoporte) autoBindeable.AddItem(estado.Value, estado.Key);
        }

        public void BindEstadoEntregaDistribucion(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            AddDefaultItems(autoBindeable);

            foreach (var estado in EstadosEntregaDistribucion.OrderBy(e => e.Value)) autoBindeable.AddItem(estado.Value, estado.Key);
        }

        public void BindEstadoViajeDistribucion(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            AddDefaultItems(autoBindeable);

            foreach (var estado in EstadosViajeDistribucion.OrderBy(e => e.Value)) autoBindeable.AddItem(estado.Value, estado.Key);
        }

        public void BindDatamart(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            AddDefaultItems(autoBindeable);

            foreach (var modulo in ModulosDatamart.OrderBy(e => e.Value)) autoBindeable.AddItem(modulo.Value, modulo.Key);
        }

        public void BindReportExecution(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            AddDefaultItems(autoBindeable);

            foreach (var exec in ReportStatus.OrderBy(e => e.Value)) autoBindeable.AddItem(exec.Value, exec.Key);
        }

        public void BindEstadoArchivo(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            AddDefaultItems(autoBindeable);

            foreach (var estado in EstadosArchivosLogicLink.OrderBy(e => e.Value)) autoBindeable.AddItem(estado.Value, estado.Key);
        }

        public void BindTipoZonaAcceso(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();

            var tiposZonaAcceso = DaoFactory.TipoZonaAccesoDAO.GetList(new[] { idEmpresa }, new[] { idLinea }).OrderBy(t => t.Descripcion);

            foreach (var tipoZonaAcceso in tiposZonaAcceso) autoBindeable.AddItem(tipoZonaAcceso.Descripcion, tipoZonaAcceso.Id);
        }

        public void BindZonaAcceso(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idsTipoZonaAcceso = autoBindeable.ParentSelectedValues<TipoZonaAcceso>();

            var zonasAcceso = DaoFactory.ZonaAccesoDAO.GetList(new[] { idEmpresa }, new[] { idLinea }, idsTipoZonaAcceso).OrderBy(z => z.Descripcion);

            foreach (var zonaAcceso in zonasAcceso) autoBindeable.AddItem(zonaAcceso.Descripcion, zonaAcceso.Id);
        }

        public void BindTipoCicloLogistico(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            
            var tipos = DaoFactory.TipoCicloLogisticoDAO.GetByEmpresa(idEmpresa).OrderBy(z => z.Descripcion);

            foreach (var tipo in tipos) autoBindeable.AddItem(tipo.Descripcion, tipo.Id);
        }

        #endregion
    }
}
