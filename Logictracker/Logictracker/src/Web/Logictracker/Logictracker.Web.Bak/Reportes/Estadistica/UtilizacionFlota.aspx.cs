using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Reportes.Estadistica
{
    public partial class Reportes_Estadistica_UtilizacionVehiculos : SecuredGridReportPage<MobileUtilizationVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "UTILIZACION_FLOTA"; } }

        protected override bool PrintButton{get{return true;}}

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "UTILIZACION_FLOTA"; }

        protected override List<MobileUtilizationVo> GetResults()
        {
            var desde = dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            return
                ReportFactory.MobileUtilizationDAO.GetMobileUtilizations(lbDistrito.SelectedValues, lbPlanta.SelectedValues,
                                                                         ddcTipoVehiculo.SelectedValues,
                                                                         ddlCentro.SelectedValues, desde, hasta,
                                                                         cbSoloImproductivos.Checked, Usuario.GmtModifier,
                                                                         user).Select(r => new MobileUtilizationVo(r)).
                    ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobileUtilizationVo dataItem)
        {
            GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexHsTurno).Text = string.Format("{0:0.00} hs", dataItem.HsTurno);
            GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexHsEsperadas).Text = string.Format("{0:0.00} hs", dataItem.HsEsperadas);
            GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexHsTurnoReales).Text = string.Format("{0:0.00} hs", dataItem.HsTurnoReales);
            GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexPorcentajeTurno).Text = string.Format("{0:0.00} %", dataItem.PorcentajeTurno);

            GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexHsFueraTurno).Text = string.Format("{0:0.00} hs", dataItem.HsFueraTurno);
            GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexHsRealesFueraTurno).Text = string.Format("{0:0.00} hs", dataItem.HsRealesFueraTurno);
            GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexPorcentajeFueraTurno).Text = string.Format("{0:0.00} %", dataItem.PorcentajeFueraTurno);

            GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexPorcentajeProd).Text = string.Format("{0:0.00} %" , dataItem.PorcentajeProd);
            GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexPorcentajeFueraTurno).Text = string.Format("{0:0.00} %" , dataItem.MovFueraTurno);
            GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexPorcentajeTotal).Text = string.Format("{0:0.00} %", dataItem.PorcentajeTotal);

            GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexPorcentajeProd).BackColor = Convert.ToDouble(GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexPorcentajeProd).Text.TrimEnd('%')) >= 0 ? Color.LightGreen : Color.Red;

            GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexHsFueraTurno).BackColor = Convert.ToDouble(GridUtils.GetCell(e.Row, MobileUtilizationVo.IndexHsFueraTurno).Text.TrimEnd('%')) < 0.01 ? Color.LightGreen : Color.Red;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override void  SelectedIndexChanged()
        {
            AddSessionParameters();
            OpenWin(String.Concat(ApplicationPath, "Reportes/Estadistica/UtilizacionVehiculos.aspx"), "Vehiculo");
        }

        #endregion

        #region CSV Methods

        //protected override void ExportToCsv()
        //{
        //    var builder = new BaseCSVBuilder(Usuario.CsvSeparator);

        //    GenerateCsvHeader(builder);
        //    GenerateColumns(builder);
        //    GenerateFields(builder);

        //    Session["CSV_EXPORT"] = builder.Build();
        //    Session["CSV_FILE_NAME"] = "report";

        //    OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        //}

        //private void GenerateFields(BaseCSVBuilder builder)
        //{
        //    var separator = Usuario.CsvSeparator;
        //    foreach (var o in GetResults())
        //    {
        //        var str = String.Concat(o.Centro, separator, o.Interno, separator, o.HsTurno, separator, o.HsEsperadas, separator, o.HsTurnoReales,
        //            separator, o.PorcentajeTurno, separator, o.HsFueraTurno, separator, o.HsRealesFueraTurno, separator, o.PorcentajeFueraTurno, separator, o.PorcentajeProd,
        //            separator, o.PorcentajeFueraTurno, separator, o.PorcentajeTotal);

        //        builder.GenerateRow(str);
        //    }
        //}

        //private static void GenerateColumns(BaseCSVBuilder builder)
        //{
        //    var separator = Usuario.CsvSeparator;
        //    builder.GenerateRow(String.Concat(separator,separator, CultureManager.GetLabel("HS_TURNO"), separator,separator,separator,separator, CultureManager.GetLabel("HS_FUERA_TURNO"),
        //                                        separator,separator,separator, CultureManager.GetLabel("RESUMEN_ESTADO")));

        //    builder.GenerateRow(String.Concat(CultureManager.GetEntity("PARENTI37"), separator, CultureManager.GetLabel("INTERNO"), separator, CultureManager.GetLabel("TOTAL"), separator,
        //        CultureManager.GetLabel("ESPERADO"), separator, CultureManager.GetLabel("REAL"), separator, CultureManager.GetLabel("PORCENTAJE_REAL"), separator, CultureManager.GetLabel("TOTAL"), separator,
        //        CultureManager.GetLabel("REAL"), separator, CultureManager.GetLabel("PORCENTAJE_REAL"), separator, CultureManager.GetLabel("PRODUCTIVIDAD"), separator,
        //        CultureManager.GetLabel("MOV_SIN_TURNO"), separator, CultureManager.GetLabel("TOTAL"), separator));
        //}

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), GetSelectedDistritos()},
                           {CultureManager.GetEntity("PARENTI02"), GetSelectedPlantas()},
                           {CultureManager.GetEntity("PARENTI17"), GetSelectedTipos()},
                           {CultureManager.GetEntity("PARENTI37"), GetSelectedCentros()},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                       };
        }

        private string GetSelectedDistritos()
        {
            if (lbDistrito.SelectedValues.Contains(0)) return "Todos";

            var str = lbDistrito.SelectedValues.Aggregate(String.Empty, (current, id) => String.Concat(current, DAOFactory.EmpresaDAO.FindById(Convert.ToInt32(id)).RazonSocial, ", "));

            return str.TrimEnd(',');
        }

        private string GetSelectedPlantas()
        {
            if (lbPlanta.SelectedValues.Contains(0)) return "Todos";

            var str = lbPlanta.SelectedValues.Aggregate(String.Empty, (current, id) => String.Concat(current, DAOFactory.LineaDAO.FindById(Convert.ToInt32(id)).Descripcion, ", "));

            return str.TrimEnd(',');
        }

        private string GetSelectedTipos()
        {
            if (ddcTipoVehiculo.SelectedValues.Contains(0)) return "Todos";

            var str = ddcTipoVehiculo.SelectedValues.Aggregate(String.Empty, (current, id) => String.Concat(current, DAOFactory.TipoCocheDAO.FindById(Convert.ToInt32(id)).Descripcion, ", "));

            return str.TrimEnd(',');
        }

        private string GetSelectedCentros()
        {
            if (ddlCentro.SelectedValues.Contains(0)) return "Todos";

            var str = ddlCentro.SelectedValues.Aggregate(String.Empty, (current, id) => String.Concat(current, DAOFactory.CentroDeCostosDAO.FindById(Convert.ToInt32(id)).Descripcion, ", "));

            return str.TrimEnd(',');
        }

        #endregion

        #region Private Methods

        private void AddSessionParameters()
        {
            Session.Add("Movil", Convert.ToInt32(Grid.SelectedDataKey[0]));
            Session.Add("Desde", dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime());
            Session.Add("Hasta", dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime());
        }

        #endregion

        protected void p_Click(object sender, EventArgs e)
        {
            Print();
        }
        
    }
}
