using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ModeloLista : SecuredListPage<ModeloVo>
    {
        protected override string RedirectUrl { get { return "ModeloAlta.aspx"; } }
        protected override string ImportUrl { get { return "ModeloImport.aspx"; } }
        protected override string VariableName { get { return "PAR_MODELOS"; } }
        protected override string GetRefference() { return "PAR_MODELOS"; }
        protected override bool ImportButton { get { return true; } }
        protected override bool ExcelButton { get { return true; } }

        protected override List<ModeloVo> GetListData()
        {
            return DAOFactory.ModeloDAO.GetList(new[] {cbEmpresa.Selected},
                                                new[] {cbLinea.Selected},
                                                new[] {cbMarca.Selected})
                .Select(v => new ModeloVo(v))
                .ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            return data;
        }

        #endregion
    }
}
