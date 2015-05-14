using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class PlantaLista : SecuredListPage<LineaVo>
    {   
        protected override string VariableName { get { return "PAR_BASE"; } }
        protected override string RedirectUrl { get { return "PlantaAlta.aspx"; } }
        protected override string GetRefference() { return "PLANTA"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<LineaVo> GetListData()
        {
            return DAOFactory.LineaDAO.GetList(new []{ cbEmpresa.Selected })
                .Select(l=>new LineaVo(l)).ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            if (empresa != null) cbEmpresa.SetSelectedValue((int)empresa);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            return data;
        }
    }
}