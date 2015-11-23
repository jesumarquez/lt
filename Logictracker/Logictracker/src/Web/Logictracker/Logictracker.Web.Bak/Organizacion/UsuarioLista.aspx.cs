using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.Organizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Organizacion
{
    public partial class ListaUsuario : SecuredListPage<UsuarioVo>
    {
        protected override string VariableName { get { return "SOC_USUARIOS"; } }
        protected override string RedirectUrl { get { return "UsuarioAlta.aspx"; } }
        protected override string GetRefference() { return "USUARIO"; }
        protected override bool ExcelButton { get { return true; } }

        #region Protected Methods

        protected override List<UsuarioVo> GetListData()
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var usuarios = DAOFactory.UsuarioDAO.FindByUsuario(user).ToList();
            if (cbEmpresa.Selected > 0)
            {
                usuarios = usuarios
                    .Where(u => u.Empresas.Count == 0 || u.Empresas.OfType<Empresa>()
                        .Select(e => e.Id).Contains(cbEmpresa.Selected))
                        .ToList();
            }

            return usuarios.Select(u => new UsuarioVo(u)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, UsuarioVo dataItem)
        {
            if (!dataItem.Inhabilitado) return;
            e.Row.BackColor = Color.Firebrick;
            e.Row.ForeColor = Color.White;
        }
        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            return data;
        }
    }
}