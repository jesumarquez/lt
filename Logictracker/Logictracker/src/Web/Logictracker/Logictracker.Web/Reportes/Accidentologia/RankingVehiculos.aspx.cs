using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logictracker.Types.ValueObjects;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.Accidentologia
{
    public partial class RankingVehiculos : SecuredGridReportPage<RankingVehiculosVo>
    {
        protected override string VariableName { get { return "RANKING_VEHICULOS"; } }
        protected override string GetRefference() { return "RANKING_VEHICULOS"; }
        protected override bool ExcelButton { get { return true; } }
        protected char Separator { get { return Usuario.CsvSeparator; } }

        protected override Empresa GetEmpresa()
        {
            return (ddlDistrito.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : null;
        }
        protected override Linea GetLinea()
        {
            return (ddlBase != null && ddlBase.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
        }

        private int VehiculosLeves
        {
            get { return ViewState["VehiculosLeves"] != null ? (int)ViewState["VehiculosLeves"] : 0; }
            set { ViewState["VehiculosLeves"] = value; }
        }
        private int VehiculosMedios
        {
            get { return ViewState["VehiculosLeves"] != null ? (int)ViewState["VehiculosLeves"] : 0; }
            set { ViewState["VehiculosLeves"] = value; }
        }
        private int VehiculosGraves
        {
            get { return ViewState["VehiculosGraves"] != null ? (int)ViewState["VehiculosGraves"] : 0; }
            set { ViewState["VehiculosGraves"] = value; }
        }

        private static Color GetColor(double qualification)
        {
            if (qualification >= 0 && qualification < 7) return Color.LightGreen;
            if (qualification >= 7 && qualification < 25) return Color.LightYellow;
            return Color.FromName("#ff6666");
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                            {
                                {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI02"), GetSelectedBases()},
                                {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                                {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                            };
        }

        protected override List<RankingVehiculosVo> GetResults()
        {
            var minDate = new DateTime(1753, 1, 1);
            var maxDate = new DateTime(3000, 1, 1);

            var desde = dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            
            if (desde < minDate || desde > maxDate) ThrowInvalidValue("Labels", "DESDE");
            if (hasta < minDate || hasta > maxDate) ThrowInvalidValue("Labels", "HASTA");

            var dist = new List<int> { ddlDistrito.Selected };
            var bases = ddlBase.SelectedValues;
            var tipos = lbTipoVehiculo.SelectedValues;
            var transportistas = lbTransportista.SelectedValues;
            var centros = lbCentroCosto.SelectedValues;

            var list = ReportFactory.RankingVehiculosDAO.GetRanking(dist, bases, transportistas, tipos, centros, desde, hasta);

            VehiculosLeves = (from o in list where o.Puntaje >= 0 && o.Puntaje < 7 select o).Count();
            VehiculosMedios = (from o in list where o.Puntaje >= 7 && o.Puntaje < 25 select o).Count();
            VehiculosGraves = (from o in list where o.Puntaje >= 25 select o).Count();

            lblLeves.Text = String.Format(CultureManager.GetLabel("PONDERACION_LEVES_RANKING"), VehiculosLeves);
            lblMedias.Text = String.Format(CultureManager.GetLabel("PONDERACION_MEDIAS_RANKING"), VehiculosMedios);
            lblGraves.Text = String.Format(CultureManager.GetLabel("PONDERACION_GRAVES_RANKING"), VehiculosGraves);
            
            tbl_totales.Visible = list.Count > 0;

            if (!chkSinActividad.Checked) list = list.Where(o => o.Kilometros > 0 || o.InfraccionesTotales > 0).ToList();

            return (from o in list select new RankingVehiculosVo(o)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, RankingVehiculosVo dataItem)
        {
            e.Row.Attributes.Remove("onclick");
            e.Row.Attributes.Add("cursor","arrow");

            if (dataItem == null) return;

            GridUtils.GetCell(e.Row, RankingVehiculosVo.IndexPuntaje).BackColor = GetColor(Convert.ToDouble(GridUtils.GetCell(e.Row, RankingVehiculosVo.IndexPuntaje).Text));
            GridUtils.GetCell(e.Row, RankingVehiculosVo.IndexKilometros).Text = String.Format("{0:0.00}", dataItem.Kilometros);
            GridUtils.GetCell(e.Row, RankingVehiculosVo.IndexHorasMovimiento).Text = string.Format(CultureManager.GetLabel("MOVIMIENTO_SIN_EVENTOS"), 
                                                                                                  dataItem.HorasMovimiento.Days, 
                                                                                                  dataItem.HorasMovimiento.Hours,
                                                                                                  dataItem.HorasMovimiento.Minutes, 
                                                                                                  dataItem.HorasMovimiento.Seconds);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        #region CSV and Print Methods

        protected override void ExportToCsv()
        {
            var builder = new BaseCsvBuilder(Usuario.CsvSeparator);

            GenerateCsvHeader(builder);
            GenerateColumns(builder);
            GenerateFields(builder);

            builder.GenerateRow(String.Empty);
            builder.GenerateRow(CultureManager.GetLabel("TOTALES"));
            builder.GenerateRow(String.Format(CultureManager.GetLabel("PONDERACION_GRAVES_RANKING"), VehiculosGraves));
            builder.GenerateRow(String.Format(CultureManager.GetLabel("PONDERACION_MEDIAS_RANKING"), VehiculosMedios));
            builder.GenerateRow(String.Format(CultureManager.GetLabel("PONDERACION_LEVES_RANKING"), VehiculosLeves));

            SetCsvSessionVars(builder.Build());

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }

		private void GenerateFields(BaseCsvBuilder builder)
        {
            foreach (var o in GetResults())
            {
                var str = String.Concat(o.Vehiculo, Separator, 
                                        o.Patente, Separator,
                                        o.TipoVehiculo, Separator,
                                        o.Transportista, Separator,
                                        o.CentroCosto, Separator,
                                        String.Format("{0:F}", o.Kilometros), Separator,
                                        String.Format("{0:00}.{1:00}:{2:00}:{3:00}", o.HorasMovimiento.Days, o.HorasMovimiento.Hours, o.HorasMovimiento.Minutes, o.HorasMovimiento.Seconds), Separator, 
                                        String.Format("{0:F}", o.Puntaje), Separator,
                                        o.InfraccionesLeves, Separator, 
                                        o.InfraccionesMedias, Separator, 
                                        o.InfraccionesGraves, Separator, 
                                        o.InfraccionesTotales);

                builder.GenerateRow(str);
            }
        }

		private void GenerateColumns(BaseCsvBuilder builder)
        {
            builder.GenerateRow(String.Concat(Separator, 
                                              Separator, 
                                              Separator, 
                                              Separator,
                                              Separator, 
                                              CultureManager.GetLabel("TOTALES"), 
                                              Separator, 
                                              Separator, 
                                              Separator, 
                                              CultureManager.GetLabel("INFRACCIONES")));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("CHOFER"), Separator, 
                                              CultureManager.GetLabel("LEGAJO"), Separator,
                                              CultureManager.GetEntity("PARENTI17"), Separator,
                                              CultureManager.GetEntity("PARENTI07"), Separator,
                                              CultureManager.GetEntity("PARENTI37"), Separator, 
                                              CultureManager.GetLabel("KILOMETROS"), Separator,
                                              CultureManager.GetLabel("MOVIMIENTO"), Separator, 
                                              CultureManager.GetLabel("PUNTAJE"), Separator,
                                              CultureManager.GetLabel("LEVES"), Separator, 
                                              CultureManager.GetLabel("MEDIAS"), Separator,
                                              CultureManager.GetLabel("GRAVES"), Separator, 
                                              CultureManager.GetLabel("TOTALES"), Separator));
        }

		private void GenerateCsvHeader(BaseCsvBuilder builder)
        {
            var param = new Dictionary<string, string>
                            {
                                {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI02"), GetSelectedBases()}
                            };

            builder.GenerateHeader(CultureManager.GetMenu("ACC_RANKING_CHOFERES"), param,
                dpDesde.SelectedDate.GetValueOrDefault(), dpHasta.SelectedDate.GetValueOrDefault());
        }

        private string GetSelectedBases()
        {
            var bases = ddlBase.SelectedValues;
            if (bases.Contains(0)) bases.Remove(0);

            var str = bases.Aggregate(String.Empty, (current, id) => String.Concat(current, DAOFactory.LineaDAO.FindById(Convert.ToInt32(id)).Descripcion, ", "));

            return str.TrimEnd(',');
        }

        protected override void OnPrePrint()
        {
            lblTotalesPrint.Text = lblTotales.Text;
            lblGravesPrint.Text = lblGraves.Text;
            lblMediasPrint.Text = lblMedias.Text;
            lblLevesPrint.Text = lblLeves.Text;
        }

        #endregion
    }
}

   


