#region Usings

using System;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Organizacion
{
    public partial class AltaFuncion : SecuredAbmPage<Funcion>
    {
        #region Protected Properties

        /// <summary>
        /// The list page associated to the abm.
        /// </summary>
        protected override string RedirectUrl { get { return "FuncionLista.aspx"; } }

        protected override string VariableName { get { return "SOC_FUNCIONES"; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Binds the current function being edited.
        /// </summary>
        protected override void Bind()
        {
            ddlMenuResource.SelectedValue = EditObject.Descripcion;

            cbGrupo.SelectedValue = EditObject.Modulo;

            txtURL.Text = EditObject.Url;
            txtParametros.Text = EditObject.Parametros;
            txtReferencia.Text = EditObject.Ref;
        }

        /// <summary>
        /// System initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbSubSistema_PreBind(object sender, EventArgs e)
        {
            if (EditMode) cbSubSistema.EditValue = EditObject.Sistema != null ? EditObject.Sistema.Id : cbSubSistema.NullValue;
        }

        /// <summary>
        /// Type initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbTipo_PreBind(object sender, EventArgs e) { if (EditMode) cbTipo.EditValue = EditObject.Tipo; }

        /// <summary>
        /// Saves the current function being edited.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Sistema = DAOFactory.SistemaDAO.FindById(Convert.ToInt32(cbSubSistema.SelectedValue));
            EditObject.Descripcion = ddlMenuResource.SelectedValue;
            EditObject.Url = txtURL.Text.Replace('\\', '/');
            EditObject.Parametros = txtParametros.Text;
            EditObject.Ref = txtReferencia.Text;
            EditObject.FechaBaja = null;
            EditObject.Modulo = cbGrupo.SelectedValue;

            DAOFactory.FuncionDAO.SaveOrUpdate(EditObject);
            
            if (Usuario.IdPerfiles.Count == 1 && !Usuario.IdPerfiles.Contains(-1) && !EditMode)
            {
                var perfil = DAOFactory.PerfilDAO.FindById(Usuario.IdPerfiles[0]);

                var movMenu = new MovMenu
                                  {
                                      Alta = true,
                                      Baja = true,
                                      Consulta = true,
                                      Modificacion = true,
                                      VerMapa = true,
                                      Perfil = perfil,
                                      Funcion = EditObject
                                  };

                perfil.AddFuncion(movMenu);
                DAOFactory.PerfilDAO.SaveOrUpdate(perfil);
            }
            
            ReloadUserFunctions();
        }

        /// <summary>
        /// Deletes the current function being edited.
        /// </summary>
        protected override void OnDelete() { DAOFactory.FuncionDAO.Delete(EditObject); }

        /// <summary>
        /// Validates current edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            if (cbSubSistema.SelectedIndex == -1) throw new Exception(string.Format(CultureManager.GetError("NO_SELECTED"), CultureManager.GetEntity("SISTEMA")));

            if (txtReferencia.Text.Contains(",")) throw new Exception(CultureManager.GetError("FUNC_REF_WRONG_CHARS"));
        }

        /// <summary>
        /// Security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "FUNCION"; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reloads user funtions.
        /// </summary>
        private void ReloadUserFunctions()
        {
            Usuario.SetModules(DAOFactory.PerfilDAO.FindMovMenuBySistema(Usuario.IdPerfiles));
            Usuario.SetSecurables(DAOFactory.PerfilDAO.GetAsegurables(Usuario.IdPerfiles));
        }

        #endregion
    }
}