using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoCocheLista : SecuredListPage<TipoCocheVo>
    { 
        protected override string RedirectUrl { get { return "TipoCocheAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPO_VEHICULO"; } }
        protected override string GetRefference() { return "TIPOCOCHE"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TipoCocheVo> GetListData()
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            return DAOFactory.TipoCocheDAO.FindByEmpresasAndLineas(ddlLocacion.SelectedValues, ddlPlanta.SelectedValues, user)
                .OfType<TipoCoche>().Select(t=>new TipoCocheVo(t)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, TipoCocheVo dataItem)
        {
            GridUtils.GetCell(e.Row, TipoCocheVo.IndexIconUrl).Text = string.Format("<img src='{0}' />", IconDir + dataItem.IconUrl);
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
