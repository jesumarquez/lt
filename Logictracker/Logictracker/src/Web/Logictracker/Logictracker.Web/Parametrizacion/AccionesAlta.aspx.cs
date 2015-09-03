using System;
using Logictracker.Configuration;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionAccionAlta : SecuredAbmPage<Accion>
    {
        protected override string RedirectUrl { get { return "AccionLista.aspx"; } }
        protected override string VariableName { get { return "PAR_ACCIONES"; } }
        protected override string GetRefference() { return "ACCION"; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) EmbedSoundPlayer();

            if (IsPostBack || EditMode) return;
            
            panelPopup.Visible = panelSonido.Visible = panelMail.Visible = panelSms.Visible = 
                panelHabilita.Visible = panelInhabilita.Visible = panelChangeIcon.Visible = panelAssistCargo.Visible = false;

            lblReportaCentroDeCosto.Visible = lblReportaDepartamento.Visible =
                chkReportaCentroDeCosto.Visible = chkReportaDepartamento.Visible = false;
        }

        protected override void Bind()
        {
            ddlDistrito.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : ddlDistrito.AllValue);
            ddlBase.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : ddlBase.AllValue);
            ddlMensaje.SetSelectedValue(EditObject.Mensaje != null ? Convert.ToInt32(EditObject.Mensaje.Codigo) : ddlMensaje.NoneValue);
            ddlTipoVehiculo.SetSelectedValue(EditObject.TipoVehiculo != null ? EditObject.TipoVehiculo.Id : ddlTipoVehiculo.AllValue);
            ddlTransportista.SetSelectedValue(EditObject.Transportista != null ? EditObject.Transportista.Id : ddlTransportista.AllValue);
            ddlDepartamento.SetSelectedValue(EditObject.Departamento != null ? EditObject.Departamento.Id : ddlDepartamento.AllValue);
            ddlCentroDeCosto.SetSelectedValue(EditObject.CentroDeCostos != null ? EditObject.CentroDeCostos.Id : ddlCentroDeCosto.AllValue);
            ddlTipoPOI.SetSelectedValue(EditObject.TipoReferenciaGeografica != null ? EditObject.TipoReferenciaGeografica.Id : ddlTipoPOI.AllValue);

            if (ddlDepartamento.Selected > 0)
            {
                lblReportaDepartamento.Visible = chkReportaDepartamento.Visible = true;
                chkReportaDepartamento.Checked = EditObject.ReportaDepartamento;
            }
            if (ddlCentroDeCosto.Selected > 0)
            {
                lblReportaCentroDeCosto.Visible = chkReportaCentroDeCosto.Visible = true;
                chkReportaCentroDeCosto.Checked = EditObject.ReportaCentroDeCostos;
            }

            ddlUsuarioHabilitado.SetSelectedValue(EditObject.UsuarioHabilitado != null ? EditObject.UsuarioHabilitado.Id : ddlUsuarioHabilitado.NullValue);
            ddlUsuarioInhabilitado.SetSelectedValue(EditObject.UsuarioInhabilitado != null ? EditObject.UsuarioInhabilitado.Id : ddlUsuarioInhabilitado.NullValue);

            if (EditObject.Sonido != null) cbSonido.SetSelectedValue(EditObject.Sonido.Id);
            txtDescripcion.Text = EditObject.Descripcion;
            cpColor.Color = EditObject.RGB;
            npAlpha.Value = EditObject.Alfa;

            chkPopUp.Checked = panelPopup.Visible = EditObject.EsPopUp;
            
            chkRequiereAtencion.Checked = EditObject.RequiereAtencion;
            cbPerfil.SetSelectedValue(EditObject.PerfilHabilitado != null ? EditObject.PerfilHabilitado.Id : cbPerfil.AllValue);
            
            txtTituloPopUp.Text = EditObject.PopUpTitle;

            selectIcon.Selected = EditObject.PopIcon;

            chkGrabaEnBase.Checked = EditObject.GrabaEnBase;

            txtCondicion.Text = EditObject.Condicion;

            chkAlarmaSonora.Checked = panelSonido.Visible = EditObject.EsAlarmaSonora;

            chkEnviaMails.Checked = panelMail.Visible = EditObject.EsAlarmaDeMail;
            txtAsuntoMail.Text = EditObject.AsuntoMail;
            txtDestinatariosMails.Text = EditObject.DestinatariosMail;

            chkEnviaSMS.Checked = panelSms.Visible = EditObject.EsAlarmaSms;
            txtDestinatariosSMS.Text = EditObject.DestinatariosSms;

            chkHabilita.Checked = panelHabilita.Visible = EditObject.Habilita;

            if (EditObject.Habilita) npHorasHabilitado.Value = EditObject.HorasHabilitado;

            chkInhabilita.Checked = panelInhabilita.Visible = EditObject.Inhabilita;

            chkChangeIcon.Checked = panelChangeIcon.Visible = EditObject.ModificaIcono;
            isChangeIcon.Selected = EditObject.IconoMensaje;

            chkEvaluaGeocerca.Checked = EditObject.EvaluaGeocerca;
            radDentro.Checked = !EditObject.EvaluaGeocerca || EditObject.DentroGeocerca;
            radFuera.Checked = !radDentro.Checked;

            chkCambiaMensaje.Checked = panelCambiaMensaje.Visible = EditObject.CambiaMensaje;
            txtMensajeACambiar.Text = EditObject.MensajeACambiar;

            chkPideFoto.Checked = EditObject.PideFoto;
            txtSegundosFoto.Text = EditObject.SegundosFoto.ToString("#0");

            chkAssistCargo.Checked = panelAssistCargo.Visible = EditObject.ReportarAssistCargo;
            txtCodigoAssistCargo.Text = EditObject.CodigoAssistCargo;

            if (EditObject.TipoGeocerca != null) cbTipoGeocerca.SetSelectedValue(EditObject.TipoGeocerca.Id);

            chkReporte.Checked = EditObject.EnviaReporte;
            //cbReporte.SetSelectedValue(EditObject.Reporte);
            txtMailReporte.Text = EditObject.DestinatariosMailReporte;
        }

        protected override void OnDelete() { DAOFactory.AccionDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    EditObject.Linea = ddlBase.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
                    EditObject.Empresa = ddlDistrito.Selected > 0
                        ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected)
                        : EditObject.Linea != null ? EditObject.Linea.Empresa : null;
                    EditObject.Mensaje =
                        DAOFactory.MensajeDAO.FindById(
                            DAOFactory.MensajeDAO.GetByCodigo(ddlMensaje.Selected.ToString("#0"), EditObject.Empresa, EditObject.Linea).Id);
                    EditObject.Descripcion = txtDescripcion.Text;
                    EditObject.GrabaEnBase = chkGrabaEnBase.Checked;

                    EditObject.EvaluaGeocerca = chkEvaluaGeocerca.Checked;
                    EditObject.DentroGeocerca = !EditObject.EvaluaGeocerca || radDentro.Checked;
                    EditObject.TipoGeocerca = EditObject.EvaluaGeocerca ? DAOFactory.TipoReferenciaGeograficaDAO.FindById(cbTipoGeocerca.Selected) : null;

                    if (cpColor.Color != string.Empty) EditObject.RGB = cpColor.Color;

                    EditObject.Alfa = (byte) npAlpha.Value;

                    SetFilterAttributes();

                    EditObject.Condicion = txtCondicion.Text;

                    SetPupUpAttributes();

                    SetSoundAttributes();

                    SetEmailAttributes();

                    SetSmsAttributes();

                    SetEnabledUserAttributes();

                    SetDisabledUserAttributes();

                    SetChangeIconAttributes();

                    SetEnviaReporteAttributes();

                    #region SetFotoAttributes

                    EditObject.PideFoto = chkPideFoto.Checked;

                    if (EditObject.PideFoto)
                    {
                        EditObject.SegundosFoto = Convert.ToInt32(txtSegundosFoto.Text.Trim());
                    }

                    #endregion

                    #region SetCambioMensajeAttributes

                    EditObject.CambiaMensaje = chkCambiaMensaje.Checked;

                    if (EditObject.CambiaMensaje)
                    {
                        EditObject.MensajeACambiar = txtMensajeACambiar.Text.Trim();
                    }

                    #endregion

                    #region SetAssistCargoAttributes

                    EditObject.ReportarAssistCargo = chkAssistCargo.Checked;
                    EditObject.CodigoAssistCargo = txtCodigoAssistCargo.Text.Trim();

                    #endregion

                    DAOFactory.AccionDAO.SaveOrUpdate(EditObject);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        protected override void ValidateSave()
        {
            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");

            if (chkAssistCargo.Checked)
            {
                ValidateEmpty(txtCodigoAssistCargo.Text, "CODIGO_ASSISTCARGO");
            }

            if (chkPideFoto.Checked)
            {
                var max = Config.MaxPictureTime;
                var segundosTxt = ValidateEmpty(txtSegundosFoto.Text, "SEGUNDOS_FOTO");
                var segundos = ValidateInt32(segundosTxt, "SEGUNDOS_FOTO");
                if (segundos < 1 || segundos > max) ThrowError("BAD_SEGUNDOS_FOTO", max);
            }

            if (chkCambiaMensaje.Checked)
            {
                ValidateEmpty(txtMensajeACambiar.Text, "MENSAJE_A_CAMBIAR");
            }
        }

        protected void LnkSonidoClick(object sender, EventArgs e)
        {
            if (cbSonido.SelectedIndex < 0) return;
            var sound = DAOFactory.SonidoDAO.FindById(Convert.ToInt32(cbSonido.SelectedValue));
            var path = string.Concat(Config.Directory.SoundsDir, sound.URL);
            PlaySound(path);
        }

        protected void DdlDepartamentoSelectedIndexChanged(object sender, EventArgs e)
        {
            lblReportaDepartamento.Visible = chkReportaDepartamento.Visible = ddlDepartamento.Selected > 0;
            if (ddlDepartamento.Selected <= 0) chkReportaDepartamento.Checked = false;
        }

        protected void DdlCentroDeCostoSelectedIndexChanged(object sender, EventArgs e)
        {
            lblReportaCentroDeCosto.Visible = chkReportaCentroDeCosto.Visible = ddlCentroDeCosto.Selected > 0;
            if (ddlCentroDeCosto.Selected <= 0) chkReportaCentroDeCosto.Checked = false;
        }

        #region Conditions Panel Events

        protected void BtnExcesoClick(object sender, EventArgs e) { AppendToConditions("[Exceso]"); }

        protected void BtnDuracionClick(object sender, EventArgs e) { AppendToConditions("[Duracion]"); }

        protected void BtnAndClick(object sender, EventArgs e) { AppendToConditions("&&"); }

        protected void BtnOrClick(object sender, EventArgs e) { AppendToConditions("||"); }

        #endregion

        #region Behaviour CheckBoxs

        protected void ChkComportamientoCheckedChanged(object sender, EventArgs e)
        {
            panelSonido.Visible = chkAlarmaSonora.Checked;
            panelMail.Visible = chkEnviaMails.Checked;
            panelSms.Visible = chkEnviaSMS.Checked;
            panelHabilita.Visible = chkHabilita.Checked;
            panelInhabilita.Visible = chkInhabilita.Checked;
            panelPopup.Visible = chkPopUp.Checked;
            panelCambiaMensaje.Visible = chkCambiaMensaje.Checked;
            panelPideFoto.Visible = chkPideFoto.Checked;
            panelChangeIcon.Visible = chkChangeIcon.Checked;
            panelAssistCargo.Visible = chkAssistCargo.Checked;
            panel1.Visible = chkReporte.Checked;
        }

        #endregion

        #region Private Methods

        private void AppendToConditions(String condition) { txtCondicion.Text = String.Concat(txtCondicion.Text, condition, " "); }

        private void SetChangeIconAttributes()
        {
            EditObject.ModificaIcono = chkChangeIcon.Checked && isChangeIcon.Selected > 0;

            if (!EditObject.ModificaIcono) return;

            EditObject.IconoMensaje = isChangeIcon.Selected;
            EditObject.PathIconoMensaje = DAOFactory.IconoDAO.FindById(isChangeIcon.Selected).PathIcono;
        }

        private void SetFilterAttributes()
        {
            EditObject.TipoVehiculo = ddlTipoVehiculo.Selected > 0 ? DAOFactory.TipoCocheDAO.FindById(ddlTipoVehiculo.Selected) : null;
            EditObject.Transportista = ddlTransportista.Selected > 0 ? DAOFactory.TransportistaDAO.FindById(ddlTransportista.Selected) : null;
            EditObject.Departamento = ddlDepartamento.Selected > 0 ? DAOFactory.DepartamentoDAO.FindById(ddlDepartamento.Selected) : null;
            EditObject.CentroDeCostos = ddlCentroDeCosto.Selected > 0 ? DAOFactory.CentroDeCostosDAO.FindById(ddlCentroDeCosto.Selected) : null;
            EditObject.TipoReferenciaGeografica = ddlTipoPOI.Selected > 0 ? DAOFactory.TipoReferenciaGeograficaDAO.FindById(ddlTipoPOI.Selected) : null;

            EditObject.ReportaDepartamento = chkReportaDepartamento.Checked;
            EditObject.ReportaCentroDeCostos = chkReportaCentroDeCosto.Checked;
        }

        private void SetEnabledUserAttributes()
        {
            EditObject.Habilita = chkHabilita.Checked;

            if (!chkHabilita.Checked) return;

            EditObject.HorasHabilitado = npHorasHabilitado.Value> 0 ? Convert.ToInt32(npHorasHabilitado.Value) : 0;
            EditObject.UsuarioHabilitado = DAOFactory.UsuarioDAO.FindById(ddlUsuarioHabilitado.Selected);
        }

        private void SetDisabledUserAttributes()
        {
            EditObject.Inhabilita = chkInhabilita.Checked;

            if (!chkInhabilita.Checked) return;

            EditObject.UsuarioInhabilitado = DAOFactory.UsuarioDAO.FindById(ddlUsuarioInhabilitado.Selected);
        }

        private void SetSmsAttributes()
        {
            EditObject.EsAlarmaSms = chkEnviaSMS.Checked;

            if (!EditObject.EsAlarmaSms) return;

            EditObject.DestinatariosSms = txtDestinatariosSMS.Text;
        }

        private void SetEmailAttributes()
        {
            EditObject.EsAlarmaDeMail = chkEnviaMails.Checked;

            if (!EditObject.EsAlarmaDeMail) return;

            EditObject.AsuntoMail = txtAsuntoMail.Text;
            EditObject.DestinatariosMail = txtDestinatariosMails.Text;
        }

        private void SetSoundAttributes()
        {
            EditObject.EsAlarmaSonora = chkAlarmaSonora.Checked;

            if (!EditObject.EsAlarmaSonora) return;

            if (cbSonido.Selected != cbSonido.NoneValue) EditObject.Sonido = DAOFactory.SonidoDAO.FindById(cbSonido.Selected);
        }

        private void SetPupUpAttributes()
        {
            EditObject.EsPopUp = chkPopUp.Checked;
            EditObject.RequiereAtencion = chkPopUp.Checked && chkRequiereAtencion.Checked;
            EditObject.PerfilHabilitado = chkPopUp.Checked && cbPerfil.Selected > 0
                                              ? DAOFactory.PerfilDAO.FindById(cbPerfil.Selected)
                                              : null;

            if (!EditObject.EsPopUp) return;

            EditObject.PopUpTitle = txtTituloPopUp.Text;

            if (selectIcon.Selected > 0) EditObject.PopIcon = selectIcon.Selected;
        }

        private void SetEnviaReporteAttributes()
        {
            EditObject.EnviaReporte = chkReporte.Checked;
            EditObject.Reporte = chkReporte.Checked && cbReporte.Selected > 0
                                    ? cbReporte.SelectedItem.Text : null;
            EditObject.DestinatariosMailReporte = chkReporte.Checked ? txtDestinatariosMails.Text : null;
        }

        #endregion
    }
}