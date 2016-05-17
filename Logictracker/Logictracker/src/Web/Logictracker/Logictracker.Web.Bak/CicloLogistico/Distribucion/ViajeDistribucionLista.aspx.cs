using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messages.Saver;
using Logictracker.Messages.Sender;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Process.CicloLogistico.Exceptions;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ValueObjects.CicloLogistico.Distribucion;
using Logictracker.Utils;
using Logictracker.Web;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using System.Drawing;
using Logictracker.Model;
using Logictracker.Web.CustomWebControls.Input;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class ViajeDistribucionLista : SecuredListPage<ViajeDistribucionVo>
    {
        protected override string RedirectUrl { get { return "ViajeDistribucionAlta.aspx"; } }
        protected override string ImportUrl { get { return "ViajeDistribucionImport.aspx"; } }
        protected override string VariableName { get { return "CLOG_DISTRIBUCION"; } }
        protected override string GetRefference() { return "DISTRIBUCION"; }
        protected override bool ImportButton { get { return true; } }
        protected override bool ExcelButton { get { return true; } }
        protected override bool EditButton { get { return true; } }
        protected override bool ListButton { get { return true; } }
        private VsProperty<int> TicketToInitId { get { return this.CreateVsProperty("TicketToInitId", -1); } }
        private bool _editMode;

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                dtpDesde.SetDate();
                dtpHasta.SetDate();
            }
            base.OnLoad(e);
        }

        protected override List<ViajeDistribucionVo> GetListData()
        {
            var list = DAOFactory.ViajeDistribucionDAO.GetList(new[]{cbEmpresa.Selected},
                                                               new[]{cbLinea.Selected},
                                                               cbTransportista.SelectedValues,
                                                               cbDepartamento.SelectedValues,
                                                               cbCentroDeCosto.SelectedValues,
                                                               new[] {-1}, // SUB CENTRO DE COSTO
                                                               cbMovil.SelectedValues, 
                                                               SecurityExtensions.ToDataBaseDateTime(dtpDesde.SelectedDate.Value),
                                                               SecurityExtensions.ToDataBaseDateTime(dtpHasta.SelectedDate.Value));

            if (_editMode) list = list.Where(v => v.Estado == ViajeDistribucion.Estados.Pendiente).ToList();

            return list.Select(v => new ViajeDistribucionVo(v)).ToList();
        }

        protected void FilterChanged(object sender, EventArgs e) { if (IsPostBack) Bind(); }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ViajeDistribucionVo dataItem)
        {
            var ticket = dataItem;

            var cellEstado = GridUtils.GetCell(e.Row, ViajeDistribucionVo.IndexEstado);
            var cellVehiculo = GridUtils.GetCell(e.Row, ViajeDistribucionVo.IndexVehiculo);
            var lblEstado = cellEstado.FindControl("lblEstado") as ResourceLabel;
            var lnkIniciar = cellEstado.FindControl("lnkIniciar") as ResourceLinkButton;
            var lnkCerrar = cellEstado.FindControl("lnkCerrar") as ResourceLinkButton;
            var lnkReenviar = cellEstado.FindControl("lnkReenviar") as ResourceLinkButton;
            var lnkAsociar = cellEstado.FindControl("lnkAsociar") as ResourceLinkButton;
            var lnkCombinar = cellEstado.FindControl("lnkCombinar") as ResourceLinkButton;
            var cbVehiculo = cellVehiculo.FindControl("cbVehiculo") as DropDownList;

            if (!ticket.Nomenclado)
            {
                e.Row.BackColor = Color.LightCoral;
                grid.Columns[ViajeDistribucionVo.IndexNoNomencladas].Visible = true;
            }

            if (lnkCerrar == null || lnkIniciar == null || lblEstado == null || lnkReenviar == null || lnkAsociar == null || lnkCombinar == null) return;

            lnkCerrar.OnClientClick = string.Concat("return confirm('", CultureManager.GetString("SystemMessages", "CONFIRM_OPERATION"), "');");
            lnkReenviar.OnClientClick = string.Concat("return confirm('", CultureManager.GetString("SystemMessages", "CONFIRM_OPERATION"), "');");

            switch (ticket.Estado)
            {
                case ViajeDistribucion.Estados.EnCurso:
                    lnkIniciar.Visible = false;
                    lnkCerrar.Visible = lnkReenviar.Visible = lblEstado.Visible = true;
                    lblEstado.VariableName = "TICKETSTATE_CURRENT";
                    break;
                case ViajeDistribucion.Estados.Anulado:
                    lnkIniciar.Visible = lnkCerrar.Visible = lnkReenviar.Visible = false;
                    lblEstado.VariableName = "TICKETSTATE_ANULADO";
                    lblEstado.Visible = true;
                    break;
                case ViajeDistribucion.Estados.Cerrado:
                    lnkIniciar.Visible = lnkCerrar.Visible = lnkReenviar.Visible = false;
                    lblEstado.VariableName = "TICKETSTATE_CLOSED";
                    lblEstado.Visible = true;
                    break;
                default:
                    lnkIniciar.Visible = dataItem.HasCoche;
                    lnkCerrar.Visible = lnkReenviar.Visible = false;
                    lblEstado.Visible = !lnkIniciar.Visible;
                    lblEstado.VariableName = "TICKETSTATE_PROGRAMMED";
                    break;
            }

            lnkAsociar.Visible = !ticket.HasCoche;
            lnkCombinar.Visible = ticket.Estado == ViajeDistribucion.Estados.Pendiente && ticket.HasCoche;

            if (_editMode)
            {
                e.Row.Attributes.Remove("onclick");
                var vehiculos = DAOFactory.CocheDAO.GetList(new[] { ticket.IdEmpresa }, new[] { ticket.IdLinea }).OrderBy(c => c.Interno);

                cbVehiculo.Items.Add(new ListItem("Ninguno", "0"));
                foreach (var vehiculo in vehiculos)
                    cbVehiculo.Items.Add(new ListItem(vehiculo.Interno, vehiculo.Id.ToString("#0")));

                cbVehiculo.Attributes.Add("IdViaje", ticket.Id.ToString("#0"));
                cbVehiculo.SelectedValue = ticket.IdVehiculo.ToString("#0");
                cbVehiculo.Enabled = ticket.Estado == ViajeDistribucion.Estados.Pendiente;
                cbVehiculo.AutoPostBack = true;
                cbVehiculo.Enabled = true;
            }
            else
            {
                cbVehiculo.Items.Add(ticket.IdVehiculo > 0
                                         ? new ListItem(ticket.Vehiculo, ticket.IdVehiculo.ToString("#0"))
                                         : new ListItem("Ninguno", "0"));
                cbVehiculo.Enabled = false;
            }
        }

        private void ComboOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            var cb = sender as DropDownList;
            if (cb == null) return;

            int idViaje;
            if (!int.TryParse(cb.Attributes["IdViaje"], out idViaje) && idViaje <= 0) return;
            var viaje = DAOFactory.ViajeDistribucionDAO.FindById(idViaje);

            int idVehiculo;
            if  (!int.TryParse(cb.SelectedValue, out idVehiculo)) return;

            var vehiculo = idVehiculo > 0 ? DAOFactory.CocheDAO.FindById(idVehiculo) : null;

            viaje.Vehiculo = vehiculo;
            DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);
        }

        protected override void CreateRowTemplate(C1GridViewRow row)
        {
            var lblEstado = row.Cells[ViajeDistribucionVo.IndexEstado].FindControl("lblEstado") as ResourceLabel;
            var lnkIniciar = row.Cells[ViajeDistribucionVo.IndexEstado].FindControl("lnkIniciar") as ResourceLinkButton;
            var lnkCerrar = row.Cells[ViajeDistribucionVo.IndexEstado].FindControl("lnkCerrar") as ResourceLinkButton;
            var lnkReenviar = row.Cells[ViajeDistribucionVo.IndexEstado].FindControl("lnkReenviar") as ResourceLinkButton;
            var lnkAsociar = row.Cells[ViajeDistribucionVo.IndexEstado].FindControl("lnkAsociar") as ResourceLinkButton;
            var lnkCombinar = row.Cells[ViajeDistribucionVo.IndexEstado].FindControl("lnkCombinar") as ResourceLinkButton;
            var cbVehiculo = row.Cells[ViajeDistribucionVo.IndexVehiculo].FindControl("cbVehiculo") as DropDownList;

            if (lblEstado == null)
            {
                lblEstado = new ResourceLabel { ResourceName = "Labels", ID = "lblEstado", Visible = false };
                row.Cells[ViajeDistribucionVo.IndexEstado].Controls.Add(lblEstado);
            }

            if (lnkIniciar == null)
            {
                lnkIniciar = new ResourceLinkButton
                {
                    ResourceName = "Labels",
                    ID = "lnkIniciar",
                    CommandName = "Start",
                    CommandArgument = row.RowIndex.ToString("#0"),
                    VariableName = "DISTRIBUCION_INICIAR",
                    Visible = false
                };
                row.Cells[ViajeDistribucionVo.IndexEstado].Controls.Add(lnkIniciar);
            }

            row.Cells[ViajeDistribucionVo.IndexEstado].Controls.Add(new Literal { Text = @"<br/>" });

            if (lnkAsociar == null)
            {
                lnkAsociar = new ResourceLinkButton
                {
                    ResourceName = "Labels",
                    ID = "lnkAsociar",
                    CommandName = "Asociar",
                    CommandArgument = row.RowIndex.ToString("#0"),
                    VariableName = "ASOCIAR_VEHICULO",
                    Visible = false,
                    OnClientClick = string.Concat("return confirm('", CultureManager.GetString("SystemMessages", "CONFIRM_OPERATION"), "');")
                };

                row.Cells[ViajeDistribucionVo.IndexEstado].Controls.Add(lnkAsociar);
            }

            if (lnkCombinar == null)
            {
                lnkCombinar = new ResourceLinkButton
                {
                    ResourceName = "Labels",
                    ID = "lnkCombinar",
                    CommandName = "Combinar",
                    CommandArgument = row.RowIndex.ToString("#0"),
                    VariableName = "DISTRIBUCION_COMBINAR",
                    Visible = false,
                    OnClientClick = string.Concat("return confirm('", CultureManager.GetString("SystemMessages", "CONFIRM_OPERATION"), "');")
                };

                row.Cells[ViajeDistribucionVo.IndexEstado].Controls.Add(lnkCombinar);
            }

            if (lnkCerrar == null)
            {
                lnkCerrar = new ResourceLinkButton
                                {
                                    ResourceName = "Labels",
                                    ID = "lnkCerrar",
                                    CommandName = "Close",
                                    CommandArgument = row.RowIndex.ToString("#0"),
                                    VariableName = "DISTRIBUCION_CERRAR",
                                    Visible = false,
                                    OnClientClick = string.Concat("return confirm('",CultureManager.GetString("SystemMessages", "CONFIRM_OPERATION"),"');")
                                };

                row.Cells[ViajeDistribucionVo.IndexEstado].Controls.Add(lnkCerrar);
            }

            if (lnkReenviar == null)
            {
                lnkReenviar = new ResourceLinkButton
                {
                    ResourceName = "Labels",
                    ID = "lnkReenviar",
                    CommandName = "Reenviar",
                    CommandArgument = row.RowIndex.ToString("#0"),
                    VariableName = "DISTRIBUCION_REENVIAR",
                    Visible = false,
                    OnClientClick = string.Concat("return confirm('", CultureManager.GetString("SystemMessages", "CONFIRM_OPERATION"), "');")
                };

                row.Cells[ViajeDistribucionVo.IndexEstado].Controls.Add(new Literal { Text = @"<br/>" });
                row.Cells[ViajeDistribucionVo.IndexEstado].Controls.Add(lnkReenviar);
            }

            if (cbVehiculo == null)
            {
                cbVehiculo = new DropDownList
                {
                    ID = "cbVehiculo",
                    Visible = true
                };

                row.Cells[ViajeDistribucionVo.IndexVehiculo].Controls.Add(cbVehiculo);
            }

            cbVehiculo.SelectedIndexChanged += ComboOnSelectedIndexChanged;
            lnkIniciar.Command += LnkbtnEstadoCommand;
            lnkCerrar.Command += LnkbtnEstadoCommand;
            lnkReenviar.Command += LnkbtnEstadoCommand;
            lnkAsociar.Command += LnkbtnEstadoCommand;
            lnkCombinar.Command += LnkbtnEstadoCommand;
        }

        protected override void Edit()
        {
            _editMode = true;
            Bind();
        }

        protected override void Open()
        {
            _editMode = false;
            Bind();
        }

        protected void LnkbtnEstadoCommand(object sender, CommandEventArgs commandEventArgs)
        {
            try
            {
                var index = Convert.ToInt32(commandEventArgs.CommandArgument);
                var id = Convert.ToInt32(Grid.DataKeys[index].Value);
                var distribucion = DAOFactory.ViajeDistribucionDAO.FindById(id);

                switch (commandEventArgs.CommandName)
                {
                    case "Start":
                        StartTicketWindow(distribucion);
                        //new InitEvent(DateTime.UtcNow); 
                        break;
                    case "Close":
                        var ciclo = new CicloLogisticoDistribucion(distribucion, DAOFactory, new MessageSaver(DAOFactory));
                        IEvent evento = new CloseEvent(DateTime.UtcNow); 
                        ciclo.ProcessEvent(evento);
                        STrace.Trace("CierreCicloLogistico", distribucion.Vehiculo.Dispositivo.Id, string.Format("Viaje {0} cerrado manualmente por {1}", distribucion.Id, WebSecurity.AuthenticatedUser.Name));
                        Bind();
                        break;
                    case "Reenviar":
                        try
                        {
                            var destinations = distribucion.Detalles.Where(d => d.PuntoEntrega != null 
                                                              && d.ReferenciaGeografica != null
                                                              && Math.Abs(d.ReferenciaGeografica.Latitude) < 90
                                                              && Math.Abs(d.ReferenciaGeografica.Longitude) < 180
                                                              && d.Estado != EntregaDistribucion.Estados.Completado
                                                              && d.Estado != EntregaDistribucion.Estados.NoCompletado
                                                              && d.Estado != EntregaDistribucion.Estados.Completado)
                                    .Select(d => new Destination(d.Id,
                                                                 new GPSPoint(DateTime.UtcNow,
                                                                              (float) d.ReferenciaGeografica.Latitude,
                                                                              (float) d.ReferenciaGeografica.Longitude),
                                                                 d.Descripcion,
                                                                 d.PuntoEntrega.Descripcion,
                                                                 d.ReferenciaGeografica.Direccion.Descripcion))
                                    .ToArray();

                            if (destinations.Any())
                            {
                                // LOAD ROUTE
                                switch (distribucion.Vehiculo.Empresa.OrdenRutaGarmin)
                                {
                                    case Empresa.OrdenRuta.DescripcionAsc:
                                        destinations = destinations.OrderBy(d => d.Text).ToArray();
                                        break;
                                    case Empresa.OrdenRuta.DescripcionDesc:
                                        destinations = destinations.OrderByDescending(d => d.Text).ToArray();
                                        break;
                                }

                                var expiration = distribucion.Fin.AddMinutes(distribucion.Empresa.EndMarginMinutes);

                                var msg = MessageSender.CreateReloadRoute(distribucion.Vehiculo.Dispositivo, new MessageSaver(DAOFactory))
                                                       .AddRouteId(distribucion.Id)
                                                       .AddDestinations(destinations)
                                                       .AddExpiration(expiration);

                                if (distribucion.Tipo == ViajeDistribucion.Tipos.Desordenado)
                                {
                                    var ordenar = distribucion.Empresa.GetParameter(Empresa.Params.CicloDistribucionOrdenar);
                                    if (ordenar != null && ordenar.ToLower() == "true")
                                        msg.AddParameter("sort", "true");
                                }
                                msg.Send();
                            }
                        }
                        catch (Exception ex)
                        {
                            STrace.Exception("ViajeDistribucion Inicio", ex, distribucion.Vehiculo.Dispositivo.Id);
                            throw;
                        }
                        break;
                    case "Asociar":
                        AsociarVehiculoWindow(distribucion);
                        break;
                    case "Combinar":
                        CombinarViajeWindow(distribucion);
                        break;
                }
            }
            catch (QueueException)
            {
                ShowError("Error de configuracion de Cola de Comandos");  
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        //protected override void StartAll()
        //{
        //    try
        //    {
        //        var list = GetListData();

        //        var anyStarted = list.Aggregate(false, (current, vo) => current | StartDistribucion(vo.Id));

        //        if(anyStarted) Bind();
        //    }
        //    catch (QueueException)
        //    {
        //        ShowError("Error de configuracion de Cola de Comandos");
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowError(ex);
        //    }
        //}
        
        protected void StartTicketWindow(ViajeDistribucion ticket)
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

            var stateList = ticket.Detalles.OfType<EntregaDistribucion>().OrderBy(t => t.Orden).ToList();

            if (stateList.Count == 0)
            {
                ShowResourceError("NO_CLOG_STATES");
                return;
            }

            foreach (EntregaDistribucion entrega in ticket.Detalles)
            {
                var geo = entrega.ReferenciaGeografica;
                if(geo.Latitude == 0 && geo.Longitude == 0)
                {
                    ShowResourceError("INVALID_CLOG_ADDRESS", entrega.PuntoEntrega.Descripcion);
                    return;
                }
            }

            TicketToInitId.Set(ticket.Id);

            var opened = DAOFactory.ViajeDistribucionDAO.FindEnCurso(ticket.Vehiculo);

            if (opened != null)
            {
                lblCodigoTicket.Text = opened.Codigo;
                lblCliente.Text = opened.EntregasTotalCountConBases.ToString("#0");
                lblFecha.Text = opened.Inicio.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
            }

            panelStartTicket.Visible = opened == null;
            panelOpenedTicket.Visible = opened != null;
            panelAsociarVehiculo.Visible = panelCombinarViaje.Visible = false;
            mpePanel.CancelControlID = opened == null ? btCancelar.ID : btOpenedCancelar.ID;

            mpePanel.Show();
        }

        protected void AsociarVehiculoWindow(ViajeDistribucion viaje)
        {
            var empresas = viaje.Empresa != null ? new[] {viaje.Empresa.Id} : new[] {-1};
            var lineas = viaje.Linea != null ? new[] {viaje.Linea.Id} : new[] {-1};
            var centros = viaje.CentroDeCostos != null ? new[] {viaje.CentroDeCostos.Id} : new[] {-1};
            var subCentros = viaje.SubCentroDeCostos != null ? new[] {viaje.SubCentroDeCostos.Id} : new[] {-1};
            var todos = new[] {-1};

            var vehiculos = DAOFactory.CocheDAO.GetList(empresas, lineas, todos, todos, todos, centros, subCentros, true);

            cbVehiculoAsociar.ClearItems();
            foreach (var vehiculo in vehiculos)
            {
                cbVehiculoAsociar.Items.Add(new ListItem(vehiculo.Interno, vehiculo.Id.ToString("#0")));
            }

            mpePanel.CancelControlID = btCancelarAsociacion.ID;
            TicketToInitId.Set(viaje.Id);

            panelOpenedTicket.Visible = panelStartTicket.Visible = panelCombinarViaje.Visible = false;
            panelAsociarVehiculo.Visible = true;
            
            mpePanel.Show();
        }

        protected void CombinarViajeWindow(ViajeDistribucion viaje)
        {
            var todos = new[] { -1 };
            var viajes = DAOFactory.ViajeDistribucionDAO.GetList(cbEmpresa.SelectedValues, cbLinea.SelectedValues, cbTransportista.SelectedValues, cbDepartamento.SelectedValues, cbCentroDeCosto.SelectedValues, todos, todos, new[] {(int)ViajeDistribucion.Estados.Pendiente}, dtpDesde.SelectedDate, dtpHasta.SelectedDate)
                .Where(v => v.Id != viaje.Id)
                .OrderBy(v => v.Codigo);

            cbViaje.Items.Clear();
            foreach (var v in viajes)
            {
                cbViaje.Items.Add(new ListItem(v.Codigo, v.Id.ToString("#0")));
            }

            mpePanel.CancelControlID = btCancelarCombinar.ID;
            TicketToInitId.Set(viaje.Id);

            panelAsociarVehiculo.Visible = panelOpenedTicket.Visible = panelStartTicket.Visible = false;
            panelCombinarViaje.Visible = true;

            mpePanel.Show();
        }
        
        protected void BtOpenedCerrarTicketClick(object sender, EventArgs e)
        {
            if (TicketToInitId.Get() <= 0) return;

            var ticket = DAOFactory.ViajeDistribucionDAO.FindById(TicketToInitId.Get());

            var opened = DAOFactory.ViajeDistribucionDAO.FindEnCurso(ticket.Vehiculo);
            if (opened != null)
            {
                var ciclo = new CicloLogisticoDistribucion(opened, DAOFactory, new MessageSaver(DAOFactory));
                var evento = new CloseEvent(DateTime.UtcNow);
                ciclo.ProcessEvent(evento);
                STrace.Trace("CierreCicloLogistico", ticket.Vehiculo.Dispositivo.Id, "Cierre manual por el usuario: " + WebSecurity.AuthenticatedUser.Name);
            }

            StartTicketWindow(ticket);
        }
        
        protected void BtCancelarClick(object sender, EventArgs e)
        {
            TicketToInitId.Set(-1);
        }
        
        //private bool StartDistribucion(int id)
        //{
        //    var distribucion = DAOFactory.ViajeDistribucionDAO.FindById(id);

        //    if (distribucion.Estado != ViajeDistribucion.Estados.Pendiente) return false;
        //    var ciclo = new CicloLogisticoDistribucion(distribucion, DAOFactory, new MessageSaver(DAOFactory));
        //    IEvent evento = new InitEvent(DateTime.UtcNow);

        //    ciclo.ProcessEvent(evento);
        //    return true;
        //}
        
        protected void BtIniciarClick(object sender, EventArgs e)
        {
            mpePanel.Hide();
            if (TicketToInitId.Get() <= 0) return;


            var ticket = DAOFactory.ViajeDistribucionDAO.FindById(TicketToInitId.Get());
            TicketToInitId.Set(-1);

            var ciclo = new CicloLogisticoDistribucion(ticket, DAOFactory, new MessageSaver(DAOFactory));

            var evento = new InitEvent(DateTime.UtcNow);

            try
            {
                ciclo.ProcessEvent(evento);
                ShowInfo(CultureManager.GetSystemMessage("CLOG_START_SENT") + ticket.Vehiculo.Interno);
                Bind();
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

        protected void BtAceptarAsociacionClick(object sender, EventArgs e)
        {
            mpePanel.Hide();
            if (TicketToInitId.Get() <= 0) return;

            var viaje = DAOFactory.ViajeDistribucionDAO.FindById(TicketToInitId.Get());
            var vehiculo = DAOFactory.CocheDAO.FindById(cbVehiculoAsociar.Selected);
            
            TicketToInitId.Set(-1);

            viaje.Vehiculo = vehiculo;
            DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);
            Bind();
        }

        protected void BtAceptarCombinarClick(object sender, EventArgs e)
        {
            mpePanel.Hide();
            if (TicketToInitId.Get() <= 0) return;

            var viaje = DAOFactory.ViajeDistribucionDAO.FindById(TicketToInitId.Get());
            var viaje2 = DAOFactory.ViajeDistribucionDAO.FindById(Convert.ToInt32((string) cbViaje.SelectedItem.Value));

            TicketToInitId.Set(-1);

            var detalles = viaje2.Detalles.Where(d => d.Linea == null);
            var i = viaje.EntregasTotalCount + 1;

            if (detalles.Any())
            {
                if (viaje.RegresoABase)
                {
                    var llegada = viaje.Detalles.OrderBy(d => d.Orden).Last();
                    llegada.Orden += detalles.Count();
                    DAOFactory.EntregaDistribucionDAO.SaveOrUpdate(llegada);
                }

                foreach (var detalle in detalles)
                {
                    detalle.Orden = i++;
                    detalle.Viaje = viaje;
                    viaje.Detalles.Add(detalle);
                    DAOFactory.EntregaDistribucionDAO.SaveOrUpdate(detalle);
                }
                DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);

                viaje2.Estado = ViajeDistribucion.Estados.Eliminado;
                DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje2);
            }

            Bind();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadLocalFilter((string) "DESDE", (DateTimePicker) dtpDesde);
            data.LoadLocalFilter((string) "HASTA", (DateTimePicker) dtpHasta);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.Add("DESDE", dtpDesde.SelectedDate);
            data.Add("HASTA", dtpHasta.SelectedDate);
            return data;
        }

        #endregion
    }
}
