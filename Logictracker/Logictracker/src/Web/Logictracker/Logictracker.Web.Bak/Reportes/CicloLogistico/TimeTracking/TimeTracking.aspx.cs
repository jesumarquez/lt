using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.CicloLogistico.TimeTracking;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.CicloLogistico.TimeTracking
{
    public partial class TimeTracking : SecuredGridReportPage<TimeTrackingVo>
    {
        protected override string VariableName { get { return "CLOG_TIMETRACKING"; } }
        protected override string GetRefference() { return "CLOG_TIMETRACKING"; }
        protected override bool ExcelButton { get { return true; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                var now = DateTime.UtcNow.ToDisplayDateTime().Date;
                dtDesde.SelectedDate = new DateTime(now.Year, now.Month, 1);
                dtHasta.SelectedDate = now;
            }
        }

        protected override List<TimeTrackingVo> GetResults()
        {
            var list = DAOFactory.EventoViajeDAO.GetList(cbEmpresa.SelectedValues, cbLinea.SelectedValues, cbVehiculo.SelectedValues, cbEmpleado.SelectedValues, dtDesde.SelectedDate.Value, dtHasta.SelectedDate.Value)
                .OrderBy(v=>v.Vehiculo)
                .ThenBy(v=>v.Fecha)
                .ToList();

            var viajes = new List<TimeTrackingVo>(list.Count);
            var numeroViaje = 0;
            EventoViaje anterior = null;
            EventoViaje entrada = null;
            var iniciado = false;
            var start = string.Empty;
            var startDate = DateTime.MinValue;
            var vehiculo = 0;
            foreach (var evento in list)
            {
                if(evento.Vehiculo.Id != vehiculo)
                {
                    anterior = null;
                    entrada = null;
                    iniciado = false;
                    start = string.Empty;
                    vehiculo = evento.Vehiculo.Id;
                }
                var inicio = !evento.EsEntrada && evento.EsInicio;
                var fin = evento.EsEntrada && evento.EsFin;
                if (!inicio && !iniciado) continue;
                if(inicio)
                {
                    var viaje = new TimeTrackingVo(null, evento, null, numeroViaje, DAOFactory);
                    iniciado = true;
                    anterior = evento;
                    entrada = null;
                    start = string.Format("{0}. De {1} a ", numeroViaje, evento.ReferenciaGeografica.Descripcion);
                    startDate = evento.Fecha;
                    viajes.Add(viaje);
                }
                else if(fin)
                {
                    var viaje = new TimeTrackingVo(evento, null, anterior, numeroViaje, DAOFactory);
                    iniciado = false;
                    anterior = null;
                    entrada = null;
                    viajes.Add(viaje);

                    foreach (var v in viajes.Where(v => v.Viaje == numeroViaje.ToString(CultureInfo.InvariantCulture)))
                    {
                        var duracionTotal = evento.Fecha.Subtract(startDate);
                        v.Viaje = start + evento.ReferenciaGeografica.Descripcion + " (" + duracionTotal.ToString()+")";
                        v.FechaDesde = startDate;
                        v.FechaHasta = evento.Fecha;
                    }

                    numeroViaje++;
                }
                else if(evento.EsEntrada)
                {
                    entrada = evento;
                }
                else if (!evento.EsEntrada)
                {
                    if (entrada != null && evento.ReferenciaGeografica.Id == entrada.ReferenciaGeografica.Id)
                    {
                        var viaje = new TimeTrackingVo(entrada, evento, anterior, numeroViaje, DAOFactory);
                        viajes.Add(viaje);
                    }
                    anterior = evento;
                    entrada = null;
                }
            }
            return viajes;
        }

        protected override void OnRowDataBound(C1.Web.UI.Controls.C1GridView.C1GridView grid, C1.Web.UI.Controls.C1GridView.C1GridViewRowEventArgs e, TimeTrackingVo dataItem)
        {
            if (dataItem.Tipo != 1)
            {
                var image = "~/images/" + (dataItem.Tipo == 0 ? "start.png" : "stop.png");
                e.Row.Cells[TimeTrackingVo.Index.Tipo].Controls.Add(new Image {ImageUrl = image});
                e.Row.Cells[TimeTrackingVo.Index.Duracion].Text = string.Empty;
                if(dataItem.Tipo == 0)
                {
                    e.Row.Cells[TimeTrackingVo.Index.KmRecorridos].Text = string.Empty;
                    e.Row.Cells[TimeTrackingVo.Index.TiempoViaje].Text = string.Empty;
                }
                    
            }
        }

        protected override void SelectedIndexChanged()
        {
            var fechaDesde = Convert.ToDateTime(Grid.SelectedDataKey[TimeTrackingVo.KeyIndex.FechaDesde]);
            var fechaHasta = Convert.ToDateTime(Grid.SelectedDataKey[TimeTrackingVo.KeyIndex.FechaHasta]);
            var idVehiculo = Convert.ToInt32(Grid.SelectedDataKey[TimeTrackingVo.KeyIndex.IdVehiculo]);
            var messages = new List<string> { MessageCode.InsideGeoRefference.GetMessageCode(), MessageCode.OutsideGeoRefference.GetMessageCode() };

            var vehiculo = DAOFactory.CocheDAO.FindById(idVehiculo);

            Session.Add("Distrito", vehiculo.Empresa != null ? vehiculo.Empresa.Id : -1);
            Session.Add("Location", vehiculo.Linea != null ? vehiculo.Linea.Id : -1);
            Session.Add("TypeMobile", vehiculo.TipoCoche != null ? vehiculo.TipoCoche.Id : -1);
            Session.Add("Mobile", idVehiculo);
            Session.Add("InitialDate", fechaDesde.AddMinutes(-5).ToDisplayDateTime());
            Session.Add("FinalDate", fechaHasta.AddMinutes(5).ToDisplayDateTime());
            Session.Add("MessagesIds", messages);

            OpenWin(ResolveUrl("~/Monitor/MonitorHistorico/monitorHistorico.aspx"), "Monitor Historico");
        }
    }
}

