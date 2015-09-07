#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionEquipoLista : SecuredListPage<EquipoVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_EQUIPOS"; } }
        protected override string RedirectUrl { get { return "EquipoAlta.aspx"; } }
        protected override string GetRefference() { return "EQUIPO"; }
        protected override bool ExcelButton { get { return true; } }


        #endregion

        #region Protected Methods

        protected override List<EquipoVo> GetListData()
        {
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : linea != null ? linea.Empresa : null;
            var cliente = cbCliente.Selected > 0 ? DAOFactory.ClienteDAO.FindById(cbCliente.Selected) : null;

            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var list = DAOFactory.EquipoDAO.FindByEmpresaLineaYCliente(empresa, linea, cliente, user);

            return (from Equipo e in list orderby e.Descripcion select new EquipoVo(e)).ToList();
        }

        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            var cliente = data[FilterData.StaticCliente];
            if (empresa != null) cbEmpresa.SetSelectedValue((int)empresa);
            if (linea != null) cbLinea.SetSelectedValue((int)linea);
            if (cliente != null) cbCliente.SetSelectedValue((int)cliente);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticCliente, cbCliente.Selected);
            return data;
        }
    }
}