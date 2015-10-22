using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoZonaAccesoLista : SecuredListPage<TipoZonaAccesoVo>
    {
        protected override string RedirectUrl { get { return "TipoZonaAccesoAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPOS_ZONA_ACCESO"; } }
        protected override string GetRefference() { return "PAR_TIPOS_ZONA_ACCESO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TipoZonaAccesoVo> GetListData()
        {
            return DAOFactory.TipoZonaAccesoDAO.GetList(new[] { ddlLocacion.Selected },
                                                        new[] { ddlPlanta.Selected })
                                               .Select(z => new TipoZonaAccesoVo(z)).ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            if (empresa != null) ddlLocacion.SetSelectedValue((int)empresa);
            if (linea != null) ddlPlanta.SetSelectedValue((int)linea);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlLocacion.Selected);
            data.AddStatic(FilterData.StaticBase, ddlPlanta.Selected);
            return data;
        }
    }
}
