using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.ValueObjects;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Security;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class Reportes_CicloLogistico_DuracionEstadosTicket : SecuredGridReportPage<DuracionEstadosVo>
    {
        protected override string VariableName { get { return "CLOG_REP_DURATION_TICKETS"; } }
        protected override string GetRefference() { return "DURATION_TICKETS"; }
        public override OutlineMode GridOutlineMode { get { return OutlineMode.StartCollapsed; } }
        public override bool HasTotalRow { get { return true; } }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool ExcelButton { get { return true; } }
        protected override Empresa GetEmpresa()
        {
            return (ddlLocacion.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected) : null;
        }
        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }

        private IList _estados;
        private IList Estados
        {
            get { return _estados ?? (_estados = DAOFactory.EstadoDAO.GetByPlanta(ddlPlanta.Selected)); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }
        }

        protected override List<DuracionEstadosVo> GetResults()
        {
            var inicio = DateTime.UtcNow;
            try
            {
                var results = DAOFactory.TicketDAO.GetList(new[] { ddlLocacion.Selected },
                                                           new[] { ddlPlanta.Selected },
                                                           new[] { -1 }, //TRANSPORTISTAS
                                                           new[] { -1 }, //DEPARTAMENTOS
                                                           new[] { -1 }, //CENTROS DE COSTO
                                                           new[] { cbTipoVehiculo.Selected },
                                                           new[] { -1 }, //VEHICULOS
                                                           new[] { -1 }, //ESTADOS
                                                           new[] { cbCliente.Selected },
                                                           new[] { cbPuntoEntrega.Selected },
														   new[] { -1 },
                                                           SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault()),
                                                           SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault()))
                                                  .Select(t => new DuracionEstadosVo(t))
                                                  .Where(t => (chkIncluirCiclo0.Checked || t.TiempoCiclo != TimeSpan.Zero) && 
                                                              (chkIncluirObra0.Checked || t.TiempoEnObra != TimeSpan.Zero) &&
                                                              (chkIncluirSinVehiculo.Checked || t.Vehiculo != string.Empty))
                                                  .ToList();
                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

				STrace.Trace("Control de Ciclo", String.Format("Duración de la consulta: {0} segundos", duracion));
				return results;
            }
            catch (Exception e)
            {
				STrace.Exception("Control de Ciclo", e, String.Format("Reporte: Control de Ciclo. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));
                throw;
            }
        }

        protected override void GenerateCustomColumns()
        {
            if (chkIncluirEstados.Checked)
            {
                for (var i = 0; i < Estados.Count - 1; i++)
                {
                    var estado = Estados[i] as Estado;
                    C1Field field = new C1TemplateField
                                        {
                                            AllowGroup = false,
                                            AllowMove = false,
                                            AllowSizing = false,
                                            HeaderText = estado.Descripcion,
                                            Visible = true,
                                            Aggregate = AggregateEnum.Custom
                                        };

                    GridUtils.AddAggregate(estado.Id.ToString("#0"), DuracionEstadosVo.IndexDynamicColumns + i, "{0}", GridAggregateType.Avg);
                    Grid.Columns.Add(field);
                }
            }
        }

        protected override Dictionary<string, string> GetFilterValuesProgramados()
        {
            return new Dictionary<string, string>
                          {
                              {"EMPRESA", ddlLocacion.SelectedValue},
                              {"LINEA", ddlPlanta.SelectedValue},
                              {"CLIENTE", cbCliente.SelectedValue != "" ? cbCliente.SelectedValue : "0"},
                              {"PUNTO", cbPuntoEntrega.SelectedValue != "" ? cbPuntoEntrega.SelectedValue : "0"},
                              {"CICLO_0", chkIncluirCiclo0.Checked.ToString()},
                              {"OBRA_0", chkIncluirObra0.Checked.ToString()},
                              {"SIN_VEHICULO", chkIncluirSinVehiculo.Checked.ToString()}
                          };
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, DuracionEstadosVo dataItem)
        {
            e.Row.Attributes.Add("onclick", string.Format("window.open('{0}?id={1}');", ResolveUrl("~/CicloLogistico/AltaTicket.aspx"), dataItem.Id));

            if (chkIncluirEstados.Checked)
            {
                for (var i = 0; i < Estados.Count - 1; i++)
                {
                    var estado = Estados[i] as Estado;
                    var key = estado.Id.ToString("#0");
                    if (!dataItem.DynamicData.ContainsKey(key)) continue;
                    GridUtils.GetCell(e.Row, DuracionEstadosVo.IndexDynamicColumns + i).Text = dataItem.DynamicData[key].ToString();
                }
            }

            if (!chkIncluirComparacionCarga.Checked)
            {
                GridUtils.GetColumn(DuracionEstadosVo.IndexCargaReal).Visible = false;
                GridUtils.GetColumn(DuracionEstadosVo.IndexDiferenciaCarga).Visible = false;
            }

            switch (dataItem.Estado)
            {
                case 1: e.Row.ForeColor = Color.DarkGreen; break;
                case 9: e.Row.ForeColor = Color.Gray; break;
            }
        }

        protected void CbCliente_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            cbPuntoEntrega.Enabled = cbCliente.Selected != -1;
        }
    }
}
