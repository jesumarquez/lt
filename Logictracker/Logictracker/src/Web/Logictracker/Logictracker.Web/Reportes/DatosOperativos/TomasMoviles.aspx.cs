using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Configuration;
using Logictracker.Mailing;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Culture;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Reportes.DatosOperativos
{
    public partial class EstadisticaTomasMoviles : SecuredGridReportPage<MobilePositionVehicleVo>
    {
        public const char PipeChar = '|';

        protected override string VariableName { get { return "DOP_VERI_VEHICULO"; } }
        protected override string GetRefference() { return "TOMAS_MOVILES"; }

        protected override bool ExcelButton { get { return true; } }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool SendReportButton { get { return true; } }
        public override C1GridView Grid { get { return grid; } }

        protected override DropDownList CbSchedulePeriodicidad { get { return cbPeriodicidad; } }
        protected override TextBox TxtScheduleMail { get { return txtMail; } }
        protected override ResourceButton BtScheduleGuardar { get { return btnGuardar; } }
        protected override ModalPopupExtender ModalSchedule { get { return mpePanel; } }

        protected override TextBox SendReportTextBoxEmail { get { return TextBoxEmailSendReport; } }
        protected override TextBox SendReportTextBoxReportName { get { return textBoxReportName; } }
        protected override ModalPopupExtender SendReportModalPopupExtender { get { return popUpSendReport; } }
        protected override ResourceButton SendReportOkButton { get { return ButtonOkSendReport; } }
        protected override RadioButton RadioButtonExcel { get { return rbutExcel; } }
        protected override RadioButton RadioButtonHtml { get { return rbutHtml; } } 

        protected override InfoLabel NotFound { get { return infoLabel1; } }
        protected override ToolBar ToolBar { get { return ToolBar1; } }
        protected override InfoLabel LblInfo { get { return infoLabel1; } }
        protected override ResourceButton BtnSearch { get { return btnActualizar; } }
        protected override UpdatePanel UpdatePanelGrid { get { return updGrid; } }
        protected override C1GridView GridPrint { get { return gridPrint; } }
        protected override UpdatePanel UpdatePanelPrint { get { return upPrint; } }
        protected override Repeater PrintFilters { get { return FiltrosPrint; } }
        public override int PageSize { get { return 10000; } }
        public override string SearchString { get { return txtBuscar.Text; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack) Bind();
        }

        protected override List<MobilePositionVehicleVo> GetResults()
        {
            var mobiles = DAOFactory.CocheDAO.GetList(ddlDistrito.SelectedValues,
                                                      ddlLinea.SelectedValues,
                                                      ddlTipoVehiculo.SelectedValues,
                                                      ddlTransportista.SelectedValues,
                                                      new[] {-1}, // DEPARTAMENTOS
                                                      ddlCostCenter.SelectedValues,
                                                      new[] {-1}, // SUB CENTROS DE COSTO
                                                      chkDispositivosAsignados.Checked,
                                                      chkSoloConGarmin.Checked)
                                             .Where(m => m.Estado != Coche.Estados.Inactivo || !chkOcultarInactivos.Checked);

            return ReportFactory.MobilePositionDAO.GetMobilesLastPosition(mobiles)
                                                  .Select(pos => new MobilePositionVehicleVo(pos))
                                                  .ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobilePositionVehicleVo dataItem)
        {
            SetBackgroundColor(e, dataItem);

            if (dataItem == null) return;

            FormatDisplayData(e, dataItem);

            if (chkVerDirecciones.Checked)
                e.Row.Cells[MobilePositionVehicleVo.IndexEsquinaCercana].Text = dataItem.EsquinaCercana;
        }

        protected override void SelectedIndexChanged()
        {
            var fechaUltimoReporte = GridUtils.GetCell(Grid.SelectedRow, MobilePositionVehicleVo.IndexFecha).Text;

            if (!string.IsNullOrEmpty(fechaUltimoReporte)) ShowPositionsDetails();
            else AddNoDetailsScript();
        }

        protected override void Bind()
        {
            base.Bind();
            grid.Columns[GridUtils.GetColumnIndex(MobilePositionVehicleVo.IndexEsquinaCercana)].Visible = chkVerDirecciones.Checked;
        }

        private void AddSessionParameters()
        {
            Session.Add("Vehiculo", Grid.SelectedDataKey[MobilePositionVehicleVo.KeyIndexIdMovil]);
            Session.Add("Dispositivo", Grid.SelectedDataKey[MobilePositionVehicleVo.KeyIndexIdDispositivo]);
        }

        private static void SetBackgroundColor(C1GridViewRowEventArgs e, MobilePositionVehicleVo dataItem)
        {
            if (dataItem.IdDispositivo != dataItem.IdDispositivoCoche)
            {
                e.Row.BackColor = Color.LightBlue;
            }
            else
            {
                switch (dataItem.EstadoReporte)
                {
                    case 0: e.Row.BackColor = Color.LightGreen; break;
                    case 1: e.Row.BackColor = Color.LightSteelBlue; break;
                    case 2: e.Row.BackColor = Color.Yellow; break;
                    case 3: e.Row.BackColor = Color.LightCoral; break;
                    default: e.Row.BackColor = Color.LightGray; break;
                }
            }
        }

        private void FormatDisplayData(C1GridViewRowEventArgs e, MobilePositionVehicleVo posicion)
        {
            using (var cellRefResp = GridUtils.GetCell(e.Row, MobilePositionVehicleVo.IndexCcReferenciaResponsableVehiculo))
            {
                var refresp = cellRefResp.Text.Split(PipeChar);
                switch (refresp.Length)
                {
                    case 3:
                        if (!string.IsNullOrEmpty(refresp[0]))
                            refresp[0] = "CC:" + NonBreakingSpace + refresp[0];
                        if (!string.IsNullOrEmpty(refresp[1]))
                            refresp[1] = "Ref:" + NonBreakingSpace +refresp[1];
                        if (!string.IsNullOrEmpty(refresp[2]))
                            refresp[2] = "Resp:" + NonBreakingSpace + refresp[2];

                        cellRefResp.Text = (!string.IsNullOrEmpty(refresp[0]) ? refresp[0] : string.Empty) +
                                           (!string.IsNullOrEmpty(refresp[1])
                                                ? (!string.IsNullOrEmpty(refresp[0]) ? BreakLine : string.Empty) + refresp[1]
                                                : string.Empty) +
                                           (!string.IsNullOrEmpty(refresp[2])
                                                ? (!string.IsNullOrEmpty(refresp[0]) ||
                                                   !string.IsNullOrEmpty(refresp[1])
                                                       ? BreakLine
                                                       : string.Empty) + refresp[2]
                                                : string.Empty);
                        break;
                    default:
                        cellRefResp.Text = NonBreakingSpace;
                        break;
                }
            }

            using (var cellDispo = GridUtils.GetCell(e.Row, MobilePositionVehicleVo.IndexDispositivo))
            {
                cellDispo.Text = cellDispo.Text + BreakLine + "(" + Convert.ToString(posicion.IdDispositivo) + ")";
            }

            using (var cellEstadoReporte = GridUtils.GetCell(e.Row, MobilePositionVehicleVo.IndexEstadoReporte))
            {
                cellEstadoReporte.Text = MobilePositionVehicleVo.GetReportStatusDescription(posicion.EstadoReporte);
            }

            GridUtils.GetColumn(MobilePositionVehicleVo.IndexEstadoStr).Visible = false;

            using (var cellFecha = GridUtils.GetCell(e.Row, MobilePositionVehicleVo.IndexFecha))
            {
                if (!posicion.Fecha.HasValue)
                    cellFecha.Text = string.Empty;
                else
                {
                    var strFecha = cellFecha.Text;
                    var arrFecha = strFecha.Split(' ');
                    cellFecha.Text = arrFecha[0] + BreakLine + arrFecha[1] + NonBreakingSpace + arrFecha[2];
                }
            }

            using (var cellLastPacket = GridUtils.GetCell(e.Row, MobilePositionVehicleVo.IndexLastPacketReceivedAt))
            {
                if (!posicion.LastPacketReceivedAt.HasValue)
                    cellLastPacket.Text = string.Empty;
                else
                {
                    var strFecha = cellLastPacket.Text;
                    var arrFecha = strFecha.Split(' ');
                    cellLastPacket.Text = arrFecha[0] + BreakLine + arrFecha[1] + NonBreakingSpace + arrFecha[2];
                }
            }

            using (var cellVehiculo = GridUtils.GetCell(e.Row, MobilePositionVehicleVo.IndexVehiculo))
            {
                if (string.IsNullOrEmpty(posicion.Vehiculo))
                {
                    cellVehiculo.Text = string.Empty;
                }
                else
                {
                    var strVehiculo = cellVehiculo.Text;
                    var arrVehiculo = strVehiculo.Split(PipeChar);
                    if (arrVehiculo.Length > 1 && arrVehiculo[0] == arrVehiculo[1])
                        cellVehiculo.Text = arrVehiculo[0];
                    else
                        cellVehiculo.Text = arrVehiculo[0] + (arrVehiculo.Length > 1 ? BreakLine + arrVehiculo[1] : string.Empty);
                }
            }

            using (var cellVehiculoId = GridUtils.GetCell(e.Row, MobilePositionVehicleVo.IndexVehiculoId))
            {
                if (string.IsNullOrEmpty(posicion.VehiculoId))
                {
                    cellVehiculoId.Text = string.Empty;
                }
                else
                {
                    var strVehiculoId = cellVehiculoId.Text;
                    var arrVehiculo = strVehiculoId.Split(PipeChar);

                    cellVehiculoId.Text = arrVehiculo[0] + (arrVehiculo.Length > 1 ? BreakLine + arrVehiculo[1] : string.Empty);
                }
            }

            using (var cellUbicacion = GridUtils.GetCell(e.Row, MobilePositionVehicleVo.IndexUbicacion))
            {

                if (string.IsNullOrEmpty(posicion.Ubicacion))
                {
                    cellUbicacion.Text = string.Empty;
                }
                else
                {
                    var strUbicacion = cellUbicacion.Text;
                    var arrUbicacion = strUbicacion.Split(PipeChar);
                    cellUbicacion.Text = arrUbicacion[0] 
                        + (arrUbicacion.Length > 1 ? BreakLine + arrUbicacion[1] : string.Empty)
                        + (arrUbicacion.Length > 2 ? BreakLine + arrUbicacion[2] : string.Empty);
                }
            }

            using (var cellVelocidad = GridUtils.GetCell(e.Row, MobilePositionVehicleVo.IndexVelocidad))
            {
                if (posicion.Velocidad.Equals(-1))
                {
                    cellVelocidad.Text = string.Empty;
                }
            }

            using (var cellTiempoAUltimoLogin = GridUtils.GetCell(e.Row, MobilePositionVehicleVo.IndexTiempoAUltimoLogin))
            {
                cellTiempoAUltimoLogin.Text =
                    posicion.TiempoDesdeUltimoLogin.Equals(TimeSpan.Zero)
                        ? string.Empty
                        : string.Format(CultureManager.GetLabel("MOVIMIENTO_SIN_EVENTOS"),
                                        posicion.TiempoDesdeUltimoLogin.Days, posicion.TiempoDesdeUltimoLogin.Hours,
                                        posicion.TiempoDesdeUltimoLogin.Minutes, posicion.TiempoDesdeUltimoLogin.Seconds);

            }

            using (var cellUltimoChoferLogin = GridUtils.GetCell(e.Row, MobilePositionVehicleVo.IndexUltimoChoferLogin))
            {
                if (!string.IsNullOrEmpty(posicion.UltimoChoferLogin))
                {
                    var strUltimoChoferLogin = cellUltimoChoferLogin.Text;
                    var arrUltimoChoferLogin = strUltimoChoferLogin.Split(PipeChar);

                    if (arrUltimoChoferLogin.Length < 2 && !string.IsNullOrEmpty(strUltimoChoferLogin))
                    {
                        cellUltimoChoferLogin.Text = CultureManager.GetLabel("REVISAR_TARJETA");
                    }
                    else
                    {
                        var arrFecha = arrUltimoChoferLogin[1].Split(' ');
                        cellUltimoChoferLogin.Text = arrUltimoChoferLogin[0] + BreakLine + arrFecha[0] + BreakLine + arrFecha[1] + NonBreakingSpace + arrFecha[2];
                    }
                }
            }
        }

        private void AddNoDetailsScript()
        {
            var interno = GridUtils.GetCell(Grid.SelectedRow, MobilePositionVehicleVo.IndexVehiculoId).Text;
            var noDetailsScript = string.Format(string.Concat("alert('", CultureManager.GetSystemMessage("NO_POSITIONS_MOBILE"), "');"), interno);

            ScriptManager.RegisterStartupScript(this, typeof (string), "NoDetails", noDetailsScript, true);
        }

        private void ShowPositionsDetails()
        {
            AddSessionParameters();

            OpenWin("TomasDetalle.aspx", "Detalle posiciones");
        }

        protected void BtScheduleGuardarClick(object sender, EventArgs e)
        {
            var reporte = ProgramacionReporte.Reportes.VerificadorVehiculos;
            var empresa = GetEmpresa();
            var linea = GetLinea();

            var prog = new ProgramacionReporte
            {
                ReportName = SendReportTextBoxReportName.Text,
                Report = reporte,
                Periodicity = CbSchedulePeriodicidad.SelectedValue[0],
                Mail = TxtScheduleMail.Text,
                Empresa = empresa ?? linea.Empresa,
                Created = DateTime.Now,
                Description = GetDescription(reporte + " " + CbSchedulePeriodicidad.SelectedValue),
                Active = false,
                Format = RadioButtonHtml.Checked
                                ? ProgramacionReporte.FormatoReporte.Html
                                : ProgramacionReporte.FormatoReporte.Excel
            };

            prog.Vehicles = GetSelectedVehicles();
            prog.Drivers = GetSelectedDrivers();
            prog.MessageTypes = GetSelectedMessageTypes();
            prog.OvercomeKilometers = GetOvercomeKilometers();

            DAOFactory.ProgramacionReporteDAO.Save(prog);

            ModalSchedule.Hide();
            SendConfirmationMail(reporte, prog.Description);
        }

        private void SendConfirmationMail(string reportType, string description)
        {
            var configFile = Config.Mailing.ReportConfirmation;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);
            sender.Config.Subject = string.Format("Se ha creado un nuevo reporte programado ({0})", reportType);
            sender.Config.Body = description;

            sender.SendMail();
        }

        protected override Empresa GetEmpresa()
        {
            var id = ddlDistrito.Selected;
            return id > 0 ? DAOFactory.EmpresaDAO.FindById(id) : null;
        }

        protected override Linea GetLinea()
        {
            var id = ddlLinea.Selected;
            return id > 0 ? DAOFactory.LineaDAO.FindById(id) : null;
        }
    }
}