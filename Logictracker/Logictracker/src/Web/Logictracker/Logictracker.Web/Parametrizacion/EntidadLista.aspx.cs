using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class EntidadLista : SecuredListPage<EntidadVo>
    {
        protected override string RedirectUrl { get { return "EntidadAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_ENTIDAD"; } }
        protected override string GetRefference() { return "PAR_ENTIDAD"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<EntidadVo> GetListData()
        {
            return DAOFactory.EntidadDAO.GetList(new[] { cbEmpresa.Selected },
                                                 new[] { cbLinea.Selected },
                                                 new[] { -1 },
                                                 new[] { cbTipoEntidad.Selected })
                                        .Select(e => new EntidadVo(e))
                                        .ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticTipoEntidad, cbTipoEntidad);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticTipoEntidad, cbTipoEntidad.Selected);
            return data;
        }

        #endregion
    }
}
