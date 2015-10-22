#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.Combustible;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

#endregion

namespace Logictracker.Reportes.CombustibleEnPozos
{
    public partial class Reportes_CombustibleEnPozos_VolumenTanque : SecuredGridReportPage<NivelesTanqueVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "COMB_VOLUMEN_TANQUE"; } }

        protected override string GetRefference() { return "VOL_TANQUE"; }

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

        protected override List<NivelesTanqueVo> GetResults()
        {
            Desde = dpDesde.SelectedDate.GetValueOrDefault();
            Hasta = dpHasta.SelectedDate.GetValueOrDefault();

            return (from o in ReportFactory.NivelesTanqueDAO.GetVolumesByDate(ddlTanque.Selected, Desde, Hasta, Convert.ToInt32(npInterval.Value))
                    select new NivelesTanqueVo(o)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, NivelesTanqueVo dataItem)
        {
            var lastVolume = (from v in ReportObjectsList where v.Fecha < dataItem.Fecha orderby v.Fecha descending select v).FirstOrDefault();

            if (lastVolume != null && lastVolume.Volumen < dataItem.Volumen) e.Row.BackColor = Color.DarkSeaGreen;
        }

        #endregion

        #region CSV Methods

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {
                               CultureManager.GetEntity("PARENTI01"),
                               ddlLocation.Selected > 0
                                   ? DAOFactory.EmpresaDAO.FindById(ddlLocation.Selected).RazonSocial
                                   : "Todos"
                               },
                           {
                               CultureManager.GetEntity("PARENTI02"),
                               ddlPlanta.Selected > 0
                                   ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected).Descripcion
                                   : "Todos"
                               },
                           {
                               CultureManager.GetEntity("PARENTI19"),
                               ddlEquipo.Selected > 0
                                   ? DAOFactory.EquipoDAO.FindById(ddlEquipo.Selected).Descripcion
                                   : String.Empty
                               },
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }

        #endregion
    }
}