#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.ValueObjects.Mensajes;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class MensajeLista : SecuredListPage<MensajeVo>
    {
        #region Protected Properties

        protected override string RedirectUrl { get { return "MensajeAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_MENSAJES"; } }
        protected override string GetRefference() { return "MENSAJE"; }
        protected override bool DuplicateButton { get { return true; } }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override List<MensajeVo> GetListData()
        {
            var tipo = cbTipoMensaje.Selected > 0 ? DAOFactory.TipoMensajeDAO.FindById(cbTipoMensaje.Selected) : null;
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;

            var empresa = linea != null ? linea.Empresa :
                cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
                
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            return (from Mensaje mensaje in DAOFactory.MensajeDAO.FindByTipo(tipo, empresa, linea, user) select new MensajeVo(mensaje)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MensajeVo dataItem)
        {
            e.Row.BackColor = GetBackgroundColor(dataItem);
        }

        protected override void Duplicate()
        {
            var mensajes = GetListData().Where(m => !m.IsGeneric).Select(m => m.Id).ToList();

            Session.Add("Mensajes", mensajes);

            OpenWin(String.Concat(ApplicationPath, "Parametrizacion/MensajeDuplicador.aspx"), "Mensajes", 130, 600);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the background color associated to the message depending if its a parent or child message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private Color GetBackgroundColor(MensajeVo message)
        {
            if (!message.IsGeneric) return Color.Yellow;
            if (message.IsParent) return Color.LightGreen;

            return !DAOFactory.MensajeDAO.HasParent(message.Id, message.Codigo) ? Color.LightCoral : Color.LightGray;
        }

        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            var tipo = data[FilterData.StaticTipoMensaje];
            if (empresa != null) cbEmpresa.SetSelectedValue((int)empresa);
            if (linea != null) cbLinea.SetSelectedValue((int)linea);
            if (tipo != null) cbTipoMensaje.SetSelectedValue((int)tipo);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticTipoMensaje, cbTipoMensaje.Selected);
            return data;
        }
    }
}