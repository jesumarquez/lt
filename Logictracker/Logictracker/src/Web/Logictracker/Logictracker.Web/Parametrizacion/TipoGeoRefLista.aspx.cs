using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReferenciasGeograficas;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoGeoRefLista : SecuredListPage<TipoReferenciaGeograficaVo>
    {
        protected override string RedirectUrl { get { return "TipoGeoRefAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPO_POI"; } }
        protected override string GetRefference() { return "TIPODOMICILIO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TipoReferenciaGeograficaVo> GetListData()
        {
            return DAOFactory.TipoReferenciaGeograficaDAO.GetList(new[]{ddlLocacion.Selected}, new[]{ddlPlanta.Selected})
                .Select(t => new TipoReferenciaGeograficaVo(t))
                .ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, TipoReferenciaGeograficaVo dataItem)
        {
            if (!string.IsNullOrEmpty(dataItem.IconUrl))
                GridUtils.GetCell(e.Row, TipoReferenciaGeograficaVo.IndexIconUrl).Text = string.Format("<img src='{0}' />", IconDir + dataItem.IconUrl);
            else if (!string.IsNullOrEmpty(dataItem.Color))
                GridUtils.GetCell(e.Row, TipoReferenciaGeograficaVo.IndexIconUrl).Text = string.Format("<div style='margin: 6px; width: 20px; height: 20px; background-color:#{0};'></div>", dataItem.Color);
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