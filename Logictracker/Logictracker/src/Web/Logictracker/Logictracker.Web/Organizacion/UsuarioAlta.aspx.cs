using System;
using System.Linq;
using System.Text.RegularExpressions;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Organizacion;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.Controls;
using Logictracker.Web.Helpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Logictracker.DAL.DAO.BaseClasses;
namespace Logictracker.Organizacion
{
    public partial class AltaUsuario : SecuredAbmPage<Usuario>
    {
        protected override string VariableName { get { return "SOC_USUARIOS"; } }

        protected override string RedirectUrl { get { return "UsuarioLista.aspx"; } }

        protected override string GetRefference() { return "USUARIO"; }


        #region Protected Methods

        /// <summary>
        /// Binds initial values.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if(!IsPostBack)
            {
                BindThemes();
                BindLogos();
            }

            base.OnLoad(e);

            if (IsPostBack || !EditMode) return;

            var usuarioOk = Usuario.AccessLevel >= Types.BusinessObjects.Usuario.NivelAcceso.AdminUser;

            ddlUsoHorario.Enabled = usuarioOk || (!EditObject.InhabilitadoCambiarUso && EditObject.Id == Usuario.Id);
            txtClave.Enabled = txtConfirmacion.Enabled = usuarioOk || (!EditObject.InhabilitadoCambiarPass && EditObject.Id == Usuario.Id);

            tabParametros.Visible = Usuario.IsSecuredAllowed(Securables.ViewUserParameters);
        }

        /// <summary>
        /// Establishes the user T.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbTipoUsuario_PreBind(object sender, EventArgs e) { if (EditMode) cbTipoUsuario.EditValue = Convert.ToInt32(EditObject.Tipo); }

