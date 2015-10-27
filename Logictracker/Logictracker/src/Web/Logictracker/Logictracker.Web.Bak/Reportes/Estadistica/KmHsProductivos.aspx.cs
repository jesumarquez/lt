using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.Estadistica
{
    public partial class KmHsProductivos : SecuredGridReportPage<KmHsProductivosVo>
    {
        protected override string VariableName { get { return "EST_KM_HS_PRODUCTIVOS"; } }
        protected override string GetRefference() { return "EST_KM_HS_PRODUCTIVOS"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<KmHsProductivosVo> GetResults()
		{
            var inicio = DateTime.UtcNow;

            try
            {
                var results = DAOFactory.CocheDAO.GetList((IEnumerable<int>) ddlLocacion.SelectedValues, (IEnumerable<int>) ddlPlanta.SelectedValues, (IEnumerable<int>) ddlTipoVehiculo.SelectedValues)
                                                 .Where(v => ddlVehiculo.SelectedValues.Contains(v.Id))
                                                 .Select(v => new KmHsProductivosVo(v, dpDesde.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.Value) : DateTime.Today, dpHasta.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.Value) : DateTime.Today.AddDays(1)))
                                                 .ToList();

                STrace.Trace("Reporte de Horas Productivas", String.Format("Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));

                return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Reporte de Horas Productivas", e, String.Format("Reporte: Reporte de Horas Productivas. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));

                throw;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();

            Bind();
        }

        protected override void GenerateCustomColumns()
        {
            if (!rbutVerTurnos.Checked)
            {
                Grid.Columns[KmHsProductivosVo.IndexKmTurnos].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexHsTotalesTurno].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexEnMovimientoTurno].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexDetenidoTurno].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexRalentiTurno].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexSinReportarTurno].Visible = false;
            }
            if (!rbutVerViajes.Checked)
            {
                Grid.Columns[KmHsProductivosVo.IndexKmViajes].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexHsTotalesViaje].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexEnMovimientoViaje].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexDetenidoViaje].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexRalentiViaje].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexSinReportarViaje].Visible = false;
            }
            if (!rbutVerTimeTracking.Checked)
            {
                Grid.Columns[KmHsProductivosVo.IndexKmTimeTracking].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexHsTotalesTimeTracking].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexEnMovimientoTimeTracking].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexDetenidoTimeTracking].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexRalentiTimeTracking].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexSinReportarTimeTracking].Visible = false;
            }
            if (!rbutVerTickets.Checked)
            {
                Grid.Columns[KmHsProductivosVo.IndexKmTickets].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexHsTotalesTicket].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexEnMovimientoTicket].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexDetenidoTicket].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexRalentiTicket].Visible = false;
                Grid.Columns[KmHsProductivosVo.IndexSinReportarTicket].Visible = false;
            }
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlLocacion.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected).RazonSocial : null},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected).DescripcionCorta : null},
                           {CultureManager.GetLabel("DESDE"), String.Concat(dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), String.Empty, dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString())},
                           {CultureManager.GetLabel("HASTA"), String.Concat(dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString(), String.Empty, dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString())}
                       };
        }
    }
}