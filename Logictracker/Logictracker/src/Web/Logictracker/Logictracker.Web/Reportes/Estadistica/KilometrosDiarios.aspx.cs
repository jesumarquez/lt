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
using Logictracker.Culture;

namespace Logictracker.Web.Reportes.Estadistica
{
    public partial class KilometrosDiarios : SecuredGridReportPage<KilometrosDiariosVo>
    {
        protected override string GetRefference() { return "KILOMETROS_DIARIOS"; }
        protected override string VariableName { get { return "KILOMETROS_DIARIOS"; } }
        protected override bool ExcelButton { get { return true; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dtFecha.SetDate();
        }

        protected override List<KilometrosDiariosVo> GetResults()
        {
            var t = new TimeElapsed();

            var desde = dtFecha.SelectedDate.Value;
            var hasta = desde.AddDays(1);
            if (QueryExtensions.IncludesAll(lbMovil.SelectedValues))
                ToogleItems(lbMovil);

            var kilometrosDiarios = DAOFactory.DatamartDAO.GetKilometrosDiarios(desde.ToDataBaseDateTime(), hasta.ToDataBaseDateTime(), lbMovil.SelectedValues);
            
            STrace.Trace("Kilómetros Diarios", string.Format("Reporte: Duración de la consulta: {0} segundos", t.getTimeElapsed().TotalSeconds));

            Session.Add("RouteInitialDate", desde);
            Session.Add("RouteFinalDate", hasta);

            return kilometrosDiarios.Select(km => new KilometrosDiariosVo(km)).ToList();
        }

        protected override void SelectedIndexChanged()
        {
            var patente = Grid.Rows[Grid.SelectedIndex].Cells[KilometrosDiariosVo.IndexPatente].Text;
            var coche = DAOFactory.CocheDAO.FindByPatente(ddlLocation.Selected, patente);

            if (coche != null)
            {
                Session.Add("RouteLocation", coche.Empresa.Id);
                Session.Add("RouteCompany", coche.Linea != null ? coche.Linea.Id : 0);
                Session.Add("RouteMobile", coche.Id);

                OpenWin("MobileRoutes.aspx", CultureManager.GetMenu("STAT_RESUMEN_RUTA"));
            }
        }
    }
}
