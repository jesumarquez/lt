using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Support;
using Logictracker.Web.BaseClasses.BaseControls;

namespace Logictracker.App_Controls
{
    public partial class TicketsList : BaseUserControl
    {
        private int Coche
        {
            get { return (int)(ViewState["Coche"] ?? -1); }
            set { ViewState["Coche"] = value; }
        }
        private DateTime Desde
        {
            get { return (DateTime)(ViewState["Desde"] ?? DateTime.Today.AddMonths(-3).ToDataBaseDateTime()); }
            set { ViewState["Desde"] = value; }
        }
        private DateTime Hasta
        {
            get { return (DateTime)(ViewState["Hasta"] ?? DateTime.Today.AddDays(1).AddMinutes(-1).ToDataBaseDateTime()); }
            set { ViewState["Hasta"] = value; }
        }

        public void LoadTickets(int coche, DateTime desde, DateTime hasta)
        {        
            var tickets = new List<SupportTicket>();

            Coche = coche;
            Desde = desde;
            Hasta = hasta;

            var vehiculo = DAOFactory.CocheDAO.FindById(coche);
            if (vehiculo != null && vehiculo.Id != 0)
            {
                tickets = DAOFactory.SupportTicketDAO.GetList(new[] { vehiculo.Empresa.Id }, new[] { -1 }, new[] { -1 }, new[] { -1 }, desde, hasta)
                                                     .Where(t => t.Vehiculo == vehiculo)
                                                     .OrderByDescending(t => t.Fecha)
                                                     .ToList();

                btnNuevo.OnClientClick = string.Format("window.open('../Soporte/TicketSoporteAlta.aspx?v={0}','Ticket')", Coche);
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

            var ticket = e.Row.DataItem as SupportTicket;

            if (ticket != null)
            {
                e.Row.Cells[0].Text = ticket.Id.ToString("#0");
                e.Row.Cells[1].Text = ticket.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
                var problema = DAOFactory.SupportTicketDAO.GetTiposProblema()[ticket.TipoProblema];
                var cat = (short)(ticket.CategoriaObj != null
                                    ? ticket.CategoriaObj.Id > 7
                                        ? ticket.CategoriaObj.Id - 9
                                        : ticket.CategoriaObj.Id - 1
                                    : 0);

                problema += ": " + DAOFactory.SupportTicketDAO.GetCategoriasProblemaByTipo(ticket.TipoProblema)[cat < 0 ? 0 : cat];
                
                e.Row.Cells[2].Text = problema;
                e.Row.Cells[3].Text = ticket.Descripcion.Replace("<", "&lt;").Replace(">", "&gt;");
                e.Row.Cells[4].Text = ticket.Nombre;
                e.Row.Cells[5].Text = ticket.Telefono;
                e.Row.Cells[6].Text = ticket.Mail;
                e.Row.Cells[7].Text = DAOFactory.SupportTicketDAO.GetEstados()[ticket.CurrentState];
                e.Row.Cells[8].Text = ticket.Empresa != null ? ticket.Empresa.RazonSocial : string.Empty;
                e.Row.Cells[9].Text = ticket.Vehiculo != null ? ticket.Vehiculo.Interno : string.Empty;
                e.Row.Cells[10].Text = ticket.Dispositivo != null ? ticket.Dispositivo.Codigo : string.Empty;

                e.Row.BackColor = DAOFactory.SupportTicketDAO.GetColoresEstados()[ticket.CurrentState];
            }
        }

        protected void GridSelectedIndexChanging(object sender, C1GridViewSelectEventArgs e)
        {
            grid.SelectedIndex = e.NewSelectedIndex;

            if (grid.SelectedIndex < 0) return;

            var id = (int) grid.DataKeys[grid.SelectedIndex].Value;

            Session.Add("id", id);

            var url = ResolveUrl("~/Soporte/TicketSoporteAlta.aspx?id=" + id);

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
