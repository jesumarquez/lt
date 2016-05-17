#region Usings

using System;
using System.Security.Cryptography;
using System.Text;
using Logictracker.Types.BusinessObjects.Postal;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion.Postal
{
    public partial class DistribuidorAlta : SecuredAbmPage<Distribuidor>
    {
        #region Protected Properties

        /// <summary>
        /// Report title.
        /// </summary>
        protected override string VariableName { get { return "PAR_DISTRIBUIDOR"; } }

        /// <summary>
        /// Associated list page url.
        /// </summary>
        protected override string RedirectUrl { get { return "DistribuidorLista.aspx"; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "DISTRIBUIDOR"; }
    
        /// <summary>
        /// Binds the data associated to the distributor beeing edited.
        /// </summary>
        protected override void Bind()
        {
            txtCodigo.Text = EditObject.Codigo;
            txtNombre.Text = EditObject.Nombre;
            txtUsuario.Text = EditObject.Usuario;
        }

        /// <summary>
        /// Deletes the current distributor.
        /// </summary>
        protected override void OnDelete() { DAOFactory.DistribuidorDAO.Delete(EditObject); }

        /// <summary>
        /// Saves the changes made tot he current distributor.
        /// </summary>
        protected override void OnSave()
        {
            if (!String.IsNullOrEmpty(txtClave.Text)) EditObject.Clave = GetMd5(txtClave.Text);

            EditObject.Codigo = txtCodigo.Text;
            EditObject.Nombre = txtNombre.Text;
            EditObject.Usuario = txtUsuario.Text;
            EditObject.FechaModificacion = DateTime.UtcNow;

            DAOFactory.DistribuidorDAO.SaveOrUpdate(EditObject);
        }

        /// <summary>
        /// Validates the distributor before saving it into database.
        /// </summary>
        protected override void ValidateSave()
        {
            var codigo = ValidateEmpty(txtCodigo.Text, "CODE");

            ValidateEmpty(txtNombre.Text, "NAME");

            var usuario = ValidateEmpty(txtUsuario.Text, "USER");

            ValidatePassword();

            var byUsuario = DAOFactory.DistribuidorDAO.FindByUsuario(usuario);
            ValidateDuplicated(byUsuario, "USER");

            var byCode = DAOFactory.DistribuidorDAO.FindByCodigo(codigo);
            ValidateDuplicated(byCode, "CODE");

            if (EditMode) return;

            CheckPasswordStrengh();
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// Validates that the specified password complains with the defined security polices.
        /// </summary>
        private void CheckPasswordStrengh()
        {
            var pass = txtClave.Text;

            if (pass == txtUsuario.Text) throw new ApplicationException("La contraseña no puede ser igual al nombre de usuario");
        }

        /// <summary>
        /// Checks that the givenn password is a valid one.
        /// </summary>
        private void ValidatePassword()
        {
            if (!EditMode && String.IsNullOrEmpty(txtClave.Text)) ThrowMustEnter("PASSWORD");

            var checkPassword = (!txtClave.Text.Equals(String.Empty) || !txtConfirmacion.Text.Equals(String.Empty));

            if (checkPassword && !txtClave.Text.Equals(txtConfirmacion.Text)) ThrowError("PASSWORDS_DONT_MATCH");
        }

        /// <summary>
        /// Gets the md5 hash code associated to the givenn input.
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        private static String GetMd5(String strSource)
        {
            var encoding = new ASCIIEncoding();

            var ary = encoding.GetBytes(strSource);

            return GetMd5(ary);
        }

        /// <summary>
        /// Gets the md5 hash code associated to the givenn input.
        /// </summary>
        /// <param name="ary"></param>
        /// <returns></returns>
        private static String GetMd5(Byte[] ary)
        {
            var md5 = new MD5CryptoServiceProvider();

            var hash = md5.ComputeHash(ary);
            var hashValue = "";

            foreach (var e in hash)
                if (e <= 15) hashValue += "0" + e.ToString("X");
                else hashValue += e.ToString("X");

            return hashValue;
        }

        #endregion
    }
}
