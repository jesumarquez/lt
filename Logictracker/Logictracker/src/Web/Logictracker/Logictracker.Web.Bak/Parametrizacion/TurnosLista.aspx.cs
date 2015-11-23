#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionTurnosLista : SecuredListPage<ShiftVo>
    {   
        #region Protected Properties

        protected override string VariableName { get { return "PAR_TURNOS"; } }
        protected override string RedirectUrl { get { return "TurnosAlta.aspx"; } }
        protected override string GetRefference() { return "PAR_TURNOS"; }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override List<ShiftVo> GetListData()
        {
            var linea = ddlBase.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
            var empresa = linea != null ? linea.Empresa : ddlDistrito.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : null;
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            return DAOFactory.ShiftDAO.FindActive(empresa, linea, user).Select(s=> new ShiftVo(s)).ToList();
        }
        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            if (empresa != null) ddlDistrito.SetSelectedValue((int)empresa);
            if (linea != null) ddlBase.SetSelectedValue((int)linea);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlDistrito.Selected);
            data.AddStatic(FilterData.StaticBase, ddlBase.Selected);
            return data;
        }
    }
}