        protected void cbTipoUsuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            if (cbTipoUsuario.Selected <= 2 && Convert.ToInt32(user.Tipo) > 2)
            {
                chkNoCambioUso.Enabled = chkNoCambioPass.Enabled = true;
            }
            else
            {
                chkNoCambioUso.Enabled = chkNoCambioPass.Enabled = false;
                if (Convert.ToInt32(user.Tipo) > 2) chkNoCambioUso.Checked = chkNoCambioPass.Checked = false;
            }
        }

        /// <summary>
        /// Determinate if the user is enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkInhabilitar_changed(object sender, EventArgs e)
        {
            if (!chkInhabilitar.Checked)
            {
                chkExpira.Enabled = true;
                dtpExpira.Enabled = chkExpira.Checked;
            }
            else
            {
                chkExpira.Enabled = false;
                dtpExpira.Enabled = false;
            }
        }

        /// <summary>
        /// Determinate if the user needs a deadline.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkExpira_changed(object sender, EventArgs e) { dtpExpira.Enabled = chkExpira.Checked; }

        /// <summary>
        /// Saves the edited user.
        /// </summary>
        protected override void OnSave()
        {
            short tipo;

            EditObject.Tipo = short.TryParse(cbTipoUsuario.SelectedValue, out tipo) ? tipo : (short)-1;
            EditObject.NombreUsuario = txtUsuario.Text;
            EditObject.Inhabilitado = chkInhabilitar.Checked;
            EditObject.FechaExpiracion = null;
            
            if (!chkInhabilitar.Checked && chkExpira.Checked && dtpExpira.SelectedDate.HasValue) EditObject.FechaExpiracion = dtpExpira.SelectedDate.Value.ToDataBaseDateTime();

            if (!txtClave.Text.Equals(string.Empty)) EditObject.Clave = txtClave.Text;

            EditObject.Entidad = AltaEntidad.GetEntidad(EditObject.Id == 0);
            EditObject.Logo = cbLogo.SelectedValue;
            EditObject.Theme = cbTheme.SelectedValue;

            EditObject.TimeZoneId = ddlUsoHorario.SelectedValue;
            EditObject.Client = ddlGrupoRecursos.SelectedValue;
            EditObject.Culture = cultureSelector.SelectedValue;

            AddProfiles();
            AddEmpresas();
            AddLineas();
            AddTransportistas();
            AddCentros();
            AddTiposMensaje();
            AddCoches();
            AddIpRanges();

            EditObject.PorCoche = !EditObject.Coches.IsEmpty();
            EditObject.PorCentroCostos = !EditObject.CentrosCostos.IsEmpty();
            EditObject.PorTipoMensaje = !EditObject.TiposMensaje.IsEmpty();
            EditObject.PorLinea =  !EditObject.Lineas.IsEmpty();
            EditObject.PorEmpresa = !EditObject.Empresas.IsEmpty();
            EditObject.InhabilitadoCambiarPass = EditObject.Tipo <= 2 && chkNoCambioPass.Checked;
            EditObject.InhabilitadoCambiarUso = EditObject.Tipo <= 2 && chkNoCambioUso.Checked;

            #region Parametros
            var parametros = new List<ParametroUsuario>();
            foreach (C1GridViewRow row in gridParametros.Rows)
            {
                var nombre = (row.FindControl("txtNombre") as TextBox).Text.Trim();
                var valor = (row.FindControl("txtValor") as TextBox).Text.Trim();
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(valor)) continue;
                var param = EditObject.Parametros.FirstOrDefault(p => p.Nombre == nombre)
                    ?? new ParametroUsuario { Nombre = nombre, Valor = valor, Usuario = EditObject };
                param.Valor = valor;
                parametros.Add(param);
            }
            EditObject.Parametros.Clear();
            foreach(var p in parametros) EditObject.Parametros.Add(p); 
            #endregion

            DAOFactory.UsuarioDAO.SaveOrUpdate(EditObject, txtClave.Text.Equals(string.Empty));

            if (EditObject.Id != Usuario.Id) return;

            WebSecurity.UpdateUser(EditObject);            
        }

        /// <summary>
        /// Deletes the user being edited.
        /// </summary>
        protected override void OnDelete() { DAOFactory.UsuarioDAO.Delete(EditObject); }

        /// <summary>
        /// Binds all properties of the user being edited.
        /// </summary>
        protected override void Bind()
        {
            AltaEntidad.SetEntidad(EditObject.Entidad);
            txtUsuario.Text = EditObject.NombreUsuario;

            cbTheme.TrySelect(EditObject.Theme);
            cbLogo.TrySelect(EditObject.Logo);
            ddlUsoHorario.TrySelect(EditObject.TimeZoneId);
            ddlGrupoRecursos.TrySelect(EditObject.Client);

            chkInhabilitar.Checked = EditObject.Inhabilitado;

            if (!EditObject.Inhabilitado)
            {
                chkExpira.Enabled = true;

                if (EditObject.FechaExpiracion != null)
                {
                    chkExpira.Checked = true;
                    dtpExpira.Enabled = true;
                    dtpExpira.SelectedDate = EditObject.FechaExpiracion;
                }
            }
            else chkExpira.Enabled = false;

            chkNoCambioPass.Checked = EditObject.InhabilitadoCambiarPass;
            chkNoCambioUso.Checked = EditObject.InhabilitadoCambiarUso;
            chkNoCambioPass.Enabled = EditObject.Tipo <= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.User && (Usuario.AccessLevel >= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.AdminUser || !EditObject.InhabilitadoCambiarPass);
            chkNoCambioUso.Enabled = EditObject.Tipo <= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.User && (Usuario.AccessLevel >= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.AdminUser || !EditObject.InhabilitadoCambiarUso);

            cultureSelector.SelectedValue = EditObject.Culture;

            ipRanges.SetIpRanges(EditObject.IpRanges);

            var list = EditObject.Perfiles.OfType<Perfil>().Select(p => p.Id).ToList();
            cbPerfil.SetSelectedIndexes(list);

            list = EditObject.Empresas.OfType<Empresa>().Select(p => p.Id).ToList();
            cbEmpresa.SetSelectedIndexes(list);

            list = EditObject.Lineas.OfType<Linea>().Select(p => p.Id).ToList();
            cbLinea.SetSelectedIndexes(list);

            list = EditObject.Transportistas.OfType<Transportista>().Select(p => p.Id).ToList();
            if(EditObject.PorTransportista && EditObject.MostrarSinTransportista) list.Add(cbTransportista.NoneValue);
            cbTransportista.SetSelectedIndexes(list);

            list = EditObject.CentrosCostos.OfType<CentroDeCostos>().Select(c => c.Id).ToList();
            cbCentroCostos.SetSelectedIndexes(list);

            list = EditObject.Coches.OfType<Coche>().Select(p => p.Id).ToList();
            cbVehiculo.SetSelectedIndexes(list);

            list = EditObject.TiposMensaje.OfType<TipoMensaje>().Select(p => p.Id).ToList();
            cbTipoMensaje.SetSelectedIndexes(list);

            gridParametros.DataSource = EditObject.Parametros;
            gridParametros.DataBind();
        }

        /// <summary>
        /// Validates the current edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            if (DAOFactory.UsuarioDAO.FindAll().OfType<Usuario>().Any(u => u.Id != EditObject.Id && u.NombreUsuario.ToLowerInvariant().Equals(txtUsuario.Text.ToLowerInvariant())))
                ThrowDuplicated("Entities", "USUARIO");

            if (txtUsuario.Text == string.Empty) ThrowMustEnter("NOMBRE_USUARIO");

            if (!EditMode && string.IsNullOrEmpty(txtClave.Text)) ThrowMustEnter("PASSWORD");

            var checkPassword = (!txtClave.Text.Equals(string.Empty) || !txtConfirmacion.Text.Equals(string.Empty));

            if (checkPassword && !txtClave.Text.Equals(txtConfirmacion.Text)) ThrowError("PASSWORDS_DONT_MATCH");

            if (chkExpira.Checked && dtpExpira.SelectedDate == null) ThrowMustEnter("FECHA");

            if (!checkPassword && EditMode) return;

            var pass = txtClave.Text;

            if (pass == txtUsuario.Text) throw new ApplicationException("La contraseña no puede ser igual al nombre de usuario");

            CheckPasswordStrenght(pass);
        }

        #region Password Strength
        /// <summary>
        /// work out how strong the text entered into the textbox is for a password
        /// and return an integer in between 1 .. 100 (ie. a percentage value)
        /// 1 = weakest
        /// 100 = strongest
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        /// <remarks>
        /// Note: Password determination involves
        ///       Length (makes up 50% of score)
        ///       Does it contain numbers (makes up 15% of score)
        ///       Does it contain upper and lowercase (makes up 15% of score)
        ///       Does it contain special symbols (makes up 20% of score)
        /// </remarks>
        private int CheckPasswordStrenght(string pwd)
        {
            var percentTotal = 0;

            // Extract the password calculation weighting ratios
            var weights = passStrength.CalculationWeightings.Split(';');

            if (weights.Length != 4) weights = new[] { "50", "15", "15", "20" };

            var ratioLen = Convert.ToInt32(weights[0]);
            var ratioNum = Convert.ToInt32(weights[1]);
            var ratioCas = Convert.ToInt32(weights[2]);
            var ratioSym = Convert.ToInt32(weights[3]);

            //***********************************************
            // Length Criteria
            // Any pwd with a length > 20 is a passphrase and is a good thing
            var ratio = pwd.Length / Math.Max(passStrength.PreferredPasswordLength, 1);
            if (ratio > 1) ratio = 1;

            var lengthStrength = (ratio * ratioLen);

            // Add to our percentage total
            percentTotal += lengthStrength;

            if (ratio < 1) throw new ApplicationException(string.Concat("La contraseña debe tener al menos ", passStrength.PreferredPasswordLength, " caracteres"));

            //***********************************************
            // Numeric Criteria
            // Does it contain numbers?
            if (passStrength.MinimumNumericCharacters > 0)
            {
                var numbersRegex = new Regex("[0-9]");
                var numCount = numbersRegex.Matches(pwd).Count;

                if (numCount >= passStrength.MinimumNumericCharacters) percentTotal += ratioNum;

                if (numCount < passStrength.MinimumNumericCharacters)
                    throw new ApplicationException(string.Concat("La contraseña debe tener al menos ", passStrength.MinimumNumericCharacters + " numeros"));
            }
            else
            {
                // If the user has not specified that the password should contain numerics, then we just figure out the ratio according
                // to the password length as that is always something
                percentTotal += (ratio * ratioNum);
            }

            //***********************************************
            // Casing Criteria
            // Does it contain lowercase AND uppercase Text
            if (passStrength.RequiresUpperAndLowerCaseCharacters)
            {
                var lowercaseRegex = new Regex("[a-z]");
                var uppercaseRegex = new Regex("[A-Z]");

                var numLower = lowercaseRegex.Matches(pwd).Count;
                var numUpper = uppercaseRegex.Matches(pwd).Count;

                if (numLower > 0 || numUpper > 0)
                {
                    if (numLower >= passStrength.MinimumLowerCaseCharacters && numUpper >= passStrength.MinimumUpperCaseCharacters) percentTotal += ratioCas;
                    else throw new ApplicationException(string.Concat("La contraseña debe tener al menos ", passStrength.MinimumLowerCaseCharacters, " letras minusculas y ",
                        passStrength.MinimumUpperCaseCharacters, " letras mayusculas"));
                }
                else throw new ApplicationException("La contraseña debe tener letras mayusculas y minusculas");
            }
            else
            {
                // If the user has not specified that the password should contain numerics, then we just figure out the ratio according
                // to the password length as  that is always something
                percentTotal += (ratio * ratioCas);
            }


            //***********************************************
            // Symbol Criteria
            // Does it contain any special symbols?
            if (passStrength.MinimumSymbolCharacters > 0)
            {
                var symbolRegex = new Regex("[^a-z,A-Z,0-9,\x20]"); // related to work item 1034
                var numCount = symbolRegex.Matches(pwd).Count;
                if (numCount >= passStrength.MinimumSymbolCharacters) percentTotal += ratioSym;

                if (numCount < passStrength.MinimumSymbolCharacters)
                    throw new ApplicationException(string.Format("La contraseña debe tener al menos {0} {1}", passStrength.MinimumSymbolCharacters, " simbolos"));

            }
            else
            {
                // If the user has not specified that the password should contain numerics, then we just figure out the ratio according
                // to the password length as that is always something
                percentTotal += (ratio * ratioSym);
            }
            return percentTotal;
        } 
        #endregion

        protected void gridParametros_RowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            var details = e.Row.DataItem as ParametroUsuario;

            if (details == null) return;
            (e.Row.FindControl("txtNombre") as TextBox).Text = details.Nombre;
            (e.Row.FindControl("txtValor") as TextBox).Text = details.Valor;
        }
        protected void btAddParameter_Click(object sender, EventArgs e)
        {
            var parametros = new List<ParametroUsuario>();
            foreach (C1GridViewRow row in gridParametros.Rows)
            {
                var nombre = (row.FindControl("txtNombre") as TextBox).Text.Trim();
                var valor = (row.FindControl("txtValor") as TextBox).Text.Trim();
                var param = new ParametroUsuario { Nombre = nombre, Valor = valor};
                parametros.Add(param);
            }
            parametros.Add(new ParametroUsuario());
            gridParametros.DataSource = parametros;
            gridParametros.DataBind();
        }
        #endregion

        #region Private Methods

        private void AddIpRanges()
        {
            var dict = EditObject.IpRanges.OfType<IpRange>().ToDictionary(p => p.IpFrom + ";" + p.IpTo);
            var list = ipRanges.GetIpRanges();

            EditObject.IpRanges.Clear();

            foreach (var range in list)
            {
                var key = range.IpFrom + ";" + range.IpTo;

                if (dict.ContainsKey(key)) EditObject.IpRanges.Add(dict[key]);
                else 
                {
                    range.Usuario = EditObject;
                    EditObject.IpRanges.Add(range);
                }
            }
        }

        /// <summary>
        /// Add the asigned profiles to the user.
        /// </summary>
        private void AddProfiles()
        {
            var list = cbPerfil.SelectedValues;
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var perfiles = user.Perfiles.Cast<Perfil>().ToList();

            EditObject.ClearPerfiles();

            if (list.Count == 0 || list.Contains(-1) || list.Contains(0)) foreach (var perfil in perfiles) EditObject.AddPerfil(perfil);
            else foreach (var id in list.Where(id => id > 0)) EditObject.AddPerfil(DAOFactory.PerfilDAO.FindById(id));
        }

        /// <summary>
        /// Adds tyhe asigned mobiles to the user
        /// </summary>
        private void AddCoches()
        {
            var list = cbVehiculo.SelectedValues;
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var coches = user.Coches.Cast<Coche>().ToList();

            EditObject.ClearCoches();

            if (list.Count == 0 || list.Contains(-1) || list.Contains(0)) foreach (var coche in coches) EditObject.AddCoche(coche);
            else foreach (var id in list.Where(id => id > 0)) EditObject.AddCoche(DAOFactory.CocheDAO.FindById(id));
        }

        /// <summary>
        /// Adds the assigned Transportistas to the user.
        /// </summary>
        private void AddTransportistas()
        {
            var list = cbTransportista.SelectedValues;
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var transp = user.Transportistas.Cast<Transportista>().ToList();

            

            var todos = (list.Count == 0 || list.Contains(-1) || list.Contains(0));
            var porTransportista = !todos || user.PorTransportista;
            var sinTransportista = (todos || list.Contains(cbTransportista.NoneValue)) && user.MostrarSinTransportista;


            /*si elige el todos se le asigna los maximos transportistas que podia ver el usuario con el que se esta haceidno la asignacion*/
            
            EditObject.ClearTransportistas();
            if (todos) foreach (var t in transp) EditObject.AddTransportista(t);
            else foreach (var id in list.Where(i => i > 0)) EditObject.AddTransportista(DAOFactory.TransportistaDAO.FindById(id));

            EditObject.MostrarSinTransportista = sinTransportista;
            EditObject.PorTransportista = porTransportista;
        }

        /// <summary>
        /// Adds the assigned Centros to the user.
        /// </summary>
        private void AddCentros()
        {
            var list = cbCentroCostos.SelectedValues;
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var centros = user.CentrosCostos.Cast<CentroDeCostos>().ToList();

            EditObject.ClearCentros();

            /*si elige el todos se le asigna los maximos centros que podia ver el usuario con el que se esta haceidno la asignacion*/
            if (!list.Any() || list.Contains(-1) || list.Contains(0))
            {
                foreach (var c in centros) EditObject.AddCentro(c);
                return;
            }

            foreach (var id in list) EditObject.AddCentro(DAOFactory.CentroDeCostosDAO.FindById(id));
        }

        private void AddTiposMensaje()
        {
            var list = cbTipoMensaje.SelectedValues;
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var tiposMensaje = user.TiposMensaje.Cast<TipoMensaje>().ToList();

            EditObject.ClearTiposMensaje();

            /*si elige el todos se le asigna los maximos tipos de mensaje que podia ver el usuario con el que se esta haciendo la asignacion*/
            if (!list.Any() || list.Contains(-1) || list.Contains(0))
            {
                foreach (var t in tiposMensaje) EditObject.AddTipoMensaje(t);
                return;
            }

            foreach (var id in list) EditObject.AddTipoMensaje(DAOFactory.TipoMensajeDAO.FindById(id));
        }

        /// <summary>
        /// Adds the assigned Lineas to the user.
        /// </summary>
        private void AddLineas()
        {
            var list = cbLinea.SelectedValues;
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var lines = user.Lineas.Cast<Linea>().ToList();

            EditObject.ClearLineas();

            //Si es el mismo usuario y no selecciono nada, deja lo que ya tenia asignado
            if (list.Count == 0 || list.Contains(-1) || list.Contains(0)) foreach (var linea in lines) EditObject.AddLinea(linea);
            else foreach (var id in list.Where(id => id > 0)) EditObject.AddLinea(DAOFactory.LineaDAO.FindById(id));
        }

        /// <summary>
        /// Adds the assigned Empresas to the user.
        /// </summary>
        private void AddEmpresas()
        {
            var list = cbEmpresa.SelectedValues;
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var empresas = user.Empresas.Cast<Empresa>().ToList();

            EditObject.ClearEmpresas();

            //Si es el mismo usuario y no selecciono nada, deja lo que ya tenia asignado
            if (list.Count <= 0 || list.Contains(-1) || list.Contains(0)) foreach (var empresa in empresas) EditObject.AddEmpresa(empresa);
            else foreach (var id in list.Where(id => id > 0)) EditObject.AddEmpresa(DAOFactory.EmpresaDAO.FindById(id));
        }

        private void BindLogos()
        {
            cbLogo.DataSource = ThemeManager.GetLogos();
            cbLogo.DataBind();
            cbLogo.Items.Insert(0,"");
        }

        private void BindThemes()
        {
            cbTheme.DataSource = ThemeManager.GetThemes();
            cbTheme.DataBind();
            cbTheme.Items.Insert(0, "");
        }

        #endregion
    }
}
