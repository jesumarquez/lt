using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ValueObjects.Mantenimiento;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.Estadistica
{
    public partial class Consumos : SecuredGridReportPage<ConsumoCCVo>
    {
        protected override string GetRefference() { return "REPORTE_CONSUMOS_CC"; }
        protected override string VariableName { get { return "REPORTE_CONSUMOS_CC"; } }
        public override OutlineMode GridOutlineMode { get { return OutlineMode.StartCollapsed; } }

        protected override Empresa GetEmpresa()
        {
            return (ddlLocation.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocation.Selected) : null;
        }
        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dtDesde.SelectedDate = DateTime.Today;
                dtHasta.SelectedDate = DateTime.Today.AddDays(1).AddMinutes(-1);
            }
        }

        protected override List<ConsumoCCVo> GetResults()
        {
            ToogleItems(lbMovil);
            ToogleItems(lbTipo);
            var inicio = DateTime.UtcNow;
            try
            {
                var consumos = DAOFactory.ConsumoCabeceraDAO.GetList(ddlLocation.SelectedValues,
                                                                     ddlPlanta.SelectedValues,
                                                                     new[] {-1}, // Departamentos
                                                                     new[] {-1}, // Transportistas
                                                                     new[] {-1}, // Centros de costo
                                                                     lbTipo.SelectedValues,
                                                                     lbMovil.SelectedValues,
                                                                     new[] {-1}, // Tipos de empleado
                                                                     new[] {-1}, // Empleados
                                                                     new[] {-1}, // Tipos de proveedor
                                                                     new[] {-1}, // Proveedores
                                                                     new[] {-1}, // Depositos Origen
                                                                     new[] {-1}, // Depositos Destino
                                                                     SecurityExtensions.ToDataBaseDateTime(dtDesde.SelectedDate.Value),
                                                                     SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.Value))
                                                            .Where(c => c.Estado != -1); ;

                var documentos = DAOFactory.DocumentoDAO.GetListForConsumos(ddlLocation.SelectedValues,
                                                                            ddlPlanta.SelectedValues,
                                                                            lbTipo.SelectedValues,
                                                                            lbMovil.SelectedValues,
                                                                            dtDesde.SelectedDate.Value,
                                                                            dtHasta.SelectedDate.Value);

                var results = new List<ConsumoCCVo>();
                foreach (var consumo in consumos)
                {
                    if (consumo.Vehiculo == null) continue;

                    var vehiculo = consumo.Vehiculo;
                    var fecha = consumo.Fecha;
                    // BUSCO DOCUMENTO DE ASIGNACION DEL VEHICULO EN LA FECHA DEL CONSUMO
                    var documento = documentos.FirstOrDefault(d => d.Vehiculo.Id == vehiculo.Id && fecha > d.Fecha && fecha < d.FechaCierre);

                    if (documento != null && documento.Id != 0)
                    {
                        var valores = documento.Parametros.Cast<DocumentoValor>();
                        valores = valores.Where(v => v.Parametro.TipoDato.Equals("centrocostos") && v.Parametro.Nombre.Equals("Destino")).ToList();

                        var centros = QueryExtensions.IncludesAll((IEnumerable<int>) lbCentroDeCostos.SelectedValues)
                                          ? DAOFactory.CentroDeCostosDAO.FindByEmpresasAndLineas(new List<int> {ddlLocation.Selected},
                                                                                                 new List<int> {ddlPlanta.Selected})
                                                                        .Cast<CentroDeCostos>().Select(cc => cc.Id).ToList()
                                          : lbCentroDeCostos.SelectedValues;
                        
                        var docs = valores.Where(v => centros.Contains(Convert.ToInt32(v.Valor)));

                        if (docs.Any())
                            results.Add(new ConsumoCCVo(consumo, Convert.ToInt32(docs.FirstOrDefault().Valor)));
                    }
                    else
                    {
                        if (vehiculo.CentroDeCostos != null 
                            && (lbCentroDeCostos.SelectedValues.Contains(vehiculo.CentroDeCostos.Id) || QueryExtensions.IncludesAll((IEnumerable<int>) lbCentroDeCostos.SelectedValues)))
                            results.Add(new ConsumoCCVo(consumo, vehiculo.CentroDeCostos.Id));
                    }
                }
                
				var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

                STrace.Trace("Reporte de Consumos por Centro de Costo", String.Format("Duración de la consulta: {0} segundos", duracion));

				return results;
            }
            catch (Exception e)
            {

                STrace.Exception("Reporte de Consumos por Centro de Costo", e, String.Format("Reporte: Reporte de Consumos por Centro de Costo. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));

                throw;
            }
        }
    }
}
