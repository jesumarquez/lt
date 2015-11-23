using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Messaging;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Reportes.M2M
{
    public partial class ReporteDeEventos : ApplicationSecuredPage
    {
        protected override string GetRefference() { return "M2M_RESUMEN_EVENTOS"; }
        protected override InfoLabel LblInfo { get { return null; } }

        private readonly string _codigoPolicia = MessageCode.KeyboardButton1.GetMessageCode();
        private readonly string _codigoAmbulancia = MessageCode.KeyboardButton2.GetMessageCode();
        private readonly string _codigoBomberos = MessageCode.KeyboardButton3.GetMessageCode();
        private const string CodigoExcesoTemperatura = "2802";
        private DateTime _desdeHoy;
        private DateTime _hastaHoy;
        private DateTime _desdeSemanal;
        private int _eventosPolicia;
        private int _eventosBomberos;
        private int _eventosAmbulancia;
        private int _eventosExcesosTemperatura;
        private double _temperaturaMaxima;
        private int _eventosPolicia7Dias;
        private int _eventosBomberos7Dias;
        private int _eventosAmbulancia7Dias;
        private int _eventosExcesosTemperatura7Dias;
        private double _temperaturaMaxima7Dias;

        protected void BtnBuscar_OnClick(object sender, EventArgs e)
        {
            SetExpiration();

            _desdeHoy = dtFecha.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtFecha.SelectedDate.Value.Date) : DateTime.MinValue;
            _hastaHoy = _desdeHoy.AddDays(1);
            _desdeSemanal = _desdeHoy.AddDays(-7);

            var entidades = DAOFactory.EntidadDAO.GetList(ddlEmpresa.SelectedValues, ddlPlanta.SelectedValues, new[] {-1}, new[] {-1});
            var entidadesId = entidades.Select(ent => ent.Id).ToList();

            _eventosPolicia = DAOFactory.LogEventoDAO.GetByEntitiesAndCodes(entidadesId, new List<string> { _codigoPolicia }, _desdeHoy, _hastaHoy).Count();
            _eventosBomberos = DAOFactory.LogEventoDAO.GetByEntitiesAndCodes(entidadesId, new List<string> { _codigoBomberos }, _desdeHoy, _hastaHoy).Count();
            _eventosAmbulancia = DAOFactory.LogEventoDAO.GetByEntitiesAndCodes(entidadesId, new List<string> { _codigoAmbulancia }, _desdeHoy, _hastaHoy).Count();
            _eventosExcesosTemperatura = DAOFactory.LogEventoDAO.GetByEntitiesAndCodes(entidadesId, new List<string> { CodigoExcesoTemperatura }, _desdeHoy, _hastaHoy).Count();

            _eventosPolicia7Dias = DAOFactory.LogEventoDAO.GetByEntitiesAndCodes(entidadesId, new List<string> { _codigoPolicia }, _desdeSemanal, _hastaHoy).Count();
            _eventosBomberos7Dias = DAOFactory.LogEventoDAO.GetByEntitiesAndCodes(entidadesId, new List<string> { _codigoBomberos }, _desdeSemanal, _hastaHoy).Count();
            _eventosAmbulancia7Dias = DAOFactory.LogEventoDAO.GetByEntitiesAndCodes(entidadesId, new List<string> { _codigoAmbulancia }, _desdeSemanal, _hastaHoy).Count();
            _eventosExcesosTemperatura7Dias = DAOFactory.LogEventoDAO.GetByEntitiesAndCodes(entidadesId, new List<string> { CodigoExcesoTemperatura }, _desdeSemanal, _hastaHoy).Count();

            var temperaturas = DAOFactory.MedicionDAO.GetList(ddlEmpresa.SelectedValues, 
                                                              ddlPlanta.SelectedValues,
                                                              new[] { -1 }, 
                                                              new[] { -1 }, 
                                                              new[] { -1 },
                                                              entidadesId, 
                                                              new[] { -1 },
                                                              new[] { 4 }, 
                                                              _desdeHoy, 
                                                              _hastaHoy);
            var temperaturas7Dias = DAOFactory.MedicionDAO.GetList(ddlEmpresa.SelectedValues,
                                                                   ddlPlanta.SelectedValues,
                                                                   new[] { -1 },
                                                                   new[] { -1 },
                                                                   new[] { -1 },
                                                                   entidadesId,
                                                                   new[] { -1 },
                                                                   new[] { 4 }, 
                                                                   _desdeSemanal, 
                                                                   _hastaHoy);

            if (!temperaturas.Any())
                _temperaturaMaxima = 0;
            else
                _temperaturaMaxima = -100;
            
            foreach (var medicion in temperaturas)
            {
                if (medicion.ValorDouble > _temperaturaMaxima)
                    _temperaturaMaxima = medicion.ValorDouble;
            }

            if (!temperaturas7Dias.Any())
                _temperaturaMaxima7Dias = 0;
            else
                _temperaturaMaxima7Dias = -100;

            foreach (var medicion in temperaturas7Dias)
            {

                if (medicion.ValorDouble > _temperaturaMaxima7Dias)
                    _temperaturaMaxima7Dias = medicion.ValorDouble;
            }

            LoadLabelsHoy();
            LoadGaugesHoy();
        }
  
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                dtFecha.SelectedDate = DateTime.UtcNow.ToDisplayDateTime();

            BtnBuscar_OnClick(null, null);
        }

        private void SetExpiration()
        {
            gaugePolicia.AbsoluteExpiration = gaugeBomberos.AbsoluteExpiration = gaugeAmbulancia.AbsoluteExpiration = DateTime.Today.AddDays(-1);
            gaugePolicia.SlidingExpiration = gaugeBomberos.SlidingExpiration = gaugeAmbulancia.SlidingExpiration = new TimeSpan(0, 0, 1);
        }

        private void LoadLabelsHoy()
        {
            lblPolicia.Text = _eventosPolicia.ToString();
            lblBomberos.Text = _eventosBomberos.ToString();
            lblAmbulancia.Text = _eventosAmbulancia.ToString();
            lblExcesosTemperatura.Text = _eventosExcesosTemperatura.ToString();
            lblTemperaturaMaxima.Text = _temperaturaMaxima.ToString();

            lblPoliciaSemanal.Text = _eventosPolicia7Dias.ToString();
            lblBomberosSemanal.Text = _eventosBomberos7Dias.ToString();
            lblAmbulanciaSemanal.Text = _eventosAmbulancia7Dias.ToString();
            lblExcesosTemperaturaSemanal.Text = _eventosExcesosTemperatura7Dias.ToString();
            lblTemperaturaMaximaSemanal.Text = _temperaturaMaxima7Dias.ToString();
        }

        private void LoadGaugesHoy()
        {
            var maxPol = (Convert.ToInt32(_eventosPolicia7Dias / 10) + 1) * 10;
            gaugePolicia.Gauges[0].Maximum = maxPol;
            gaugePolicia.Gauges[0].Value = _eventosPolicia;
            gaugePolicia.Gauges[0].MorePointers[0].Value = _eventosPolicia7Dias;

            var maxBomb = (Convert.ToInt32(_eventosBomberos7Dias / 10) + 1) * 10;
            gaugeBomberos.Gauges[0].Maximum = maxBomb;
            gaugeBomberos.Gauges[0].Value = _eventosBomberos;
            gaugeBomberos.Gauges[0].MorePointers[0].Value = _eventosBomberos7Dias;

            var maxAmb = (Convert.ToInt32(_eventosAmbulancia7Dias / 10) + 1) * 10;
            gaugeAmbulancia.Gauges[0].Maximum = maxAmb;
            gaugeAmbulancia.Gauges[0].Value = _eventosAmbulancia;
            gaugeAmbulancia.Gauges[0].MorePointers[0].Value = _eventosAmbulancia7Dias;
            
            var maxTemp = (Convert.ToInt32(_eventosExcesosTemperatura / 10) + 1) * 10;
            gaugeExcesosTemperatura.Gauges[0].Maximum = maxTemp;
            gaugeExcesosTemperatura.Gauges[0].Value = _eventosExcesosTemperatura;
            gaugeExcesosTemperatura.Gauges[0].MorePointers[0].Value = _eventosExcesosTemperatura7Dias;

            gaugeTemperaturaMaxima.Gauges[0].Value = _temperaturaMaxima;
            gaugeTemperaturaMaximaSemanal.Gauges[0].Value = _temperaturaMaxima7Dias;
        }
    }
}
