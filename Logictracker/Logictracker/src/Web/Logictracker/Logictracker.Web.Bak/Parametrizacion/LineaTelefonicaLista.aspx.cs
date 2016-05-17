using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class LineaTelefonicaLista : SecuredListPage<LineaTelefonicaVo>
    {
        protected override string RedirectUrl { get { return "LineaTelefonicaAlta.aspx"; } }

        protected override string ImportUrl { get { return "LineaTelefonicaImport.aspx"; } }

        protected override string VariableName { get { return "PAR_LINEA_TELEFONICA"; } }

        protected override string GetRefference() { return "PAR_LINEA_TELEFONICA"; }
        
        protected override bool ExcelButton { get { return true; } }

        protected override bool ImportButton { get { return true; } }
        
        protected override List<LineaTelefonicaVo> GetListData()
        {
            var list = DAOFactory.LineaTelefonicaDAO.GetList(new[] { cbEmpresa.Selected });

            return list.Select(v => new LineaTelefonicaVo(v)).ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticEmpresaTelefonica, cbEmpresa);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticEmpresaTelefonica, cbEmpresa.Selected);
            return data;
        }

        #endregion
    }
}
