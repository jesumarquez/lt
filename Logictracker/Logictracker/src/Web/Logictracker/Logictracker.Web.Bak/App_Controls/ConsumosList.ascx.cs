using System.Collections.Generic;
using C1.Web.UI.Controls.C1GridView;
using System;
using System.Linq;
using System.Web.UI;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BaseControls;

namespace Logictracker.App_Controls
{
    public partial class AppControlsConsumosList : BaseUserControl
    {
        private int Coche
        {
            get { return (int)(ViewState["Coche"] ?? -1); }
            set { ViewState["Coche"] = value; }
        }
        private DateTime Desde
        {
            get { return (DateTime)(ViewState["Desde"] ?? DateTime.Today.AddMonths(-1).ToDataBaseDateTime()); }
            set { ViewState["Desde"] = value; }
        }
        private DateTime Hasta
        {
            get { return (DateTime)(ViewState["Hasta"] ?? DateTime.Today.AddDays(1).AddMinutes(-1).ToDataBaseDateTime()); }
            set { ViewState["Hasta"] = value; }
        }
    
        public void LoadConsumos(int coche, DateTime desde, DateTime hasta)
        {        
            Coche = coche;
            var vehiculo = DAOFactory.CocheDAO.FindById(coche);
            Desde = desde;
            Hasta = hasta;

            var consumos = new List<ConsumoDetalle>();

            if (vehiculo != null && vehiculo.Id != 0)
            {
                consumos = DAOFactory.ConsumoDetalleDAO.GetList(new[] {vehiculo.Empresa != null ? vehiculo.Empresa.Id : -1},
                                                                new[] {vehiculo.Linea != null ? vehiculo.Linea.Id : -1},
                                                                new[] {vehiculo.Transportista != null ? vehiculo.Transportista.Id : -1},
                                                                new[] {-1},
                                                                new[] {vehiculo.CentroDeCostos != null ? vehiculo.CentroDeCostos.Id : -1},
                                                                new[] {vehiculo.TipoCoche != null ? vehiculo.TipoCoche.Id : -1},
                                                                new[] {Coche},
                                                                new[] {-1},
                                                                new[] {-1},
                                                                new[] {-1},
                                                                new[] {-1},
                                                                new[] {-1},
                                                                new[] {-1},
                                                                Desde,
                                                                Hasta,
                                                                new[] {-1})
                                                       .ToList();
            }

            grid.DataSource = consumos;
            grid.DataBind();
        }
    
        public void ClearConsumos()
        {
            grid.DataSource = null;
            grid.DataBind();
        }

        protected void GridConsumos_ItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(grid, string.Format("Select:{0}", e.Row.RowIndex)));
            e.Row.Attributes.Add("onmouseover", "this.className = 'Grid_MouseOver_Item';");
            e.Row.Attributes.Add("onmouseout", string.Format("this.className = '{0}';", e.Row.RowIndex % 2 == 0 ? "Grid_Item" : "Grid_Alternating_Item"));

            if (e.Row.RowType != C1GridViewRowType.DataRow || e.Row.RowIndex < 0) return;

            var con = e.Row.DataItem as ConsumoDetalle;

            if (con != null)
            {
                e.Row.Cells[0].Text = con.ConsumoCabecera.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
                e.Row.Cells[1].Text = con.ConsumoCabecera.NumeroFactura;
                e.Row.Cells[2].Text = con.ConsumoCabecera.KmDeclarados.ToString("#0.00");
                e.Row.Cells[3].Text = con.ConsumoCabecera.Proveedor != null
                                          ? con.ConsumoCabecera.Proveedor.Descripcion
                                          : string.Empty;
                e.Row.Cells[4].Text = con.ConsumoCabecera.Deposito != null
                                          ? con.ConsumoCabecera.Deposito.Descripcion
                                          : string.Empty;
                e.Row.Cells[5].Text = con.Insumo.Descripcion;
                e.Row.Cells[6].Text = con.Insumo.UnidadMedida != null ? con.Insumo.UnidadMedida.Descripcion : string.Empty;
                e.Row.Cells[7].Text = con.Cantidad.ToString("#0.00");
                e.Row.Cells[8].Text = con.ImporteUnitario.ToString("#0.00");
                e.Row.Cells[9].Text = con.ImporteTotal.ToString("#0.00");
                e.Row.Cells[10].Text = (con.ConsumoCabecera.Estado == ConsumoCabecera.Estados.Pendiente) ? "PENDIENTE" : (con.ConsumoCabecera.Estado == ConsumoCabecera.Estados.Pagado) ? "PAGADO" : string.Empty;
            }
        }

        protected void GridConsumos_SelectedIndexChanging(object sender, C1GridViewSelectEventArgs e)
        {
            grid.SelectedIndex = e.NewSelectedIndex;

            if (grid.SelectedIndex < 0) return;

            var id = (int) grid.DataKeys[grid.SelectedIndex].Value;

            var con = DAOFactory.ConsumoDetalleDAO.FindById(id);

            Session.Add("id", id);
            Session.Add("ViewOnly", true);

            var url = ResolveUrl("~/Mantenimiento/ConsumoAlta.aspx?t=" + con.ConsumoCabecera.Id);

            ScriptManager.RegisterStartupScript(Page, typeof(string), "viewCon", "window.open('" + url + "');", true);
        }

        protected void GridConsumos_PageIndexChanging(object sender, C1GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            LoadConsumos(Coche, Desde, Hasta);
        }

        protected void Grid_SortingCommand(object sender, C1GridViewSortEventArgs e)
        {
            LoadConsumos(Coche, Desde, Hasta);
        }

        protected void GridConsumos_GroupColumnMoved(object sender, C1GridViewGroupColumnMovedEventArgs e){}
        protected void GridConsumos_ColumnMoved(object sender, C1GridViewColumnMovedEventArgs e){}
    }
}
