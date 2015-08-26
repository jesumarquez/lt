using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.ValueObjects.Soporte;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.Estadistica.Soporte
{
    public partial class ReporteTickets : SecuredListPage<ReporteTicketVo>
    {
        protected override string VariableName { get { return "EST_TICKET_SOPORTE"; } }
        protected override string GetRefference() { return "EST_TICKET_SOPORTE"; }
        protected override string RedirectUrl { get { return "../../../Soporte/TicketSoporteAlta.aspx"; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }
            else
            {
                Bind();
            }
        }

        protected override List<ReporteTicketVo> GetListData()
        {
            var inicio = DateTime.UtcNow;
            var desde = dpDesde.SelectedDate.HasValue
                            ? SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.Value)
                            : inicio;
            var hasta = dpHasta.SelectedDate.HasValue
                            ? SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.Value)
                            : inicio;

            try
            {
                var results = DAOFactory.SupportTicketDAO.GetList(cbEmpresa.SelectedValues,
                                                                  cbCategoria.SelectedValues,
                                                                  cbSubcategoria.SelectedValues,
                                                                  cbNivel.SelectedValues,
                                                                  desde,
                                                                  hasta);
                
                var estados = cbEstados.GetSelectedIndices();
                if (Enumerable.Any<int>(estados) && !Enumerable.Contains(estados, -1))
                    results = results.Where(ts => Enumerable.Contains<int>(estados, ts.CurrentState)).ToList();

                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

                STrace.Trace("Reporte de Soporte", String.Format("Duración de la consulta: {0} segundos", duracion));

				return results.Select(st => new ReporteTicketVo(st)).ToList();
            }
            catch (Exception e)
            {
                STrace.Exception("Reporte de Soporte", e, String.Format("Reporte: Reporte de Soporte. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));

                throw;
            }
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ReporteTicketVo dataItem)
        {
            e.Row.Attributes.Remove("onclick");

            var celda = GridUtils.GetCell(e.Row, ReporteTicketVo.IndexId);
            var id = celda.Text;

            celda.Controls.Clear();
            var lnk = new LinkButton
                          {
                              ID = id,
                              ForeColor = Color.Black,
                              Text = id, 
                              OnClientClick = "javascript:window.open('" + RedirectUrl + "?id=" + id + "', 'ticket'); return false;"
                              
                          };
            lnk.Font.Bold = true;
            celda.Controls.Add(lnk);
        }

        protected override void CreateHeaderTemplate(C1GridViewRow row)
        {
            if (row == null) return;

            //var chk = row.FindControl("chkSelectAll") as CheckBox;
            //if (chk == null) GridUtils.GetCell(row, ReporteTicketVo.IndexCheck).Controls.Add(chk = new CheckBox { ID = "chkSelectAll", AutoPostBack = true });
            //chk.CheckedChanged += ChkSelectAllCheckedChanged;
        }

        protected override void CreateRowTemplate(C1GridViewRow row)
        {
            var check = GridUtils.GetCell(row, ReporteTicketVo.IndexCheck).FindControl("chkSelected") as CheckBox;
            if (check == null) 
                GridUtils.GetCell(row, ReporteTicketVo.IndexCheck).Controls.Add(new CheckBox { ID = "chkSelected" });
        }

        protected void ChkSelectAllCheckedChanged(object sender, EventArgs e)
        {
            var enabled = Grid.Rows.OfType<C1GridViewRow>().Select(r => GridUtils.GetCell(r, ReporteTicketVo.IndexCheck).FindControl("chkSelected")).OfType<CheckBox>().Where(chk => chk.Enabled);
            var checkAll = !enabled.Any(c => c.Checked);

            foreach (var box in enabled) box.Checked = checkAll;

            if ((sender as CheckBox) != null)
                (sender as CheckBox).Checked = checkAll;
        }

    }
}