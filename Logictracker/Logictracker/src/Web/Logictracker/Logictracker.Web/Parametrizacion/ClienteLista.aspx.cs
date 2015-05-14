using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{

    public partial class ClienteLista : SecuredListPage<ClienteVo>
    {
        protected override string VariableName { get { return "PAR_CLIENTES"; } }
        protected override string RedirectUrl { get { return "ClienteAlta.aspx"; } }
        protected override string GetRefference() { return "RAMAL"; }

        protected override bool ImportButton { get { return true; } }
        protected override string ImportUrl { get { return "ClienteImport.aspx"; } }
        protected override bool ExcelButton { get { return true; } }

        #region Protected Methods

        protected override List<ClienteVo> GetListData()
        {
            return DAOFactory.ClienteDAO.GetList(new[] {ddlDistrito.Selected}, new[] {ddlBase.Selected})
                .Select(c => new ClienteVo(c))
                .ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ClienteVo dataItem)
        {
            if (!dataItem.Nomenclado)
            {
                e.Row.BackColor = Color.Firebrick;
            }
        }

        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, ddlDistrito);
            data.LoadStaticFilter(FilterData.StaticBase, ddlBase);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlDistrito.Selected);
            data.AddStatic(FilterData.StaticBase, ddlBase.Selected);
            return data;
        }
    }
}