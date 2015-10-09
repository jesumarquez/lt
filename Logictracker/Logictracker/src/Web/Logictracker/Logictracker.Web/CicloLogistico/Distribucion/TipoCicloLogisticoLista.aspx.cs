using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ValueObjects.CicloLogistico.Distribucion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class TipoCicloLogisticoLista : SecuredListPage<TipoCicloLogisticoVo>
    {
        protected override string VariableName { get { return "PAR_TIPO_CICLO_LOGISTICO"; } }
        protected override string GetRefference() { return "PAR_TIPO_CICLO_LOGISTICO"; }
        protected override string RedirectUrl { get { return "TipoCicloLogisticoAlta.aspx"; } }
        protected override bool ExcelButton { get { return true; } }

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
    }
}
