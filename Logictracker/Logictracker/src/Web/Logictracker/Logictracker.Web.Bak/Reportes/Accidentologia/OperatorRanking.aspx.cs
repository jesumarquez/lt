using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using System.Text;

namespace Logictracker.Reportes.Accidentologia
{
    public partial class ReportesOperatorRanking : SecuredGridReportPage<OperatorRankingVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "ACC_RANKING_CHOFERES"; } }
        protected override string GetRefference() { return "REP_OPERATOR_RANKING"; }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool ExcelButton { get { return true; } }
        public override bool HasTotalRow { get { return true; } }
        protected char Separator { get { return Usuario.CsvSeparator; } }

        protected override Empresa GetEmpresa()
        {
            return (ddlDistrito.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : null;
        }

        protected override Linea GetLinea()
        {
            return (ddlBase != null && ddlBase.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Amount of Operators ranked as "Infracciones leves"
        /// </summary>
        private int OperadoresLeves
        {
            get { return ViewState["OperadoresLeves"] != null ? (int)ViewState["OperadoresLeves"] : 0; }
            set { ViewState["OperadoresLeves"] = value; }
        }

        /// <summary>
        /// Amount of Operators ranked as "Infracciones Medias"
        /// </summary>
        private int OperadoresMedios
        {
            get { return ViewState["OperadoresMedios"] != null ? (int)ViewState["OperadoresMedios"] : 0; }
            set { ViewState["OperadoresMedios"] = value; }
        }

        /// <summary>
        /// Amount of Operators ranked as "Infracciones graves"
        /// </summary>
        private int OperadoresGraves
        {
            get { return ViewState["OperadoresGraves"] != null ? (int)ViewState["OperadoresGraves"] : 0; }
            set { ViewState["OperadoresGraves"] = value; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the color associated to the givenn qualification.
        /// </summary>
        /// <param name="qualification"></param>
        /// <returns></returns>
        private static Color GetColor(double qualification)
        {
            if (qualification >= 0 && qualification < 7) return Color.LightGreen;
            if (qualification >= 7 && qualification < 25) return Color.LightYellow;
            return Color.FromName("#ff6666");
        }

        /// <summary>
        /// Adds The parameters to the session for the infractionsDetails report
        /// </summary>
        /// <param name="operatorId"></param>
        private void AddSessionParametersForDetail(int operatorId)
        {
            var empleado = DAOFactory.EmpleadoDAO.FindById(operatorId);

            Session.Add("District_infractionsDetails", ddlDistrito.SelectedValue);
            Session.Add("Location_infractionDetails", empleado.Linea != null ? empleado.Linea.Id : 0);
            Session.Add("Operator_infractionDetails", operatorId);
            Session.Add("InitialDate_infractionDetails", dpDesde.SelectedDate.GetValueOrDefault());
            Session.Add("EndDate_infractionDetails", dpHasta.SelectedDate.GetValueOrDefault());
        }

        #endregion

        #region Protected Methods

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

        protected override Dictionary<string, string> GetFilterValuesProgramados()
        {
            ToogleItems(ddlBase);
            ToogleItems(lbTipoEmpleado);
            ToogleItems(lbTransportista);

            var dic = new Dictionary<string, string>();
            var sTipos = new StringBuilder();
            var sTransportistas = new StringBuilder();
            var sBases = new StringBuilder();

            foreach (var tipo in lbTipoEmpleado.SelectedValues)
            {
                if (!sTipos.ToString().Equals(""))
                    sTipos.Append(",");

                sTipos.Append((string) tipo.ToString());
            }
            foreach (var transportista in lbTransportista.SelectedValues)
            {
                if (!sTransportistas.ToString().Equals(""))
                    sTransportistas.Append(",");

                sTransportistas.Append((string) transportista.ToString());
            }
            foreach (var bas in ddlBase.SelectedValues)
            {
                if (!sBases.ToString().Equals(""))
                    sBases.Append(",");

                sBases.Append((string) bas.ToString());
            }

            dic.Add("TRANS", sTransportistas.ToString());
            dic.Add("TIPOS", sTipos.ToString());
            dic.Add("BASES", sBases.ToString());

            return dic;
        }

        /// <summary>
        /// Gets the report objects filtered by search criteria.
        /// </summary>
        /// <returns></returns>
        protected override List<OperatorRankingVo> GetResults()
        {
            var minDate = new DateTime(1753, 1, 1);
            var maxDate = new DateTime(3000, 1, 1);

            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());
            
            if (desde < minDate || desde > maxDate) ThrowInvalidValue("Labels", "DESDE");
            if (hasta < minDate || hasta > maxDate) ThrowInvalidValue("Labels", "HASTA");

            //ToogleItems(ddlBase);
            //ToogleItems(lbTipoEmpleado);
            //ToogleItems(lbTransportista);

            var dist = new List<int> { ddlDistrito.Selected };
            var bases = ddlBase.SelectedValues;
            var tipos = lbTipoEmpleado.SelectedValues;
            var transportistas = lbTransportista.SelectedValues;
            var centros = lbCentroCosto.SelectedValues;
            var deptos = lbDepartamento.SelectedValues;

            var list = ReportFactory.OperatorRankingDAO.GetRanking(dist, bases, transportistas, tipos, centros, deptos, desde, hasta);

            OperadoresLeves = (from o in list where o.Puntaje >= 0 && o.Puntaje < 7 select o).Count();
            OperadoresMedios = (from o in list where o.Puntaje >= 7 && o.Puntaje < 25 select o).Count();
            OperadoresGraves = (from o in list where o.Puntaje >= 25 select o).Count();

            lblGraves.Text = String.Format(CultureManager.GetLabel("PONDERACION_GRAVES_RANKING"), OperadoresGraves);
            lblMedias.Text = String.Format(CultureManager.GetLabel("PONDERACION_MEDIAS_RANKING"), OperadoresMedios);
            lblLeves.Text = String.Format(CultureManager.GetLabel("PONDERACION_LEVES_RANKING"), OperadoresLeves);

            tbl_totales.Visible = list.Count > 0;

            if (!chkSinActividad.Checked)
            {
                list = list.Where(o => o.Kilometros > 0 || o.InfraccionesTotales > 0).ToList();
            }

            return (from o in list select new OperatorRankingVo(o)).ToList();
        }

        /// <summary>
        /// Formats each row witch color.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="e"></param>
        /// <param name="dataItem"></param>
        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, OperatorRankingVo dataItem)
        {
            if (dataItem == null) return;

            GridUtils.GetCell(e.Row, OperatorRankingVo.IndexPuntaje).BackColor = GetColor(Convert.ToDouble(GridUtils.GetCell(e.Row, OperatorRankingVo.IndexPuntaje).Text));
            GridUtils.GetCell(e.Row, OperatorRankingVo.IndexKilometros).Text = String.Format("{0:0.00}", dataItem.Kilometros);
            GridUtils.GetCell(e.Row, OperatorRankingVo.IndexHorasMovimiento).Text = string.Format(CultureManager.GetLabel("MOVIMIENTO_SIN_EVENTOS"), 
                                                                                                  dataItem.HorasMovimiento.Days, 
                                                                                                  dataItem.HorasMovimiento.Hours,
                                                                                                  dataItem.HorasMovimiento.Minutes, 
                                                                                                  dataItem.HorasMovimiento.Seconds);
        }

        protected override void SelectedIndexChanged()
        {
            AddSessionParametersForDetail(Convert.ToInt32(Grid.DataKeys[Grid.SelectedIndex][OperatorRankingVo.KeyIndexIdOperador]));

            OpenWin("infractionsDetails.aspx", CultureManager.GetMenu("DETALLE_DE_INFRACCIONES"));

            foreach(C1GridViewRow row in Grid.Rows)
            {
                GridUtils.GetCell(row, OperatorRankingVo.IndexPuntaje).BackColor = GetColor(Convert.ToDouble(GridUtils.GetCell(row, OperatorRankingVo.IndexPuntaje).Text));
            }
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
            builder.GenerateRow(String.Format(CultureManager.GetLabel("PONDERACION_GRAVES_RANKING"), OperadoresGraves));
            builder.GenerateRow(String.Format(CultureManager.GetLabel("PONDERACION_MEDIAS_RANKING"), OperadoresMedios));
            builder.GenerateRow(String.Format(CultureManager.GetLabel("PONDERACION_LEVES_RANKING"), OperadoresLeves));

            SetCsvSessionVars(builder.Build());

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }

		private void GenerateFields(BaseCsvBuilder builder)
        {
            foreach (var o in GetResults())
            {
                var str = String.Concat(o.Operador, Separator, 
                                        o.Legajo, Separator, 
                                        o.CentroCosto, Separator,
                                        o.Departamento, Separator, Separator, 
                                        String.Format("{0:F}", o.Kilometros), Separator,
                                        String.Format("{0:00}.{1:00}:{2:00}:{3:00}", o.HorasMovimiento.Days, o.HorasMovimiento.Hours, o.HorasMovimiento.Minutes, o.HorasMovimiento.Seconds), Separator, 
                                        String.Format("{0:F}", o.Puntaje), Separator, Separator, 
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
                                              CultureManager.GetLabel("TOTALES"), 
                                              Separator, 
                                              Separator, 
                                              Separator, 
                                              Separator, 
                                              CultureManager.GetLabel("INFRACCIONES")));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("CHOFER"), Separator, 
                                              CultureManager.GetLabel("LEGAJO"), Separator,
                                              CultureManager.GetEntity("PARENTI37"), Separator,
                                              CultureManager.GetEntity("PARENTI04"), Separator, Separator, 
                                              CultureManager.GetLabel("KILOMETROS"), Separator,
                                              CultureManager.GetLabel("MOVIMIENTO"), Separator, 
                                              CultureManager.GetLabel("PUNTAJE"), Separator, Separator, 
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

            var str = Enumerable.Aggregate<int, string>(bases, String.Empty, (current, id) => String.Concat(current, DAOFactory.LineaDAO.FindById(Convert.ToInt32((int) id)).Descripcion, ", "));

            return str.TrimEnd(',');
        }

        /// <summary>
        /// Fills the ContentPrintDetails with data.
        /// </summary>
        protected override void OnPrePrint()
        {
            lblTotalesPrint.Text = lblTotales.Text;
            lblGravesPrint.Text = lblGraves.Text;
            lblMediasPrint.Text = lblMedias.Text;
            lblLevesPrint.Text = lblLeves.Text;
        }

        #endregion

        #endregion
    }
}

   


