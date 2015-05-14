using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.Messages.Saver;
using Logictracker.Messages.Sender;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.CustomWebControls.DropDownLists;

namespace Logictracker.App_Controls
{
    public partial class App_Controls_Mensajeria : BaseUserControl
    {
        private bool bindEstados;
        public string ParentControls 
        {
            get { return (string)(ViewState["ParentControls"] ?? string.Empty); }
            set { ViewState["ParentControls"] = cbVehiculoMensaje.ParentControls = value; cbTipoMensaje.ParentControls = value; } 
        }

        public int IdEmpresa
        {
            get { return (int)(ViewState["IdEmpresa"] ?? -1); }
            set
            {
                if (IdEmpresa == value) return;
                ViewState["IdEmpresa"] = value;
                var bind = IdLinea == -1;
                IdLinea = -1;
                if (bind)
                {
                    cbTipoMensaje.DataBind();
                    UpdatePanelMensajePre.Update();
                    UpdatePanelMensajeMovil.Update();
                    BindEstadosLog();
                }
            }
        }
        public int IdLinea
        {
            get { return (int)(ViewState["IdLinea"] ?? -1); }
            set {
                if (IdLinea == value) return;
                ViewState["IdLinea"] = value;
                cbTipoMensaje.DataBind();
                UpdatePanelMensajePre.Update();
                UpdatePanelMensajeMovil.Update();
                BindEstadosLog();
            }
        }

        public void UpdateVehicles() { UpdatePanelMensajeMovil.Update(); }

        protected override void OnInit(EventArgs e)
        {
            if (WebSecurity.ShowDriver) cbVehiculoMensaje.ShowDriverName = true;
            base.OnInit(e);
        }

        protected void cbTipoMensaje_SelectedIndexChanged(object sender, EventArgs e)
        {

            // Los putos controles custom no funcionan adentro del accordion
            // Se van a la cocha de su madre y me copio el BindingManager
            cbMensajes.ClearItems();

            var ddlbtm = sender as TipoMensajeDropDownList;
            var ddlbl = cbMensajes.GetParent<Linea>();
            var ddle = cbMensajes.GetParent<Empresa>();

            var tipoUsuario = DAOFactory.UsuarioDAO.GetByNombreUsuario(cbMensajes.Usuario.Name).Tipo;
            var tipoMensaje = ddlbtm != null && ddlbtm.Selected > 0 ? DAOFactory.TipoMensajeDAO.FindById(ddlbtm.Selected) : null;
            var empresa = ddle != null && ddle.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddle.Selected) : null;
            var linea = ddlbl != null && ddlbl.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlbl.Selected) : null;
            var user = DAOFactory.UsuarioDAO.FindById(cbMensajes.Usuario.Id);
            var mensajes = DAOFactory.MensajeDAO.FindByTipo(tipoMensaje, empresa, linea, user);

            var messages = mensajes.Where(mensaje => mensaje.Acceso <= tipoUsuario);

            foreach (var mensaje in messages) cbMensajes.AddItem(mensaje.Descripcion, Convert.ToInt32(mensaje.Codigo));

