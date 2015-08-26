using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ZonaAccesoLista : SecuredListPage<ZonaAccesoVo>
    {
        protected override string RedirectUrl { get { return "ZonaAccesoAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_ZONAS_ACCESO"; } }
        protected override string GetRefference() { return "PAR_ZONAS_ACCESO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<ZonaAccesoVo> GetListData()
        {
            return DAOFactory.ZonaAccesoDAO.GetList(new[] { ddlLocacion.Selected },
                                                    new[] { ddlPlanta.Selected },
                                                    new[] { ddlTipoZonaAcceso.Selected})
                                           .Select(z => new ZonaAccesoVo(z)).ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            var tipoZonaAcceso = data[FilterData.StaticTipoZonaAcceso];
            if (empresa != null) ddlLocacion.SetSelectedValue((int)empresa);
            if (linea != null) ddlPlanta.SetSelectedValue((int)linea);
            if (tipoZonaAcceso != null) ddlTipoZonaAcceso.SetSelectedValue((int)tipoZonaAcceso);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlLocacion.Selected);
            data.AddStatic(FilterData.StaticBase, ddlPlanta.Selected);
            data.AddStatic(FilterData.StaticTipoZonaAcceso, ddlTipoZonaAcceso.Selected);
            return data;
        }
    }
}
