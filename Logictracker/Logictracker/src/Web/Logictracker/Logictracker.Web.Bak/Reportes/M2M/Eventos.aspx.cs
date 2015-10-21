using System.Globalization;
using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.ValueObjects.Entidades;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.M2M
{
    public partial class ReporteEventos : SecuredGridReportPage<LogEventoVo>
    {
        [Serializable]
        private class SearchData
        {
            public List<int> EntidadesId { get; set; }
            public List<int> SubentidadesId { get; set; }
            public List<string> MensajesId { get; set; }
            public DateTime InitialDate { get; set; }
            public DateTime FinalDate{ get; set; }
        }

        private void SaveSearchData(SearchData data)
        {
            ViewState["SearchData"] = data;
        }
        private SearchData LoadSearchData() 
        {
            var data = ViewState["SearchData"] as SearchData;
            if(data != null) return data;

            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            if (cbEntidad.SelectedValues.Contains(0)) cbEntidad.ToogleItems();

            data = new SearchData
                       {
                           EntidadesId = cbEntidad.SelectedValues,
                           SubentidadesId = cbSubEntidad.SelectedValues,
                           MensajesId = cbMensajes.SelectedStringValues,
                           InitialDate = desde,
                           FinalDate = hasta
                       };
            SaveSearchData(data);
            return data;
        }

        protected override string VariableName { get { return "M2M_REPORTE_EVENTOS"; } }
        protected override string GetRefference() { return "M2M_REPORTE_EVENTOS"; }
        protected override Empresa GetEmpresa()
        {
            return (ddlLocacion.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected) : null;
        }
        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }
        private int Location
        {
            get
            {
                if (ViewState["EventsLocation"] == null)
                {
                    ViewState["EventsLocation"] = Session["EventsLocation"];

                    Session["EventsLocation"] = null;
                }

                return ViewState["EventsLocation"] != null ? Convert.ToInt32(ViewState["EventsLocation"]) : 0;
            }
        }
        private int Company
        {
            get
            {
                if (ViewState["EventsCompany"] == null)
                {
                    ViewState["EventsCompany"] = Session["EventsCompany"];

                    Session["EventsCompany"] = null;
                }

                return ViewState["EventsCompany"] != null ? Convert.ToInt32(ViewState["EventsCompany"]) : 0;
            }
        }
        private int TipoEntidad
        {
            get
            {
                if (ViewState["TipoEntidad"] == null)
                {
                    ViewState["TipoEntidad"] = Session["TipoEntidad"];

                    Session["TipoEntidad"] = null;
                }

                return ViewState["TipoEntidad"] != null ? Convert.ToInt32(ViewState["TipoEntidad"]) : 0;
            }
        }
        private int Entidad
        {
            get
            {
                if (ViewState["Entidad"] == null)
                {
                    ViewState["Entidad"] = Session["Entidad"];

                    Session["Entidad"] = null;
                }

                return ViewState["Entidad"] != null ? Convert.ToInt32(ViewState["Entidad"]) : 0;
            }
        }
        private DateTime InitialDate
        {
            get
            {
                if (ViewState["EventsFrom"] == null)
                {
                    ViewState["EventsFrom"] = Session["EventsFrom"];

                    Session["EventsFrom"] = null;
                }

                return ViewState["EventsFrom"] != null ? Convert.ToDateTime(ViewState["EventsFrom"]) : DateTime.MinValue;
            }
        }
        private DateTime FinalDate
        {
            get
            {
                if (ViewState["EventsTo"] == null)
                {
                    ViewState["EventsTo"] = Session["EventsTo"];

                    Session["EventsTo"] = null;
                }

                return ViewState["EventsTo"] != null ? Convert.ToDateTime(ViewState["EventsTo"]) : DateTime.MinValue;
            }
        }

        #region Protected Methods

        protected override void OnBinding()
        {
            SaveSearchData(null);
        }

        protected override List<LogEventoVo> GetResults()
		{
			var data = LoadSearchData();
            var inicio = DateTime.UtcNow;

            try
            {
                if (QueryExtensions.IncludesAll(data.EntidadesId))
                    data.EntidadesId = DAOFactory.EntidadDAO.GetList(new[] { ddlLocacion.Selected },
                                                                     new[] { ddlPlanta.Selected },
                                                                     new[] { -1 },
                                                                     new[] { ddlTipoEntidad.Selected })
                                                            .Select(e => e.Id)
                                                            .ToList();

                if (ctrlFiltros.Filtros.Count > 0)
                {    
                    data.EntidadesId = DAOFactory.EntidadDAO.GetEntidadesByFiltros(data.EntidadesId, ctrlFiltros.Filtros)
                                                            .Cast<EntidadPadre>()
                                                            .Select(e => e.Id)
                                                            .ToList();

                    if (data.EntidadesId.Count == 0)
                        data.EntidadesId.Add(-100);
                }

                var results = DAOFactory.LogEventoDAO.GetList(ddlLocacion.SelectedValues,
                                                              ddlPlanta.SelectedValues,
                                                              new[] {-1},
                                                              new[] {-1},
                                                              new[] {-1},
                                                              data.EntidadesId,
                                                              data.SubentidadesId,
                                                              data.MensajesId,
                                                              data.InitialDate,
                                                              data.FinalDate)
                                                     .Select(e => new LogEventoVo(e))
                                                     .ToList();
				var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

				STrace.Trace("Reporte de Eventos M2M", String.Format("Duración de la consulta: {0} segundos", duracion));

				return results;
			}
			catch (Exception e)
			{
				STrace.Exception("Reporte de Eventos M2M", e, String.Format("Reporte: Reporte de Eventos M2M. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));

                throw;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }
            
            if (IsPostBack) return;

            if (Entidad.Equals(0)) return;

            SetInitialFilterValues();

            Bind();

            if (ctrlFiltros.Empresa != ddlLocacion.Selected || ctrlFiltros.Linea != ddlPlanta.Selected
                || ListasDistintas(ctrlFiltros.TipoEntidad, ddlTipoEntidad.SelectedValues))
            {
                ctrlFiltros.Empresa = ddlLocacion.Selected > 0 ? ddlLocacion.Selected : -1;
                ctrlFiltros.Linea = ddlPlanta.Selected > 0 ? ddlPlanta.Selected : -1;
                ctrlFiltros.TipoEntidad = ddlTipoEntidad.SelectedValues ?? new List<int> {-1};

                ctrlFiltros.Filtros = null;
                ctrlFiltros.Grilla.DataBind();
            }
        }

        protected override void GenerateCustomColumns()
        {
            if (chkVerColumnasFiltradas.Checked)
            {
                foreach (var filtro in ctrlFiltros.Filtros)
                {
                    var detalle = DAOFactory.DetalleDAO.FindById(filtro.IdDetalle);
                    C1Field field = new C1TemplateField
                                        {
                                            AllowGroup = false,
                                            AllowMove = false,
                                            AllowSizing = false,
                                            HeaderText = detalle.Nombre,
                                            Visible = true
                                        };

                    Grid.Columns.Add(field);
                }
            }
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, LogEventoVo dataItem)
        {
            if (chkVerColumnasFiltradas.Checked)
            {
                for (var i = 0; i < ctrlFiltros.Filtros.Count; i++)
                {
                    var filtro = ctrlFiltros.Filtros[i];
                    var pto = DAOFactory.EntidadDAO.FindByDispositivo(new[] { -1 },
                                                                      new[] { -1 },
                                                                      new[] { dataItem.IdDispositivo });
                    var detalle = pto != null ? pto.GetDetalle(filtro.IdDetalle) : null;
                    var valor = string.Empty;
                    if (detalle != null)
                    {
                        switch (detalle.Detalle.Tipo)
                        {
                            case 1:
                                valor = detalle.ValorStr;
                                break;
                            case 2:
                                valor = detalle.ValorNum.ToString(CultureInfo.InvariantCulture);
                                break;
                            case 3:
                                valor = detalle.ValorDt.ToString();
                                break;
                        }
                    }

                    GridUtils.GetCell(e.Row, LogEventoVo.IndexDynamicColumns + i).Text = valor;
                }
            }
        }

        protected void DdlLocacion_InitialBinding(object sender, EventArgs e) { if (!IsPostBack && Location > 0) ddlLocacion.EditValue = Location; }

        protected void DdlPlanta_InitialBinding(object sender, EventArgs e) { if (!IsPostBack && Company > 0) ddlPlanta.EditValue = Company; }

        protected void DdlTipoEntidad_InitialBinding(object sender, EventArgs e) { if (!IsPostBack && TipoEntidad > 0) ddlTipoEntidad.EditValue = TipoEntidad; }

        #endregion

        #region Private Methods

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           { CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text },
                           { CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text },
                           { CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.ToString()}, {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.ToString() }
                       };
        }

        protected override Dictionary<string, string> GetFilterValuesProgramados()
        {
            var dic = new Dictionary<string, string>();
            var sEntidades = new StringBuilder();
            var sSubentidades = new StringBuilder();

            foreach (var entidad in cbEntidad.SelectedValues)
            {
                if (!sEntidades.ToString().Equals(""))
                    sEntidades.Append(",");

                sEntidades.Append((string) entidad.ToString(CultureInfo.InvariantCulture));
            }
            foreach (var subentidad in cbSubEntidad.SelectedValues)
            {
                if (!sSubentidades.ToString().Equals(""))
                    sSubentidades.Append(",");

                sSubentidades.Append((string) subentidad.ToString(CultureInfo.InvariantCulture));
            }

            dic.Add("ENTIDADES", sEntidades.ToString());
            dic.Add("SUBENTIDADES", sSubentidades.ToString());

            return dic;
        }

        private void SetInitialFilterValues()
        {
            ddlTipoEntidad.SetSelectedValue(-1);

            cbEntidad.SetSelectedIndexes(new List<int> { Entidad });
            
            cbSubEntidad.ToogleItems();

            dpDesde.SelectedDate = InitialDate;
            dpHasta.SelectedDate = FinalDate;
        }

        private static bool ListasDistintas(IList<int> a, IList<int> b)
        {
            return a.Count != b.Count || a.Where((t, i) => t != b[i]).Any();
        }
        
        #endregion
    }
}