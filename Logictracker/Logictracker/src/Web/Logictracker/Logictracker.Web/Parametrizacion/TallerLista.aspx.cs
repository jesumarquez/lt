using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReferenciasGeograficas;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionTallerLista : SecuredListPage<TallerVo>
    {
        protected override string VariableName { get { return "PAR_TALLERES"; } }
        protected override string RedirectUrl { get { return "TallerAlta.aspx"; } }
        protected override string GetRefference() { return "TALLER"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TallerVo> GetListData()
        {
                return DAOFactory.TallerDAO.GetList(ddlDistrito.SelectedValues, 
                                               ddlBase.SelectedValues)
                            .Where(t=>!t.Baja)
                            .Select(t=> new TallerVo(t))
                            .ToList();
        }
    

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, TallerVo dataItem)
        {
            GridUtils.GetCell(e.Row, TallerVo.IndexIconUrl).Text = string.Format("<img src=\"{0}\" />", IconDir + dataItem.IconUrl);
        }
    }
}
