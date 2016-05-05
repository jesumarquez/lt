using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class ReporteAsigno : SecuredGridReportPage<ReporteAsignoVo>
    {
        protected override string VariableName { get { return "REPORTE_ASIGNO"; } }
        protected override string GetRefference() { return "REPORTE_ASIGNO"; }
        protected override bool ExcelButton { get { return true; } }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }
        }

        protected override List<ReporteAsignoVo> GetResults()
        {
            var inicio = DateTime.UtcNow;
            try
            {
                var results = new List<ReporteAsignoVo>();

                var vehiculos = DAOFactory.CocheDAO.GetList(ddlLocacion.SelectedValues,
                                                            ddlPlanta.SelectedValues,
                                                            new[] {-1}, // TIPOS
                                                            lbTransportista.SelectedValues,
                                                            new[] {-1}, // DEPTOS
                                                            new[] {-1}) // CENTROS
                                                   .Where(v => lbVehiculo.SelectedValues.Contains(v.Id) 
                                                            || QueryExtensions.IncludesAll(lbVehiculo.SelectedValues));
                
                var viajes = DAOFactory.ViajeDistribucionDAO.GetList(new[] {ddlLocacion.Selected},
                                                                     new[] {ddlPlanta.Selected},
                                                                     lbTransportista.SelectedValues,
                                                                     new[] {-1}, // DEPARTAMENTOS
                                                                     new[] {-1}, // CENTROS
                                                                     new[] {-1}, // SUBCENTROS
                                                                     vehiculos.Select(v => v.Id).ToList(),
                                                                     dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime(),
                                                                     dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime());


                foreach (var viaje in viajes)
                {
                    var entregas = viaje.Detalles.Where(d => d.Linea == null);
                    foreach (var entrega in entregas)
                    {
                        var remitos = entrega.Remitos;
                        if (!remitos.Any())
                            results.Add(new ReporteAsignoVo(entrega, null));
                        else
                        {   
                            foreach (var remito in remitos)
                            {
                                foreach (var detalle in remito.Detalles)
                                {
                                    foreach (var orden in detalle.Ordenes)
                                    {
                                        results.Add(new ReporteAsignoVo(entrega, orden));
                                    }
                                }
                            }
                        }
                    }
                }

                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

                STrace.Trace("Reporte de asigno", string.Format("Duración de la consulta: {0} segundos", duracion));
				return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Reporte de asigno", e);
                throw;
            }
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           { CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text },
                           { CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text },
                           { CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.Value.ToString("dd/MM/yyyy HH:mm") }, 
                           { CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.Value.ToString("dd/MM/yyyy HH:mm") }
                       };
        }
    }
}
