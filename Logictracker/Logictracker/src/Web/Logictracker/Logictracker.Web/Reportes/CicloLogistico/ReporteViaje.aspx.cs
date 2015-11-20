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

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class ReporteViaje : SecuredGridReportPage<ReporteViajeVo>
    {
        protected override string VariableName { get { return "REPORTE_VIAJE"; } }
        protected override string GetRefference() { return "REPORTE_VIAJE"; }
        public override int PageSize { get { return 500; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }
        }

        protected override List<ReporteViajeVo> GetResults()
        {
            var inicio = DateTime.UtcNow;
            try
            {
                var results = new List<ReporteViajeVo>();

                var vehiculos = DAOFactory.CocheDAO.GetList(ddlLocacion.SelectedValues,
                                                            ddlPlanta.SelectedValues,
                                                            new[] {-1}, // TIPOS
                                                            lbTransportista.SelectedValues,
                                                            new[] {-1}, // DEPTOS
                                                            new[] {-1}) // CENTROS
                                                   .Where(v => lbVehiculo.SelectedValues.Contains(v.Id) 
                                                            || QueryExtensions.IncludesAll(lbVehiculo.SelectedValues));
                
                var entregas = DAOFactory.DatamartDistribucionDAO.GetList(new[] {ddlLocacion.Selected},
                                                                          new[] {ddlPlanta.Selected},
                                                                          lbTransportista.SelectedValues,
                                                                          new[] {-1}, // DEPARTAMENTOS
                                                                          new[] {-1}, // CENTROS
                                                                          vehiculos.Select(v => v.Id).ToList(),
                                                                          dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime(),
                                                                          dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime())
                                                                 .Where(e => e.Detalle.Linea == null);

                var idsViajes = entregas.Select(e => e.Viaje.Id).Distinct();
                var viajes = DAOFactory.DatamartViajeDAO.GetList(idsViajes.ToArray());

                if (entregas.Any())
                {
                    foreach (var vehiculo in vehiculos)
                    {
                        var tiempoTotal = new TimeSpan();
                        var tiempoDetenido = new TimeSpan();
                        var entregasVehiculo = entregas.Where(e => e.Vehiculo.Id == vehiculo.Id);
                        var viajesVehiculo = viajes.Where(v => v.Vehiculo.Id == vehiculo.Id);

                        var visitadas = entregasVehiculo.Where(e => EntregaDistribucion.Estados.EstadosOk.Contains(e.Detalle.Estado));
                        var paradasVisitadas = visitadas.Count();
                        var minutosEnEntrega = entregasVehiculo.Sum(e => e.TiempoEntrega);
                        var tiempoEntrega = new TimeSpan(0, 0, (int) minutosEnEntrega * 60);
                        
                        foreach (var viaje in viajesVehiculo)
                        {
                            tiempoTotal = tiempoTotal.Add(new TimeSpan(0, 0, (int) (viaje.Duracion * 60)));
                            tiempoDetenido = tiempoDetenido.Add(new TimeSpan(0, 0, (int) (viaje.HorasDetenido * 3600)));
                        }
                        results.Add(new ReporteViajeVo(vehiculo, paradasVisitadas, tiempoEntrega, tiempoTotal, tiempoDetenido));
                    }
                }
                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

                STrace.Trace("Reporte de viaje", String.Format("Duración de la consulta: {0} segundos", duracion));
				return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Reporte de viaje", e);
                throw;
            }
        }


    }
}
