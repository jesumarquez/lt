using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Types.ValueObjects.Combustible;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionTanqueLista : SecuredListPage<TanqueVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_TANQUE"; } }
        protected override string RedirectUrl { get { return "TanqueAlta.aspx"; } }
        protected override string GetRefference() { return "TANQUE"; }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override List<TanqueVo> GetListData()
        {
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var distrito = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : linea != null ? linea.Empresa : null;
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            var tanques = DAOFactory.TanqueDAO.FindByEmpresaAndLinea(distrito, linea, user)
                .OfType<Tanque>().Select(t=>new TanqueVo(t)).ToList();
            tanques.AddRange(DAOFactory.TanqueDAO.FindByEquipo(user, distrito, linea, cbEquipo.Selected)
                .OfType<Tanque>().Select(t=>new TanqueVo(t)).ToList());

            return tanques;
        }
        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            var equipo = data[FilterData.StaticEquipo];
            if (empresa != null) cbEmpresa.SetSelectedValue((int)empresa);
            if (linea != null) cbLinea.SetSelectedValue((int)linea);
            if (equipo != null) cbEquipo.SetSelectedValue((int)equipo);
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