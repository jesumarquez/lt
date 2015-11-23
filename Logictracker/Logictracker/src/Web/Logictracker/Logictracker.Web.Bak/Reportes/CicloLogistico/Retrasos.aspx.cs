using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Process.CicloLogistico;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using C1.Web.UI.Controls.C1GridView;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class Retrasos : SecuredGridReportPage<RetrasoVo>
    {
        protected override string GetRefference() { return "REPORTE_RETRASOS"; }
        protected override string VariableName { get { return "REPORTE_RETRASOS"; } }
        protected override bool ExcelButton { get { return true; } }

        protected override Empresa GetEmpresa()
        {
            return (ddlLocation.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocation.Selected) : null;
        }
        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }

        protected override List<RetrasoVo> GetResults()
        {
            var inicio = DateTime.UtcNow;
            try
            {
                var coches = DAOFactory.CocheDAO.GetList(new[] { ddlLocation.Selected },
                                                         new[] { ddlPlanta.Selected },
                                                         lbTipoMovil.SelectedValues,
                                                         new[] { -1 },
                                                         lbCentroCosto.SelectedValues)
                                                .Where(c => QueryExtensions.IncludesAll((IEnumerable<int>) lbMovil.SelectedValues) 
                                                         || lbMovil.SelectedValues.Contains(c.Id));

                var fecha = DateTime.UtcNow;
                fecha = new DateTime(fecha.Year, fecha.Month, fecha.Day, fecha.Hour, fecha.Minute, fecha.Second);

                var results = coches.Select(c => CicloLogisticoFactory.GetCiclo(c, null))
                                   .Where(c => c != null)
                                   .Where(c => c.Iniciado.AddMinutes(npEnCiclo.Value) < fecha
                                            || (c.EnGeocerca 
                                             && c.EnGeocercaDesde.HasValue 
                                             && c.EnGeocercaDesde.Value.AddMinutes(npEnGeocerca.Value) < fecha))
                                   .Select(c => new RetrasoVo(c, fecha))
                                   .ToList();

                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

				STrace.Trace("Reporte de Retrasos", String.Format("Duración de la consulta: {0} segundos", duracion));

				return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Reporte de Retrasos", e, String.Format("Reporte: Reporte de Retrasos. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));

                throw;
            }
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, RetrasoVo dataItem)
        {
            if (dataItem.Atraso.TotalMinutes > npEnGeocerca.Value)
                e.Row.Cells[RetrasoVo.IndexEnGeocerca].ForeColor = Color.Red;

            if (dataItem.TiempoEnCiclo.TotalMinutes > npEnCiclo.Value)
                e.Row.Cells[RetrasoVo.IndexEnCiclo].ForeColor = Color.Red;
        }
    }
}
