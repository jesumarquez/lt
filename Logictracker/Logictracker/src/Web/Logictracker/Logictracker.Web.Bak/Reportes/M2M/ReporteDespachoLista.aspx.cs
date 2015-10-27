using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.M2M
{
    public partial class ReporteDespachoLista : SecuredGridReportPage<ReporteDespachoVo>
    {
        protected override string VariableName { get { return "REP_DESPACHOS_LISTA"; } }
        protected override string GetRefference() { return "REP_DESPACHOS_LISTA"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<ReporteDespachoVo> GetResults()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dtFechaDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dtFechaHasta.SelectedDate.GetValueOrDefault());

            var consumos = DAOFactory.ConsumoCabeceraDAO.GetList(cbEmpresa.SelectedValues,
                                                                 cbLinea.SelectedValues,
                                                                 cbTransportista.SelectedValues,
                                                                 new[] { -1 }, // DEPARTAMENTOS
                                                                 new[] { -1 }, // CENTROS DE COSTO
                                                                 cbTipoVehiculo.SelectedValues,
                                                                 cbVehiculo.SelectedValues,
                                                                 new[] { -1 }, // TIPOS DE EMPLEADO
                                                                 new[] { -1 }, // EMPLEADOS
                                                                 new[] { -1 }, // TIPOS DE PROVEEDOR
                                                                 new[] { -1 }, // PROVEEDORES
                                                                 new[] { -1 }, // DEPOSITOS ORIGEN
                                                                 new[] { -1 }, // DEPOSITOS DESTINO
                                                                 desde,
                                                                 hasta)
                                                        .Where(c => c.Estado != ConsumoCabecera.Estados.Eliminado
                                                                 && c.Vehiculo != null)
                                                        .OrderBy(c => c.Vehiculo.Interno).ThenBy(c => c.Fecha);
            
            var detalles = (from consumo in consumos
                            from detalle in consumo.Detalles.Cast<ConsumoDetalle>()
                            where detalle.Insumo.TipoInsumo.DeCombustible
                            select detalle).ToList();

            var ret = new List<ReporteDespachoVo>();
            for (var i = 0; i < detalles.Count-1; i++)
            {
                if (detalles[i].ConsumoCabecera.Vehiculo == detalles[i+1].ConsumoCabecera.Vehiculo)
                    ret.Add(new ReporteDespachoVo(detalles[i], detalles[i+1]));
            }

            return ret;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dtFechaDesde.SetDate();
                dtFechaHasta.SetDate();
            }
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), cbEmpresa.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), cbLinea.SelectedItem.Text}
                       };
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ReporteDespachoVo despacho)
        {
            if (despacho.Coche.TipoCoche.AlarmaConsumo 
                && (despacho.DifPorc > despacho.Coche.TipoCoche.DesvioMaximo
                    || ((despacho.DifPorc * -1) > despacho.Coche.TipoCoche.DesvioMinimo)))
            {
                e.Row.BackColor = Color.Red;
                e.Row.ForeColor = Color.White;
                e.Row.Font.Bold = true;
            }
        }
    }
}