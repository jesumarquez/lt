using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.CicloLogistico;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico
{
    public partial class ListaPedido : SecuredListPage<PedidoVo>
    {
        protected override string RedirectUrl { get { return "PedidoAlta.aspx"; } }
        protected override string VariableName { get { return "CLOG_PEDIDO"; } }
        protected override string GetRefference() { return "PEDIDO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                dtDesde.SelectedDate = DateTime.Today;
                dtHasta.SelectedDate = DateTime.Today.AddHours(23).AddMinutes(59);
            }
            base.OnPreLoad(e);
        }
        
        protected override List<PedidoVo> GetListData()
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
                                       .Select(p => new PedidoVo(p))
                                       .ToList();
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
    }
}
