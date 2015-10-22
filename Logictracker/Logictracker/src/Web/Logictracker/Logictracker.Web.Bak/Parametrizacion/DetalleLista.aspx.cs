using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class DetalleLista : SecuredListPage<DetalleVo>
    {
        protected override string RedirectUrl { get { return "DetalleAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_DETALLE"; } }
        protected override string GetRefference() { return "PAR_DETALLE"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<DetalleVo> GetListData()
        {
            return DAOFactory.DetalleDAO.GetList(new[] { cbEmpresa.Selected },
                                                 new[] { cbLinea.Selected },
                                                 new[] { cbTipoEntidad.Selected },
                                                 new[] { cbDetalle.Selected })
                                        .Select(e => new DetalleVo(e))
                                        .ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticTipoEntidad, cbTipoEntidad);
            data.LoadStaticFilter(FilterData.StaticDetalle, cbDetalle);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticTipoEntidad, cbTipoEntidad.Selected);
            data.AddStatic(FilterData.StaticDetalle, cbDetalle.Selected);
            return data;
        }

        #endregion
    }
}
