using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;
using System;
using System.Linq;
using System.Collections.Generic;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class KpiDistribucion : ApplicationSecuredPage
    {   
        protected override string GetRefference() { return "KPI_DISTRIBUCION"; }
        protected override InfoLabel LblInfo { get { return null; } }

        #region Private Properties

        private static readonly DateTime DesdeHoy = DateTime.Today;
        private static readonly DateTime HastaHoy = DateTime.UtcNow;
        private static readonly DateTime DesdeMes = DateTime.Today.Date.AddDays(-(DateTime.Today.Day-1));
        private static readonly DateTime HastaMes = DesdeMes.AddMonths(1);
        private double _kmTotalHoy;
        private double _kmTotalMes;
        private double _kmTotalPromedio;
        private double _kmViajeHoy;
        private double _kmViajeMes;
        private double _kmViajePromedio;
        private double _kmImproductivosHoy;
        private double _kmProductivosHoy;
        private double _kmImproductivosMes;
        private double _kmProductivosMes;
        private double _kmImproductivosPromedio;
        private double _kmProductivosPromedio;
        private double _tiempoMovimientoHoy;
        private double _tiempoMovimientoMes;
        private double _tiempoMovimientoPromedio;
        private int _entregasHoy;
        private int _entregasMes;
        private int _entregasPromedio;
        private int _realizadasHoy;
        private int _realizadasMes;
        private int _realizadasPromedio;
        private TimeSpan _minDia;
        private TimeSpan _maxDia;
        private TimeSpan _minMes;
        private TimeSpan _maxMes;
       
        #endregion

        protected void BtnCalcular_OnClick(object sender, EventArgs e)
        {
            var coches = DAOFactory.CocheDAO.GetList(new[] { ddlEmpresa.Selected }, new[] { ddlPlanta.Selected })
                                            .Where(c => ddlMovil.SelectedValues.Contains(c.Id) || QueryExtensions.IncludesAll((IEnumerable<int>) ddlMovil.SelectedValues))
                                            .ToList();

            CalcularReportes(coches);

            LoadLabelsHoy();
            LoadGaugesHoy();
            divHoy.Visible = true;
        }
        protected void Timer_OnTick(object sender, EventArgs e) { }
        protected void LnkInfracciones_OnClick(object sender, EventArgs e) 
        { 
            Response.Redirect("~/Reportes/Accidentologia/infractionsDetails.aspx");
        }
        protected void LnkTickets_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/Reportes/CicloLogistico/DuracionEstadosTicket.aspx");
        }
        protected void LnkCercas_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/Reportes/DatosOperativos/GeocercasEvents.aspx");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            SetExpiration();
        }

        private void CalcularReportes(List<Coche> coches)
        {
            var dmsMes = DAOFactory.DatamartDAO.GetBetweenDates(coches.Select(c => c.Id).ToList(), DesdeMes.ToDataBaseDateTime(), HastaMes.ToDataBaseDateTime());
            _kmTotalMes += dmsMes.Sum(dm => dm.Kilometers);
            _tiempoMovimientoMes += dmsMes.Sum(dm => dm.HorasMarcha);

            foreach (var coche in coches.Where(c => c.Dispositivo != null))
            {
                _kmTotalHoy += DAOFactory.CocheDAO.GetDistance(coche.Id, DesdeHoy.ToDataBaseDateTime(), HastaHoy.ToDataBaseDateTime());
                _tiempoMovimientoHoy += DAOFactory.CocheDAO.GetRunningHours(coche.Id, DesdeHoy.ToDataBaseDateTime(), HastaHoy.ToDataBaseDateTime());
            }

            _kmTotalMes += _kmTotalHoy;
            _tiempoMovimientoMes += _tiempoMovimientoHoy;

            var viajes = DAOFactory.ViajeDistribucionDAO.GetList(new[] { ddlEmpresa.Selected },
                                                                 new[] { ddlPlanta.Selected },
                                                                 new[] { -1 },
                                                                 new[] { -1 },
                                                                 new[] { -1 },
                                                                 new[] { -1 },
                                                                 coches.Select(c => c.Id),
                                                                 DesdeHoy.ToDataBaseDateTime(),
                                                                 HastaHoy.ToDataBaseDateTime())
                                                        .Where(v => v.InicioReal.HasValue);

            foreach (var viaje in viajes)
            {
                var finViaje = viaje.Estado == ViajeDistribucion.Estados.Cerrado
                                   ? viaje.Fin
                                   : HastaHoy.ToDataBaseDateTime();
                _kmViajeHoy += DAOFactory.CocheDAO.GetDistance(viaje.Vehiculo.Id, 
                                                               viaje.InicioReal.Value,
                                                               finViaje);

                var detalles = viaje.Detalles.Where(d => d.Linea == null);

                var primera = detalles.Min(e => e.Entrada);
                if (primera.HasValue) _kmImproductivosHoy += DAOFactory.CocheDAO.GetDistance(viaje.Vehiculo.Id, viaje.InicioReal.Value, primera.Value);
                
                var ultima = detalles.Max(e => e.Salida);
                if (ultima.HasValue) _kmImproductivosHoy += DAOFactory.CocheDAO.GetDistance(viaje.Vehiculo.Id, ultima.Value, finViaje);
                
                if (!primera.HasValue && !ultima.HasValue) _kmImproductivosHoy += DAOFactory.CocheDAO.GetDistance(viaje.Vehiculo.Id, viaje.InicioReal.Value, finViaje);

                if (primera.HasValue && ultima.HasValue) _kmProductivosHoy += DAOFactory.CocheDAO.GetDistance(viaje.Vehiculo.Id, primera.Value, ultima.Value);
                
                _entregasHoy += detalles.Count();
                _realizadasHoy += detalles.Count(d => EntregaDistribucion.Estados.EstadosOk.Contains(d.Estado));

                var entregados = detalles.Where(d => d.Entrada.HasValue && d.Salida.HasValue);
                if (entregados.Any())
                {
                    var tiempos = entregados.Select(e => e.Salida.Value.Subtract(e.Entrada.Value));
                    if (_minDia.TotalMinutes < 1 || tiempos.Min() < _minDia)
                        _minDia = tiempos.Min();
                    if (_maxDia.TotalMinutes < 1 || tiempos.Max() > _maxDia)
                        _maxDia = tiempos.Max();
                }
            }
            _minMes = _minDia;
            _maxMes = _maxDia;

            var viajesMes = DAOFactory.ViajeDistribucionDAO.GetList(ddlEmpresa.SelectedValues,
                                                                    ddlPlanta.SelectedValues,
                                                                    new[] { -1 }, // TRANS
                                                                    new[] { -1 }, // DEPTOS
                                                                    new[] { -1 }, // CC
                                                                    new[] { -1 }, // SUBCC
                                                                    coches.Select(c => c.Id),
                                                                    DesdeMes.ToDataBaseDateTime(),
                                                                    DesdeHoy.ToDataBaseDateTime())
                                                           .Where(v => v.InicioReal.HasValue);

            foreach (var viaje in viajesMes)
            {
                var detalles = viaje.Detalles.Where(d => d.Linea == null);

                var primera = detalles.Min(e => e.Entrada);
                if (primera.HasValue)
                {
                    var dm = DAOFactory.DatamartDAO.GetMobilesKilometers(viaje.InicioReal.Value, primera.Value, new List<int> {viaje.Vehiculo.Id});
                    if (dm.Any()) _kmImproductivosMes += dm.First().Kilometers;
                }
                
                var ultima = detalles.Max(e => e.Salida);
                if (ultima.HasValue)
                {
                    var dm = DAOFactory.DatamartDAO.GetMobilesKilometers(ultima.Value, viaje.Fin, new List<int> {viaje.Vehiculo.Id});
                    if (dm.Any()) _kmImproductivosMes += dm.First().Kilometers;
                }
                
                if (!primera.HasValue && !ultima.HasValue)
                {
                    var dm = DAOFactory.DatamartDAO.GetMobilesKilometers(viaje.InicioReal.Value, viaje.Fin, new List<int> {viaje.Vehiculo.Id});
                    if (dm.Any()) _kmImproductivosMes += dm.First().Kilometers;
                }

                if (primera.HasValue && ultima.HasValue)
                {
                    var dm = DAOFactory.DatamartDAO.GetMobilesKilometers(primera.Value, ultima.Value, new List<int> {viaje.Vehiculo.Id});
                    var mk = dm.First();
                    _kmProductivosMes += mk != null ? mk.Kilometers : 0.0;
                }

                _entregasMes += detalles.Count();
                _realizadasMes += detalles.Count(d => EntregaDistribucion.Estados.EstadosOk.Contains(d.Estado));

                var entregados = detalles.Where(d => d.Entrada.HasValue && d.Salida.HasValue);
                if (entregados.Any())
                {
                    var tiempos = entregados.Select(e => e.Salida.Value.Subtract(e.Entrada.Value));
                    if (_minMes.TotalMinutes < 1 || tiempos.Min() < _minMes)
                        _minMes = tiempos.Min();
                    if (_maxMes.TotalMinutes < 1 || tiempos.Max() > _maxMes)
                        _maxMes = tiempos.Max();
                }
            }

            _kmProductivosMes += _kmProductivosHoy;
            _kmImproductivosMes += _kmImproductivosHoy;
            _kmViajeMes += _kmProductivosMes + _kmImproductivosMes;
            _entregasMes += _entregasHoy;
            _realizadasMes += _realizadasHoy;

            _kmTotalPromedio = _kmTotalMes / DesdeHoy.Day;
            _kmViajePromedio = _kmViajeMes / DesdeHoy.Day;
            _kmImproductivosPromedio = _kmImproductivosMes / DesdeHoy.Day;
            _kmProductivosPromedio = _kmProductivosMes / DesdeHoy.Day;
            _tiempoMovimientoPromedio = _tiempoMovimientoMes / DesdeHoy.Day;
            _entregasPromedio = _entregasMes / DesdeHoy.Day;
            _realizadasPromedio = _realizadasMes / DesdeHoy.Day;
        }

        private void LoadLabelsHoy()
        {
            lblKmTotales.Text = _kmTotalHoy.ToString("#0.00");
            lblKmEnViaje.Text = _kmViajeHoy.ToString("#0.00");

            lblKmMensual.Text = _kmTotalMes.ToString("#0.00");
            lblKmEnViajeMensual.Text = _kmViajeMes.ToString("#0.00");

            lblKmPromedio.Text = _kmTotalPromedio.ToString("#0.00");
            lblKmEnViajePromedio.Text = _kmViajePromedio.ToString("#0.00");

            lblKmImproductivos.Text = _kmImproductivosHoy.ToString("#0.00");
            lblKmProductivos.Text = _kmProductivosHoy.ToString("#0.00");

            lblKmImproductivosMensual.Text = _kmImproductivosMes.ToString("#0.00");
            lblKmProductivosMensual.Text = _kmProductivosMes.ToString("#0.00");

            lblKmImproductivosPromedio.Text = _kmImproductivosPromedio.ToString("#0.00");
            lblKmProductivosPromedio.Text = _kmProductivosPromedio.ToString("#0.00");

            lblTiempoMovimiento.Text = _tiempoMovimientoHoy.ToString("#0.00");
            lblTiempoMovimientoMensual.Text = _tiempoMovimientoMes.ToString("#0.00");
            lblTiempoMovimientoPromedio.Text = _tiempoMovimientoPromedio.ToString("#0.00");

            lblEntregas.Text = _entregasHoy.ToString("#0");
            lblEntregasMensual.Text = _entregasMes.ToString("#0");
            lblEntregasPromedio.Text = _entregasPromedio.ToString("#0");

            lblRealizadas.Text = _realizadasHoy.ToString("#0");
            lblRealizadasMensual.Text = _realizadasMes.ToString("#0");
            lblRealizadasPromedio.Text = _realizadasPromedio.ToString("#0");

            lblMaximo.Text = _maxDia.ToString();
            lblMinimo.Text = _minDia.ToString();
            lblMaximoMes.Text = _maxMes.ToString();
            lblMinimoMes.Text = _minMes.ToString();

            lblActiva.Text = DAOFactory.CocheDAO.GetList(new[] { ddlEmpresa.Selected }, new[] { ddlPlanta.Selected }, new short[] { 0 }).Count.ToString("#0");
            lblInactiva.Text = DAOFactory.CocheDAO.GetList(new[] { ddlEmpresa.Selected }, new[] { ddlPlanta.Selected }, new short[] { 2 }).Count.ToString("#0");
            lblMantenimiento.Text = DAOFactory.CocheDAO.GetList(new[] { ddlEmpresa.Selected }, new[] { ddlPlanta.Selected }, new short[] { 1 }).Count.ToString("#0");
        }

        private void LoadGaugesHoy()
        {
            var maxKm = _kmTotalHoy > _kmViajeHoy 
                                ? (Convert.ToInt32(_kmTotalHoy / 1000) + 1) * 1000
                                : (Convert.ToInt32(_kmViajeHoy / 1000) + 1) * 1000;
            gaugeKilometros.Gauges[0].Maximum = maxKm;
            gaugeKilometros.Gauges[0].Value = _kmTotalHoy;
            gaugeKilometros.Gauges[0].MorePointers[0].Value = _kmViajeHoy;

            var maxKmMensual = _kmTotalMes > _kmViajeMes 
                                    ? (Convert.ToInt32(_kmTotalMes / 2000) + 1) * 2000
                                    : (Convert.ToInt32(_kmViajeMes / 2000) + 1) * 2000;
            gaugeKmMensual.Gauges[0].Maximum = maxKmMensual;
            gaugeKmMensual.Gauges[0].Value = _kmTotalMes;
            gaugeKmMensual.Gauges[0].MorePointers[0].Value = _kmViajeMes;

            var maxKmPromedio = _kmTotalPromedio > _kmViajePromedio 
                                    ? (Convert.ToInt32(_kmTotalPromedio / 1000) + 1) * 1000
                                    : (Convert.ToInt32(_kmViajePromedio / 1000) + 1) * 1000;
            gaugeKmPromedio.Gauges[0].Maximum = maxKmPromedio;
            gaugeKmPromedio.Gauges[0].Value = _kmTotalPromedio;
            gaugeKmPromedio.Gauges[0].MorePointers[0].Value = _kmViajePromedio;

            var maxKmProductivosHoy = _kmProductivosHoy > _kmImproductivosHoy 
                                            ? (Convert.ToInt32(_kmProductivosHoy / 500) + 1) * 500
                                            : (Convert.ToInt32(_kmImproductivosHoy / 500) + 1) * 500;
            gaugeProductivos.Gauges[0].Maximum = maxKmProductivosHoy;
            gaugeProductivos.Gauges[0].Value = _kmProductivosHoy;
            gaugeProductivos.Gauges[0].MorePointers[0].Value = _kmImproductivosHoy;

            var maxKmProductivosMes = _kmProductivosMes > _kmImproductivosMes 
                                            ? (Convert.ToInt32(_kmProductivosMes / 2000) + 1) * 2000
                                            : (Convert.ToInt32(_kmImproductivosMes / 2000) + 1) * 2000;
            gaugeProductivosMes.Gauges[0].Maximum = maxKmProductivosMes;
            gaugeProductivosMes.Gauges[0].Value = _kmProductivosMes;
            gaugeProductivosMes.Gauges[0].MorePointers[0].Value = _kmImproductivosMes;

            var maxKmProductivosPromedio = _kmProductivosPromedio > _kmImproductivosPromedio 
                                                ? (Convert.ToInt32(_kmProductivosPromedio / 500) + 1) * 500
                                                : (Convert.ToInt32(_kmImproductivosPromedio / 500) + 1) * 500;
            gaugeKmProductivosPromedio.Gauges[0].Maximum = maxKmProductivosPromedio;
            gaugeKmProductivosPromedio.Gauges[0].Value = _kmProductivosPromedio;
            gaugeKmProductivosPromedio.Gauges[0].MorePointers[0].Value = _kmImproductivosPromedio;

            var maxMovimiento = (Convert.ToInt32(_tiempoMovimientoHoy / 200) + 1) * 200;
            gaugeMovimiento.Gauges[0].Maximum = maxMovimiento;
            gaugeMovimiento.Gauges[0].Value = _tiempoMovimientoHoy;

            var maxMovimientoMes = (Convert.ToInt32(_tiempoMovimientoMes / 1000) + 1) * 1000;
            gaugeMovimientoMes.Gauges[0].Maximum = maxMovimientoMes;
            gaugeMovimientoMes.Gauges[0].Value = _tiempoMovimientoMes;

            var maxMovimientoPromedio = (Convert.ToInt32(_tiempoMovimientoPromedio / 200) + 1) * 200;
            gaugeMovimientoPromedio.Gauges[0].Maximum = maxMovimientoPromedio;
            gaugeMovimientoPromedio.Gauges[0].Value = _tiempoMovimientoPromedio;

            var maxEntregas = (Convert.ToInt32(_entregasHoy / 100) + 1) * 100;
            gaugeEntregas.Gauges[0].Maximum = maxEntregas;
            gaugeEntregas.Gauges[0].Value = _entregasHoy;
            gaugeEntregas.Gauges[0].MorePointers[0].Value = _realizadasHoy;

            var maxEntregasMes = (Convert.ToInt32(_entregasMes / 2000) + 1) * 2000;
            gaugeEntregasMes.Gauges[0].Maximum = maxEntregasMes;
            gaugeEntregasMes.Gauges[0].Value = _entregasMes;
            gaugeEntregasMes.Gauges[0].MorePointers[0].Value = _realizadasMes;

            var maxEntregasPromedio = (Convert.ToInt32(_entregasPromedio / 100) + 1) * 100;
            gaugeEntregasPromedio.Gauges[0].Maximum = maxEntregasPromedio;
            gaugeEntregasPromedio.Gauges[0].Value = _entregasPromedio;
            gaugeEntregasPromedio.Gauges[0].MorePointers[0].Value = _realizadasPromedio;
        }

        private void SetExpiration()
        {
            gaugeKilometros.AbsoluteExpiration = gaugeKmMensual.AbsoluteExpiration = gaugeKmPromedio.AbsoluteExpiration =
            gaugeProductivos.AbsoluteExpiration = gaugeProductivosMes.AbsoluteExpiration = gaugeKmProductivosPromedio.AbsoluteExpiration =
            gaugeMovimiento.AbsoluteExpiration = gaugeMovimientoMes.AbsoluteExpiration = gaugeMovimientoPromedio.AbsoluteExpiration =
            gaugeEntregas.AbsoluteExpiration = gaugeEntregasMes.AbsoluteExpiration = gaugeEntregasPromedio.AbsoluteExpiration =
            DateTime.Today.AddDays(-1);

            gaugeKilometros.SlidingExpiration = gaugeKmMensual.SlidingExpiration = gaugeKmPromedio.SlidingExpiration =
            gaugeProductivos.SlidingExpiration = gaugeProductivosMes.SlidingExpiration = gaugeKmProductivosPromedio.SlidingExpiration =
            gaugeMovimiento.SlidingExpiration = gaugeMovimientoMes.SlidingExpiration = gaugeMovimientoPromedio.SlidingExpiration =
            gaugeEntregas.SlidingExpiration = gaugeEntregasMes.SlidingExpiration = gaugeEntregasPromedio.SlidingExpiration = 
            new TimeSpan(0, 0, 1);
        }
    }
}
