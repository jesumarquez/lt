using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos.Partes;
using Logictracker.Web.Documentos.Interfaces;
using Logictracker.Web.Documentos.Partes;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Documentos
{
    public partial class Documentos_ParteReport : SecuredGridReportPage<ParteReportVo>
    {
        #region Constants

        private char Separator { get { return Usuario.CsvSeparator; } }
        private readonly string[] PublicColumns = new[] { "Fecha", "Empresa", "Nº Parte", "Vehículo", "Tipo de Servicio", "Equipo", "Centro de Costos", "Salida", "Llegada", "Proveedor - Hs Reportadas", "GPS - Hs Controladas", "Proveedor - Km Reportado", "GPS - Km Controlado", "GPS - Importe Controlado", "Estado" };
        private readonly string[] PrivateColumns = new[] { "Fecha", "Empresa", "Nº Parte", "Vehículo", "Tipo de Servicio", "Equipo", "Centro de Costos", "Salida", "Llegada", "Proveedor - Hs Reportadas", "GPS - Hs Controladas", "Diferencia Hs", "Proveedor - Km Reportado", "GPS - Km Controlado", " Diferencia Km", "Proveedor - Importe Reportado", "GPS - Importe Controlado", "Diferencia Importe", "Estado" };
        private string PublicTemplate { get { return "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14}".Replace(';', Separator); } }
        private string PrivateTemplate { get { return "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18}".Replace(';', Separator); } }

        #endregion

        private IReportStrategy Reporter;

        #region Protected Properties

        protected int Anio
        {
            get { return Convert.ToInt32(lblAnioPeriodo.Text); }
            set { lblAnioPeriodo.Text = value.ToString(); }
        }

        protected override string VariableName { get { return "COST_CERTIFICACION"; } }
        protected override string GetRefference() { return "PARTE_REPORT"; }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override void OnPreLoad(EventArgs e)
        {
            Reporter = new ParteReportStrategy(DAOFactory);
            base.OnPreLoad(e);
        }

        protected override void Bind()
        {
            base.Bind();

            if (WebSecurity.AuthenticatedUser.AccessLevel <= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.Public)
            {
                GridUtils.GetColumn(ParteReportVo.IndexDiffHoras).Visible = false;
                GridUtils.GetColumn(ParteReportVo.IndexDiffKmTotal).Visible = false;
                GridUtils.GetColumn(ParteReportVo.IndexImporte).Visible = false;
                GridUtils.GetColumn(ParteReportVo.IndexDiffImporte).Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {   
                Anio = DateTime.Now.Year;
                BindPeriodos();
                dtInicio.SetDate();
                dtFin.SetDate();
            }
        }

        protected void ChangeAnio(object sender, CommandEventArgs e)
        {
            var year = Anio;
            if (e.CommandName == "Down") year--;
            else if (e.CommandName == "Up") year++;

            Anio = year;
            BindPeriodos();
        }

        protected void BindPeriodos()
        {
            var list = DAOFactory.PeriodoDAO.GetByYear(cbLocacion.Selected, Anio);
        
            cbPeriodo.Items.Clear();
            cbPeriodo.Items.Add(new ListItem("", "0"));
            foreach (Periodo periodo in list)
                cbPeriodo.Items.Add(new ListItem(periodo.Descripcion + (periodo.Estado == Periodo.Abierto ? " (Abierto)" : " (Cerrado)"), periodo.Id.ToString()));
        }

        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            var data = new SearchData
                           {
                               Transportista = cbAseguradora.Selected,
                               Locacion = cbLocacion.Selected,
                               Planta = cbPlanta.Selected,
                               Movil = ddlMovil.Selected,
                               Equipo = ddlEquipo.Selected,
                               Inicio = dtInicio.SelectedDate.GetValueOrDefault(),
                               Fin = dtFin.SelectedDate.GetValueOrDefault(),
                               Estado = Convert.ToInt32(cbEstado.SelectedValue)
                           };
            data.Save(ViewState);
            base.BtnSearchClick(sender, e);
        }

        protected override List<ParteReportVo> GetResults()
        {
            return (from PartePersonal p in GetPartes() orderby p.Equipo select new ParteReportVo(p)).ToList();
        }

        private List<PartePersonal> GetPartes()
        {
            var data = SearchData.Load(ViewState);
            var list = Reporter.GetData(data.Transportista, data.Locacion, data.Planta, data.Movil, data.Equipo,
                                        data.Inicio.ToDataBaseDateTime(), data.Fin.ToDataBaseDateTime(), data.Estado);
            if (data.Planta <= 0)
            {
                var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
                var lineas = user.Lineas.OfType<Linea>().Select(l => l.Id).ToList();
                var empresas = user.Empresas.OfType<Empresa>().Select(l => l.Id).ToList();
                return list.OfType<PartePersonal>()
                    .Where(p => empresas.Count == 0 || (data.Locacion <= 0 || empresas.Contains(data.Locacion)))
                    .Where(p => lineas.Count == 0 || (data.Planta <= 0 || lineas.Contains(data.Planta))).ToList();
            }

            return list.OfType<PartePersonal>().ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ParteReportVo dataItem)
        {
        
            GridUtils.GetCell(e.Row, ParteReportVo.IndexEstado).Text = GetStringEstado(dataItem.Estado);

            GridUtils.GetCell(e.Row, ParteReportVo.IndexHorasReportadas).ForeColor = Color.Gray;
            GridUtils.GetCell(e.Row, ParteReportVo.IndexHorasControladas).ForeColor = Color.Green;
            GridUtils.GetCell(e.Row, ParteReportVo.IndexDiffHoras).ForeColor = Color.Red;

            GridUtils.GetCell(e.Row, ParteReportVo.IndexKmTotal).ForeColor = Color.Gray;
            GridUtils.GetCell(e.Row, ParteReportVo.IndexKmTotalCalculado).ForeColor = Color.Green;
            GridUtils.GetCell(e.Row, ParteReportVo.IndexDiffKmTotal).ForeColor = Color.Red;

            GridUtils.GetCell(e.Row, ParteReportVo.IndexImporte).ForeColor = Color.Gray;
            GridUtils.GetCell(e.Row, ParteReportVo.IndexImporteControlado).ForeColor = Color.Green;
            GridUtils.GetCell(e.Row, ParteReportVo.IndexDiffImporte).ForeColor = Color.Red;
        }

        protected void cbPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(cbPeriodo.SelectedValue);
            if(id == 0) return;
            var periodo = DAOFactory.PeriodoDAO.FindById(id);
            dtInicio.SelectedDate = periodo.FechaDesde;
            dtFin.SelectedDate = periodo.FechaHasta;
        }

        protected override void SelectedIndexChanged()
        {
            var id = Convert.ToInt32(Grid.SelectedDataKey);
            var doc = DAOFactory.DocumentoDAO.FindById(id);
            var parte = new PartePersonal(doc, DAOFactory);

            // los usuarios publicos no pueden ver los mapas de los partes sin controlar
            if (WebSecurity.AuthenticatedUser.AccessLevel <= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.Public && parte.Estado == 0) return;

            var coche = DAOFactory.CocheDAO.FindById(parte.Vehiculo);

            Session["Location"] = coche.Linea.Id;
            Session["TypeMobile"] = coche.TipoCoche.Id;
            Session["Mobile"] = parte.Vehiculo;
            Session["InitialDate"] = parte.Turnos[0].AlPozoSalida;
            Session["FinalDate"] = parte.Turnos[parte.Turnos.Count - 1].DelPozoLlegada;
            if (WebSecurity.AuthenticatedUser.AccessLevel <= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.Public)
                Session["LockFilters"] = true;
            OpenWin("../Monitor/MonitorDeCalidad/monitorDeCalidad.aspx", "Monitor De Calidad");
        }

        #endregion

        #region Private Methods

        private static string GetStringEstado(int estado)
        {
            return estado == 1 ? "Controlado" : (estado == 2 ? "Verificado" : "Sin Controlar");
        }

        #endregion

        #region Nested T: SearchData

        [Serializable]
        private class SearchData
        {
            public int Equipo;
            public int Estado;
            public DateTime Fin;
            public DateTime Inicio;
            public int Locacion;
            public int Movil;
            public int Planta;
            public int Transportista;

            public void Save(StateBag viewstate)
            {
                viewstate["SearchData"] = this;
            }
            public static SearchData Load(StateBag viewstate)
            {
                return viewstate["SearchData"] as SearchData;
            }
        }

        #endregion

        #region CSV and Print Methods

        protected override void ExportToCsv()
        {
            var publicuser = (WebSecurity.AuthenticatedUser.AccessLevel <= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.Public);

            var data = new SearchData
                           {
                               Transportista = cbAseguradora.Selected,
                               Locacion = cbLocacion.Selected,
                               Planta = cbPlanta.Selected,
                               Movil = ddlMovil.Selected,
                               Equipo = ddlEquipo.Selected,
                               Inicio = dtInicio.SelectedDate.GetValueOrDefault(),
                               Fin = dtFin.SelectedDate.GetValueOrDefault(),
                               Estado = Convert.ToInt32(cbEstado.SelectedValue)
                           };
            data.Save(ViewState);

            var sb = new StringBuilder();
            sb.AppendLine("Reporte de Partes de Transporte de Personal" + Separator);
            sb.AppendLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            sb.AppendLine("");
            sb.AppendLine("Empresa " + Separator + (cbAseguradora.Selected > 0 ? DAOFactory.TransportistaDAO.FindById(cbAseguradora.Selected).Descripcion : "Todas"));
            sb.AppendLine("Distrito " + Separator + DAOFactory.EmpresaDAO.FindById(cbLocacion.Selected).RazonSocial);
            sb.AppendLine("Base " + Separator + (cbPlanta.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbPlanta.Selected).Descripcion : "Todas"));
            sb.AppendLine("Movil " + Separator + (ddlMovil.Selected > 0 ? DAOFactory.CocheDAO.FindById(ddlMovil.Selected).Interno : "Todos"));
            sb.AppendLine("Equipo " + Separator + (ddlEquipo.Selected > 0 ? DAOFactory.EquipoDAO.FindById(ddlEquipo.Selected).Descripcion : "Todos"));
            sb.AppendLine("Desde " + Separator + dtInicio.SelectedDate.GetValueOrDefault().ToString("dd/MM/yyyy"));
            sb.AppendLine("Hasta " + Separator + dtFin.SelectedDate.GetValueOrDefault().ToString("dd/MM/yyyy"));
            sb.AppendLine("Estado " + Separator + cbEstado.SelectedItem.Text);
            sb.AppendLine("");

            sb.AppendLine(string.Join(Separator.ToString(), publicuser ? PublicColumns : PrivateColumns));

            IList partes = GetPartes();
            var alternateSeparator = Separator == ';' ? ',' : ';';
            foreach (PartePersonal parte in partes)
            {
                var estado = GetStringEstado(parte.Estado);
                var temp = publicuser ? PublicTemplate : PrivateTemplate;

                var par = new List<object>();
                par.Add(parte.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy"));
                par.Add(parte.Empresa);
                par.Add(parte.Codigo);
                par.Add(parte.Interno);
                par.Add(parte.TipoServicio);
                par.Add(parte.Equipo);
                par.Add(parte.CentroCostos);
                par.Add(parte.Salida.Replace(Separator, alternateSeparator));
                par.Add(parte.Llegada.Replace(Separator, alternateSeparator));
                par.Add(parte.Horas);
                par.Add(parte.HorasControladas);
                if (!publicuser) par.Add(parte.DiffHoras);
                par.Add(parte.KmTotal);
                par.Add(parte.KmTotalCalculado);
                if (!publicuser) par.Add(parte.DiffKmTotal);
                if (!publicuser) par.Add(parte.Importe);
                par.Add(parte.ImporteControlado);
                if (!publicuser) par.Add(parte.DiffImporte);
                par.Add(estado);

                sb.AppendLine(string.Format(temp, par.ToArray()));
            }
            sb.AppendLine("");
            var content = sb.ToString();

            SetCsvSessionVars(content);

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            var desde = dtInicio.SelectedDate.GetValueOrDefault();
            var hasta = dtFin.SelectedDate.GetValueOrDefault();

            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), cbLocacion.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbLocacion.Selected).RazonSocial : "Todas"},
                           {CultureManager.GetEntity("PARENTI02"), cbPlanta.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbPlanta.Selected).Descripcion : "Todas"},
                           {CultureManager.GetEntity("PARENTI19"), ddlEquipo.Selected > 0 ? DAOFactory.EquipoDAO.FindById(ddlEquipo.Selected).Descripcion : "Todos"},
                           {CultureManager.GetLabel("DESDE"), desde.ToShortDateString()},
                           {CultureManager.GetLabel("HASTA"), hasta.ToShortDateString()}
                       };
        }

        //protected override Dictionary<string, string> GetHeaderParameters()
        //{
        //    var data = SearchData.Load(ViewState);
        //    var header = new Dictionary<string, string>();
        //    header.Add("lblEmpresa", DAOFactory.TransportistaDAO.FindById(data.Transportista).Descripcion);
        //    header.Add("lblDistrito", DAOFactory.EmpresaDAO.FindById(data.Locacion).RazonSocial);
        //    header.Add("lblBase", data.Planta <= 0 ? "Todas" : DAOFactory.LineaDAO.FindById(data.Planta).Descripcion);
        //    header.Add("lblVehiculo", data.Movil <= 0 ? "Todos" : DAOFactory.CocheDAO.FindById(data.Movil).Interno);
        //    header.Add("lblEquipo", data.Equipo <= 0 ? "Todos" : DAOFactory.EquipoDAO.FindById(data.Equipo).Descripcion);
        //    header.Add("lblFechaDesde", data.Inicio.ToString("dd/MM/yyyy"));
        //    header.Add("lblFechaHasta", data.Fin.ToString("dd/MM/yyyy"));
        //    return header;
        //}

        #endregion




        //protected override void CalculateSubtotals()
        //{
        //    var subtotals = new Dictionary<string, double>();

        //    double km = 0, real = 0, diff = 0;
        //    TimeSpan hs = TimeSpan.Zero, hsreal = TimeSpan.Zero, hsdiff = TimeSpan.Zero;
        //    double imp = 0, impreal = 0, impdiff = 0;
        //    foreach (var personal in ReportObjectsList)
        //    {
        //        var group = personal.Equipo;

        //        if (subtotals.ContainsKey("hsTotal_" + group))
        //            subtotals["hsTotal_" + group] += personal.Horas.TotalMinutes;
        //        else subtotals.Add("hsTotal_" + group, personal.Horas.TotalMinutes);
        //        if (subtotals.ContainsKey("hsReal_" + group))
        //            subtotals["hsReal_" + group] += personal.HorasControladas.TotalMinutes;
        //        else subtotals.Add("hsReal_" + group, personal.HorasControladas.TotalMinutes);
        //        if (subtotals.ContainsKey("hsDiff_" + group))
        //            subtotals["hsDiff_" + group] += personal.DiffHoras.TotalMinutes;
        //        else subtotals.Add("hsDiff_" + group, personal.DiffHoras.TotalMinutes);

        //        km += personal.KmTotal;
        //        real += personal.KmTotalCalculado;
        //        diff += personal.DiffKmTotal;
        //        hs = hs.Add(personal.Horas);
        //        hsreal = hsreal.Add(personal.HorasControladas);
        //        hsdiff = hsdiff.Add(personal.DiffHoras);
        //        imp += personal.Importe;
        //        impreal += personal.ImporteControlado;
        //        impdiff += personal.DiffImporte;
        //    }
        //    subtotals.Add("kmTotal", km);
        //    subtotals.Add("kmReal", real);
        //    subtotals.Add("kmDiff", diff);
        //    subtotals.Add("hsTotal", hs.TotalMinutes);
        //    subtotals.Add("hsReal", hsreal.TotalMinutes);
        //    subtotals.Add("hsDiff", hsdiff.TotalMinutes);
        //    subtotals.Add("impTotal", hs.TotalMinutes);
        //    subtotals.Add("impReal", hsreal.TotalMinutes);
        //    subtotals.Add("impDiff", hsdiff.TotalMinutes);

        //    grid.Columns[IndexKmTotal].FooterText = km.ToString("00.00");
        //    grid.Columns[IndexKmReal].FooterText = real.ToString("00.00");
        //    grid.Columns[IndexKmDiff].FooterText = diff.ToString("00.00");
        //    grid.Columns[IndexHsTotal].FooterText = hs.ToString();
        //    grid.Columns[IndexHsReal].FooterText = hsreal.ToString();
        //    grid.Columns[IndexHsDiff].FooterText = hsdiff.ToString();
        //    grid.Columns[IndexImpTotal].FooterText = imp.ToString("00.00");
        //    grid.Columns[IndexImpReal].FooterText = impreal.ToString("00.00");
        //    grid.Columns[IndexImpDiff].FooterText = impdiff.ToString("00.00");

        //    SubTotals = subtotals;
        //}

        //protected override void grd_GroupAggregate(object sender, C1GroupTextEventArgs e)
        //{
        //    base.grd_GroupAggregate(sender, e);

        //    if (e.Col == grid.Columns[IndexHsTotal]) e.Text = GetSubTotalTime("hsTotal_" + e.GroupText);
        //    else if (e.Col == grid.Columns[IndexHsReal]) e.Text = GetSubTotalTime("hsReal_" + e.GroupText);
        //    else if (e.Col == grid.Columns[IndexHsDiff]) e.Text = GetSubTotalTime("hsDiff_" + e.GroupText);
        //}

    }
}
