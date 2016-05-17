using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.UI.HtmlControls;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReporteDeViajes : ApplicationSecuredPage
    {
        protected override string GetRefference() { return "OPE_MON_VIAJES"; }
        protected override InfoLabel LblInfo { get { return null; } }
        
        private const string Verde = "#78E27D";
        private const string Rojo = "#E27878";
        private const string Azul = "#78A2E2";
        private const string Amarillo = "#EBE478";
        private const string Gris = "#CCCCCC";
        private const string Naranja = "#FF8000";
        private const string Blanco = "#FFFFFF";

        #region Control Events

        protected void cbLocacion_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void cbPlanta_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void cbTipoVehiculo_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void cbVehiculo_SelectedIndexChanged(object sender, EventArgs e) { }

        #endregion

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            C1WebChart1.AbsoluteExpiration = DateTime.Today.AddDays(-1);
            C1WebChart1.SlidingExpiration = new TimeSpan(0, 0, 1);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterExtJsStyleSheet();

                dtFecha.SelectedDate = DateTime.Today;
                dtFecha.SelectedDate = DateTime.Today.AddHours(23).AddMinutes(59);
            }
        }

        protected void BtnBuscar_OnClick(object sender, EventArgs e)
        {
            C1WebChart1.Visible = false;
            divVehiculo.Visible = false;
            lnkHistorico.Visible = false;
            LoadTable();
        }

        protected void DivRoute_OnClick(object sender, EventArgs e)
        {
            LoadPieChart();
            LoadTable();
            lnkHistorico.Visible = true;

            var coche = DAOFactory.CocheDAO.FindById(Convert.ToInt32((string) hidden.Value));

            var link = string.Format(
                        "../../Monitor/MonitorHistorico/monitorHistorico.aspx?Planta={0}&TypeMobile={1}&Movil={2}&InitialDate={3}&FinalDate={4}&ShowMessages=0&ShowPOIS=0&Empresa={5}",
                        coche.Linea != null ? coche.Linea.Id : -1, 
                        coche.TipoCoche.Id, 
                        coche.Id, 
                        hidDesde.Value,
                        hidHasta.Value, 
                        coche.Empresa != null ? coche.Empresa.Id : coche.Linea != null ? coche.Linea.Empresa.Id : -1);

            lnkHistorico.OnClientClick = string.Format("window.open('{0}', '" + CultureManager.GetMenu("OPE_MON_HISTORICO") + "')", link);
        }

        private void LoadPieChart()
        {
            var routes = GetRoutes(Convert.ToInt32((string) hidden.Value));
            var blancos = 0.0f;
            var amarillos = 0.0f;
            var azules = 0.0f;
            var grises = 0.0f;
            var rojos = 0.0f;
            var naranjas = 0.0f;
            var verdes = 0.0f;
            var total = 0.0f;
            
            foreach (var route in routes)
            {
                switch (route.VehicleStatus)
                {
                    case "Detenido":
                        switch (route.EngineStatus)
                        {
                            case "Encendido":
                                if (route.Duration > 0.83)      // 0.83 horas = 5 minutos
                                    azules += (float)route.Duration;
                                else
                                    verdes += (float)route.Duration;

                                break;
                            case "Apagado":
                                if (route.Duration >= 8)
                                    blancos += (float)route.Duration;
                                else
                                    if (route.Duration >= 1)
                                        amarillos += (float)route.Duration;
                                    else
                                        grises += (float)route.Duration;

                                break;
                        }
                        break;
                    case "En Movimiento":
                        if (route.AverageSpeed >= 120)
                            rojos += (float)route.Duration;
                        else
                            if (route.AverageSpeed >= 80)
                                naranjas += (float)route.Duration;
                            else
                                if (route.AverageSpeed > 2)
                                    verdes += (float)route.Duration;
                                else
                                    grises += (float)route.Duration;
                        break;
                    default:
                        blancos += (float)route.Duration;
                        break;
                }
                total += (float)route.Duration;
            }

            C1WebChart1.Visible = true;
            divVehiculo.Visible = true;
            var coche = DAOFactory.CocheDAO.FindById(Convert.ToInt32((string) hidden.Value));
            divVehiculo.Rows[0].Cells[0].InnerText = coche.Interno + " - " + coche.Patente
                                                   + (coche.Marca != null && coche.Marca.Descripcion.Trim() != "" ? " - " + coche.Marca.Descripcion : "")
                                                   + (coche.Modelo != null && coche.Modelo.Descripcion.Trim() != "" ? " - " + coche.Modelo.Descripcion : (coche.ModeloDescripcion.Trim() != "" ? coche.ModeloDescripcion : ""));
            
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[0].PointData.RemoveAt(0);
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[0].PointData.Add(new PointF(1, blancos));
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[0].TooltipTextLegend = ((total!=0?blancos / total * 100:0)).ToString("0.00%");
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[1].PointData.RemoveAt(0);
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[1].PointData.Add(new PointF(1, amarillos));
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[1].TooltipTextLegend = ((total!=0?amarillos / total * 100:0)).ToString("0.00%");
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[2].PointData.RemoveAt(0);
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[2].PointData.Add(new PointF(1, azules));
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[2].TooltipTextLegend = ((total!=0?azules / total * 100:0)).ToString("0.00%");
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[3].PointData.RemoveAt(0);
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[3].PointData.Add(new PointF(1, grises));
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[3].TooltipTextLegend = ((total!=0?grises / total * 100:0)).ToString("0.00%");
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[4].PointData.RemoveAt(0);
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[4].PointData.Add(new PointF(1, rojos));
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[4].TooltipTextLegend = ((total!=0?rojos / total * 100:0)).ToString("0.00%");
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[5].PointData.RemoveAt(0);
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[5].PointData.Add(new PointF(1, naranjas));
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[5].TooltipTextLegend = ((total!=0?naranjas / total * 100:0)).ToString("0.00%");
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[6].PointData.RemoveAt(0);
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[6].PointData.Add(new PointF(1, verdes));
            C1WebChart1.ChartGroups.ChartGroupsCollection[0].ChartData.SeriesList[6].TooltipTextLegend = ((total != 0 ? verdes / total * 100 : 0)).ToString("0.00%");
        }

        private void LoadTable()
        {
            dias.InnerHtml = string.Empty;

            var horas = npHoraHasta.Value - npHoraDesde.Value + 1;
            var ancho = (100 / horas) - 0.2;
            var indice = 0.0;

            for (var i = (int)npHoraDesde.Value; i < npHoraHasta.Value + 1; i++)
            {
                var div = string.Format(@"<div style=""position: absolute; height: 20px; left: {0}%; width: {1}%; background-color:{2}"">{3}</div>",
                                        indice.ToString("0.00"),
                                        ancho.ToString("0.00"),
                                        Gris,
                                        "<b>" + i + "</b>");
                indice += ancho + 0.2;

                dias.InnerHtml += div.Replace(',', '.');
            }

            var coches = Enumerable.Where<int>(cbVehiculo.SelectedValues, id => id > 0).Select(id => DAOFactory.CocheDAO.FindById(id)).Where(c => c.Dispositivo != null);

            foreach (var coche in coches)
            {
                var routes = GetRoutes(coche.Id);

                var rowContent = new HtmlTableRow();
                var cellContent = new HtmlTableCell();
                var cellVehiculos = new HtmlTableCell();

                cellVehiculos.InnerHtml += "<div id='" + "cont" + coche.Id + "' runat='server' style='position: relative; height:30px;'>";
                cellContent.InnerHtml += "<div id='" + "content" + coche.Id + "' runat='server' style='position: relative; height:30px;'>";

                var porcentajeAcumulado = 0.0;

                if (routes.Count() == 0)
                {
                    cellContent.InnerText = "NO SE ENCONTRARON POSICIONES PARA EL PERIODO SELECCIONADO.";
                    cellContent.Align = "center";
                    cellContent.VAlign = "middle";
                    cellContent.Style.Add("font-weight", "bold");
                }
                else
                {
                    foreach (var route in routes)
                    {
                        var porcentajeDelDia = route.Duration/horas*100;
                        var color = Blanco;

                        switch (route.VehicleStatus)
                        {
                            case "Detenido":
                                switch (route.EngineStatus)
                                {
                                    case "Encendido":
                                        color = route.Duration > 0.083
                                                    ? Azul
                                                    : Verde; // 0.083 horas = 5 minutos
                                        break;
                                    case "Apagado":
                                        color = route.Duration >= 8
                                                    ? Blanco
                                                    : route.Duration >= 1
                                                          ? Amarillo
                                                          : Gris;
                                        break;
                                }
                                break;
                            case "En Movimiento":
                                color = route.AverageSpeed >= 120
                                            ? Rojo
                                            : route.AverageSpeed >= 80
                                                  ? Naranja
                                                  : route.AverageSpeed > 2
                                                        ? Verde
                                                        : Gris;
                                break;
                        }

                        var desde = SecurityExtensions.ToDataBaseDateTime(dtFecha.SelectedDate.GetValueOrDefault().AddHours(npHoraDesde.Value)).ToString(CultureInfo.InvariantCulture);
                        var hasta = SecurityExtensions.ToDataBaseDateTime(dtFecha.SelectedDate.GetValueOrDefault().AddHours(npHoraHasta.Value)).AddMinutes(59).ToString(CultureInfo.InvariantCulture);

                        var div =
                            string.Format(
                                @"<div style=""position: absolute; cursor:pointer; font-size:bold; height: 30px; left: {0}%; width: {1}%; background-color:{2}"" onclick=""{3}"" runat=""server"" ></div>",
                                porcentajeAcumulado.ToString(CultureInfo.InvariantCulture),
                                (porcentajeDelDia + 0.1).ToString(CultureInfo.InvariantCulture),
                                color,
                                "docb(" + coche.Id + ",'" + desde + "','" + hasta + "')"
                                );

                        cellContent.InnerHtml += div;
                        porcentajeAcumulado += porcentajeDelDia;
                    }
                }

                cellContent.InnerHtml += "</div>";
                cellVehiculos.InnerText = coche.Interno;
                cellVehiculos.Align = "center";
                cellVehiculos.VAlign = "middle";
                cellVehiculos.Style.Add("font-weight", "bold");
                cellVehiculos.Style.Add("background-color", "#EEEEEE");

                rowContent.Cells.Add(cellVehiculos);
                rowContent.Cells.Add(cellContent);
                tblContent.Rows.Add(rowContent);
            }
            updContent.Update();
            tblReferencias.Visible = true;

        }

        private IEnumerable<MobileRoutes> GetRoutes(int vehicleId)
        {
            var desde = dtFecha.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dtFecha.SelectedDate.GetValueOrDefault().AddHours(npHoraDesde.Value)) : DateTime.Today.ToDataBaseDateTime().AddHours(npHoraDesde.Value);
            var hasta = dtFecha.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dtFecha.SelectedDate.GetValueOrDefault().AddHours(npHoraHasta.Value)).AddMinutes(59) : DateTime.Today.ToDataBaseDateTime().AddHours(npHoraHasta.Value).AddMinutes(59);

            var routes = ReportFactory.MobileRoutesDAO.GetMobileRoutes(vehicleId, desde, hasta);

            return MergeResults(routes);
        }

        private static IEnumerable<MobileRoutes> MergeResults(List<MobileRoutes> routes)
        {
            if (routes == null || routes.Count.Equals(0))
                routes = new List<MobileRoutes>();

            for (var i = 1; i < routes.Count; i++)
            {
                if (routes[i - 1].EqualState(routes[i]))
                {
                    MergeRouteFragments(routes[i - 1], routes[i]);
                    routes.RemoveAt(i);
                    i--;
                }
            }

            return routes;
        }

        private static void MergeRouteFragments(MobileRoutes pastFragment, MobileRoutes currentFragment)
        {
            pastFragment.AverageSpeed = pastFragment.AverageSpeed >= currentFragment.AverageSpeed ? pastFragment.AverageSpeed : currentFragment.AverageSpeed;
            pastFragment.Duration += currentFragment.Duration;
            pastFragment.FinalTime = currentFragment.FinalTime;
            pastFragment.InfractionsDuration += currentFragment.InfractionsDuration;
            pastFragment.Infractions += currentFragment.Infractions;
            pastFragment.Kilometers += currentFragment.Kilometers;
            pastFragment.MaxSpeed = pastFragment.MaxSpeed >= currentFragment.MaxSpeed ? pastFragment.MaxSpeed : currentFragment.MaxSpeed;
            pastFragment.MinSpeed = pastFragment.MinSpeed <= currentFragment.MinSpeed ? pastFragment.MinSpeed : currentFragment.MinSpeed;
        }
    }
}