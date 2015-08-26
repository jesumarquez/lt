#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion.Postal;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion.Postal
{
    public partial class MotivoLista : SecuredListPage<MotivoVo>
    {
        protected override string VariableName { get { return "PAR_MOTIVOS"; } }
        protected override string RedirectUrl { get { return "MotivoAlta.aspx"; } }
        protected override string GetRefference() { return "MOTIVO"; }

        protected override List<MotivoVo> GetListData()
        {
            return DAOFactory.MotivoDAO.FindAll().Select(t => new MotivoVo(t)).ToList();
        }
    }
}
