#region Usings

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects;
using System.Web.UI.WebControls;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionPtoEntregaLista : SecuredListPage<PuntoEntregaVo>
    {
        protected int SelectedClient = -1;
        protected int Distrito = -1;
        protected int Base = -1;
        protected int totalVirtualRows;
        #region Protected Properties

        protected override string VariableName { get { return "PTOS_ENTREGA"; } }
        protected override string RedirectUrl { get { return "PtoEntregaAlta.aspx"; } }
        protected override string GetRefference() { return "PTO_ENTREGA"; }
        protected override bool ImportButton { get { return true; } }
        protected override string ImportUrl { get { return "PtoEntregaImport.aspx"; } }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override void OnLoad(System.EventArgs e)
        {
            if (Session["SelectedClient"] == null)
                Session["SelectedClient"] = SelectedClient;
            if (Session["Distrito"] == null)
                Session["Distrito"] = Distrito;
            if (Session["Base"] == null)
                Session["Base"] = Base;
            if (Session["totalVirtualRows"] == null)
                Session["totalVirtualRows"] = totalVirtualRows;
            Grid.AllowPaging = true;
            Grid.AllowCustomPaging = true;
            Grid.PagerSettings.Mode = System.Web.UI.WebControls.PagerButtons.NumericFirstLast;
            Grid.PageIndexChanging += Grid_PageIndexChanging;
            GridUtils.CustomPagination = true;
            base.OnLoad(e);
        }

        void Grid_PageIndexChanging(object sender, C1GridViewPageEventArgs e)
        {
            Grid.PageIndex = e.NewPageIndex;
        }
             
        protected override List<PuntoEntregaVo> GetListData()
        {

            var puntos = new List<PuntoEntrega>();
            if (ddlCliente.Selected > 0)
            {
                SelectedClient = (int)Session["SelectedClient"];
                Base = (int)Session["Base"];
                Distrito = (int)Session["Distrito"];
                totalVirtualRows = (int)Session["totalVirtualRows"];

                bool recount = true;
                if (SelectedClient.Equals(-1))
                {
                    SelectedClient = ddlCliente.Selected;
                    Session["SelectedClient"] = ddlCliente.Selected;
                    Session["Distrito"] = ddlDistrito.Selected;
                    Session["Base"] = ddlBase.Selected;
                }
                else
                {
                    if (SelectedClient.Equals(ddlCliente.Selected))
                    {
                        recount = false;
                        SelectedClient = ddlCliente.Selected;
                        Session["SelectedClient"] = ddlCliente.Selected;
                        Session["Distrito"] = ddlDistrito.Selected;
                        Session["Base"] = ddlBase.Selected;
                    }
                }

                puntos = DAOFactory.PuntoEntregaDAO.GetByCliente(ddlCliente.Selected, Grid.PageIndex, this.PageSize, ref totalVirtualRows, recount);
            
                if (recount)
                {
                    Session["totalVirtualRows"] = totalVirtualRows;
                    Grid.VirtualItemCount = totalVirtualRows;
                }
            }
            
            return puntos.Select(p => new PuntoEntregaVo(p)).ToList();               
        }


        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, PuntoEntregaVo dataItem)
        {
            if (!dataItem.Nomenclado)
            {
                e.Row.BackColor = Color.Firebrick;
            }
        }
        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            var cliente = data[FilterData.StaticCliente];
            if (empresa != null) ddlDistrito.SetSelectedValue((int)empresa);
            if (linea != null) ddlBase.SetSelectedValue((int)linea);
            if (cliente != null) ddlCliente.SetSelectedValue((int)cliente);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlDistrito.Selected);
            data.AddStatic(FilterData.StaticBase, ddlBase.Selected);
            data.AddStatic(FilterData.StaticCliente, ddlCliente.Selected);
            return data;
        }
    }
}
