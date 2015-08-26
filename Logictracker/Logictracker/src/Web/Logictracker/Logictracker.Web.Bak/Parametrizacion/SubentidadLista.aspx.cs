using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class SubentidadLista : SecuredListPage<SubEntidadVo>
    {
        protected override string RedirectUrl { get { return "SubentidadAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_SUBENTIDAD"; } }
        protected override string GetRefference() { return "PAR_SUBENTIDAD"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<SubEntidadVo> GetListData()
        {
            return DAOFactory.SubEntidadDAO.GetList(new[] { cbEmpresa.Selected },
                                                    new[] { cbLinea.Selected },
                                                    new[] { cbTipoEntidad.Selected },
                                                    new[] { cbEntidad.Selected },
                                                    new[] { -1 },
                                                    new[] { -1 })
                                           .Select(s => new SubEntidadVo(s))
                                           .ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticTipoEntidad, cbTipoEntidad);
            data.LoadStaticFilter(FilterData.StaticEntidad, cbEntidad);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticTipoEntidad, cbTipoEntidad.Selected);
            data.AddStatic(FilterData.StaticEntidad, cbEntidad.Selected);
            return data;
        }

        #endregion
    }
}
