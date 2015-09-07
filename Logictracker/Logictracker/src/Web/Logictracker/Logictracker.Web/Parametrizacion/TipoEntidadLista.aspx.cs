using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoEntidadLista : SecuredListPage<TipoEntidadVo>
    {
        protected override string RedirectUrl { get { return "TipoEntidadAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPO_ENTIDAD"; } }
        protected override string GetRefference() { return "PAR_TIPO_ENTIDAD"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TipoEntidadVo> GetListData()
        {
            return DAOFactory.TipoEntidadDAO.GetList(new[] { cbEmpresa.Selected },
                                                     new[] { cbLinea.Selected })
                                            .Select(v => new TipoEntidadVo(v))
                                            .ToList();
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
