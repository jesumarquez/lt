#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class Parametrizacion_CaudalimetroLista : SecuredListPage<CaudalimetroVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_CAUDALIMETRO"; } }
        protected override string RedirectUrl { get { return "CaudalimetroAlta.aspx"; } }
        protected override string GetRefference() { return "CAUDALIMETRO"; }

        #endregion

        #region Protected Methods

        protected override List<CaudalimetroVo> GetListData()
        {
            var equipo = cbEquipo.Selected > 0 ? DAOFactory.EquipoDAO.FindById(cbEquipo.Selected) : null;
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            return (from Caudalimetro c in DAOFactory.CaudalimetroDAO.FindByEquipoEmpresaAndLinea(equipo, empresa, linea, false, user)
                    select new CaudalimetroVo(c)).ToList();
        }
        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticEquipo, cbEquipo);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticEquipo, cbEquipo.Selected);
            return data;
        }
    }
}
