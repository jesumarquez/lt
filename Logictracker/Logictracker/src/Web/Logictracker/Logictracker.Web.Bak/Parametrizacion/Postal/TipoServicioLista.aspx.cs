#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion.Postal;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion.Postal
{
    public partial class TipoServicioLista : SecuredListPage<TipoServicioVo>
    {
        protected override string VariableName { get { return "PAR_TIPO_SERVICIO"; } }
        protected override string RedirectUrl { get { return "TipoServicioAlta.aspx"; } }
        protected override string GetRefference() { return "TIPOSERVICIO"; }

        protected override List<TipoServicioVo> GetListData()
        {
            return DAOFactory.TipoServicioDAO.FindAll().Select(t => new TipoServicioVo(t)).ToList();
        }
    }
}
