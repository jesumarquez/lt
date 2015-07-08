using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Utils;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Web.Reportes.Estadistica
{
    public partial class KilometrosDiarios : SecuredGridReportPage<KilometrosDiariosVo>
    {
        protected override string GetRefference() { return "KILOMETROS_DIARIOS"; }
        protected override string VariableName { get { return "KILOMETROS_DIARIOS"; } }
        
        protected override bool ExcelButton { get { return true; } }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool SendReportButton { get { return true; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dtDesde.SetDate();
            dtHasta.SetDate();
        }

        protected override List<KilometrosDiariosVo> GetResults()
        {
            var t = new TimeElapsed();

            var desde = dtDesde.SelectedDate.Value.ToDataBaseDateTime();
            var hasta = dtHasta.SelectedDate.Value.ToDataBaseDateTime();
            if (QueryExtensions.IncludesAll(lbMovil.SelectedValues))
                ToogleItems(lbMovil);

            var empresa = DAOFactory.EmpresaDAO.FindById(ddlLocation.Selected);
            var timeZoneId = empresa != null ? empresa.TimeZoneId : string.Empty;
            var gmt = string.IsNullOrEmpty(timeZoneId) ? -3 : (int)TimeZoneInfo.FindSystemTimeZoneById(timeZoneId).BaseUtcOffset.TotalHours;

            var kilometrosDiarios = DAOFactory.DatamartDAO.GetKilometrosDiarios(desde, hasta, lbMovil.SelectedValues, gmt);
            
            STrace.Trace("Kilómetros Diarios", string.Format("Reporte: Duración de la consulta: {0} segundos", t.getTimeElapsed().TotalSeconds));

            return kilometrosDiarios.Select(km => new KilometrosDiariosVo(km)).ToList();
        }

        protected override Empresa GetEmpresa()
        {
            return (ddlLocation.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocation.Selected) : null;
        }

        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }
    }
}
