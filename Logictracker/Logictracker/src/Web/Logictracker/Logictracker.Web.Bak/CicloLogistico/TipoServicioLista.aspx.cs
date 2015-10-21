using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.CicloLogistico;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico
{
    public partial class TipoServicioLista : SecuredListPage<TipoServicioCicloVo>
    {
        protected override string RedirectUrl { get { return "TipoServicioAlta.aspx"; } }
        protected override string VariableName { get { return "CLOG_TIPOSERVICIO"; } }
        protected override string GetRefference() { return "CLOG_TIPOSERVICIO"; }
        protected override bool ExcelButton { get { return true; } }
        private static readonly string TrueIcon = String.Concat(ImagesDir, "accept.png");
        private static readonly string FalseIcon = String.Concat(ImagesDir, "cancel.png");

        protected override List<TipoServicioCicloVo> GetListData()
        {
            return DAOFactory.TipoServicioCicloDAO.GetList(cbEmpresa.SelectedValues, cbLinea.SelectedValues)
                .Select(r => new TipoServicioCicloVo(r))
                .ToList();
        }
        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, TipoServicioCicloVo dataItem)
        {
            ((Image)GridUtils.GetCell(e.Row, TipoServicioCicloVo.IndexDefault).FindControl("imgDefaut")).ImageUrl = dataItem.Default ? TrueIcon : FalseIcon;
        }
        protected override void CreateRowTemplate(C1GridViewRow row)
        {
            CreateImageTemplate(row, "imgDefaut", TipoServicioCicloVo.IndexDefault);
        }
        private void CreateImageTemplate(C1GridViewRow row, string controlId, int originalColumnIndex)
        {
            var imgbtn = GridUtils.GetCell(row, originalColumnIndex).FindControl(controlId) as Image;
            if (imgbtn == null) GridUtils.GetCell(row, originalColumnIndex).Controls.Add(new Image { ID = controlId });
        }
        
    }
}
