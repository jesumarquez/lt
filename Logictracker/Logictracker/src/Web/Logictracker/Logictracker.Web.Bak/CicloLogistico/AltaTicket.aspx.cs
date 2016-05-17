using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Process.CicloLogistico;
using Logictracker.Services.Helpers;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web;
using Logictracker.Types.BusinessObjects;
using Logictracker.Culture;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico
{
    public partial class AltaTicket : SecuredAbmPage<Ticket>
    {
        protected override string VariableName { get { return "CLOG_TICKETS"; } }
        protected override string RedirectUrl { get { return "ListTicket.aspx"; } }
        protected override string GetRefference() { return "TICKET"; }

        protected override bool MapButton { get { return EditMode; } }
        protected override bool EventButton { get { return EditMode; } }
        protected override bool SplitButton { get { return EditMode && EditObject.Estado != Ticket.Estados.Anulado; } }
        protected override bool RegenerateButton { get { return EditMode && EditObject.Estado != Ticket.Estados.Anulado; } }
        protected override bool AnularButton { get { return EditMode && EditObject.Estado != Ticket.Estados.Anulado; } }
        protected override bool SaveButton { get { return !EditMode || EditObject.Estado != Ticket.Estados.Anulado; } }

        private VsProperty<List<TimeSpan>> DeltaTime { get { return this.CreateVsProperty("DeltaTime", new List<TimeSpan>()); } } 
        private VsProperty<DateTime> StartDate { get { return this.CreateVsProperty("StartDate", DateTime.UtcNow.ToDisplayDateTime()); } }
 
        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (EditMode && EditObject.Estado == Ticket.Estados.Anulado)
            {
                var text = CultureManager.GetSystemMessage("TICKET_ANULADO_USER_DAY_REASON");
                var user = EditObject.UsuarioAnulacion != null ? EditObject.UsuarioAnulacion.NombreUsuario : " ";
                var fecha = EditObject.FechaAnulacion.HasValue ? EditObject.FechaAnulacion.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : " ";
                var motivo = EditObject.MotivoAnulacion;
                ShowInfo(string.Format(text, string.Concat("<b>", user, "</b>"), string.Concat("<b>", fecha, "</b>"), string.Concat("<b>", motivo, "</b>")));
            }

            if (IsPostBack) return;

            if (!EditMode) dtFecha.SetDate();

            BindEstados();

            lblOrdenDiarioText.Visible = lblOrdenDiario.Visible = EditMode;
        }

        protected override void Bind()
        {
            if (EditObject.Linea != null)
            {
                cbEmpresa.SetSelectedValue(EditObject.Empresa != null 
                                                ? EditObject.Empresa.Id 
                                                : EditObject.Linea.Empresa.Id);
                cbLinea.SetSelectedValue(EditObject.Linea.Id);
            }

            if (EditObject.Cliente != null) cbCliente.SetSelectedValue(EditObject.Cliente.Id);
            if (EditObject.Vehiculo != null) cbMovil.SetSelectedValue(EditObject.Vehiculo.Id);
            if (EditObject.Empleado != null) cbChofer.SetSelectedValue(EditObject.Empleado.Id);
            if (EditObject.PuntoEntrega != null) cbPuntoEntrega.SetSelectedValue(EditObject.PuntoEntrega.Id);
            if (EditObject.Vehiculo != null && EditObject.Vehiculo.Transportista != null) cbTransportista.SetSelectedValue(EditObject.Vehiculo.Transportista.Id);
            cbBaseDestino.SetSelectedValue(EditObject.BaseDestino != null ? EditObject.BaseDestino.Id : cbBaseDestino.AllValue);

            if (EditObject.FechaTicket.HasValue) dtFecha.SelectedDate = EditObject.FechaTicket.Value.ToDisplayDateTime();

            txtCodigo.Text = EditObject.Codigo;

            txtCodigoProducto.Text = EditObject.CodigoProducto;
            txtDescripcionProducto.Text = EditObject.DescripcionProducto;

            txtCantidadCarga.Text = EditObject.CantidadCarga;
            txtCantidadCargaReal.Text = EditObject.CantidadCargaReal;
            txtCantidadPedido.Text = EditObject.CantidadPedido;
            txtCantidadAcumulada.Text = EditObject.CumulativeQty;
            txtUnidad.Text = EditObject.Unidad;

            txtComentario1.Text = EditObject.UserField1;
            txtComentario2.Text = EditObject.UserField2;
            txtComentario3.Text = EditObject.UserField3;

            lblOrdenDiario.Text = EditObject.OrdenDiario.ToString();
        }

        protected override void OnDelete()
        {
            DAOFactory.TicketDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Cliente = DAOFactory.ClienteDAO.FindById(cbCliente.Selected);

            EditObject.Vehiculo = cbMovil.Selected > 0 ? DAOFactory.CocheDAO.FindById(cbMovil.Selected) : null;
            EditObject.Dispositivo = EditObject.Vehiculo != null ? EditObject.Vehiculo.Dispositivo : null;
            EditObject.Empleado = cbChofer.Selected > 0 ? DAOFactory.EmpleadoDAO.FindById(cbChofer.Selected) : null;
            EditObject.BaseDestino = cbBaseDestino.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbBaseDestino.Selected) : null;

            EditObject.Codigo = txtCodigo.Text;
            EditObject.FechaTicket = dtFecha.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();

            EditObject.CodigoProducto = txtCodigoProducto.Text;
            EditObject.DescripcionProducto = txtDescripcionProducto.Text;

            EditObject.CantidadCarga = txtCantidadCarga.Text;
            EditObject.CantidadCargaReal = "0";
            EditObject.CantidadPedido = txtCantidadPedido.Text;
            EditObject.CumulativeQty = txtCantidadAcumulada.Text;
            EditObject.Unidad = txtUnidad.Text;

            EditObject.UserField1 = txtComentario1.Text;
            EditObject.UserField2 = txtComentario2.Text;
            EditObject.UserField3 = txtComentario3.Text;

            EditObject.PuntoEntrega = DAOFactory.PuntoEntregaDAO.FindById(cbPuntoEntrega.Selected);

            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;

            if (!EditMode)
            {
                EditObject.EstadoLogistico = null;
                EditObject.Estado = 0;

                // Valores en null
                EditObject.FechaDescarga = null;
                EditObject.FechaFin = null;

                // Orden Diario
                EditObject.OrdenDiario = DAOFactory.TicketDAO.FindNextOrdenDiario(EditObject.Linea != null ? EditObject.Linea.Empresa.Id : -1, EditObject.Linea != null ? EditObject.Linea.Id : -1, EditObject.FechaTicket.Value);
            }

            // Valores default
            EditObject.SourceFile = "LOGICTRACKER";
            EditObject.SourceStation = "LOGICTRACKER";

            //Detalles
            var dic = EditObject.Detalles.Cast<DetalleTicket>().ToDictionary(detalle => detalle.EstadoLogistico.Id);

            EditObject.Detalles.Clear();

            var previous = EditObject.FechaTicket.Value.ToDisplayDateTime();
            var first = true;
            foreach (C1GridViewRow item in gridEstados.Rows)
            {
                var dataKey = gridEstados.DataKeys[item.DataItemIndex];

                if (dataKey == null) continue;

                var id = Convert.ToInt32(dataKey.Value);
                var chk = item.FindControl("chkIncluirEstado") as CheckBox;
                var includeEstado = chk != null && chk.Checked;
                var est = DAOFactory.EstadoDAO.FindById(id);

                if (!includeEstado) continue;

                var tic = dic.ContainsKey(id) ? dic[id] : new DetalleTicket { EstadoLogistico = est, Ticket = EditObject };

                var txt = item.FindControl("txtHoraEstado") as TextBox;

                if (txt != null)
                {
                    previous = GetHours(txt.Text, previous);
                    tic.Programado = previous.ToDataBaseDateTime();
                }

                EditObject.Detalles.Add(tic);

                if (!first) continue;

                if (tic.Programado != null) EditObject.FechaTicket = tic.Programado.Value;

                first = false;
            }

            DAOFactory.TicketDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbPuntoEntrega.Selected, "PARENTI44");

            ValidateDouble(txtCantidadCarga.Text, "CANTIDAD_CARGA");
            ValidateDouble(txtCantidadPedido.Text, "CANTIDAD_PEDIDO");
            ValidateDouble(txtCantidadAcumulada.Text, "CANTIDAD_ACUMULADA");

            var code = ValidateEmpty(txtCodigo.Text, "CODE");

            var byCode = DAOFactory.TicketDAO.FindByCode(new[]{cbEmpresa.Selected}, new[]{cbLinea.Selected}, code);
            ValidateDuplicated(byCode, "CODE");

            if (!ValidateDocuments()) throw new ApplicationException("Verificar Documentación");
        }

        protected void owner_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsPostBack) return;

            BindEstados();

            SelectChoferFromMovil();

            if (cbBaseDestino.Selected != cbBaseDestino.AllValue)
                cbBaseDestino.SetSelectedValue(cbLinea.Selected);
        }

        protected void cbPuntoEntrega_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecalcularHoras(0);
        }

        protected void txtHoraEstado_Click(object sender, EventArgs e)
        {
            var index = gridEstados.Rows.Cast<C1GridViewRow>().Select((r, i) => new {row = r, index = i}).Where(
                    i => i.row.FindControl("txtHoraEstado") == sender).Select(r => r.index).FirstOrDefault();
            RecalcularHoras(index); 
        }

        protected void cbMovil_SelectedIndexChanged(object sender, EventArgs e) { SelectChoferFromMovil(); }

        protected override void Cancel()
        {
            ModalPopupExtender1.Show();
            updAnular.Update();
        }

        protected void btAnular_Click(object sender, EventArgs e)
        {
            ModalPopupExtender1.Hide();
            updAnular.Update();
            ValidateEmpty(txtMotivo.Text, "MOTIVO");
            EditObject.Anular(txtMotivo.Text, DAOFactory.UsuarioDAO.FindById(Usuario.Id));
            DAOFactory.TicketDAO.SaveOrUpdate(EditObject);
            Open();
        }
        protected override void OnDuplicate()
        {
            EditObject.Estado = Ticket.Estados.Pendiente;
            //UpdateToolbar();
        }
        #endregion

        #region Private Methods

        private List<TimeSpan> GetDeltas()
        {
            var delta = DeltaTime.Get();
            var previous = dtFecha.SelectedDate;


            StartDate.Set(previous.GetValueOrDefault().Add(StartDate.Get().TimeOfDay));

            foreach (C1GridViewRow item in gridEstados.Rows)
            {
                var txt = item.FindControl("txtHoraEstado") as TextBox;

                if (txt != null)
                {
                    var time = GetHours(txt.Text, previous.GetValueOrDefault());
                    var ts = time.Subtract(previous.GetValueOrDefault());

                    if (item.RowIndex == 0) StartDate.Set(time);

                    previous = time;

                    if (ts == delta[item.RowIndex]) continue;

                    delta[item.RowIndex] = ts;
                }

                break;
            }

            return delta;
        }

        private void SelectChoferFromMovil()
        {
            if (cbMovil.Selected <= 0) return;

            var movil = DAOFactory.CocheDAO.FindById(cbMovil.Selected);

            if (movil.Chofer == null) return;

            cbChofer.SetSelectedValue(movil.Chofer.Id);
        }

        private void RecalcularHoras(int index)
        {
            var delta = GetDeltas();
            DeltaTime.Set(delta);

            var d = StartDate.Get();
            var saleDePlanta = DateTime.MinValue;
            var saleDeObra = DateTime.MinValue;

            var coche = cbMovil.Selected > 0 ? DAOFactory.CocheDAO.FindById(cbMovil.Selected) : null;
            var velocidadPromedio = coche != null ? (coche.VelocidadPromedio > 0 ? coche.VelocidadPromedio
                : (coche.TipoCoche.VelocidadPromedio > 0 ? coche.TipoCoche.VelocidadPromedio : 20 ))
                                        : 20;

            var direccionLinea = GetDireccionLinea();
            var direccionMapa = GetDireccionPuntoDeEntrega();

            foreach (C1GridViewRow item in gridEstados.Rows)
            {
                var estadoId = (int)gridEstados.DataKeys[item.RowIndex].Value;
                var estado = DAOFactory.EstadoDAO.FindById(estadoId);

                if (estado.EsPuntoDeControl == 5) saleDePlanta = d; //Sale de Planta
                if (estado.EsPuntoDeControl == 7) saleDeObra = d; //Sale de Obra

                //if (item.RowIndex == 0) continue;
                if (item.RowIndex <= index)
                {
                    var txt = item.FindControl("txtHoraEstado") as TextBox;
                    if (txt != null)
                    {
                        d = GetHours(txt.Text, d);
                    }
                    continue;
                }

                if (estado.EsPuntoDeControl == 6 && saleDePlanta != DateTime.MinValue)
                {
                    //Llega a Obra
                    var distancia = direccionLinea != null && direccionMapa != null ?
                        GeocoderHelper.CalcularDistacia(direccionLinea.Latitude, direccionLinea.Longitude, direccionMapa.Latitude, direccionMapa.Longitude)
                        : 0;

                    var horas = distancia / velocidadPromedio;

                    d = d.Add(horas > 0 ? TimeSpan.FromHours(horas) : delta[item.RowIndex]);

                    var txt = item.FindControl("txtHoraEstado") as TextBox;
                    var lbl = item.FindControl("lblDiaProgramado") as Label;
                    if (txt != null) txt.Text = d.ToString("HH:mm");
                    if (lbl != null) lbl.Text = d.ToString("dd-MM-yyyy");

                }
                else if (estado.EsPuntoDeControl == 8 && saleDeObra != DateTime.MinValue)
                {
                    ////Llega a Planta

                    var distancia = direccionLinea != null && direccionMapa != null ?
                        GeocoderHelper.CalcularDistacia(direccionMapa.Latitude, direccionMapa.Longitude, direccionLinea.Latitude, direccionLinea.Longitude)
                        : 0;

                    var horas = distancia / velocidadPromedio;

                    d = d.Add(horas > 0 ? TimeSpan.FromHours(horas) : delta[item.RowIndex]);

                    var txt = item.FindControl("txtHoraEstado") as TextBox;
                    var lbl = item.FindControl("lblDiaProgramado") as Label;
                    if (txt != null) txt.Text = d.ToString("HH:mm");
                    if (lbl != null) lbl.Text = d.ToString("dd-MM-yyyy");
                }
                else
                {
                    //Normal
                    d = d.Add(delta[item.RowIndex]);

                    var txt = item.FindControl("txtHoraEstado") as TextBox;
                    var lbl = item.FindControl("lblDiaProgramado") as Label;
                    if (txt != null) txt.Text = d.ToString("HH:mm");
                    if (lbl != null) lbl.Text = d.ToString("dd-MM-yyyy");
                }
            }

            updGridEstados.Update();
        }

        private ReferenciaGeografica GetDireccionPuntoDeEntrega()
        {
            var referencia = cbPuntoEntrega.Selected > 0 ? DAOFactory.PuntoEntregaDAO.FindById(cbPuntoEntrega.Selected).ReferenciaGeografica : null;
            referencia = referencia ?? (EditObject != null && EditObject.PuntoEntrega != null ? EditObject.PuntoEntrega.ReferenciaGeografica : null);
            return referencia;
        }

        private ReferenciaGeografica GetDireccionLinea()
        {
            if (cbLinea.Selected <= 0) return null;
            var linea = DAOFactory.LineaDAO.FindById(cbLinea.Selected);
            return linea.ReferenciaGeografica;
        }

        private static DateTime GetHours(string hour, DateTime previous)
        {
            if (string.IsNullOrEmpty(hour)) return previous;

            var parts = hour.Trim().Split(':');

            if (parts.Length != 2) return previous;

            int hh, mm;

            if (!int.TryParse(parts[0], out hh) || !int.TryParse(parts[1], out mm)) return previous;
            if (hh < 0 || hh > 23 || mm < 0 || mm > 59) return previous;

            var dt = new DateTime(previous.Year, previous.Month, previous.Day, hh, mm, 0);

            //no puede ser menor a la anterior
            while (dt < previous) dt = dt.AddDays(1);

            return dt;
        }

        private void BindEstados()
        {
            var estados = DAOFactory.EstadoDAO.GetByPlanta(cbLinea.Selected);
            gridEstados.DataSource = estados;
            gridEstados.DataBind();

            var delta = DeltaTime.Get();
            delta.AddRange(from Estado estado in estados select TimeSpan.FromMinutes(estado.Deltatime));

            if (delta.Count.Equals(0)) return;

            delta[0] = TimeSpan.Zero;

            DeltaTime.Set(delta);

            if (gridEstados.Rows.Count > 0) if (gridEstados != null) ((TextBox)gridEstados.Rows[0].FindControl("txtHoraEstado")).Text = StartDate.Get().ToString("HH:mm");

            if (EditMode)
            {
                var dic = GetDiccionarioEstados();

                if (gridEstados != null)
                    foreach (C1GridViewRow item in gridEstados.Rows)
                    {
                        var id = Convert.ToInt32(gridEstados.DataKeys[item.RowIndex].Value);

                        if (!dic.ContainsKey(id)) continue;

                        var est = dic[id];
                        var chk = item.FindControl("chkIncluirEstado") as CheckBox;
                        var txt = item.FindControl("txtHoraEstado") as TextBox;
                        var lbl = item.FindControl("lblDiaProgramado") as Label;

                        if (est.Programado == null) continue;

                        var programado = est.Programado.Value.ToDisplayDateTime();

                        if (chk != null) chk.Checked = true;
                        if (txt != null) txt.Text = programado.ToString("HH:mm");
                        if (lbl != null) lbl.Text = programado.ToString("dd-MM-yyyy");
                        if (est.Manual.HasValue)
                        {
                            var manual = est.Manual.Value.ToDisplayDateTime();

                            item.Cells[3].Text = string.Format("{0} ({1}m)", manual.ToString("dd-MM-yyyy HH:mm"), programado.Subtract(manual).TotalMinutes.ToString("0"));
                        }

                        if (!est.Automatico.HasValue) continue;

                        var auto = est.Automatico.Value.ToDisplayDateTime();

                        item.Cells[4].Text = string.Format("{0} ({1}m)", auto.ToString("dd-MM-yyyy HH:mm"), programado.Subtract(auto).TotalMinutes.ToString("0"));
                    }
            }

            if (!EditMode) RecalcularHoras(0);
        }

        private Dictionary<int, DetalleTicket> GetDiccionarioEstados()
        {
            return EditObject.Detalles.Cast<DetalleTicket>()
                .ToDictionary(detalle => detalle.EstadoLogistico.Id);
        }

        #endregion

        #region Actions (ViewMap, ViewEvent, Split, Regenerate)

        protected override void ViewMap()
        {
            if (EditObject.Detalles.Count == 0)
            {
                ShowInfo("No hay datos para mostrar");
                return;
            }

            var all = EditObject.Detalles.OfType<DetalleTicket>().Where(d => d.Manual.HasValue).Select(d => d.Manual.Value)
                .Union(EditObject.Detalles.OfType<DetalleTicket>().Where(d => d.Automatico.HasValue).Select(d => d.Automatico.Value))
                .Union(EditObject.Detalles.OfType<DetalleTicket>().Select(d => d.Programado.Value));
            var start = all.Min();

            if (start > DateTime.UtcNow)
            {
                ShowInfo("No hay datos para mostrar");
                return;
            }

            OpenWin(ResolveUrl(UrlMaker.MonitorLogistico.GetUrlHormigon(EditObject.Id)), "_blank");
        }

        protected override void ViewEvent()
        {
            if (EditObject.Vehiculo == null)
            {
                ShowInfo("El ticket no tiene un vehiculo asignado");
                return;
            }
            if (EditObject.Detalles.Count == 0)
            {
                ShowInfo("No hay datos para mostrar");
                return;
            }

//            var all = EditObject.Detalles.OfType<DetalleTicket>().Where(d => d.Manual.HasValue).Select(d => d.Manual.Value)
  //              .Union(EditObject.Detalles.OfType<DetalleTicket>().Where(d => d.Automatico.HasValue).Select(d => d.Automatico.Value))
    //            .Union(EditObject.Detalles.OfType<DetalleTicket>().Select(d => d.Programado.Value));
            var detalleInicio = EditObject.Detalles.OfType<DetalleTicket>().FirstOrDefault();
            var detalleFinal = EditObject.Detalles.OfType<DetalleTicket>().LastOrDefault();

            if (detalleInicio == null || detalleFinal == null) return;
            
            var start = detalleInicio.Automatico.HasValue
                            ? detalleInicio.Automatico.Value
                            : detalleInicio.Manual.HasValue
                                  ? detalleInicio.Manual.Value
                                  : detalleInicio.Programado.HasValue
                                        ? detalleInicio.Programado.Value
                                        : DateTime.UtcNow.AddDays(1);
            var end = detalleFinal.Automatico.HasValue
                          ? detalleFinal.Automatico.Value
                          : detalleFinal.Manual.HasValue
                                ? detalleFinal.Manual.Value
                                : detalleFinal.Programado.HasValue
                                      ? detalleFinal.Programado.Value
                                      : DateTime.UtcNow.AddDays(1);


            if (start > DateTime.UtcNow)
            {
                ShowInfo("No hay datos para mostrar");
                return;
            }

            var mobile = EditObject.Vehiculo;

            Session.Add("EventsLocation", mobile.Empresa != null ? mobile.Empresa.Id : mobile.Linea != null ? mobile.Linea.Empresa.Id : -1);
            Session.Add("EventsCompany", mobile.Linea != null ? mobile.Linea.Id : -1);
            Session.Add("EventsMobileType", mobile.TipoCoche.Id);
            Session.Add("EventsMobile", mobile.Id);
            Session.Add("EventsFrom", start.AddMinutes(-1).ToDisplayDateTime());
            Session.Add("EventsTo", end.AddMinutes(1).ToDisplayDateTime());

            OpenWin(ResolveUrl("~/Reportes/DatosOperativos/eventos.aspx"), "Reporte de Eventos");
        }

        protected override void Split()
        {
            double d;
            var cantPedido = ValidateDouble(txtCantidadPedido.Text, "CANTIDAD_PEDIDO");
            var cantCarga = ValidateDouble(txtCantidadCarga.Text, "CANTIDAD_CARGA");
            var cantAcumulada = double.TryParse(txtCantidadAcumulada.Text.Trim(), out d) ? d : 0;

            if (cantAcumulada == 0)
            {
                cantAcumulada = cantCarga;
                txtCantidadAcumulada.Text = cantCarga.ToString();
            }

            if (cantPedido <= cantAcumulada) return;

            var oldCode = EditObject.Codigo;
            txtCodigo.Text = string.Format("{0}-01", EditObject.Codigo);

            ValidateSave();
            Save();

            var i = 2;
            var c = cantAcumulada + cantCarga;
            cantAcumulada = cantAcumulada + cantCarga < cantPedido ? cantAcumulada + cantCarga : cantPedido;
            while (c - cantCarga < cantPedido)
            {
                CloneActualTicket(cantAcumulada, oldCode, i);
                cantAcumulada = cantAcumulada + cantCarga < cantPedido ? cantAcumulada + cantCarga : cantPedido;
                c = c + cantCarga;
                i++;
            }
        }

        protected override void Regenerate()
        {
            if (EditObject.Vehiculo == null) ThrowMustEnter("Entities", "PARENTI03");

            var ticket = DAOFactory.TicketDAO.FindById(EditObject.Id);
            var ciclo = new CicloLogisticoHormigon(ticket, DAOFactory, null/*new MessageSaver(DAOFactory)*/);

            ciclo.Regenerate();

            ShowInfo(CultureManager.GetSystemMessage("TCIKET_REGENERATE_DONE"));
            Bind();
            updRefreshTabGeneral.Update();
            updRefreshTabDetalles.Update();
        }

        #endregion

        #region Private Methods

        private void CloneActualTicket(double cantAcumulada, string originalCode, int ticketNumber)
        {
            var newTicket = CloneTicket(EditObject, originalCode);

            if (newTicket == null) return;

            newTicket.Codigo = string.Format("{0}-{1}", newTicket.Codigo, ticketNumber.ToString().PadLeft(2, '0'));
            newTicket.CumulativeQty = String.Format("{0:0.00}", cantAcumulada);
            newTicket.PuntoEntrega = DAOFactory.PuntoEntregaDAO.FindById(EditObject.PuntoEntrega.Id);

            DAOFactory.TicketDAO.SaveOrUpdate(newTicket);
        }

        #region Ticket Clone

        public Ticket CloneTicket(Ticket t, string originalCode)
        {
            var ticket = new Ticket
            {
                CantidadCarga = t.CantidadCarga,
                CantidadPedido = t.CantidadPedido,
                Empleado = t.Empleado,
                Cliente = t.Cliente,
                Vehiculo = null, //el coche no se duplica. (#1499)
                Codigo = originalCode,
                CodigoProducto = t.CodigoProducto,
                CumulativeQty = t.CumulativeQty,
                DescripcionProducto = t.DescripcionProducto,
                Dispositivo = t.Dispositivo,
                Estado = t.Estado,
                EstadoLogistico = t.EstadoLogistico,
                FechaDescarga = t.FechaDescarga,
                FechaFin = t.FechaFin,
                FechaTicket = t.FechaTicket,
                Linea = t.Linea,
                BaseDestino = t.Linea,
                //ReferenciaGeografica = t.ReferenciaGeografica,
                Id = 0, /*its a new Ticket so Id=0*/
                SourceFile = t.SourceFile,
                SourceStation = t.SourceStation,
                Unidad = t.Unidad,
                UserField1 = t.UserField1,
                UserField2 = t.UserField2,
                UserField3 = t.UserField3,
                OrdenDiario = DAOFactory.TicketDAO.FindNextOrdenDiario(t.Linea != null ? t.Linea.Empresa.Id : -1, t.Linea != null ? t.Linea.Id : -1, t.FechaTicket.Value)
            };

            var detalles = (from DetalleTicket d in t.Detalles
                            select new DetalleTicket
                                {
                                    Id = 0,
                                    Automatico = d.Automatico,
                                    EstadoLogistico = d.EstadoLogistico,
                                    Manual = d.Manual,
                                    Programado = d.Programado,
                                    Ticket = ticket,
                                }).ToList();

            foreach (var detalleTicket in detalles)
            {
                ticket.Detalles.Add(detalleTicket);
            }
            return ticket;
        }

        #endregion

        private bool ValidateDocuments()
        {
            if (NoValidarDocumentos) return true;

            cbDocumentosVencidos.Items.Clear();

            var tiposVehiculoSinPresentar = new List<TipoDocumento>();
            var tiposEmpleadoSinPresentar = new List<TipoDocumento>();
            var tiposVehiculoVencidos = new List<TipoDocumento>();
            var tiposEmpleadoVencidos = new List<TipoDocumento>();

            
            if (cbMovil.Selected > 0)
            {
                var coche = DAOFactory.CocheDAO.FindById(cbMovil.Selected);
                var empresa = coche.Empresa != null ? coche.Empresa.Id : -1;
                var linea = coche.Linea != null ? coche.Linea.Id : -1;
                var tiposVehiculo = DAOFactory.TipoDocumentoDAO.FindObligatorioVehiculo(empresa, linea);
                foreach (var tipo in tiposVehiculo)
                {
                    var docs = DAOFactory.DocumentoDAO.FindForVehiculo(tipo.Id, coche.Id);
                    if (docs.Count == 0 || !docs.Any(d => d.Presentacion.HasValue))
                    {
                        tiposVehiculoSinPresentar.Add(tipo);
                    }
                    else if (!docs.Any(d => d.Presentacion.HasValue && (!d.Vencimiento.HasValue || d.Vencimiento.Value > dtFecha.SelectedDate.Value)))
                    {
                        tiposVehiculoVencidos.Add(tipo);
                    }
                } 
            }
            
            if (cbChofer.Selected > 0)
            {
                var chofer = DAOFactory.EmpleadoDAO.FindById(cbChofer.Selected);
                var empresa = chofer.Empresa != null ? chofer.Empresa.Id : -1;
                var linea = chofer.Linea != null ? chofer.Linea.Id : -1;
                var tiposEmpleado = DAOFactory.TipoDocumentoDAO.FindObligatorioEmpleado(empresa, linea);
                foreach(var tipo in tiposEmpleado)
                {
                    var docs = DAOFactory.DocumentoDAO.FindForEmpleado(tipo.Id, chofer.Id);
                    if(docs.Count == 0 || !docs.Any(d=>d.Presentacion.HasValue))
                    {
                        tiposEmpleadoSinPresentar.Add(tipo);
                    }
                    if(!docs.Any(d=>d.Presentacion.HasValue && (!d.Vencimiento.HasValue || d.Vencimiento.Value > dtFecha.SelectedDate.Value)))
                    {
                        tiposEmpleadoVencidos.Add(tipo);
                    }
                }
            }

            if(tiposVehiculoSinPresentar.Count > 0 || tiposEmpleadoSinPresentar.Count > 0 ||
                tiposVehiculoVencidos.Count > 0 || tiposEmpleadoVencidos.Count > 0 )
            {
                foreach (var tipo in tiposVehiculoSinPresentar)
                    cbDocumentosVencidos.Items.Add(string.Concat(tipo.Nombre,"(",CultureManager.GetEntity("PARENTI03"),")"));
                foreach (var tipo in tiposVehiculoVencidos)
                    cbDocumentosVencidos.Items.Add(string.Concat(tipo.Nombre, "(", CultureManager.GetEntity("PARENTI03"), ")"));
                foreach (var tipo in tiposEmpleadoSinPresentar)
                    cbDocumentosVencidos.Items.Add(string.Concat(tipo.Nombre, "(", CultureManager.GetEntity("PARENTI09"), ")"));                
                foreach (var tipo in tiposEmpleadoVencidos)
                    cbDocumentosVencidos.Items.Add(string.Concat(tipo.Nombre, "(", CultureManager.GetEntity("PARENTI09"), ")"));

                ModalPopupExtenderDocumentos.Show();
                updDocumentos.Update();
                return false;
            }

            return true;
        }

        protected bool NoValidarDocumentos;
        protected void btAceptarDocumentos_Click(object sender, EventArgs e)
        {
            NoValidarDocumentos = true;
            Save();
        }

        #endregion

    }
}
