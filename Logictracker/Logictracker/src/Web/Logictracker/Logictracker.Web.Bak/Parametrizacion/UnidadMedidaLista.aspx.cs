using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class UnidadMedidaLista : SecuredListPage<UnidadMedidaVo>
    {
        protected override string RedirectUrl { get { return "UnidadMedidaAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_UNIDAD_MEDIDA"; } }
        protected override string GetRefference() { return "PAR_UNIDAD_MEDIDA"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<UnidadMedidaVo> GetListData()
        {
            return DAOFactory.UnidadMedidaDAO.GetList()
                                             .Select(v => new UnidadMedidaVo(v))
                                             .ToList();
        }
    }
}
