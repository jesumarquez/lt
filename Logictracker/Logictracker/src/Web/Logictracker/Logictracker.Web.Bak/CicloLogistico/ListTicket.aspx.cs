using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.Messages.Saver;
using Logictracker.Messaging;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Process.CicloLogistico.Exceptions;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.ValueObjects.CicloLogistico;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.Buttons;
using System.Drawing;
using Logictracker.Web.CustomWebControls.Input;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.CicloLogistico
{
    public partial class ListaTicket : SecuredListPage<TicketVo>
    {
        protected override string RedirectUrl { get { return "AltaTicket.aspx"; } }
        protected override string VariableName { get { return "CLOG_TICKETS"; } }
        protected override string GetRefference() { return "TICKET"; }
        protected override bool ExcelButton { get { return true; } }

        private VsProperty<int> TicketToInitId { get { return this.CreateVsProperty("TicketToInitId", -1); } }
        private VsProperty<bool> TieneDocumentosVencidos { get { return this.CreateVsProperty("TieneDocumentosVencidos", false); } }

        #region Protected Methods

        protected override List<TicketVo> GetListData()
        {
            var list = DAOFactory.TicketDAO.GetList(new[] {cbEmpresa.Selected},
                                                    new[] {cbLinea.Selected},
                                                    new[] {cbTransportista.Selected},
                                                    new[] {-1},
                                                    new[] {-1},
                                                    new[] {-1},
                                                    new[] {cbMovil.Selected},
                                                    new[] {cbEstado.Selected},
                                                    new[] {-1},
                                                    new[] {-1},
                                                    new[] {cbBocaDeCarga.Selected},
                                                    SecurityExtensions.ToDataBaseDateTime(dtpDesde.SelectedDate.Value),
                                                    SecurityExtensions.ToDataBaseDateTime(dtpHasta.SelectedDate.Value));

            return (from Ticket t in list orderby t.FechaTicket descending select new TicketVo(t)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, TicketVo dataItem)
        {
            var ticket = dataItem;

            var cellEstado = GridUtils.GetCell(e.Row, TicketVo.IndexEstado);
            var lblEstado = cellEstado.FindControl("lblEstado") as ResourceLabel;
            var lnkIniciar = cellEstado.FindControl("lnkIniciar") as ResourceLinkButton;
            var lnkCerrar = cellEstado.FindControl("lnkCerrar") as ResourceLinkButton;

            if (lnkCerrar == null || lnkIniciar == null || lblEstado == null) return;

            if (!dataItem.PuntoEntregaNomenclado)
            {
                e.Row.BackColor = Color.Firebrick;
            }

            lnkCerrar.OnClientClick = string.Concat("return confirm('", CultureManager.GetString("SystemMessages", "CONFIRM_OPERATION"), "');");


            switch (ticket.Estado)
            {
                case Ticket.Estados.EnCurso:
                    lnkIniciar.Visible = false;
                    lnkCerrar.Visible = true;
                    lblEstado.VariableName = "TICKETSTATE_CURRENT";
                    lblEstado.Visible = true;
                    break;
                case Ticket.Estados.Cerrado:
                    lnkIniciar.Visible = false;
                    lnkCerrar.Visible = false;
                    lblEstado.VariableName = "TICKETSTATE_CLOSED";
                    lblEstado.Visible = true;
                    break;
                case Ticket.Estados.Anulado:
                    lnkIniciar.Visible = false;
                    lnkCerrar.Visible = false;
                    lblEstado.VariableName = "TICKETSTATE_ANULADO";
                    lblEstado.Visible = true;
                    break;
                default:
                    lnkIniciar.Visible = dataItem.PuntoEntregaNomenclado && dataItem.HasCoche;
                    lnkCerrar.Visible = false;
                    lblEstado.VariableName = "TICKETSTATE_PROGRAMMED";
                    lblEstado.Visible = !lnkIniciar.Visible;
                    break;
            }
        }

        protected void lnkbtnEstado_Command(object sender, CommandEventArgs commandEventArgs)
        {
            var index = Convert.ToInt32(commandEventArgs.CommandArgument);
            var id = Convert.ToInt32(Grid.DataKeys[index].Value);
            var ticket = DAOFactory.TicketDAO.FindById(id);

            switch (commandEventArgs.CommandName)
            {
                case "Start":
                        StartTicketWindow(ticket);
                        break;
                case "Close":
                        ticket.UserField3 += "(cerrado manual)";
                        var ciclo = new CicloLogisticoHormigon(ticket, DAOFactory, new MessageSaver(DAOFactory));
                        var evento = new CloseEvent(DateTime.UtcNow);
                        ciclo.ProcessEvent(evento);
                        Bind();
                        break;
            }
        }

        protected void StartTicketWindow(Ticket ticket)
        {
            if (ticket.Vehiculo == null)
            {
                ShowResourceError("TICKET_NO_VEHICLE_ASSIGNED");
                return;
            }

            if (ticket.Vehiculo.Dispositivo == null)
            {
                ShowResourceError("VEHICLE_NO_DEVICE_ASSIGNED");
                return;
            }

            var stateList = ticket.Detalles.OfType<DetalleTicket>().OrderBy(t => t.EstadoLogistico.Orden).ToList();

            if (stateList.Count == 0)
            {
                ShowResourceError("NO_CLOG_STATES");
                return;
            }

            var primerEstado = stateList[0].EstadoLogistico;

            if (primerEstado == null || primerEstado.Mensaje == null)
            {
                ShowResourceError("NO_MESSAGE_FOR_START_CLOG");
                return;
            }

            TicketToInitId.Set(ticket.Id);

            var opened = DAOFactory.TicketDAO.FindEnCurso(ticket.Vehiculo.Dispositivo);

            if (opened != null)
            {
                lblCodigoTicket.Text = opened.Codigo;
                lblCliente.Text = opened.Cliente.Descripcion;
                lblPuntoEntrega.Text = opened.PuntoEntrega.Descripcion;
                lblFecha.Text = opened.FechaTicket.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
            }
            else
            {
                var tieneDocumentosVencidos = !ValidateDocuments(ticket);
                TieneDocumentosVencidos.Set(tieneDocumentosVencidos);
                if (!NoValidarDocumentos && tieneDocumentosVencidos) return;
            }

            panelStartTicket.Visible = opened == null;
            panelOpenedTicket.Visible = opened != null;
            mpePanel.CancelControlID = opened == null ? btCancelar.ID : btOpenedCancelar.ID;

            mpePanel.Show();

            dtHora.SelectedDate = ticket.FechaTicket.Value.ToDisplayDateTime();
        }
        protected void btOpenedCerrarTicket_Click(object sender, EventArgs e)
        {
            if (TicketToInitId.Get() <= 0) return;

            var ticket = DAOFactory.TicketDAO.FindById(TicketToInitId.Get());

            var opened = DAOFactory.TicketDAO.FindEnCurso(ticket.Vehiculo.Dispositivo);
            if (opened != null)
            {
                opened.UserField3 += "(cerrado manual x inicio)";
                var ciclo = new CicloLogisticoHormigon(opened, DAOFactory, new MessageSaver(DAOFactory));
                var evento = new CloseEvent(DateTime.UtcNow);
                ciclo.ProcessEvent(evento);
            }

            StartTicketWindow(ticket);
        }
        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                dtpDesde.SetDate();
                dtpHasta.SetDate();
            }
            base.OnLoad(e);
        }

        protected void btNow_Click(object sender, EventArgs e)
        {
            dtHora.SelectedDate = DateTime.UtcNow.ToDisplayDateTime();
            mpePanel.Show();
        }
        protected void btCancelar_Click(object sender, EventArgs e)
        {
            TicketToInitId.Set(-1);
        }
        protected void btIniciar_Click(object sender, EventArgs e)
        {
            mpePanel.Hide();
            if (TicketToInitId.Get() <= 0) return;

            var date = dtHora.SelectedDate;

            if (!date.HasValue)
            {
                ThrowMustEnter("FECHA");
            }

            var ticket = DAOFactory.TicketDAO.FindById(TicketToInitId.Get());
            TicketToInitId.Set(-1);

            SetStartDate(ticket, SecurityExtensions.ToDataBaseDateTime(date.Value));
            
            var messageSaver = new MessageSaver(DAOFactory);

            var ciclo = new CicloLogisticoHormigon(ticket, DAOFactory, messageSaver);

            var evento = new InitEvent(DateTime.UtcNow);

            try
            {
                ciclo.ProcessEvent(evento);
                ShowInfo(CultureManager.GetSystemMessage("CLOG_START_SENT") + ticket.Vehiculo.Interno);
                Bind();
                if(TieneDocumentosVencidos.Get())
                {
                    messageSaver.Save(MessageCode.CicloLogisticoIniciadoDocumentosInvalidos.GetMessageCode(),
                                      ticket.Vehiculo, evento.Date.AddSeconds(1), null, string.Empty);
                }
            }
            catch (NoVehicleException)
            {
                ShowError(new ApplicationException(CultureManager.GetError("TICKET_NO_VEHICLE_ASSIGNED")));
            }
            catch //AlreadyOpenException, QueueException, Exception
            {
                ShowError(new ApplicationException(CultureManager.GetError("CLOG_MESSAGE_NOT_SENT") + ticket.Vehiculo.Interno));
            }         
        }

        private void SetStartDate(Ticket ticket, DateTime date)
        {
            var startDate = date;
            var detalles = ticket.Detalles.OfType<DetalleTicket>().ToList();
            var lastOriginalDate = detalles[0].Programado;

            if (startDate == lastOriginalDate) return;

            var lastDate = startDate;
            detalles[0].Programado = startDate;
            for (var i = 1; i < detalles.Count; i++)
            {
                var detalle = detalles[i];
                var diff = detalle.Programado.Value.Subtract(lastOriginalDate.Value);
                lastOriginalDate = detalle.Programado;
                lastDate = lastDate.Add(diff);
                detalle.Programado = lastDate;
            }
            ticket.FechaTicket = startDate;
            DAOFactory.TicketDAO.SaveOrUpdate(ticket);
        }

        #endregion

        #region Private Methods

        protected override void CreateRowTemplate(C1GridViewRow row)
        {
            var lblEstado = row.Cells[TicketVo.IndexEstado].FindControl("lblEstado") as ResourceLabel;
            var lnkIniciar = row.Cells[TicketVo.IndexEstado].FindControl("lnkIniciar") as ResourceLinkButton;
            var lnkCerrar = row.Cells[TicketVo.IndexEstado].FindControl("lnkCerrar") as ResourceLinkButton;

            if (lblEstado == null)
            {
                lblEstado = new ResourceLabel { ResourceName = "Labels", ID = "lblEstado", Visible = false };
                row.Cells[TicketVo.IndexEstado].Controls.Add(lblEstado);
            }

            if (lnkIniciar == null)
            {
                lnkIniciar = new ResourceLinkButton
                {
                    ResourceName = "Labels",
                    ID = "lnkIniciar",
                    CommandName = "Start",
                    CommandArgument = row.RowIndex.ToString(),
                    VariableName = "TICKET_INICIAR",
                    Visible = false,
                    CausesValidation = false
                };
                row.Cells[TicketVo.IndexEstado].Controls.Add(lnkIniciar);
            }

            if (lnkCerrar == null)
            {
                lnkCerrar = new ResourceLinkButton
                {
                    ResourceName = "Labels",
                    ID = "lnkCerrar",
                    CommandName = "Close",
                    CommandArgument = row.RowIndex.ToString(),
                    VariableName = "TICKET_CERRAR",
                    Visible = false,
                    CausesValidation = false
                };
                row.Cells[TicketVo.IndexEstado].Controls.Add(new Literal { Text = "<br/>" });
                row.Cells[TicketVo.IndexEstado].Controls.Add(lnkCerrar);
            }


            lnkIniciar.Command += lnkbtnEstado_Command;
            lnkCerrar.Command += lnkbtnEstado_Command;
        }

        #endregion

        #region Validar Documentos
        private bool ValidateDocuments(Ticket ticket)
        {
            cbDocumentosVencidos.Items.Clear();

            var tiposVehiculoSinPresentar = new List<TipoDocumento>();
            var tiposEmpleadoSinPresentar = new List<TipoDocumento>();
            var tiposVehiculoVencidos = new List<TipoDocumento>();
            var tiposEmpleadoVencidos = new List<TipoDocumento>();

            if (ticket.Vehiculo != null)
            {
                var empresa = ticket.Vehiculo.Empresa != null ? ticket.Vehiculo.Empresa.Id : -1;
                var linea = ticket.Vehiculo.Linea != null ? ticket.Vehiculo.Linea.Id : -1;
                var tiposVehiculo = DAOFactory.TipoDocumentoDAO.FindObligatorioVehiculo(empresa, linea);
                foreach (var tipo in tiposVehiculo)
                {
                    var docs = DAOFactory.DocumentoDAO.FindForVehiculo(tipo.Id, ticket.Vehiculo.Id);
                    if (docs.Count == 0 || !docs.Any(d => d.Presentacion.HasValue))
                    {
                        tiposVehiculoSinPresentar.Add(tipo);
                    }
                    else if (!docs.Any(d => d.Presentacion.HasValue && (!d.Vencimiento.HasValue || d.Vencimiento.Value > ticket.FechaTicket.Value)))
                    {
                        tiposVehiculoVencidos.Add(tipo);
                    }
                }
            }
            if (ticket.Empleado != null)
            {
                var empresa = ticket.Empleado.Empresa != null ? ticket.Empleado.Empresa.Id : -1;
                var linea = ticket.Empleado.Linea != null ? ticket.Empleado.Linea.Id : -1;
                var tiposEmpleado = DAOFactory.TipoDocumentoDAO.FindObligatorioEmpleado(empresa, linea);
                foreach (var tipo in tiposEmpleado)
                {
                    var docs = DAOFactory.DocumentoDAO.FindForEmpleado(tipo.Id, ticket.Empleado.Id);
                    if (docs.Count == 0 || !docs.Any(d => d.Presentacion.HasValue))
                    {
                        tiposEmpleadoSinPresentar.Add(tipo);
                    }
                    if (!docs.Any(d => d.Presentacion.HasValue && (!d.Vencimiento.HasValue || d.Vencimiento.Value > ticket.FechaTicket.Value)))
                    {
                        tiposEmpleadoVencidos.Add(tipo);
                    }
                }
            }

            if (tiposVehiculoSinPresentar.Count > 0 || tiposEmpleadoSinPresentar.Count > 0 ||
                tiposVehiculoVencidos.Count > 0 || tiposEmpleadoVencidos.Count > 0)
            {
                foreach (var tipo in tiposVehiculoSinPresentar)
                    cbDocumentosVencidos.Items.Add(string.Concat(tipo.Nombre, "(", CultureManager.GetEntity("PARENTI03"), ")"));
                foreach (var tipo in tiposVehiculoVencidos)
                    cbDocumentosVencidos.Items.Add(string.Concat(tipo.Nombre, "(", CultureManager.GetEntity("PARENTI03"), ")"));
                foreach (var tipo in tiposEmpleadoSinPresentar)
                    cbDocumentosVencidos.Items.Add(string.Concat(tipo.Nombre, "(", CultureManager.GetEntity("PARENTI09"), ")"));
                foreach (var tipo in tiposEmpleadoVencidos)
                    cbDocumentosVencidos.Items.Add(string.Concat(tipo.Nombre, "(", CultureManager.GetEntity("PARENTI09"), ")"));

                if (!NoValidarDocumentos)
                {
                    ModalPopupExtenderDocumentos.Show();
                    updDocumentos.Update();
                }
                return false;
            }

            return true;
        }

        protected bool NoValidarDocumentos;
        protected void btAceptarDocumentos_Click(object sender, EventArgs e)
        {
            if (TicketToInitId.Get() <= 0) return;
            NoValidarDocumentos = true;
            var ticket = DAOFactory.TicketDAO.FindById(TicketToInitId.Get());
            StartTicketWindow(ticket);
        } 
        #endregion

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticTransportista, cbTransportista);
            data.LoadStaticFilter(FilterData.StaticVehiculo, cbMovil);
            data.LoadLocalFilter((string) "ESTADO", (DropDownListBase) cbEstado);
            data.LoadLocalFilter((string) "DESDE", (DateTimePicker) dtpDesde);
            data.LoadLocalFilter((string) "HASTA", (DateTimePicker) dtpHasta);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticTransportista, cbTransportista.Selected);
            data.AddStatic(FilterData.StaticVehiculo, cbMovil.Selected);
            data.Add("ESTADO", cbEstado.Selected);
            data.Add("DESDE", dtpDesde.SelectedDate);
            data.Add("HASTA", dtpHasta.SelectedDate);
            return data;
        }

        #endregion
    }
}
