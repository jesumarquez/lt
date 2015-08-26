using System;
using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.DAL.DAO.BusinessObjects.Messages;
using Logictracker.Messaging;
using Logictracker.Process.Geofences;
using Logictracker.Services.Helpers;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using Logictracker.Utils;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Reportes.CicloLogistico.Hormigon
{
    public partial class ReporteTolva : SecuredGridReportPage<TolvaTourVo>
    {
        protected override string VariableName { get { return "REP_TOLVA"; } }
        protected override string GetRefference() { return "REP_TOLVA"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TolvaTourVo> GetResults()
        {
            if (cbVehiculo.GetSelectedIndices().Length == 0) cbVehiculo.ToogleItems();

            var vehiculos = cbVehiculo.SelectedValues;
            var desde = SecurityExtensions.ToDataBaseDateTime(dpInitDate.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpEndDate.SelectedDate.GetValueOrDefault());
            var distancia = ValidateInt32(txtDistancia.Text, "DISTANCIA");
            var codigoInicio = ((int)MessageIdentifier.TolvaActivated).ToString();
            var codigoFin = ((int)MessageIdentifier.TolvaDeactivated).ToString();
            var ocultarHuerfanos = chkHuerfanos.Checked;
            IEnumerable<LogMensajeDAO.MobileTour> eventos = DAOFactory.LogMensajeDAO.GetMobileTour(vehiculos, codigoInicio, codigoFin, desde, hasta, distancia, ocultarHuerfanos);

            return eventos.Select(o => new TolvaTourVo(o)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, TolvaTourVo dataItem)
        {
            var vehiculo = DAOFactory.CocheDAO.FindById(dataItem.IdVehiculo);

            var posInicio = new GPSPoint(dataItem.EntradaHora, (float) dataItem.LatitudInicio, (float) dataItem.LongitudInicio);
            var posFin = new GPSPoint(dataItem.SalidaHora, (float) dataItem.LatitudFin, (float) dataItem.LongitudFin);

            var estadoInicio = GeocercaManager.CalcularEstadoVehiculo(vehiculo, posInicio, DAOFactory);
            var estadoFin = GeocercaManager.CalcularEstadoVehiculo(vehiculo, posFin, DAOFactory);

            var geocerca = estadoInicio.GeocercasDentro.Intersect(estadoFin.GeocercasDentro).FirstOrDefault();
            var text = geocerca != null 
                ? geocerca.Geocerca.Descripcion
                : GeocoderHelper.GetDescripcionEsquinaMasCercana(dataItem.LatitudInicio, dataItem.LongitudInicio);

            e.Row.Cells[TolvaTourVo.Index.Ubicacion].Text = text;
            if (dataItem.Entrada == DateTime.MinValue)
            {
                e.Row.Cells[TolvaTourVo.Index.Entrada].Text = "";
                e.Row.Cells[TolvaTourVo.Index.EntradaHora].Text = "";
            }
            if (dataItem.Salida == DateTime.MinValue)
            {
                e.Row.Cells[TolvaTourVo.Index.Salida].Text = "";
                e.Row.Cells[TolvaTourVo.Index.SalidaHora].Text = "";
            }
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
                           {CultureManager.GetEntity("PARENTI01"), cbEmpresa.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), cbLinea.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI17"), cbTipoVehiculo.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpInitDate.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpInitDate.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           {CultureManager.GetLabel("HASTA"), dpEndDate.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpEndDate.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }
    }
}
