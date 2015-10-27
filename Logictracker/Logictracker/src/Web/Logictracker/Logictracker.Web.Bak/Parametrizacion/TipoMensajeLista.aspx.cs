using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.ValueObjects.Mensajes;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoMensajeLista : SecuredListPage<TipoMensajeVo>
    {
        protected override string VariableName { get { return "PAR_TIPO_MENSAJE"; } }
        protected override string RedirectUrl { get { return "TipoMensajeAlta.aspx"; } }
        protected override string GetRefference() { return "TIPOMENSAJE"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TipoMensajeVo> GetListData()
        {
            var linea = ddlBase.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
            var empresa = ddlDistrito.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : linea != null ? linea.Empresa : null;
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            return DAOFactory.TipoMensajeDAO.FindByEmpresaLineaYUsuario(empresa, linea, user)
                .OfType<TipoMensaje>().Select(t => new TipoMensajeVo(t)).ToList();
        }
        
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