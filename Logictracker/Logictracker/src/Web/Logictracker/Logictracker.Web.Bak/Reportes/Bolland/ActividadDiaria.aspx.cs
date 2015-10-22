using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.ReportObjects.Bolland;
using Logictracker.Culture;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.Bolland
{
    public partial class ActividadDiaria : SecuredGridReportPage<ActividadDiariaVo>
    {
        protected override string VariableName { get { return "REP_ACTIVIDAD_DIARIA"; } }
        protected override string GetRefference() { return "REP_ACTIVIDAD_DIARIA"; }
        protected override bool ExcelButton { get { return true; } }

        const string sensorKm = "Tacometro_1_Odometro";
        const string sensorVelo = "Tacometro_1_Velocidad";

        protected override List<ActividadDiariaVo> GetResults()
        {
            var empresas = cbEmpresa.SelectedValues;
            var lineas = cbLinea.SelectedValues;
            var vehiculo = cbVehiculo.Selected;
            var desde = dpDesde.SelectedDate.Value;
            var hasta = dpHasta.SelectedDate.Value;

            var dispositivo = DAOFactory.CocheDAO.FindById(vehiculo).Dispositivo;
            if(dispositivo == null) return new List<ActividadDiariaVo>(0);

            var mediciones = DAOFactory.MedicionDAO.GetList(empresas, lineas, new[]{dispositivo.Id}, new[] {-1}, new[] {-1}, new[] {-1}, new[] {-1}, new[] {-1}, desde, hasta)
                .Select(m => new ActividadDiariaVo
                                 {
                                     Evento = "Movil Detenido",
                                    Fecha = m.FechaMedicion.Date,
                                     Kilometraje = m.Sensor.Codigo == sensorKm ? Convert.ToInt32(m.ValorDouble) : 0,
                                     Velocidad = m.Sensor.Codigo == sensorVelo ? Convert.ToInt32(m.ValorDouble) : 0,
                                     Hora = m.FechaMedicion,
                                     Id = m.Sensor.Codigo == sensorVelo ? 1 
                                        : m.Sensor.Codigo == sensorKm ? 2 : 3
                                    
                                 });

            const int maxMonths = 3;
            var eventos = DAOFactory.LogMensajeDAO.GetEvents(vehiculo, desde, hasta, maxMonths)
                .Select(m => new ActividadDiariaVo
                {
                    Evento = m.Texto,
                    Fecha = m.Fecha.Date,
                    Kilometraje = 0,
                    Velocidad = 0,
                    Hora = m.Fecha,
                    Id = 0,
                    Empleado = m.Chofer != null ? m.Chofer.Entidad.Descripcion : string.Empty
                });

            var union = mediciones.Union(eventos).OrderBy(u => u.Hora).ThenByDescending(u => u.Id);

            var result = new List<ActividadDiariaVo>();

            var lastDate = DateTime.MinValue;
            ActividadDiariaVo reg = null;
            foreach (var med in union)
            {
                if (lastDate != med.Hora)
                {
                    reg = med;
                    reg.Id = 0;
                    result.Add(reg);
                }
                else
                {
                    reg.Evento = med.Evento;
                    switch (med.Id)
                    {
                        case 2: reg.Kilometraje = med.Kilometraje; break;
                        case 1: reg.Velocidad = med.Velocidad;
                            if(med.Velocidad > 0) reg.Evento =  "Movil en movimiento";
                            break;
                        case 0: reg.Empleado = med.Empleado; break;
                    }
                }
                lastDate = med.Hora;
            }
            

            return result;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dpDesde.SelectedDate = DateTime.Today;
                dpHasta.SelectedDate = DateTime.Today.AddDays(1).AddMinutes(-1);
            }
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