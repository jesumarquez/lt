using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.ValueObjects.ReportObjects.Bolland;
using Logictracker.Culture;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.Bolland
{
    public partial class Performance : SecuredGridReportPage<PerformanceVo>
    {
        protected override string VariableName { get { return "REP_PERFORMANCE"; } }
        protected override string GetRefference() { return "REP_PERFORMANCE"; }
        protected override bool ExcelButton { get { return true; } }

        private const string sensorKm = "Tacometro_1_Odometro";
        private const string sensorVelo = "Tacometro_1_Velocidad";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dpDesde.SelectedDate = DateTime.Today;
                dpHasta.SelectedDate = DateTime.Today.AddDays(1).AddMinutes(-1);
            }
        }

        protected override List<PerformanceVo> GetResults()
        {
            var empresas = cbEmpresa.SelectedValues;
            var lineas = cbLinea.SelectedValues;
            var vehiculos = cbVehiculo.SelectedValues;
            var desde = dpDesde.SelectedDate.Value;
            var hasta = dpHasta.SelectedDate.Value;

            var coches = DAOFactory.CocheDAO.GetList(empresas, lineas);
            if (vehiculos.Count > 0 && !vehiculos.Any(v => v <= 0))
                coches = coches.Where(c => vehiculos.Contains(c.Id)).ToList();

            var dispositivos = coches.Where(c => c.Dispositivo != null).Select(c => c.Dispositivo.Id);

            var mediciones = DAOFactory.MedicionDAO.GetList(empresas, lineas, dispositivos, new[] {-1}, new[] {-1},
                                                            new[] {-1}, new[] {-1}, new[] {-1}, desde, hasta);

            var byDispositivo = mediciones.GroupBy(m => m.Dispositivo.Id);


            var eventos = DAOFactory.InfraccionDAO.GetByVehiculos(vehiculos, desde, hasta);
            var evByDispositivo = eventos.Where(x=>x.Vehiculo.Dispositivo != null).GroupBy(e => e.Vehiculo.Dispositivo.Id);

            var result = new List<PerformanceVo>(coches.Count);
            foreach (var coche in coches)
            {
                var performance = new PerformanceVo();
                var medi = coche.Dispositivo != null
                               ? byDispositivo.FirstOrDefault(m => m.Key == coche.Dispositivo.Id)
                               : null;
                if (medi != null)
                {
                    var bySensor = medi.GroupBy(m => m.Sensor.Codigo);

                    var velocidad = bySensor.FirstOrDefault(m => m.Key == sensorVelo);
                    if (velocidad != null)
                    {
                        performance.VelocidadMaxima = Convert.ToInt32(velocidad.Max(m => m.ValorDouble));
                    }

                    var km = bySensor.FirstOrDefault(m => m.Key == sensorKm);
                    if (km != null)
                    {
                        performance.Kilometros = Convert.ToInt32(km.Max(m => m.ValorDouble) - km.Min(m => m.ValorDouble));
                    }
                }
                var infracc = coche.Dispositivo != null
                                  ? evByDispositivo.FirstOrDefault(m => m.Key == coche.Dispositivo.Id)
                                  : null;
                if (infracc != null)
                {
                    performance.Infracciones =
                        infracc.Count(m => m.CodigoInfraccion == Infraccion.Codigos.ExcesoVelocidad);
                    performance.Aceleraciones =
                        infracc.Count(m => m.CodigoInfraccion == Infraccion.Codigos.AceleracionBrusca);
                    performance.Desaceleraciones =
                        infracc.Count(m => m.CodigoInfraccion == Infraccion.Codigos.FrenadaBrusca);
                    performance.Desconexiones =
                        infracc.Count(m => m.CodigoInfraccion == Infraccion.Codigos.BateriaDesconectada);
                }
                performance.Interno = coche.Interno;
                performance.Patente = coche.Patente;



                result.Add(performance);
            }
            return result;
        }

        protected void cbPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPeriodo.Selected <= 0) return;
            var periodo = DAOFactory.PeriodoDAO.FindById(cbPeriodo.Selected);
            dpDesde.SelectedDate = periodo.FechaDesde;
            dpHasta.SelectedDate = periodo.FechaHasta;
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected).RazonSocial : null},
                           {CultureManager.GetEntity("PARENTI02"), cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected).DescripcionCorta : null},
                           {CultureManager.GetEntity("PARENTI17"), cbTipoVehiculo.Selected > 0 ? DAOFactory.TipoCocheDAO.FindById(cbTipoVehiculo.Selected).Descripcion : null},
                           {CultureManager.GetEntity("PARENTI03"), cbVehiculo.Selected > 0 ? DAOFactory.CocheDAO.FindById(cbVehiculo.Selected).Interno : null},
                           {CultureManager.GetLabel("DESDE"), String.Concat(dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), String.Empty, dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString())},
                           {CultureManager.GetLabel("HASTA"), String.Concat(dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString(), String.Empty, dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString())}
                       };
        }
    }
}