using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.Organizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Organizacion
{
    public partial class ListaFuncion : SecuredListPage<FuncionVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "SOC_FUNCIONES"; } }
        protected override string RedirectUrl { get { return "FuncionAlta.aspx"; } }
        protected override string GetRefference() { return "FUNCION"; }
        protected override bool ExcelButton { get { return true; } }


        #endregion

        #region Protected Methods

        protected override List<FuncionVo> GetListData()
        {
            var funciones = (cbSubSistema.Selected > 0 ? DAOFactory.FuncionDAO.GetBySistema(cbSubSistema.Selected) : DAOFactory.FuncionDAO.FindAll())
                            .Where(f => f.FechaBaja == null||f.FechaBaja == string.Empty).Select(f => new FuncionVo(f));

            return funciones.ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, FuncionVo dataItem)
        {
            GridUtils.GetCell(e.Row, FuncionVo.IndexIconUrl).Text = string.Format("<img src='{0}' />", ResolveUrl(dataItem.IconUrl));
        }

        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticSistema, cbSubSistema);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticSistema, cbSubSistema.Selected);
            return data;
        }
    }
}