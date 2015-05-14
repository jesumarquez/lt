#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ReportObjects.ControlDeCombustible;
using Logictracker.Types.ValueObjects.Combustible;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Reportes.CombustibleEnPozos
{
    public partial class ReportesCombustibleEnPozosEventosCombustible : SecuredGridReportPage<CombustibleEventVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "COMB_EVENTOS"; } }

        protected override string GetRefference() { return "EVENTOS_COMB,REP_EVENT_COMB"; }

        protected override bool ScheduleButton { get { return true; } }

        protected override Empresa GetEmpresa()
        {
            if (ddlLocation.Selected > 0)
                return DAOFactory.EmpresaDAO.FindById(ddlLocation.Selected);

            return null;
        }

        protected override Linea GetLinea()
        {
            if (ddlPlanta.Selected > 0)
                return DAOFactory.LineaDAO.FindById(ddlPlanta.Selected);

            return null;
        }

        /// <summary>
        /// Beggining Report Date
        /// </summary>
        private DateTime Desde
        {
            get { return ViewState["Desde"] != null ? Convert.ToDateTime(ViewState["Desde"]) : DateTime.MinValue; }
            set { ViewState["Desde"] = value; }
        }

        /// <summary>
        /// End Report Date
        /// </summary>
        private DateTime Hasta
        {
            get { return ViewState["Hasta"] != null ? Convert.ToDateTime(ViewState["Hasta"]) : DateTime.MinValue; }
            set { ViewState["Hasta"] = value; }
        }

        #endregion

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        /// <summary>
        /// Gets the results to fill the grid
        /// </summary>
        /// <returns></returns>
        protected override List<CombustibleEventVo> GetResults()
        {
            Desde = dpDesde.SelectedDate.GetValueOrDefault();
            Hasta = dpHasta.SelectedDate.GetValueOrDefault();

            var results = ReportFactory.CombustibleEventsDAO.FindByMotoresMensajesAndFecha(
                lbMotores.SelectedValues, lbTanques.SelectedValues, GetSelectedMensajes(), Desde, Hasta);

            return (from CombustibleEvent m in results select new CombustibleEventVo(m)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, CombustibleEventVo dataItem)
        {
            if (e.Row.RowIndex < 0 ) return;

            var idAccion = Convert.ToInt32(Grid.DataKeys[e.Row.RowIndex].Value); 
            var accion =  idAccion > 0 ? DAOFactory.AccionDAO.FindById(idAccion) : null;

            var col = accion != null ? Color.FromArgb(100, accion.Red, accion.Green, accion.Blue) : Color.White;

            e.Row.BackColor = col;
            e.Row.ForeColor = col.GetBrightness() > 0.45 ? Color.Black : Color.White;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the ids of all the motores selected and puts them into a coma-sepparated string.
        /// </summary>
        /// <returns></returns>
        private string GetSelectedMotores(bool showdescription)
        {
            var ids = string.Empty;

            foreach (var index in lbMotores.GetSelectedIndices())
                ids = string.Concat(ids, string.Format("{0},", showdescription ? lbMotores.Items[index].Text : lbMotores.Items[index].Value));

            return ids.TrimEnd(',');
        }

        /// <summary>
        /// Gets the ids of all the tanques selected and puts them into a coma-sepparated string.
        /// </summary>
        /// <returns></returns>
        private string GetSelectedTanques(bool showDescription)
        {
            var ids = string.Empty;

            foreach (var index in lbTanques.GetSelectedIndices())
                ids = string.Concat(ids, string.Format("{0},", showDescription ? lbTanques.Items[index].Text : lbTanques.Items[index].Value));

            return ids.TrimEnd(',');
        }

        /// <summary>
        /// Gets the ids of all the mensajes selected and puts them into a coma-sepparated string.
        /// </summary>
        /// <returns></returns>
        private List<string> GetSelectedMensajes()
        {
            if (lbMensajes.GetSelectedIndices().Length == 0) lbMensajes.ToogleItems();

            return (from o in lbMensajes.SelectedValues select o.ToString()).ToList();
        }

        #endregion

        #region CSV Methods

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                                 {
                                     { CultureManager.GetEntity("PARENTI01"), ddlLocation.SelectedItem.Text },
                                     { CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text },
                                     { CultureManager.GetEntity("PARENTI19"), ddlEquipo.SelectedItem.Text},
                                     {CultureManager.GetEntity("PARENTI36"), GetSelectedTanques(true)},
                                     {CultureManager.GetEntity("PARENTI39"), GetSelectedMotores(true)},
                                     {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                                     {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                                 };
        }

        protected override Dictionary<string, string> GetFilterValuesProgramados()
        {
            var sMotores = GetSelectedMotores(false);
            var sTanques = GetSelectedTanques(false);
            var mensajes = GetSelectedMensajes();
            var sMensajes = new StringBuilder();
            var dic = new Dictionary<string, string>();
            
            foreach (var mensaje in mensajes)
            {
                if (!sMensajes.ToString().Equals(""))
                    sMensajes.Append(",");

                sMensajes.Append(mensaje.ToString());
            }

            dic.Add("MOTORES", sMotores);
            dic.Add("TANQUES", sTanques);
            dic.Add("MENSAJES", sMensajes.ToString());

            return dic;
        }

        #endregion
    }
}