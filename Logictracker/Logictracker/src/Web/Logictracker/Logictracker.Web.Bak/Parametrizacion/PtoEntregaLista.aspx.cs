#region Usings

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionPtoEntregaLista : SecuredListPage<PuntoEntregaVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PTOS_ENTREGA"; } }
        protected override string RedirectUrl { get { return "PtoEntregaAlta.aspx"; } }
        protected override string GetRefference() { return "PTO_ENTREGA"; }
        protected override bool ImportButton { get { return true; } }
        protected override string ImportUrl { get { return "PtoEntregaImport.aspx"; } }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override List<PuntoEntregaVo> GetListData()
        {
            var puntos = new List<PuntoEntrega>();
            if (ddlCliente.Selected > 0)
                puntos = DAOFactory.PuntoEntregaDAO.GetByCliente(ddlCliente.Selected);
            
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
