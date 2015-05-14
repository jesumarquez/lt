using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class MarcaLista : SecuredListPage<MarcaVo>
    {
        protected override string RedirectUrl { get { return "MarcaAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_MARCAS"; } }
        protected override string GetRefference() { return "PAR_MARCAS"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<MarcaVo> GetListData()
        {
            var list = DAOFactory.MarcaDAO.GetList(new[]{cbEmpresa.Selected}, new[]{cbLinea.Selected});

            return list.Select(v => new MarcaVo(v)).ToList();
        }

        #region Filter Memory

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

        #endregion
    }
}
