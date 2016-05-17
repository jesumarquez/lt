using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.ValueObjects.CicloLogistico.Hormigon;
using Logictracker.Security;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.CicloLogistico.Hormigon
{
    public partial class ConsolaTickets : SecuredListPage<ConsolaTicketsVo>
    {
        protected override string RedirectUrl { get { return "ConsolaTickets.aspx"; } }
        protected override string VariableName { get { return GetRefference(); } }
        protected override string GetRefference() { return "CLOG_CONSOLA_TICKETS"; }
        protected override bool AddButton { get { return false; } }
        protected override bool CsvButton { get { return false; } }
        protected VsProperty<int> IdPedido { get { return this.CreateVsProperty("IdPedido", 0); } } 

        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                dtDesde.SelectedDate = DateTime.Today;
                dtHasta.SelectedDate = DateTime.Today.AddHours(23).AddMinutes(59);
            }
            base.OnPreLoad(e);
        }

        protected override List<ConsolaTicketsVo> GetListData()
        {
            return DAOFactory.PedidoDAO.GetList(new[] { cbEmpresa.Selected },
                                                new[] { cbLinea.Selected },
                                                new[] { cbCliente.Selected },
                                                new[] { cbPuntoEntrega.Selected },
                                                new[] { cbBocaDeCarga.Selected },
                                                new[] { cbEstado.Selected },
                                                new[] { cbProducto.Selected },
                                                dtDesde.SelectedDate,
                                                dtHasta.SelectedDate)
                                       .Select(p => new ConsolaTicketsVo(p))
                                       .ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ConsolaTicketsVo dataItem)
        {
            var cellTickets = GridUtils.GetCell(e.Row, ConsolaTicketsVo.Index.Tickets);
            var cellM3 = GridUtils.GetCell(e.Row, ConsolaTicketsVo.Index.M3);

            var tickets = DAOFactory.TicketDAO.GetByPedido(dataItem.Id);
            var total = tickets.Count();
            var pendientes = tickets.Count(x => x.Estado == Ticket.Estados.Pendiente);
            cellTickets.Text = string.Format("<strong>{0}</strong> ({1} pendientes)", total, pendientes);

            double i;
            var m3total = double.TryParse(dataItem.Cantidad.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out i) ? i : 0;
            var m3Pendiente = m3total - tickets
                .Where(x => double.TryParse(x.CantidadCargaReal.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out i))
                .Sum(x => Convert.ToDouble(x.CantidadCargaReal.Replace(',', '.'), CultureInfo.InvariantCulture));

            cellM3.Text = string.Format("<strong>{0:0.0}</strong> ({1:0.0} pendientes)", dataItem.Cantidad, m3Pendiente);

            CreateRowTemplate(e.Row);
        }

        protected override void OnRowCommand(C1GridView grid, C1GridViewCommandEventArgs e)
        {
            if(e.CommandName == "TicketAjuste")
            {
                var id = Convert.ToInt32(Grid.DataKeys[e.Row.RowIndex].Value);
                IdPedido.Set(id);
                var cellCodigo = GridUtils.GetCell(e.Row, ConsolaTicketsVo.Index.Codigo);
                var cellTickets = GridUtils.GetCell(e.Row, ConsolaTicketsVo.Index.Tickets);
                var cellM3 = GridUtils.GetCell(e.Row, ConsolaTicketsVo.Index.M3);
                var cellCliente = GridUtils.GetCell(e.Row, ConsolaTicketsVo.Index.Cliente);
                var cellPunto = GridUtils.GetCell(e.Row, ConsolaTicketsVo.Index.PuntoEntrega);

                litAjustePedido.Text = cellCodigo.Text;
                lblAjusteCliente.Text = cellCliente.Text;
                lblAjustePuntoEntrega.Text = cellPunto.Text;
                lblAjusteTickets.Text = cellTickets.Text;
                lblAjusteM3.Text = cellM3.Text;
                dtTicketAjuste.SelectedDate = DateTime.UtcNow.ToDisplayDateTime();
                modalPanel.Show();
            }
        }

        protected override void CreateRowTemplate(C1GridViewRow row)
        {
            var cellAjuste = GridUtils.GetCell(row, ConsolaTicketsVo.Index.Ajuste);
            var lnkAjuste = cellAjuste.FindControl("lnkAjuste") as ResourceLinkButton;

            if (lnkAjuste == null)
            {
                lnkAjuste = new ResourceLinkButton
                {
                    ResourceName = "Labels",
                    ID = "lnkAjuste",
                    CommandName = "TicketAjuste",
                    VariableName = "Crear Ticket Ajuste",
                    CausesValidation = false
                };
                cellAjuste.Controls.Add(lnkAjuste);
            }
        }


        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            var cliente = data[FilterData.StaticCliente];
            var puntoEntrega = data[FilterData.StaticPuntoEntrega];
            var producto = data[FilterData.StaticProducto];
            var desde = data["desde"];
            var hasta = data["hasta"];
            var estado = data["estado"];
            if (empresa != null) cbEmpresa.SetSelectedValue((int)empresa);
            if (linea != null) cbLinea.SetSelectedValue((int)linea);
            if (cliente != null) cbCliente.SetSelectedValue((int)cliente);
            if (producto!= null) cbProducto.SetSelectedValue((int)producto);
            if (puntoEntrega != null) cbPuntoEntrega.SetSelectedValue((int)puntoEntrega);
            if (desde != null) dtDesde.SelectedDate = (DateTime?) desde;
            if (hasta != null) dtHasta.SelectedDate = (DateTime?)hasta;
            if (estado != null) cbEstado.SetSelectedValue((int)estado);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticCliente, cbCliente.Selected);
            data.AddStatic(FilterData.StaticPuntoEntrega, cbPuntoEntrega.Selected);
            data.AddStatic(FilterData.StaticProducto, cbProducto.Selected);
            data.Add("desde", dtDesde.SelectedDate);
            data.Add("hasta", dtHasta.SelectedDate);
            data.Add("estado", cbEstado.Selected);
            return data;
        }

        #endregion

        protected void btAceptarAjuste_Click(object sender, EventArgs e)
        {
            var id = IdPedido.Get();
            if(id <= 0) return;
            var pedido = DAOFactory.PedidoDAO.FindById(id);
            GenerarTicket(pedido);
        }
        private void GenerarTicket(Pedido pedido)
        {
            var cantidad = pedido.Cantidad + pedido.CantidadAjuste;
            var estados = DAOFactory.EstadoDAO.GetByPlanta(pedido.Linea.Id);
            var total = estados.Sum(e => Convert.ToInt32(e.Deltatime));
            if (total == 0) total = pedido.TiempoCiclo;

            if (estados.All(e => e.EsPuntoDeControl != Estado.Evento.LlegaAObra)) ThrowError("PROGRAMACION_NO_OBRA");

            var deltas = estados.Select(e => Convert.ToInt32(((e.Deltatime == 0 ? total / estados.Count : e.Deltatime) * pedido.TiempoCiclo * 1.0) / total)).ToList();
            var inicioServicio = dtTicketAjuste.SelectedDate.Value.ToDataBaseDateTime();

            var tickets = DAOFactory.TicketDAO.GetByPedido(pedido.Id);
            var deAjuste = tickets.Count(x => x.Codigo.StartsWith(pedido.Codigo + "-A"));

                var ticket = new Ticket
                {
                    CantidadCarga = "0",
                    CantidadCargaReal = "0",
                    CantidadPedido = cantidad.ToString(),
                    Cliente = pedido.Cliente,
                    Codigo = pedido.Codigo + "-A" + (deAjuste+1).ToString().PadLeft(2, '0'),
                    CodigoProducto = pedido.Producto != null ? pedido.Producto.Codigo : "",
                    CumulativeQty = cantidad.ToString(),
                    DescripcionProducto = pedido.Producto != null ? pedido.Producto.Descripcion : "",
                    Empresa = pedido.BocaDeCarga.Linea != null ? pedido.BocaDeCarga.Linea.Empresa : null,
                    Linea = pedido.BocaDeCarga.Linea,
                    BaseDestino = pedido.BocaDeCarga.Linea,
                    Estado = Ticket.Estados.Pendiente,
                    FechaTicket = inicioServicio,
                    Pedido = pedido,
                    PuntoEntrega = pedido.PuntoEntrega,
                    SourceFile = "TicketAjuste",
                    SourceStation = "WEB",
                    Unidad = "m3",
                    UserField1 = "Ticket de Ajuste " + (deAjuste + 1).ToString(),
                    UserField2 = string.Empty,
                    UserField3 = string.Empty,
                    OrdenDiario = DAOFactory.TicketDAO.FindNextOrdenDiario(pedido.BocaDeCarga.Linea != null ? pedido.BocaDeCarga.Linea.Empresa.Id : -1, pedido.BocaDeCarga.Linea != null ? pedido.BocaDeCarga.Linea.Id : -1, inicioServicio),
                    ASincronizar = true
                };

                var minutosTranscurridos = 0;

                for (int i = 0; i < estados.Count; i++)
                {
                    var estado = estados[i];
                    var detalle = new DetalleTicket
                    {
                        Ticket = ticket,
                        EstadoLogistico = estado,
                        Programado = inicioServicio.AddMinutes(minutosTranscurridos)
                    };
                    ticket.Detalles.Add(detalle);
                    minutosTranscurridos += deltas[i];
                }

            DAOFactory.TicketDAO.SaveOrUpdate(ticket);
            Bind();
        }
    }
}
