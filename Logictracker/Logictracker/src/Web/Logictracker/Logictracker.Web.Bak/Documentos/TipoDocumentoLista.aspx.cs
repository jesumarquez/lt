using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Documentos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Documentos
{
    public partial class Documentos_TipoDocumentoLista : SecuredListPage<TipoDocumentoVo>
    {
        protected override string VariableName { get { return "DOC_TIPO_DOCUMENTO"; } }
        protected override string RedirectUrl { get { return "TipoDocumentoAlta.aspx"; } }
        protected override string GetRefference() { return "TIPODOCUMENTO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TipoDocumentoVo> GetListData()
        {
            return DAOFactory.TipoDocumentoDAO.GetList(new[]{cbEmpresa.Selected}, new[]{cbLinea.Selected})
                .Select(t => new TipoDocumentoVo(t))
                .ToList();
        }

    }
}
