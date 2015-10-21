using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ZonaLista : SecuredListPage<ZonaVo>
    {
        protected override string RedirectUrl { get { return "ZonaAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_ZONAS"; } }
        protected override string GetRefference() { return "PAR_ZONAS"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<ZonaVo> GetListData()
        {
            return DAOFactory.ZonaDAO.GetList(new[] { ddlLocacion.Selected },
                                              new[] { ddlPlanta.Selected },
                                              new[] { cbTipoZona.Selected })
                                     .Select(z => new ZonaVo(z))
                                     .ToList();
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