            UpdatePanelMensajePre.Update();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Usuario == null) return;

            AccPaneConfig.Visible = Usuario.AccessLevel >= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.AdminUser;

            if(!IsPostBack)
            {
                dtFotoValidator.MaxRange = TimeSpan.FromSeconds(Config.MaxPictureTime);
            }
            if(bindEstados)
            {
                BindEstadosLog();
            }
        }

        private void BindEstadosLog()
        {
            if (cbEstadoLog == null)
            {
                bindEstados = true;
                return;
            }

            cbEstadoLog.Items.Clear();


        
            var linea = IdLinea > 0 ? DAOFactory.LineaDAO.FindById(IdLinea) : null;
            var empresa = linea != null ? linea.Empresa : IdEmpresa > 0 ? DAOFactory.EmpresaDAO.FindById(IdEmpresa) : null;

            var estados = DAOFactory.MensajeDAO.FindCicloLogistico(empresa, linea);

            foreach (var estado in estados) cbEstadoLog.Items.Add(new ListItem(estado.Descripcion, estado.Codigo));
        
            updEstadoLog.Update();
        }

        #region Mensaje Estado Logistico

        protected void btEstadoLog_Click(object sender, EventArgs e)
        {
            SendMessage(MessageSender.Comandos.SetWorkflowState, cbEstadoLog.SelectedValue);
        } 

        #endregion


        #region Mensaje Pedir Foto

        protected void btPedirFoto_Click(object sender, EventArgs e)
        {
            if(!dtFotoDesde.SelectedDate.HasValue)
            {
                JsAlert(string.Format(CultureManager.GetError("NO_SELECTED"), CultureManager.GetLabel("DESDE")));
                return;
            }
            if (!dtFotoHasta.SelectedDate.HasValue)
            {
                JsAlert(string.Format(CultureManager.GetError("NO_SELECTED"), CultureManager.GetLabel("HASTA")));
                return;
            }
            SendMessage(MessageSender.Comandos.RetrievePictures, null);
        } 
        protected void btAhora_Click(object sender, EventArgs e)
        {
            dtFotoHasta.SelectedDate = DateTime.UtcNow.ToDisplayDateTime();
            dtFotoDesde.SelectedDate = dtFotoHasta.SelectedDate.Value.AddSeconds(-Math.Min(60, Config.MaxPictureTime));
        }

        #endregion 
    

        #region Mensaje Personalizado

        /// <summary>
        /// Enviar mensaje Personalizado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btMensajePer_Click(object sender, EventArgs e)
        {
            var msg = txtMensajePer.Text.Trim();
            if (string.IsNullOrEmpty(msg))
            {
                JsAlert(CultureManager.GetError("ENTER_MESSAGE_TEXT"));
                return;
            }
            SendMessage(MessageSender.Comandos.SubmitTextMessage, msg);
        }
        #endregion

        #region Mensaje Predefinido (de3pirnfindos)

        /// <summary>
        /// Enviar Mensaje Predefinido
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btMensajePre_Click(object sender, EventArgs e)
        {
            SendMessage(MessageSender.Comandos.SubmitCannedMessage, cbMensajes.SelectedValue);
        }

        #endregion

        #region Mensajes Corte Combustible
        protected void btCorte_Click(object sender, EventArgs e) { SendMessage(MessageSender.Comandos.DisableFuel, "0"); }
        protected void btCorteYa_Click(object sender, EventArgs e) { SendMessage(MessageSender.Comandos.DisableFuel, "1"); }
        protected void btHabilitar_Click(object sender, EventArgs e) { SendMessage(MessageSender.Comandos.EnableFuel); } 
        #endregion

        #region Mensajes de Configuracion

        protected void btPurgarMensajes_Click(object sender, EventArgs e) { SendMessage(MessageSender.Comandos.ReloadMessages, "0"); }

        /// <summary>
        /// Envia el mensaje de actualizacion de Qtree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btSendQTree_Click(object sender, EventArgs e) { SendMessage(MessageSender.Comandos.Qtree); }

        /// <summary>
        /// Envia el mensaje de Full Qtree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btSendFullQTree_Click(object sender, EventArgs e) { SendMessage(MessageSender.Comandos.FullQtree); }

        /// <summary>
        /// Enviar comando de Reboot al dispositivo (SAMPE)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btMensajeReboot_Click(object sender, EventArgs e) { SendMessage(MessageSender.Comandos.RebootSolicitation); }

        /// <summary>
        /// Envia mensaje para purgar todas las colas del dispositivo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClearQueues_Click(object sender, EventArgs e) { SendMessage(MessageSender.Comandos.ResetStateMachine); }

        /// <summary>
        /// Envia mensaje para purgar la configuracion del dispositivo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPurgarConfiguracion_Click(object sender, EventArgs e) { SendMessage(MessageSender.Comandos.ReloadConfiguration); }

        /// <summary>
        /// Envia mensaje para Resetear el FMI en el Garmin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnResetFMIOnGarmin_Click(object sender, EventArgs e) { SendMessage(MessageSender.Comandos.ResetFMIOnGarmin); }


        /// <summary>
        /// Actualizar firmware del dispositivo (FUMI)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btMensajeFirm_Click(object sender, EventArgs e) { SendMessage(MessageSender.Comandos.ReloadFirmware); }

        #endregion

        #region Envio de Mensajes

        private void SendMessage(string cmd) { SendMessage(cmd, string.Empty); }

        private void SendMessage(string cmd, string param)
        {
            if (cbVehiculoMensaje.SelectedIndex < 0)
            {
                // No hay un vehiculo seleccionado
                JsAlert(string.Format(CultureManager.GetError("NO_SELECTED"), CultureManager.GetEntity("PARENTI03")));
                return;
            }

            var ok = 0;
            var error = 0;
            var nogarmin = 0;

            var sent = string.Empty;
            var failed = string.Empty;
            var nogarminfailed = string.Empty;

            foreach (ListItem li in cbVehiculoMensaje.Items)
            {
                if (!li.Selected) continue;

                var c = DAOFactory.CocheDAO.FindById(Convert.ToInt32(li.Value));

                if (c.Dispositivo == null)
                {
                    // El vehiculo no tiene dispositivo asignado
                    JsAlert(string.Format(CultureManager.GetSystemMessage("UNASIGNED_VEHICLE"), c.Interno));
                    continue;
                }

                bool sendState;

                var dispositivo = c.Dispositivo;


                if(cmd == "PurgeConfiguration")
                {
                    DAOFactory.DispositivoDAO.PurgeConfiguration(dispositivo);
                    if (dispositivo.DetallesDispositivo.Cast<DetalleDispositivo>().Any(detail => detail.TipoParametro.RequiereReset))
                        MessageSender.CreateReboot(dispositivo, null);
                    sendState = true;
                }
                else
                {
                    var message = MessageSender.Create(dispositivo, new MessageSaver(DAOFactory)).AddCommand(cmd);
                    switch (cmd)
                    {
                        case MessageSender.Comandos.Qtree: break;
                        case MessageSender.Comandos.FullQtree: break;
                        case MessageSender.Comandos.ReloadFirmware: break;
                        case MessageSender.Comandos.ReloadConfiguration: break;
                        case MessageSender.Comandos.ReloadMessages: break;
                        case MessageSender.Comandos.ResetFMIOnGarmin:
                            var detalle = DAOFactory.DetalleDispositivoDAO.GetGteMessagingDeviceValue(dispositivo.Id);
                            if (!(detalle != null && detalle == "GARMIN"))
                            {
                                nogarmin++;
                                nogarminfailed += "\\n" + li.Text;
                                continue;
                            }
                            break;
                        case MessageSender.Comandos.Reboot: break;
                        case MessageSender.Comandos.RebootSolicitation: break;
                        case MessageSender.Comandos.SubmitCannedMessage: message.AddCannedMessageCode(param); break;
                        case MessageSender.Comandos.SubmitTextMessage: message.AddMessageText(param); break;
                        case MessageSender.Comandos.SetWorkflowState: message.AddWorkflowState(param); break;
                        case MessageSender.Comandos.DeleteCannedMessage: message.AddCannedMessageInfo(param, 0); break;
                        case MessageSender.Comandos.ResetStateMachine: break;
                        case MessageSender.Comandos.RetrievePictures: message.AddDateRange(dtFotoDesde.SelectedDate.Value.ToDataBaseDateTime(), dtFotoHasta.SelectedDate.Value.ToDataBaseDateTime()); break;
                        case MessageSender.Comandos.DisableFuel: message.AddInmediately(param == "1"); break;
                        case MessageSender.Comandos.EnableFuel: break;
                        default: sendState = false; break;
                    }
                    sendState = message.Send();
                }

                if (sendState)
                {
                    ok++;
                    sent += "\\n" + li.Text;
                }
                else
                {
                    error++;
                    failed += "\\n" + li.Text;
                }
            }

            if (ok > 0) JsAlert(string.Format(CultureManager.GetSystemMessage("MESSAGE_SENT"), sent));
            if (nogarmin > 0) JsAlert(string.Format(CultureManager.GetError("DEVICE_WITHOUT_GARMIN_CONFIGURED"), nogarminfailed));
            if (error > 0) JsAlert(string.Format(CultureManager.GetError("MESSAGE_NOT_SENT"), failed));        
        }

        private void JsAlert(string msg) { ScriptManager.RegisterStartupScript(Page, typeof(string), "al", "alert(\'" + msg + "\');", true); }

        #endregion
    }
}
