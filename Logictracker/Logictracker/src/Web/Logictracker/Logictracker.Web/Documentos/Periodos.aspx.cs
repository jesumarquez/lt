using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Configuration;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.Documentos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Documentos
{
    public partial class Documentos_Periodos : SecuredListPage<PeriodoVo>
    {
        protected override string VariableName { get { return "COST_PERIODOS"; } }
        protected override string RedirectUrl { get { return "PeriodoAlta.aspx"; } }
        protected override string GetRefference() { return "PERIODO"; }
        protected override bool ExcelButton { get { return true; } }

        #region Protected Methods

        protected override List<PeriodoVo> GetListData()
        {
            return DAOFactory.PeriodoDAO.GetByYear(cbEmpresa.Selected, Anio).OfType<Periodo>().Select(p => new PeriodoVo(p)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, PeriodoVo dataItem)
        {
            var btCambiarEstado = e.Row.FindControl("btCambiarEstado") as LinkButton;

            if (btCambiarEstado == null) return;

            btCambiarEstado.CommandArgument = dataItem.Id.ToString();
            btCambiarEstado.Visible = dataItem.Action;
        }

        protected override void CreateRowTemplate(C1GridViewRow row)
        {
            var cell = GridUtils.GetCell(row, PeriodoVo.IndexAction);
            var button = cell.FindControl("btCambiarEstado") as LinkButton;
            if (button == null) cell.Controls.Add(new LinkButton { ID = "btCambiarEstado", OnClientClick = "return confirm('Esta seguro?');", CommandName = "CambiarEstado", Text = @"Cerrar" });
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Anio = DateTime.Now.Year;
                panelEmpresa.Visible = !Config.PeriodosGlobales;
            }
        }
        protected override void OnRowCommand(C1GridView grid, C1GridViewCommandEventArgs e)
        {
            base.OnRowCommand(grid, e);

            if (e.CommandName != "CambiarEstado") return;

            var id = Convert.ToInt32(e.CommandArgument);

            var periodo = DAOFactory.PeriodoDAO.FindById(id);
            if (periodo.Estado > 0) return;
            periodo.Estado = 1;
            DAOFactory.PeriodoDAO.SaveOrUpdate(periodo);
            Bind();
        }

        protected void GenerarPeriodos()
        {
            var periodos = DAOFactory.PeriodoDAO.GetByYear(cbEmpresa.Selected, Anio);
            if (periodos.Count > 1)
            {
                ShowError(new Exception("Ya se generaron los periodos para el año " + Anio));
                return;
            }
            var empresa = Config.PeriodosGlobales ? null : DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            var inicio = Config.PeriodosGlobales ? Config.PeriodosInicio : empresa.PeriodosInicio;

            int day = inicio;//26;
            var month = day > 15 ? 12 : 1;
            var year = day > 15 ? Anio - 1 : Anio;
            var maxDay = DateTime.DaysInMonth(year, month);
            var dateStart = new DateTime(year, month, Math.Min(day, maxDay));

            for (var i = 1; i <= 12; i++)
            {
                var monthLater = dateStart.AddMonths(1);
                var maxDayEnd = DateTime.DaysInMonth(monthLater.Year, monthLater.Month);
                var nextDate = new DateTime(monthLater.Year, monthLater.Month, Math.Min(day, maxDayEnd));
                var dateEnd = nextDate.AddDays(-1);

                var periodo = new Periodo
                                  {
                                      Descripcion = GetDescripcion(dateStart, dateEnd),
                                      FechaDesde = dateStart,
                                      FechaHasta = dateEnd,
                                      Estado = 0,
                                      Empresa = empresa
                                  };
                DAOFactory.PeriodoDAO.SaveOrUpdate(periodo);
                dateStart = nextDate;
            }
            Bind();
            ShowInfo("Se generaron exitosamente los periodos del año " + Anio);
        }
        protected void btGenerar_Click(object sender, EventArgs e)
        {
            GenerarPeriodos();
        }
        protected void ChangeAnio(object sender, CommandEventArgs e)
        {
            var year = Anio;
            if (e.CommandName == "Down") year--;
            else if (e.CommandName == "Up") year++;

            Anio = year;
            Bind();
        }

        #endregion

        #region Private Methods

        private static string GetDescripcion(DateTime start, DateTime end)
        {
            if (start.Day > 20) return end.ToString("MMMM yyyy");
            if( start.Day < 10) return start.ToString("MMMM yyyy");
            return (start.Month < 12
                        ? start.ToString("MMMM")
                        : start.ToString("MMMM yyyy"))
                   + " - " + 
                   (start.Month < 12
                        ? end.ToString("MMMM")
                        : end.ToString("MMMM yyyy"))
                   + " " +
                   (start.Month < 12
                        ? start.ToString("yyyy")
                        : string.Empty);
        }

        private int Anio
        {
            get { return Convert.ToInt32(lblAnio.Text); }
            set
            {
                lblAnio.Text = value.ToString();
                btGenerar.Text = @"Generar " + value;
            }
        }

        #endregion
    }
}
