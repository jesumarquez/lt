using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.CicloLogistico.Distribucion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class ViajeProgramadoLista : SecuredListPage<ViajeProgramadoVo>
    {
        protected override string RedirectUrl { get { return "ViajeProgramadoAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_VIAJE_PROGRAMADO"; } }
        protected override string GetRefference() { return "PAR_VIAJE_PROGRAMADO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<ViajeProgramadoVo> GetListData()
        {
            return DAOFactory.ViajeProgramadoDAO.GetList(cbEmpresa.SelectedValues,
                                                         cbTransportista.SelectedValues,
                                                         cbTipoVehiculo.SelectedValues)
                                                .Select(v => new ViajeProgramadoVo(v)).ToList();
        }

        protected void FilterChanged(object sender, EventArgs e) { if (IsPostBack) Bind(); }
        
        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticTransportista, cbTransportista);
            data.LoadStaticFilter(FilterData.StaticTipoVehiculo, cbTipoVehiculo);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticTransportista, cbTransportista.Selected);
            data.AddStatic(FilterData.StaticTipoVehiculo, cbTipoVehiculo.Selected);
            return data;
        }

        #endregion
    }
}
