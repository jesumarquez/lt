using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.ReferenciasGeograficas;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoZonaLista : SecuredListPage<TipoZonaVo>
    {
        protected override string RedirectUrl { get { return "TipoZonaAlta.aspx"; } }
        protected override string VariableName { get { return GetRefference(); } }
        protected override string GetRefference() { return "PAR_TIPO_ZONA"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TipoZonaVo> GetListData()
        {
            return DAOFactory.TipoZonaDAO.GetList(new[] { ddlLocacion.Selected },
                                                        new[] { ddlPlanta.Selected })
                                               .Select(z => new TipoZonaVo(z)).ToList();
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
