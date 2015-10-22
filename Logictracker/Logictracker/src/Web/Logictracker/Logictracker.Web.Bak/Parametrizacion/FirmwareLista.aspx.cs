using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionFirmwareLista : SecuredListPage<FirmwareVo>
    {   
        protected override string RedirectUrl { get { return "FirmwareAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_FIRMWARE"; } }
        protected override string GetRefference() { return "FIRMWARE"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<FirmwareVo> GetListData()
        {
            return DAOFactory.FirmwareDAO.FindAll().Select(f => new FirmwareVo(f)).ToList();
        }
    }
}
