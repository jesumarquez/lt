using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ValueObjects.CicloLogistico.Distribucion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class EstadoLogisticoLista : SecuredListPage<EstadoLogisticoVo>
    {
        protected override string VariableName { get { return "PAR_ESTADO_LOGISTICO"; } }
        protected override string GetRefference() { return "PAR_ESTADO_LOGISTICO"; }
        protected override string RedirectUrl { get { return "EstadoLogisticoAlta.aspx"; } }
        protected override bool ExcelButton { get { return true; } }
        
        protected override List<EstadoLogisticoVo> GetListData()
        {
            return DAOFactory.EstadoLogisticoDAO.GetByEmpresa(cbEmpresa.Selected)
                                                .Select(e => new EstadoLogisticoVo(e)).ToList();
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
