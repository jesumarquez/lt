#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class Parametrizacion_FeriadoLista : SecuredListPage<FeriadoVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "FERIADOS"; } }
        protected override string RedirectUrl { get { return "FeriadoAlta.aspx"; } }
        protected override string GetRefference() { return "FERIADO"; }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override List<FeriadoVo> GetListData()
        {
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var usuario = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            return DAOFactory.FeriadoDAO.FindByEmpresaYLineaAndUser(empresa, linea, usuario).Select(f=>new FeriadoVo(f)).ToList();
        }
        #endregion

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
