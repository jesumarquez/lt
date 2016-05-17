using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ValueObjects.CicloLogistico.Distribucion;
using Logictracker.Web.BaseClasses.BasePages;
using C1.Web.UI.Controls.C1GridView;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class TipoCicloLogisticoLista : SecuredListPage<TipoCicloLogisticoVo>
    {
        protected override string VariableName { get { return "PAR_TIPO_CICLO_LOGISTICO"; } }
        protected override string GetRefference() { return "PAR_TIPO_CICLO_LOGISTICO"; }
        protected override string RedirectUrl { get { return "TipoCicloLogisticoAlta.aspx"; } }
        protected override bool ExcelButton { get { return true; } }
        private static readonly string TrueIcon = String.Concat(ImagesDir, "accept.png");
        private static readonly string FalseIcon = String.Concat(ImagesDir, "cancel.png");

        protected override List<TipoCicloLogisticoVo> GetListData()
        {
            return DAOFactory.TipoCicloLogisticoDAO.GetByEmpresa(cbEmpresa.Selected)
                                                  .Select(e => new TipoCicloLogisticoVo(e)).ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            if (empresa != null) cbEmpresa.SetSelectedValue((int)empresa);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            return data;
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, TipoCicloLogisticoVo dataItem)
        {
            ((Image)GridUtils.GetCell(e.Row, TipoCicloLogisticoVo.IndexDefault).FindControl("imgDefaut")).ImageUrl = dataItem.Default ? TrueIcon : FalseIcon;
        }
        protected override void CreateRowTemplate(C1GridViewRow row)
        {
            CreateImageTemplate(row, "imgDefaut", TipoCicloLogisticoVo.IndexDefault);
        }
        private void CreateImageTemplate(C1GridViewRow row, string controlId, int originalColumnIndex)
        {
            var imgbtn = GridUtils.GetCell(row, originalColumnIndex).FindControl(controlId) as Image;
            if (imgbtn == null) GridUtils.GetCell(row, originalColumnIndex).Controls.Add(new Image { ID = controlId });
        }
    }
}
