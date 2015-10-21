using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.CicloLogistico;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico
{
    public partial class ListaCicloLogistico : SecuredListPage<CicloLogisticoVo>
    {
        protected override string RedirectUrl { get { return "CicloLogisticoAlta.aspx"; } }
        protected override string VariableName { get { return "CLOG_CICLOS_LOGISTICOS"; } }
        protected override string GetRefference() { return "CICLO_LOGISTICO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<CicloLogisticoVo> GetListData()
        {
            var ciclos = (from o in DAOFactory.CicloLogisticoDAO.GetList(cbEmpresa.Selected, cbLinea.Selected)
                          select new CicloLogisticoVo(o)).ToList();
            return ciclos;
        }

    }
}
