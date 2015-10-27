using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.CicloLogistico;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico
{
    public partial class ListaServicio : SecuredListPage<ServicioVo>
    {
        protected override string RedirectUrl { get { return "ServicioAlta.aspx"; } }
        protected override string VariableName { get { return "CLOG_SERVICIOS"; } }
        protected override string GetRefference() { return "SERVICIO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<ServicioVo> GetListData()
        {
            var ciclos = (from o in DAOFactory.ServicioDAO.GetList(cbEmpresa.Selected, cbLinea.Selected)
                          select new ServicioVo(o)).ToList();
            return ciclos;
        }
    }
}