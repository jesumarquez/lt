using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class SensorLista : SecuredListPage<SensorVo>
    {
        protected override string RedirectUrl { get { return "SensorAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_SENSOR"; } }
        protected override string GetRefference() { return "PAR_SENSOR"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<SensorVo> GetListData()
        {
            return DAOFactory.SensorDAO.GetList(new[] { cbEmpresa.Selected },
                                                new[] { cbLinea.Selected },
                                                new[] { -1 },
                                                new[] { cbTipoMedicion.Selected })
                                       .Select(s => new SensorVo(s))
                                       .ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticTipoMedicion, cbTipoMedicion);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticTipoMedicion, cbTipoMedicion.Selected);
            return data;
        }

        #endregion
    }
}
