#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion.Postal;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion.Postal
{
    public partial class DistribuidorLista : SecuredListPage<DistribuidorVo>
    {
        protected override string VariableName { get { return "PAR_DISTRIBUIDOR"; } }
        protected override string RedirectUrl { get { return "DistribuidorAlta.aspx"; } }
        protected override string GetRefference() { return "DISTRIBUIDOR"; }

        protected override List<DistribuidorVo> GetListData() { return DAOFactory.DistribuidorDAO.FindAll().Select(t => new DistribuidorVo(t)).ToList(); }
    }
}
