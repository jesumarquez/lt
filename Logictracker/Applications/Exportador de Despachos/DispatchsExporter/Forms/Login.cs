#region Usings

using System;
using System.Windows.Forms;
using DispatchsExporter.Types.SessionObejcts;
using Logictracker.DAL.Factories;

#endregion

namespace DispatchsExporter.Forms
{
    public partial class Login : Form
    {
        private readonly DAOFactory daof = new DAOFactory();

        public Login() { InitializeComponent(); }

        private void btnCancelar_Click(object sender, EventArgs e) { Application.Exit(); }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if(txtUser.Text.Equals(String.Empty))
            {
                lblError.Text = "Debe ingresar un usuario";
                return;
            }

            if(txtPassword.Text.Equals(String.Empty))
            {
                lblError.Text = "Debe ingresar una contraseña";
                return;
            }

            if(!ValidateUser()) 
            {
                lblError.Text = "Usuario o contraseña incorrectos";
                txtPassword.Text = String.Empty;
                return;
            }
            var importForm = ImportMenu.Instance;
            Hide();
            importForm.Show();
        }

        private bool ValidateUser()
        {
            Session.user = daof.UsuarioDAO.FindForLogin(txtUser.Text, txtPassword.Text);

            return Session.user != null;
        }
    }
}