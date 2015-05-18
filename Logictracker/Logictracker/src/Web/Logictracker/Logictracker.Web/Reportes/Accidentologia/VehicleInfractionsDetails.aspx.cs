using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Web.Reportes.Accidentologia
{
    public partial class VehicleInfractionsDetails : SecuredGridReportPage<VehicleInfractionDetailVo>
    {
        private const string Leve = "LEVE";
        private const string Media = "MEDIA";
        private const string Grave = "GRAVE";
        private const string SinDefinir = "SIN_DEFINIR";

        protected override string GetRefference() { return "VEHICLE_INFRACTIONS_DETAILS"; }
        protected override string VariableName{ get { return "ACC_DET_INFRACCIONES_X_VEHICULO"; }  }
        protected override bool ExcelButton { get { return true; } }

        protected override Empresa GetEmpresa()
        {
            return (ddlDistrito.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : null;
        }
        protected override Linea GetLinea()
        {
            return (ddlBase != null && ddlBase.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
        }
        private int District
        {
            get
            {
                if (ViewState["District_infractionsDetails"] == null)
                {
                    ViewState["District_infractionsDetails"] = Session["District_infractionsDetails"];
                    Session["District_infractionsDetails"] = null;
                }

                return (ViewState["District_infractionsDetails"] != null) ? Convert.ToInt32(ViewState["District_infractionsDetails"]) : 0;
            }
        }
        private int Location
        {
            get
            {
                if (ViewState["Location_infractionDetails"] == null)
                {
                    ViewState["Location_infractionDetails"] = Session["Location_infractionDetails"];
                    Session["Location_infractionDetails"] = null;
                }

                return (ViewState["Location_infractionDetails"] != null) ? Convert.ToInt32(ViewState["Location_infractionDetails"]) : 0;
            }            
        }
        private int Vehicle
        {
            get
            {
                if (ViewState["Vehicle_infractionDetails"] == null)
                {
                    ViewState["Vehicle_infractionDetails"] = Session["Vehicle_infractionDetails"];
                    Session["Vehicle_infractionDetails"] = null;
                }

                return (ViewState["Vehicle_infractionDetails"] != null) ? Convert.ToInt32(ViewState["Vehicle_infractionDetails"]) : 0;
            }
        }
        private DateTime Desde
        {
            get
            {
                if (ViewState["InitialDate_infractionDetails"] == null)
                {
                    ViewState["InitialDate_infractionDetails"] = Session["InitialDate_infractionDetails"];
                    Session["InitialDate_infractionDetails"] = null;
                }

                return (ViewState["InitialDate_infractionDetails"] != null) ? Convert.ToDateTime(ViewState["InitialDate_infractionDetails"]) : DateTime.MinValue;
            }
        }
        private DateTime Hasta
        {
            get
            {
                if (ViewState["EndDate_infractionDetails"] == null)
                {
                    ViewState["EndDate_infractionDetails"] = Session["EndDate_infractionDetails"];
                    Session["EndDate_infractionDetails"] = null;
                }

                return (ViewState["EndDate_infractionDetails"] != null) ? Convert.ToDateTime(ViewState["EndDate_infractionDetails"]) : DateTime.MinValue;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetInitialValuesAndBindGrid();
        }

        protected override List<VehicleInfractionDetailVo> GetResults()
        {
            var desde = dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();

            var inicio = DateTime.UtcNow;
            try
            {
                var results = ReportFactory.InfractionDetailDAO.GetInfractionsDetailsByVehicles(GetVehicles(), desde, hasta)
                                                               .Select(o => new VehicleInfractionDetailVo(o) { HideCornerNearest = !chkVerEsquinas.Checked})
                                                               .ToList();
                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

				STrace.Trace("Detalle de Infracciones por Vehículo", String.Format("Duración de la consulta: {0} segundos", duracion));
				return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Detalle de Infracciones por Vehículo", e, String.Format("Reporte: Detalle de Infracciones por Vehículo. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));
                throw;
            }
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return  new Dictionary<string, string>
                                 {
                                     {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                                     {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                                 };
        }

        private List<int> GetVehicles()
        {
            if (lbVehiculo.GetSelectedIndices().Length == 0) lbVehiculo.ToogleItems();
            return lbVehiculo.SelectedValues;
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, VehicleInfractionDetailVo dataItem)
        {
            var cellCalificacion = GridUtils.GetCell(e.Row, VehicleInfractionDetailVo.IndexCalificacion);
            switch (Convert.ToInt32(cellCalificacion.Text))
            {
                case 1:
                    e.Row.BackColor = Color.FromArgb(238, 221, 130);
                    cellCalificacion.Text = CultureManager.GetLabel(Leve);
                    break;
                case 2:
                    e.Row.BackColor = Color.FromArgb(255, 140, 0);
                    cellCalificacion.Text = CultureManager.GetLabel(Media);
                    break;
                case 3:
                    e.Row.BackColor = Color.FromArgb(255, 69, 0);
                    cellCalificacion.Text = CultureManager.GetLabel(Grave);
                    break;
                default:
                    cellCalificacion.Text = CultureManager.GetLabel(SinDefinir);
                    break;
            }
            grid.Columns[InfractionDetailVo.IndexCornerNearest].Visible = chkVerEsquinas.Checked;
        }

        protected override void SelectedIndexChanged()
        {
            AddSessionParameters();
            OpenWin(String.Concat(ApplicationPath, "Monitor/MonitorHistorico/monitorHistorico.aspx"), "Monitor Historico");
        }

        private void AddSessionParameters()
        {
            var message = DAOFactory.LogMensajeDAO.FindById(Convert.ToInt32(Grid.SelectedDataKey.Values[0]));

            Session.Add("Distrito", message.Coche.Empresa != null ? message.Coche.Empresa.Id : message.Coche.Linea != null ? message.Coche.Linea.Empresa.Id : -1);
            Session.Add("Location", message.Coche.Linea != null ? message.Coche.Linea.Id : -1);
            Session.Add("TypeMobile", message.Coche.TipoCoche.Id);
            Session.Add("Mobile", message.Coche.Id);
            Session.Add("InitialDate", message.Fecha.AddMinutes(-1).ToDisplayDateTime());
            Session.Add("FinalDate", (message.FechaFin.HasValue ? message.FechaFin.Value : message.Fecha).AddMinutes(1).ToDisplayDateTime());
            Session.Add("MessageType", message.Mensaje.TipoMensaje.Id);

            Session.Add("MessagesIds", new List<string>{message.Mensaje.Codigo});

            Session.Add("MessageCenterIndex", message.Id);
            Session.Add("ShowPOIS", 0);
        }

        private void SetInitialValuesAndBindGrid()
        {
            if (IsPostBack) return;

            if (District != 0) ddlDistrito.SelectedValue = District.ToString();

            if (Location != 0) ddlBase.SelectedValue = Location.ToString();

            if (Vehicle != 0) lbVehiculo.SelectedValue = Vehicle.ToString();

            if (Desde != DateTime.MinValue) dpDesde.SelectedDate = Desde;
            else dpDesde.SetDate();

            if (Hasta != DateTime.MinValue) dpHasta.SelectedDate = Hasta;
            else dpHasta.SetDate();

            if (Vehicle != 0) Bind();
        }
    }
}
