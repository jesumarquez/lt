using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class DepartamentoLista : SecuredListPage<DepartamentoVo>
    {
        protected override string VariableName { get { return "PAR_DEPARTAMENTO"; } }
        protected override string RedirectUrl { get { return "DepartamentoAlta.aspx"; } }
        protected override string GetRefference() { return "PAR_DEPARTAMENTO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<DepartamentoVo> GetListData()
        {
            return DAOFactory.DepartamentoDAO.GetList(new[] { cbEmpresa.Selected }, new[] { cbLinea.Selected })
                    .Select(d => new DepartamentoVo(d))
                    .ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            if (empresa != null) cbEmpresa.SetSelectedValue((int)empresa);
            if (linea != null) cbLinea.SetSelectedValue((int)linea);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            return data;
        }
    }
}