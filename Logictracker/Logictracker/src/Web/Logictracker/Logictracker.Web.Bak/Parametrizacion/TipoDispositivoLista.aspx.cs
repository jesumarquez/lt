using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.ValueObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoDispositivoLista : SecuredListPage<TipoDispositivoVo>
    {
        protected override string VariableName { get { return "PAR_TIPO_DISPOSITIVO"; } }
        protected override string RedirectUrl { get { return "TipoDispositivoAlta.aspx"; } }
        protected override string GetRefference() { return "TIPO_DISPOSITIVO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TipoDispositivoVo> GetListData()
        {
            return DAOFactory.TipoDispositivoDAO.FindAll().OfType<TipoDispositivo>()
                .Select(t => new TipoDispositivoVo(t)).ToList();
        }
    }
}
