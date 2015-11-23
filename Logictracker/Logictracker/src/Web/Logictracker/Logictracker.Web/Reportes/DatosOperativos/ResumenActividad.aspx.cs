using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Culture;
using Logictracker.Messaging;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Web.Reportes.DatosOperativos
{
    public partial class ResumenActividad : SecuredGridReportPage<ResumenActividadVo>
    {
        protected override string VariableName { get { return "RESUMEN_ACTIVIDAD"; } }
        protected override string GetRefference() { return "RESUMEN_ACTIVIDAD"; }
        protected override bool ExcelButton { get { return true; } }
        protected override bool CsvButton { get { return false; } }
        protected override bool PrintButton { get { return false; } }

        protected override List<ResumenActividadVo> GetResults()
        {
            var results = new List<ResumenActividadVo>();

            if (lbMobile.GetSelectedIndices().Length == 0) lbMobile.ToogleItems();
            var vehiculos = lbMobile.SelectedValues;
            var vehiculosSinInfo = new List<int>();
            var desde = dpInitDate.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpEndDate.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var fin = hasta > DateTime.UtcNow ? DateTime.UtcNow : hasta;

            if (desde > DateTime.UtcNow)
            {
                LblInfo.Mode = InfoLabelMode.ERROR;
                LblInfo.Text = "Fecha DESDE no puede ser mayor a la fecha actual";
                return new List<ResumenActividadVo>();
            }

            if (hasta > DateTime.UtcNow.Date.AddDays(1).ToDataBaseDateTime())
            {
                LblInfo.Mode = InfoLabelMode.ERROR;
                LblInfo.Text = "Fecha HASTA no puede ser mayor a la fecha actual";
                return new List<ResumenActividadVo>();
            }

            var eventos = DAOFactory.LogMensajeDAO.GetEventos(vehiculos, MessageCode.PrivacyOn.GetMessageCode(), MessageCode.PrivacyOff.GetMessageCode(), desde, hasta);
            eventos = eventos.OrderBy(e => e.Fecha).ToList();

            foreach (var vehiculo in vehiculos)
            {
                var eventosVehiculo = eventos.Where(ev => ev.IdCoche == vehiculo);
                var tiempoEncendido = new TimeSpan();
                var tiempoApagado = new TimeSpan();

                if (!eventosVehiculo.Any())
                {
                    vehiculosSinInfo.Add(vehiculo);
                    continue;
                }

                var inicio = desde;
                foreach (var evento in eventosVehiculo)
                {
                    if (evento.CodigoMensaje == MessageCode.PrivacyOn.GetMessageCode())
                    { // Estaba Encendido
                        tiempoEncendido = tiempoEncendido.Add(evento.Fecha.Subtract(inicio));
                        inicio = evento.Fecha;
                    }
                    else if (evento.CodigoMensaje == MessageCode.PrivacyOff.GetMessageCode())
                    { // Estaba Apagado
                        tiempoApagado = tiempoApagado.Add(evento.Fecha.Subtract(inicio));
                        inicio = evento.Fecha;
                    }
                }

                var ultimoEvento = eventosVehiculo.Last();

                if (ultimoEvento.CodigoMensaje == MessageCode.PrivacyOn.GetMessageCode())
                { // Quedo Apagado
                    tiempoApagado = tiempoApagado.Add(fin.Subtract(ultimoEvento.Fecha));
                }
                else if (ultimoEvento.CodigoMensaje == MessageCode.PrivacyOff.GetMessageCode())
                { // Quedo Encendido
                    tiempoEncendido = tiempoEncendido.Add(fin.Subtract(ultimoEvento.Fecha));
                }

                var result = new ResumenActividadVo(DAOFactory.CocheDAO.FindById(ultimoEvento.IdCoche).Patente, tiempoEncendido, tiempoApagado);
                results.Add(result);
            }

            if (vehiculosSinInfo.Any())
            {
                foreach (var vehicle in vehiculosSinInfo)
                {
                    var lastEvent = DAOFactory.LogMensajeDAO.GetLastByVehicleAndCodes(vehicle, MessageCode.PrivacyOn.GetMessageCode(), MessageCode.PrivacyOff.GetMessageCode(), desde.AddMonths(-3), desde);
                    if (lastEvent != null)
                    {
                        if (lastEvent.Mensaje.Codigo == MessageCode.PrivacyOn.GetMessageCode())
                        {
                            // Está apagado
                            var result = new ResumenActividadVo(lastEvent.Coche.Patente, new TimeSpan(), fin.Subtract(desde));
                            results.Add(result);
                        }
                        else if (lastEvent.Mensaje.Codigo == MessageCode.PrivacyOff.GetMessageCode())
                        {
                            // Está encendido
                            var result = new ResumenActividadVo(lastEvent.Coche.Patente, fin.Subtract(desde), new TimeSpan());
                            results.Add(result);
                        }
                    }
                    else
                    {
                        // Sin información
                        var vehiculo = DAOFactory.CocheDAO.FindById(vehicle);
                        var lastPosition = DAOFactory.LogPosicionDAO.GetLastVehiclePosition(vehiculo);

                        if (lastPosition != null && lastPosition.FechaMensaje > DateTime.UtcNow.AddMonths(-3))
                        {
                            // Está encendido
                            var result = new ResumenActividadVo(vehiculo.Patente, hasta.Subtract(desde), new TimeSpan());                            
                            results.Add(result);
                        }
                        else
                        {
                            // Está apagado
                            var result = new ResumenActividadVo(vehiculo.Patente,new TimeSpan(), hasta.Subtract(desde));                          
                            results.Add(result);
                        }                        
                    }
                }
            }

            return results;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsPostBack) return;
            dpInitDate.SetDate();
            dpEndDate.SetDate();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                            {CultureManager.GetLabel("DESDE"), dpInitDate.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpInitDate.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                            {CultureManager.GetLabel("HASTA"), dpEndDate.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpEndDate.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }
    }
}
