using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.CicloLogistico;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico
{
    public partial class CicloLogistico_RecorridoLista : SecuredListPage<RecorridoVo>
    {
        protected override string RedirectUrl { get { return "RecorridoAlta.aspx"; } }
        protected override string VariableName { get { return "CLOG_RECORRIDO"; } }
        protected override string GetRefference() { return "CLOG_RECORRIDO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<RecorridoVo> GetListData()
        {
            return DAOFactory.RecorridoDAO.GetList(cbEmpresa.SelectedValues, cbLinea.SelectedValues)
                .Select(r => new RecorridoVo(r))
                .ToList();
        }
    }
}
