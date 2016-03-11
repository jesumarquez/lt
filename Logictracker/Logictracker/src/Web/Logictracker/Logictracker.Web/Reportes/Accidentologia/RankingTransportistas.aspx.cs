using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.Accidentologia
{
    public partial class RankingTransportistas : SecuredGridReportPage<RankingTransportistasVo>
    {
        protected override string VariableName { get { return "RANKING_TRANSPORTISTAS"; } }
        protected override string GetRefference() { return "RANKING_TRANSPORTISTAS"; }
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

        private int TransportistasLeves
        {
            get { return ViewState["TransportistasLeves"] != null ? (int)ViewState["TransportistasLeves"] : 0; }
            set { ViewState["TransportistasLeves"] = value; }
        }
        private int TransportistasMedios
        {
            get { return ViewState["TransportistasMedios"] != null ? (int)ViewState["TransportistasMedios"] : 0; }
            set { ViewState["TransportistasMedios"] = value; }
        }
        private int TransportistasGraves
        {
            get { return ViewState["TransportistasGraves"] != null ? (int)ViewState["TransportistasGraves"] : 0; }
            set { ViewState["TransportistasGraves"] = value; }
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

        protected override List<RankingTransportistasVo> GetResults()
        {
            var minDate = new DateTime(1753, 1, 1);
            var maxDate = new DateTime(3000, 1, 1);

            var desde = dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            
            if (desde < minDate || desde > maxDate) ThrowInvalidValue("Labels", "DESDE");
            if (hasta < minDate || hasta > maxDate) ThrowInvalidValue("Labels", "HASTA");

            var dist = new List<int> { ddlDistrito.Selected };
            var bases = ddlBase.SelectedValues;
            var transportistas = lbTransportista.SelectedValues;

            var list = ReportFactory.RankingTransportistasDAO.GetRanking(dist, bases, transportistas, desde, hasta);

            TransportistasLeves = (from o in list where o.Puntaje >= 0 && o.Puntaje < 7 select o).Count();
            TransportistasMedios = (from o in list where o.Puntaje >= 7 && o.Puntaje < 25 select o).Count();
            TransportistasGraves = (from o in list where o.Puntaje >= 25 select o).Count();

            lblLeves.Text = string.Format(CultureManager.GetLabel("PONDERACION_LEVES_RANKING"), TransportistasLeves);
            lblMedias.Text = string.Format(CultureManager.GetLabel("PONDERACION_MEDIAS_RANKING"), TransportistasMedios);
            lblGraves.Text = string.Format(CultureManager.GetLabel("PONDERACION_GRAVES_RANKING"), TransportistasGraves);
            
            tbl_totales.Visible = list.Count > 0;

            if (!chkSinActividad.Checked) list = list.Where(o => o.Kilometros > 0 || o.InfraccionesTotales > 0).ToList();

            return (from o in list select new RankingTransportistasVo(o)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, RankingTransportistasVo dataItem)
        {
            e.Row.Attributes.Remove("onclick");
            e.Row.Attributes.Add("cursor","arrow");

            if (dataItem == null) return;

            GridUtils.GetCell(e.Row, RankingTransportistasVo.IndexPuntaje).BackColor = GetColor(Convert.ToDouble(GridUtils.GetCell(e.Row, RankingTransportistasVo.IndexPuntaje).Text));
            GridUtils.GetCell(e.Row, RankingTransportistasVo.IndexKilometros).Text = String.Format("{0:0.00}", dataItem.Kilometros);
            GridUtils.GetCell(e.Row, RankingTransportistasVo.IndexHorasMovimiento).Text = string.Format(CultureManager.GetLabel("MOVIMIENTO_SIN_EVENTOS"), 
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

            builder.GenerateRow(string.Empty);
            builder.GenerateRow(CultureManager.GetLabel("TOTALES"));
            builder.GenerateRow(string.Format(CultureManager.GetLabel("PONDERACION_GRAVES_RANKING"), TransportistasGraves));
            builder.GenerateRow(string.Format(CultureManager.GetLabel("PONDERACION_MEDIAS_RANKING"), TransportistasMedios));
            builder.GenerateRow(string.Format(CultureManager.GetLabel("PONDERACION_LEVES_RANKING"), TransportistasLeves));

            SetCsvSessionVars(builder.Build());

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }

        private void GenerateCsvHeader(BaseCsvBuilder builder)
        {
            var param = new Dictionary<string, string>
                            {
                                {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI02"), GetSelectedBases()}
                            };

            builder.GenerateHeader(CultureManager.GetMenu("ACC_RANKING_TRANSPORTISTAS"), param,
                dpDesde.SelectedDate.GetValueOrDefault(), dpHasta.SelectedDate.GetValueOrDefault());
        }

		private void GenerateFields(BaseCsvBuilder builder)
        {
            foreach (var o in GetResults())
            {
                var str = string.Concat(o.Transportista, Separator,
                                        o.Vehiculos, Separator,
                                        string.Format("{0:F}", o.Kilometros), Separator,
                                        string.Format("{0:00}.{1:00}:{2:00}:{3:00}", o.HorasMovimiento.Days, o.HorasMovimiento.Hours, o.HorasMovimiento.Minutes, o.HorasMovimiento.Seconds), Separator, 
                                        string.Format("{0:F}", o.Puntaje), Separator,
                                        o.InfraccionesLeves, Separator, 
                                        o.InfraccionesMedias, Separator, 
                                        o.InfraccionesGraves, Separator, 
                                        o.InfraccionesTotales);

                builder.GenerateRow(str);
            }
        }

		private void GenerateColumns(BaseCsvBuilder builder)
        {
            builder.GenerateRow(String.Concat(CultureManager.GetEntity("PARENTI07"), Separator,
                                              CultureManager.GetMenu("PAR_VEHICULOS"), Separator,
                                              CultureManager.GetLabel("KILOMETROS"), Separator,
                                              CultureManager.GetLabel("MOVIMIENTO"), Separator, 
                                              CultureManager.GetLabel("PUNTAJE"), Separator,
                                              CultureManager.GetLabel("LEVES"), Separator, 
                                              CultureManager.GetLabel("MEDIAS"), Separator,
                                              CultureManager.GetLabel("GRAVES"), Separator, 
                                              CultureManager.GetLabel("TOTALES"), Separator));
        }

        private string GetSelectedBases()
        {
            var bases = ddlBase.SelectedValues;
            if (bases.Contains(0)) bases.Remove(0);

            var str = bases.Aggregate(String.Empty, (current, id) => String.Concat(current, DAOFactory.LineaDAO.FindById(Convert.ToInt32(id)).Descripcion, ", "));

            return str.TrimEnd(',');
        }

        #endregion
    }
}

   


