using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoMedicionLista : SecuredListPage<TipoMedicionVo>
    {
        protected override string RedirectUrl { get { return "TipoMedicionAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPO_MEDICION"; } }
        protected override string GetRefference() { return "PAR_TIPO_MEDICION"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TipoMedicionVo> GetListData()
        {
            return DAOFactory.TipoMedicionDAO.GetList()
                                             .Select(v => new TipoMedicionVo(v))
                                             .ToList();
        }
    }
}
