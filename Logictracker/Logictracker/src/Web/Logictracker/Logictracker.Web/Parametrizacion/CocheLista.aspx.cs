using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class CocheLista : SecuredListPage<CocheVo>
    {
        protected override string RedirectUrl { get { return "CocheAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_VEHICULOS"; } }
        protected override string GetRefference() { return "COCHE"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<CocheVo> GetListData()
        {
            var verCosto = WebSecurity.IsSecuredAllowed(Securables.ViewCost);
            if (!verCosto)
            {
                var cant = Grid.Columns.Count;

                Grid.Columns.RemoveAt(cant - 1);
                Grid.Columns.RemoveAt(cant - 2);
            }

            var ocultarConDispo = chkOcultarConDispo.Checked;
            var ocultarSinDispo = chkOcultarSinDispo.Checked;
            if (ocultarConDispo && ocultarSinDispo) return new List<CocheVo>(0);

            return DAOFactory.CocheDAO.GetList(cbEmpresa.SelectedValues, 
                                               cbLinea.SelectedValues, 
                                               ddlTipoVehiculo.SelectedValues, 
                                               ddlTransportista.SelectedValues,
                                               cbDepartamento.SelectedValues,
                                               ddlCentroCostos.SelectedValues)
                            .Where(c => !ocultarConDispo || c.Dispositivo == null)
                            .Where(c => !ocultarSinDispo || c.Dispositivo != null)
                            .Select(c=> new CocheVo(c))
                            .ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, CocheVo dataItem)
        {
            GridUtils.GetCell(e.Row, CocheVo.IndexIcono).Text = string.Format("<img src='{0}' />", IconDir + dataItem.Icono);
            e.Row.BackColor = GetMobileStatusColor(dataItem);

            if (Coche.Estados.Inactivo == dataItem.Estado) e.Row.ForeColor = Color.White;
        }

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticDepartamento, cbDepartamento);
            data.LoadStaticFilter(FilterData.StaticTransportista, ddlTransportista);
            data.LoadStaticFilter(FilterData.StaticTipoVehiculo, ddlTipoVehiculo);
            data.LoadStaticFilter(FilterData.StaticCentroCostos, ddlCentroCostos);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticDepartamento, cbDepartamento.Selected);
            data.AddStatic(FilterData.StaticTransportista, ddlTransportista.Selected);
            data.AddStatic(FilterData.StaticTipoVehiculo, ddlTipoVehiculo.Selected);
            data.AddStatic(FilterData.StaticCentroCostos, ddlCentroCostos.Selected);
            return data;
        }

        private static Color GetMobileStatusColor(CocheVo coche)
        {
            switch (coche.Estado)
            {
                case Coche.Estados.Activo: return Color.FromArgb(152, 251, 152);
                case Coche.Estados.EnMantenimiento: return Color.FromArgb(238, 221, 130);
                case Coche.Estados.Inactivo: return Color.FromArgb(205, 92, 92);
                case Coche.Estados.Revisar: return Color.FromArgb(205, 92, 92);
            }

            return Color.Empty;
        }
    }
}
