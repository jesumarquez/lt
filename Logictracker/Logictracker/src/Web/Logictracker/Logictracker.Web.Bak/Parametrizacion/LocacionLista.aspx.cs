using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class Parametrizacion_LocacionLista : SecuredListPage<EmpresaVo>
    {
        protected override string VariableName { get { return "PAR_REGION"; } }
        protected override string RedirectUrl { get { return "LocacionAlta.aspx"; } }
        protected override string GetRefference() { return "LOCACION"; }
        protected override bool ExcelButton { get { return true; } }


        protected override List<EmpresaVo> GetListData()
        {
            return DAOFactory.EmpresaDAO.GetList().Select(e => new EmpresaVo(e)).ToList();
        }

    }
}
