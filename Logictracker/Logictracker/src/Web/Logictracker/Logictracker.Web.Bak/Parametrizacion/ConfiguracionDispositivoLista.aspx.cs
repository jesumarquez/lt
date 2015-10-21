#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class Parametrizacion_ConfiguracionDispositivoLista : SecuredListPage<ConfiguracionDispositivoVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_CONFIG_DISPOSITIVO"; } }
        protected override string RedirectUrl { get { return "ConfiguracionDispositivoAlta.aspx"; } }
        protected override string GetRefference() { return "CONFIGURACION"; }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override List<ConfiguracionDispositivoVo> GetListData()
        {
            return DAOFactory.ConfiguracionDispositivoDAO.FindAll().Select(cd => new ConfiguracionDispositivoVo(cd)).ToList();
        }
        #endregion
    }
}
