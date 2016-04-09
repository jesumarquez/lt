using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Security;
using Logictracker.Tracker.Application.Reports;
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
        protected override bool ScheduleButton { get { return true; } }
        protected override bool SendReportButton { get { return true; } }

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
            var desde = dpDesde.SelectedDate.GetValueOrDefault();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault();

            var reportService = new ReportService(DAOFactory, ReportFactory);
            var results = reportService.VehicleInfractionsReport(GetVehicleList(), desde, hasta, chkVerEsquinas.Checked);

            return results;
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return  new Dictionary<string, string>
                                 {
                                     {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                                     {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                                 };
        }

        protected override List<int> GetVehicleList()
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
            var message = DAOFactory.InfraccionDAO.FindById(Convert.ToInt32(Grid.SelectedDataKey.Values[0]));

            Session.Add("Distrito", message.Vehiculo.Empresa != null ? message.Vehiculo.Empresa.Id : message.Vehiculo.Linea != null ? message.Vehiculo.Linea.Empresa.Id : -1);
            Session.Add("Location", message.Vehiculo.Linea != null ? message.Vehiculo.Linea.Id : -1);
            Session.Add("TypeMobile", message.Vehiculo.TipoCoche.Id);
            Session.Add("Mobile", message.Vehiculo.Id);
            Session.Add("InitialDate", message.Fecha.AddMinutes(-1));
            Session.Add("FinalDate", (message.FechaFin.HasValue ? message.FechaFin.Value : message.Fecha).AddMinutes(1));
            //Session.Add("MessageType", message.Mensaje.TipoMensaje.Id);
            //Session.Add("MessagesIds", new List<string>{message.Mensaje.Codigo});

            //Session.Add("MessageCenterIndex", message.Id);
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
        
        protected override DateTime GetToDateTime()
        {
            return dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }

        protected override DateTime GetSinceDateTime()
        {
            return dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }

        protected override string GetDescription(string reporte)
        {
            var linea = GetLinea();
            ToogleItems(lbVehiculo);

            var sDescription = new StringBuilder(GetEmpresa().RazonSocial + " - ");
            if (linea != null) sDescription.AppendFormat("Base {0} - ", linea.Descripcion);
            sDescription.AppendFormat("Reporte: {0} - ", reporte);
            sDescription.AppendFormat("Tipo de Vehiculo: {0} - ", ddlTipoVehiculo.SelectedItem.Text);
            sDescription.AppendFormat("Linea: {0} ", ddlBase.SelectedItem.Text);
            sDescription.AppendFormat("Cantidad Vehiculos: {0} ", lbVehiculo.SelectedStringValues.Count);

            return sDescription.ToString();
        }
    }
}
