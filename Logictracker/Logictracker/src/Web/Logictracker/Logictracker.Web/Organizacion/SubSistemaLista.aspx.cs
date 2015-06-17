using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.Organizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Organizacion
{
    public partial class ListaSistema : SecuredListPage<SistemaVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "SOC_SISTEMAS"; } }
        protected override string RedirectUrl { get { return "SubSistemaAlta.aspx"; } }
        protected override string GetRefference() { return "SUBSISTEMA"; }
        protected override bool ExcelButton { get { return true; }}

        #endregion

        #region Protected Methods

        protected override List<SistemaVo> GetListData()
        {
            var list = new List<SistemaVo>();
            var query = DAOFactory.SistemaDAO.FindAll();

            foreach(Logictracker.Types.BusinessObjects.Sistema sistema in query)
            {
                list.Add(new SistemaVo(sistema));
            }

            return list;
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, SistemaVo dataItem)
        {
            GridUtils.GetCell(e.Row, SistemaVo.IndexIconUrl).Text = string.Format("<img src='{0}' />", ResolveUrl(dataItem.IconUrl));
        }

        #endregion
    }
}