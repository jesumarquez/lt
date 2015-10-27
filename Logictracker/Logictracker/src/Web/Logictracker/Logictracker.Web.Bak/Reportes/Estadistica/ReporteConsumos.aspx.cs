using System;
using System.Linq;
using System.Collections.Generic;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using System.Drawing;

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReporteConsumos : SecuredGridReportPage<ConsumoCombustibleVo>
    {
        public override int PageSize { get { return 15; } }
        protected override string VariableName { get { return "STAT_REPORTE_CONSUMOS"; } }
        protected override string GetRefference() { return "REPORTE_CONSUMOS"; }
        
        protected void DdlDistritoInitialBinding(object sender, EventArgs e) { if (!IsPostBack && Location > 0) ddlDistrito.EditValue = Location; }
        protected void DdlBaseInitialBinding(object sender, EventArgs e) { if (!IsPostBack && Company > 0) ddlBase.EditValue = Company; }
        protected void DdlModeloInitialBinding(object sender, EventArgs e) { if (!IsPostBack && Model > 0) ddlModelo.EditValue = Model; }
        protected void DdlVehiculoInitialBinding(object sender, EventArgs e) { if (!IsPostBack && Mobile > 0) ddlVehiculo.EditValue = Mobile; }

        #region Private Properties

        private int Location
        {
            get
            {
                if (ViewState["RouteLocation"] == null)
                {
                    ViewState["RouteLocation"] = Session["RouteLocation"];

                    Session["RouteLocation"] = null;
                }

                return ViewState["RouteLocation"] != null ? Convert.ToInt32(ViewState["RouteLocation"]) : 0;
            }
        }
        private int Company
        {
            get
            {
                if (ViewState["RouteCompany"] == null)
                {
                    ViewState["RouteCompany"] = Session["RouteCompany"];

                    Session["RouteCompany"] = null;
                }

                return ViewState["RouteCompany"] != null ? Convert.ToInt32(ViewState["RouteCompany"]) : 0;
            }
        }
        private int Model
        {
            get
            {
                if (ViewState["Model"] == null)
                {
                    ViewState["Model"] = Session["Model"];

                    Session["Model"] = null;
                }

                return ViewState["Model"] != null ? Convert.ToInt32(ViewState["Model"]) : 0;
            }
        }
        private int Mobile
        {
            get
            {
                if (ViewState["RouteMobile"] == null)
                {
                    ViewState["RouteMobile"] = Session["RouteMobile"];

                    Session["RouteMobile"] = null;
                }

                return ViewState["RouteMobile"] != null ? Convert.ToInt32(ViewState["RouteMobile"]) : 0;
            }
        }
        private DateTime InitialDate
        {
            get
            {
                if (ViewState["RouteInitialDate"] == null)
                {
                    ViewState["RouteInitialDate"] = Session["RouteInitialDate"];

                    Session["RouteInitialDate"] = null;
                }

                return (DateTime)ViewState["RouteInitialDate"];
            }
        }
        private DateTime FinalDate
        {
            get
            {
                if (ViewState["RouteFinalDate"] == null)
                {
                    ViewState["RouteFinalDate"] = Session["RouteFinalDate"];

                    Session["RouteFinalDate"] = null;
                }

                return (DateTime)ViewState["RouteFinalDate"];
            }
        }

        #endregion

        #region Protected Methods
                
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();

                if (!Mobile.Equals(0))
                {
                    SetInitialFilterValues();
                    Bind();
                }
            }
        }

        protected override List<ConsumoCombustibleVo> GetResults()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            var consumos = DAOFactory.ConsumoCabeceraDAO.GetList(ddlDistrito.SelectedValues,
                                                                 ddlBase.SelectedValues,
                                                                 new[] { -1 }, // TRANSPORTISTAS
                                                                 new[] { -1 }, // DEPARTAMENTOS
                                                                 new[] { -1 }, // CENTROS DE COSTO
                                                                 new[] { -1 }, // TIPOS DE VEHICULO
                                                                 ddlVehiculo.SelectedValues,
                                                                 new[] { -1 }, // TIPOS DE EMPLEADO
                                                                 new[] { -1 }, // EMPLEADOS
                                                                 new[] { -1 }, // TIPOS DE PROVEEDOR
                                                                 new[] { -1 }, // PROVEEDORES
                                                                 new[] { -1 }, // DEPOSITOS ORIGEN
                                                                 new[] { -1 }, // DEPOSITOS DESTINO
                                                                 desde,
                                                                 hasta);

            var detalles = (from consumo in consumos
                            from detalle in consumo.Detalles.Cast<ConsumoDetalle>()
                            where detalle.Insumo.TipoInsumo.DeCombustible
                            select detalle).ToList();

            return detalles.Select(c => new ConsumoCombustibleVo(c)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ConsumoCombustibleVo dataItem)
        {
            var consumoGps = dataItem.ConsumoCalculadoGps;
            var vehiculo = DAOFactory.CocheDAO.FindById(dataItem.IdVehiculo);
            var desvioMinimo = consumoGps * vehiculo.TipoCoche.DesvioMinimo / 100;
            var desvioMaximo = consumoGps * vehiculo.TipoCoche.DesvioMaximo / 100;

            if (dataItem.Cantidad <= dataItem.Capacidad && dataItem.Cantidad <= consumoGps + desvioMinimo)
                e.Row.BackColor = Color.FromArgb(190, 255, 190);
            
            if (dataItem.Cantidad > consumoGps + desvioMaximo)
            {
                e.Row.BackColor = Color.FromArgb(255, 140, 140);
                GridUtils.GetCell(e.Row, ConsumoCombustibleVo.IndexCantidad).Font.Bold = true;
                GridUtils.GetCell(e.Row, ConsumoCombustibleVo.IndexConsumoCalculadoGps).Font.Bold = true;
            } 
            else
            {
                if (dataItem.Cantidad > consumoGps + desvioMinimo)
                {
                    e.Row.BackColor = Color.FromArgb(255, 255, 180);
                    GridUtils.GetCell(e.Row, ConsumoCombustibleVo.IndexCantidad).Font.Bold = true;
                    GridUtils.GetCell(e.Row, ConsumoCombustibleVo.IndexConsumoCalculadoGps).Font.Bold = true;
                }
            }

            if (dataItem.Cantidad > dataItem.Capacidad)
            {
                e.Row.BackColor = Color.FromArgb(255, 140, 140);
                GridUtils.GetCell(e.Row, ConsumoCombustibleVo.IndexCantidad).Font.Bold = true;
                GridUtils.GetCell(e.Row, ConsumoCombustibleVo.IndexCapacidad).Font.Bold = true;
            }
        } 
        
        #endregion

        #region Private Methods

        private void SetInitialFilterValues()
        {
            ddlDistrito.SetSelectedValue(Location);
            ddlBase.SetSelectedValue(Company);
            ddlModelo.SetSelectedValue(Model);
            ddlVehiculo.SetSelectedValue(Mobile);
            dpDesde.SelectedDate = InitialDate;
            dpHasta.SelectedDate = FinalDate;
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetLabel("INTERNO"), ddlVehiculo.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()} 
                       };
        }

        #endregion
    }
}
