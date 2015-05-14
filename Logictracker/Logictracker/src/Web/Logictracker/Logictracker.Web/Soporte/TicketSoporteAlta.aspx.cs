using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Mailing;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Support;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Soporte
{
    public partial class SoporteTicketSoporteAlta : SecuredAbmPage<SupportTicket>
    {
        protected override string RedirectUrl { get { return "TicketSoporteLista.aspx"; } }
        protected override string VariableName { get { return "SUP_TICKETSOPORTE"; } }
        protected override string GetRefference() { return "SUPPORT"; }
        protected override bool DeleteButton { get { return false; } }
        private string AttachDirectory { get { return Server.MapPath("Attach"); } }

        private const string Uno = "FSGDTSJALSOPLOKIUYTRENBLSKDTNWQZ";
        private const string Cero = "LLAPSLEMRTAQIEURJKFMNXXCVBXMSLKQ";
        private readonly List<TimeSpan> _tiempoEstado = new List<TimeSpan>();

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            ToolBar.AsyncPostBacks = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (Request.QueryString["t"] != null) ViewState["id"] = DecodeId(Request.QueryString["t"]);

            if (Request.QueryString["v"] != null)
            {
                int id;
                if (int.TryParse(Request.QueryString["v"], out id) && id > 0)
                    cbVehiculo.SetSelectedValue(id);
            }

            BindResoluciones();
            BindTipoProblema();
            BindComboEstados();
            
            SetConditionalUi();

            if (EditMode) return;

            var u = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            txtNombre.Text = u.Entidad.Descripcion;

            dtInicio.SelectedDate = DateTime.UtcNow.ToDisplayDateTime();
        }

        protected override void Bind()
        {
            txtNombre.Text = EditObject.Nombre;
            txtTelefono.Text = EditObject.Telefono;
            txtMail.Text = EditObject.Mail;
            cbTipoProblema.SelectedIndex = EditObject.TipoProblema;

            txtDescripcion.Text = EditObject.Descripcion;
            cbEstado.SelectedIndex = EditObject.CurrentState;
            if (EditObject.NivelObj != null) cbNivel.SetSelectedValue(EditObject.NivelObj.Id);
            BindCategoria();
            if (EditObject.CategoriaObj != null) cbCategoria.SetSelectedValue(EditObject.CategoriaObj.Id);
            if (EditObject.Subcategoria != null) cbSubcategoria.SetSelectedValue(EditObject.Subcategoria.Id);
            txtMailList.Text = EditObject.MailList;
            titPanelIncidencia.Title = string.Concat(CultureManager.GetLabel("INCIDENCE"), " <span style='font-size: 16px;'>#", EditObject.Id, "</span>");
            dtInicio.SelectedDate = EditObject.FechaInicio.ToDisplayDateTime();
            
            txtComentario.Text = string.Empty;

            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbVehiculo.SetSelectedValue(EditObject.Vehiculo != null ? EditObject.Vehiculo.Id : cbVehiculo.NoneValue);
            cbDispositivo.SetSelectedValue(EditObject.Dispositivo != null ? EditObject.Dispositivo.Id : cbDispositivo.NoneValue);

            txtNroParte.Text = EditObject.NroParte;
            txtNroParte.Enabled = string.IsNullOrEmpty(EditObject.NroParte);

            if(!string.IsNullOrEmpty(EditObject.FileName))
            {
                litFiles.Text = @"<ul>";

                var files = EditObject.FileName.Split(';');

                foreach (var file in files) litFiles.Text += string.Format("<li><a href='Attach/{0}' target='_blank'>{0}</a></li>", file);

                litFiles.Text += @"</ul>";
            }

            BindEstados();
            BindChat();

            ShowTiempos();
        }

        protected override void OnDelete() { }

        protected override void OnSave()
        {
            var lastState = EditObject.CurrentState;
            var now = DateTime.UtcNow;
            var usuario = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            EditObject.Nombre = txtNombre.Text.Trim();
            EditObject.Telefono = txtTelefono.Text.Trim();
            EditObject.Mail = txtMail.Text.Trim();
            EditObject.TipoProblema = (short) cbTipoProblema.SelectedIndex;
            EditObject.Descripcion = txtDescripcion.Text.Trim();
            EditObject.Nivel = (short)cbNivel.Selected;
            EditObject.NivelObj = cbNivel.Selected > 0 ? DAOFactory.NivelDAO.FindById(cbNivel.Selected) : null;
            EditObject.CategoriaObj = cbCategoria.Selected > 0 ? DAOFactory.CategoriaDAO.FindById(cbCategoria.Selected) : null;
            EditObject.Subcategoria = cbSubcategoria.Selected > 0 ? DAOFactory.SubCategoriaDAO.FindById(cbSubcategoria.Selected) : null;
            
            EditObject.MailList = txtMailList.Text.Trim();
            if (dtInicio.SelectedDate.HasValue) EditObject.FechaInicio = SecurityExtensions.ToDataBaseDateTime(dtInicio.SelectedDate.Value);
            EditObject.Empresa = cbEmpresa.Selected <= 0 ? null : DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Vehiculo = cbVehiculo.Selected > 0 ? DAOFactory.CocheDAO.FindById(cbVehiculo.Selected) : null;
            EditObject.Dispositivo = cbDispositivo.Selected > 0 ? DAOFactory.DispositivoDAO.FindById(cbDispositivo.Selected) : null;
            EditObject.NroParte = EditMode ? txtNroParte.Text.Trim() : string.Empty;

            if (!EditMode)
            {
                EditObject.Fecha = now;
                EditObject.Usuario = usuario;
            }

            var estado = (short)(!EditMode ? 0 : cbEstado.SelectedIndex);

            if(EditObject.CurrentState == 4 && cbResolucion.SelectedIndex != 0) estado = (short)(cbResolucion.SelectedIndex == 1 ? 5 : 6);

            var detail = new SupportTicketDetail
                             {
                                 Estado = estado,
                                 Fecha = now,
                                 SupportTicket = EditObject,
                                 Usuario = usuario,
                                 Descripcion = Server.HtmlEncode(txtComentario.Text.Trim()).Replace("\r\n", "<br />")
                             };

            EditObject.AddDetail(detail);

            if (lastState != estado)
            {
                var le = DAOFactory.SupportTicketDAO.GetEstados()[lastState];
                var ne = DAOFactory.SupportTicketDAO.GetEstados()[estado];

                if (!string.IsNullOrEmpty(detail.Descripcion)) detail.Descripcion += "<br /><br />";

                detail.Descripcion += string.Concat("[",CultureManager.GetLabel("SUPPORT_STATE_CHANGED"),": ", le, " > ", ne, "]");
                upGridEstados.Update();
            }

            if (filUpload.HasFile)
            {
                CheckDirectory();

                var filename = GetFileName(filUpload.FileName);

                filUpload.SaveAs(Path.Combine(AttachDirectory, filename));

                if(string.IsNullOrEmpty(EditObject.FileName))  EditObject.FileName = filename;
                else EditObject.FileName += ";" + filename;

                if (!string.IsNullOrEmpty(detail.Descripcion)) detail.Descripcion += "<br /><br />";

                detail.Descripcion += string.Concat("[",CultureManager.GetLabel("SUPPORT_FILE_UPLOADED"),": ", filename, "]");
            }

            DAOFactory.SupportTicketDAO.SaveOrUpdate(EditObject);

            InformarPorMail(!EditMode);

            upComentarios.Update();
        }

        protected override void AfterSave()
        {
            Session["Id"] = EditObject.Id;
            Response.Redirect(Request.Path);
        }

        protected override void ValidateSave()
        {
            base.ValidateSave();

            ValidateEntity(cbCategoria.Selected, "AUDSUP03");
            ValidateEntity(cbNivel.Selected, "AUDSUP05");

            if (string.IsNullOrEmpty(txtNombre.Text)) ThrowMustEnter("NOMBRE");
            if (string.IsNullOrEmpty(txtDescripcion.Text)) ThrowMustEnter("DESCRIPCION");
            if (!dtInicio.SelectedDate.HasValue) ThrowMustEnter("FECHA");
            if (dtInicio.SelectedDate.HasValue && dtInicio.SelectedDate.Value > DateTime.UtcNow.ToDisplayDateTime()) ThrowError("INCIDENCE_CANNOT_START_IN_FUTURE");
        }

        protected void GridEstadosItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;

            var detail = e.Row.DataItem as SupportTicketDetail;

            if (detail == null) return;

            e.Row.BackColor = DAOFactory.SupportTicketDAO.GetColoresEstados()[detail.Estado];
            e.Row.Cells[0].Text = detail.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
            e.Row.Cells[1].Text = DAOFactory.SupportTicketDAO.GetEstados()[detail.Estado];

            e.Row.Cells[2].Text = detail.Usuario != null ? detail.Usuario.NombreUsuario : "";

            var duracion = _tiempoEstado[e.Row.RowIndex];

            e.Row.Cells[3].Text = (duracion.Days > 0 ? duracion.Days + "d " : "") + (duracion.Hours > 0 ? duracion.Hours.ToString("00") + "h " : "") + duracion.Minutes.ToString("00m");
        }

        protected void RepChatItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var lblFecha = e.Item.FindControl("lblFecha") as Label;
            var lblUsuario = e.Item.FindControl("lblUsuario") as Label;
            var lblDescripcion = e.Item.FindControl("lblDescripcion") as Label;
            var detail = e.Item.DataItem as SupportTicketDetail;

            if (lblFecha != null && detail != null) lblFecha.Text = detail.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
            if (lblUsuario != null && detail != null) lblUsuario.Text = detail.Usuario.NombreUsuario;
            if (lblDescripcion != null && detail != null) lblDescripcion.Text = detail.Descripcion;
        }

        protected void CbTipoProblemaSelectedIndexChanged(object sender, EventArgs e)
        {
            BindCategoria();
        }

        #endregion

        #region Private Methods

        private void SetConditionalUi()
        {
            var lockEdit = (EditObject.CurrentState == 5 && Usuario.AccessLevel < Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.SysAdmin) || EditObject.CurrentState == 7 || EditObject.CurrentState == 8;

            LockEdit(!lockEdit);

            panelCambioEstado.Visible = Usuario.AccessLevel >= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.SysAdmin;
            panelResolucion.Visible = EditObject.CurrentState == 4;
        }

        private void CheckDirectory() { if(!Directory.Exists(AttachDirectory)) Directory.CreateDirectory(AttachDirectory); }

        private string GetFileName(string filename)
        {
            var name = Path.GetFileNameWithoutExtension(filename);
            var ext = Path.GetExtension(filename);
            var newFilename = filename;
            var count = 2;

            while (File.Exists(Path.Combine(AttachDirectory, newFilename)))
            {
                newFilename = string.Concat(name, "(", count, ")", ext);

                count++;
            }

            return newFilename;
        }

        private void InformarPorMail(bool nuevo)
        {
            try
            {
                var lastState = EditObject.States.OfType<SupportTicketDetail>().Last();

                if (string.IsNullOrEmpty(lastState.Descripcion) && !nuevo) return;

                var mails = new List<string>();

                if (!string.IsNullOrEmpty(EditObject.Mail.Trim())) AddMails(EditObject.Mail, mails);

                if (!string.IsNullOrEmpty(EditObject.MailList.Trim())) AddMails(EditObject.MailList, mails);

                //Send mail
                var ms = new MailSender(Config.Mailing.SupportMailingConfig);

                AddMails(ms.Config.ToAddress, mails);

                foreach (var mail in mails)
                {
                    ms.Config.ToAddress = mail;
                    ms.SendMail(EditObject.Id.ToString("#0"),
                                DAOFactory.SupportTicketDAO.GetEstados()[lastState.Estado],
                                EditObject.Usuario == null ? "" : EditObject.Usuario.NombreUsuario,
                                EditObject.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") + " (GMT " + Usuario.GmtModifier.ToString("#")+")",
                                DAOFactory.SupportTicketDAO.GetNiveles()[EditObject.Nivel],
                                DAOFactory.SupportTicketDAO.GetTiposProblema()[EditObject.TipoProblema],
                                DAOFactory.SupportTicketDAO.GetCategoriasProblemaByTipo(EditObject.TipoProblema)[EditObject.Categoria],
                                EditObject.Descripcion,
                                string.IsNullOrEmpty(lastState.Descripcion) ? "" : lastState.Fecha.ToString("dd/MM/yyyy HH:mm"),
                                string.IsNullOrEmpty(lastState.Descripcion) ? "" : lastState.Usuario.NombreUsuario,
                                string.IsNullOrEmpty(lastState.Descripcion) ? "" : lastState.Descripcion,
                                EncodeId(EditObject.Id),
                                EditObject.Empresa != null ? EditObject.Empresa.RazonSocial : "");
                }
            }
            catch { }
        }

        private static void AddMails(String adress, List<String> mails)
        {
            var m = adress.Trim().Split(';');

            mails.AddRange(from ml in m where !string.IsNullOrEmpty(ml.Trim()) select ml.Trim());
        }

        private void ShowTiempos()
        {
            const string template = "<div style='width:{0}; background-color: {3};padding: 3px; margin: 3px;'><div style='position: relative; width: 200px;'>{1}: {2}</div></div>";

            var atencion = Convert.ToInt32((EditObject.TiempoAtencion.TotalMinutes * 100 / EditObject.TiempoIncidencia.TotalMinutes));
            var resolucion = Convert.ToInt32((EditObject.TiempoResolucion.TotalMinutes * 100 / EditObject.TiempoIncidencia.TotalMinutes));
            var espera = Convert.ToInt32((EditObject.TiempoEsperaCliente.TotalMinutes * 100 / EditObject.TiempoIncidencia.TotalMinutes));
            if (atencion == 0 && EditObject.TiempoAtencion > TimeSpan.Zero) atencion = 1;
            if (resolucion == 0 && EditObject.TiempoResolucion > TimeSpan.Zero) resolucion = 1;
            if (espera == 0 && EditObject.TiempoEsperaCliente > TimeSpan.Zero) espera = 1;

            litTiempos.Text = @"<div style='margin: 10px'>";
            litTiempos.Text += string.Format(template, "100%", "Incidencia", string.Format("{0:00}:{1:00}:{2:00}", (int)EditObject.TiempoIncidencia.TotalHours,
                                                                EditObject.TiempoIncidencia.Minutes, EditObject.TiempoIncidencia.Seconds), "#FFFFAA");
            litTiempos.Text += string.Format(template, atencion + "%", "Atencion", string.Format("{0:00}:{1:00}:{2:00}", (int)EditObject.TiempoAtencion.TotalHours,
                                                                EditObject.TiempoAtencion.Minutes, EditObject.TiempoAtencion.Seconds), atencion > 0 ? "#AAAAFF" : "none");
            litTiempos.Text += string.Format(template, resolucion + "%", "Resolucion", string.Format("{0:00}:{1:00}:{2:00}", (int)EditObject.TiempoResolucion.TotalHours,
                                                                EditObject.TiempoResolucion.Minutes, EditObject.TiempoResolucion.Seconds), resolucion > 0 ? "#AAFFAA" : "none");
            litTiempos.Text += string.Format(template, espera + "%", "Espera", string.Format("{0:00}:{1:00}:{2:00}", (int)EditObject.TiempoEsperaCliente.TotalHours,
                                                                EditObject.TiempoEsperaCliente.Minutes, EditObject.TiempoEsperaCliente.Seconds), espera > 0 ? "#FFAAAA" : "none");
            litTiempos.Text += @"</div>";
        }

        private void LockEdit(bool lck)
        {
            dtInicio.Enabled = 
                txtDescripcion.Enabled =
                cbTipoProblema.Enabled =
                cbNivel.Enabled =
                filUpload.Enabled =
                txtNombre.Enabled =
                txtTelefono.Enabled =
                txtMail.Enabled =
                txtMailList.Enabled =
                txtComentario.Enabled = lck;
        }

        private static int DecodeId(string id)
        {
            var s = string.Empty;

            for (var i = id.Length - 1; i >= 0; i--)
            {
                if(id[i] == Uno[i]) s += '1';
                else if(id[i] == Cero[i]) s += '0';
                else return 0;
            }

            return Convert.ToInt32(s, 2);
        }

        private static string EncodeId(int id)
        {
            var s = string.Empty;
            var cnt = 0;

            for (var i = 1; i <= id; i *= 2)
            {
                s += ((i & id) == i) ? Uno[cnt] : Cero[cnt];

                cnt++;
            }

            return s;
        }

        #endregion

        #region Bind Methods

        protected void BindResoluciones()
        {
            cbResolucion.Items.Clear();
            cbResolucion.Items.Add("");
            cbResolucion.Items.Add(CultureManager.GetLabel("SUPPORT_RESOLUTION_SOLVED"));
            cbResolucion.Items.Add(CultureManager.GetLabel("SUPPORT_RESOLUTION_NOTSOLVED"));
        }

        private void BindComboEstados()
        {
            cbEstado.Items.Clear();

            foreach (var state in DAOFactory.SupportTicketDAO.GetEstados()) cbEstado.Items.Add(state);
        }

        private void BindTipoProblema()
        {
            cbTipoProblema.DataSource = DAOFactory.SupportTicketDAO.GetTiposProblema();
            cbTipoProblema.DataBind();

            BindCategoria();
        }

        private void BindChat()
        {
            repChat.DataSource = EditObject.States.OfType<SupportTicketDetail>().Where(d => !string.IsNullOrEmpty(d.Descripcion));
            repChat.DataBind();
        }

        private void BindEstados()
        {
            var estados = EditObject.States.OfType<SupportTicketDetail>().OrderBy(d => d.Fecha);
            var est = new List<SupportTicketDetail>(estados.Count());

            SupportTicketDetail last = null;

            foreach (var estado in estados)
            {
                if (last == null || estado.Estado != last.Estado) est.Add(estado);

                last = estado;
            }

            var list = est.OrderByDescending(d => d.Fecha);

            last = null;

            foreach (var detail in list)
            {
                _tiempoEstado.Add(last != null ? last.Fecha.Subtract(detail.Fecha) : DateTime.UtcNow.Subtract(detail.Fecha));

                last = detail;
            }

            gridEstados.DataSource = list;
            gridEstados.DataBind();
        }

        private void BindCategoria()
        {
            cbCategoria.TipoProblema = cbTipoProblema.SelectedIndex;
            cbCategoria.DataBind();
        }

        #endregion
    }
}
