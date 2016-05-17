using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.CicloLogistico.Distribucion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class PreasignacionViajeVehiculoLista : SecuredListPage<PreasignacionViajeVehiculoVo>
    {
        protected override string RedirectUrl { get { return "PreasignacionViajeVehiculoAlta.aspx"; } }
        protected override string VariableName { get { return "PREASIGNACION_VIAJE_VEHICULO"; } }
        protected override string GetRefference() { return "PREASIGNACION_VIAJE_VEHICULO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<PreasignacionViajeVehiculoVo> GetListData()
        {
            return DAOFactory.PreasignacionViajeVehiculoDAO.GetList(new[]{cbEmpresa.Selected},
                                                                    new[]{cbLinea.Selected},
                                                                    cbTransportista.SelectedValues,
                                                                    cbMovil.SelectedValues)
                                                           .Select(v => new PreasignacionViajeVehiculoVo(v))
                                                           .ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            return data;
        }
    }
}
