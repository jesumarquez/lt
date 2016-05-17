using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class PrecintoLista : SecuredListPage<PrecintoVo>
    {
        protected override string RedirectUrl { get { return "PrecintoAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_PRECINTO"; } }
        protected override string GetRefference() { return "PAR_PRECINTO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<PrecintoVo> GetListData()
        {
            var list = DAOFactory.PrecintoDAO.GetList();

            return list.Select(v => new PrecintoVo(v)).ToList();
        }
    }
}
