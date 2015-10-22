using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BaseControls;

namespace Logictracker.App_Controls
{
    public partial class TicketsMantenimientoList : BaseUserControl
    {
        private int Coche
        {
            get { return (int)(ViewState["Coche"] ?? -1); }
            set { ViewState["Coche"] = value; }
        }
        private DateTime Desde
        {
            get { return (DateTime)(ViewState["Desde"] ?? DateTime.Today.AddMonths(-6).ToDataBaseDateTime()); }
            set { ViewState["Desde"] = value; }
        }
        private DateTime Hasta
        {
            get { return (DateTime)(ViewState["Hasta"] ?? DateTime.Today.AddDays(1).AddMinutes(-1).ToDataBaseDateTime()); }
            set { ViewState["Hasta"] = value; }
        }

        public void LoadTickets(int coche, DateTime desde, DateTime hasta)
        {        
            var tickets = new List<TicketMantenimiento>();

            Coche = coche;
            Desde = desde;
            Hasta = hasta;

            var vehiculo = DAOFactory.CocheDAO.FindById(coche);
            if (vehiculo != null && vehiculo.Id != 0)
            {
                tickets = DAOFactory.TicketMantenimientoDAO.GetList(new[]{-1},new[]{vehiculo.Empresa.Id},new[]{-1},new[]{vehiculo.Id}, desde, hasta)
                                                           .OrderByDescending(t => t.Id)
                                                           .ToList();

                btnNuevo.OnClientClick = string.Format("window.open('../Mantenimiento/TicketMantenimientoAlta.aspx?v={0}','Ticket')", Coche);
            }

            grid.DataSource = tickets;
            grid.DataBind();
        }
    
        public void ClearTickets()
        {
            grid.DataSource = null;
            grid.DataBind();
        }

        protected void GridItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(grid, string.Format("Select:{0}", e.Row.RowIndex)));
            e.Row.Attributes.Add("onmouseover", "this.className = 'Grid_MouseOver_Item';");
            e.Row.Attributes.Add("onmouseout", string.Format("this.className = '{0}';", e.Row.RowIndex % 2 == 0 ? "Grid_Item" : "Grid_Alternating_Item"));

            if (e.Row.RowType != C1GridViewRowType.DataRow || e.Row.RowIndex < 0) return;

            var ticket = e.Row.DataItem as TicketMantenimiento;

            if (ticket != null)
            {
                e.Row.Cells[0].Text = ticket.Id.ToString("#0");
                e.Row.Cells[1].Text = ticket.FechaSolicitud.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
                e.Row.Cells[2].Text = ticket.Codigo;
                e.Row.Cells[3].Text = ticket.Taller != null ? ticket.Taller.Descripcion : string.Empty;
                e.Row.Cells[4].Text = CultureManager.GetLabel(TicketMantenimiento.EstadosTicket.GetLabelVariableName(ticket.Estado));
                e.Row.Cells[5].Text = ticket.Descripcion;

                e.Row.BackColor = TicketMantenimiento.EstadosTicket.GetColor(ticket.Estado);
            }
        }

        protected void GridSelectedIndexChanging(object sender, C1GridViewSelectEventArgs e)
        {
            grid.SelectedIndex = e.NewSelectedIndex;

            if (grid.SelectedIndex < 0) return;

            var id = (int) grid.DataKeys[grid.SelectedIndex].Value;

            Session.Add("id", id);

            var url = ResolveUrl("~/Mantenimiento/TicketMantenimientoAlta.aspx?id=" + id);

            ScriptManager.RegisterStartupScript(Page, typeof(string), "viewCon", "window.open('" + url + "');", true);
        }

        protected void GridPageIndexChanging(object sender, C1GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            LoadTickets(Coche, Desde, Hasta);
        }

        protected void GridSortingCommand(object sender, C1GridViewSortEventArgs e)
        {
            LoadTickets(Coche, Desde, Hasta);
        }

        protected void GridGroupColumnMoved(object sender, C1GridViewGroupColumnMovedEventArgs e){}
        protected void GridColumnMoved(object sender, C1GridViewColumnMovedEventArgs e){}
    }
}
