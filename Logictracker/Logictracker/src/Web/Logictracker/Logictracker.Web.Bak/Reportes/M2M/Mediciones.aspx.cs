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
    public partial class ReporteMediciones : SecuredGridReportPage<MedicionVo>
    {
        [Serializable]
        private class SearchData
        {
            public List<int> EntidadesId { get; set; }
            public List<int> SubentidadesId { get; set; }
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

            data = new SearchData
                       {
                           EntidadesId = cbEntidad.SelectedValues,
                           SubentidadesId = cbSubEntidad.SelectedValues,
                           InitialDate = desde,
                           FinalDate = hasta
                       };
            SaveSearchData(data);
            return data;
        }

        protected override string VariableName { get { return "M2M_REPORTE_MEDICIONES"; } }
        protected override string GetRefference() { return "M2M_REPORTE_MEDICIONES"; }
        protected override bool ExcelButton { get { return true; } }
        protected override Empresa GetEmpresa()
        {
            return (ddlLocacion.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected) : null;
        }
        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }
        private IEnumerable<int> Entidades
        {
            get
            {
                if (ViewState["Entidades"] == null)
                {
                    ViewState["Entidades"] = Session["Entidades"];

                    Session["Entidades"] = null;
                }

                return ViewState["Entidades"] != null ? ViewState["Entidades"] as List<int> : new List<int>();
            }
        }
        private IEnumerable<int> SubEntidades
        {
            get
            {
                if (ViewState["SubEntidades"] == null)
                {
                    ViewState["SubEntidades"] = Session["SubEntidades"];

                    Session["SubEntidades"] = null;
                }

                return ViewState["SubEntidades"] != null ? ViewState["SubEntidades"] as List<int> : new List<int>();
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

        protected override List<MedicionVo> GetResults()
        {
            var data = LoadSearchData();
            var inicio = DateTime.UtcNow;

            try
            {
                if (QueryExtensions.IncludesAll(data.EntidadesId))
                    data.EntidadesId = DAOFactory.EntidadDAO.GetList(new[] {ddlLocacion.Selected},
                                                                     new[] {ddlPlanta.Selected},
                                                                     new[] {-1},
                                                                     new[] {ddlTipoEntidad.Selected})
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

                var results = DAOFactory.MedicionDAO.GetList(ddlLocacion.SelectedValues,
                                                             ddlPlanta.SelectedValues,
                                                             new[] {-1},
                                                             new[] {-1},
                                                             new[] {-1},
                                                             data.EntidadesId,
                                                             data.SubentidadesId,
                                                             new[] {-1},
                                                             data.InitialDate,
                                                             data.FinalDate)
                                                    .Select(o => new MedicionVo(o))
                                                    .ToList();
                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

				STrace.Trace("Reporte de Mediciones", String.Format("Duración de la consulta: {0} segundos", duracion));

				return results;
            }
            catch (Exception e)
            {
				STrace.Exception("Reporte de Mediciones", e, String.Format("Reporte: Reporte de Mediciones. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));

                throw;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ctrlFiltros.Empresa != ddlLocacion.Selected || ctrlFiltros.Linea != ddlPlanta.Selected
                || ListasDistintas(ctrlFiltros.TipoEntidad, ddlTipoEntidad.SelectedValues))
            {

                ctrlFiltros.Empresa = ddlLocacion.Selected > 0 ? ddlLocacion.Selected : -1;
                ctrlFiltros.Linea = ddlPlanta.Selected > 0 ? ddlPlanta.Selected : -1;
                ctrlFiltros.TipoEntidad = ddlTipoEntidad.SelectedValues ?? new List<int> {-1};

                ctrlFiltros.Filtros = null;
                ctrlFiltros.Grilla.DataBind();
            }


            if (IsPostBack) return;
            dpDesde.SetDate();
            dpHasta.SetDate();
            if (!Entidades.Any()) return;

            SetInitialFilterValues();

            Bind();
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

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MedicionVo dataItem)
        {
            if (chkVerColumnasFiltradas.Checked)
            {
                for (var i = 0; i < ctrlFiltros.Filtros.Count; i++)
                {
                    var filtro = ctrlFiltros.Filtros[i];
                    var pto = DAOFactory.EntidadDAO.FindByDispositivo(new[] {-1}, new[] {-1}, new[] {dataItem.IdDispositivo});
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
                                valor = detalle.ValorNum.ToString();
                                break;
                            case 3:
                                valor = detalle.ValorDt.ToString();
                                break;
                        }
                    }
                    
                    GridUtils.GetCell(e.Row, MedicionVo.IndexDynamicColumns + i).Text = valor;
                }
            }
        }

        #endregion

        #region Private Methods

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           { CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text },
                           { CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text },
                           { CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.ToString() }, 
                           { CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.ToString() }
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

                sEntidades.Append((string) entidad.ToString());
            }
            foreach (var subentidad in cbSubEntidad.SelectedValues)
            {
                if (!sSubentidades.ToString().Equals(""))
                    sSubentidades.Append(",");

                sSubentidades.Append((string) subentidad.ToString());
            }

            dic.Add("ENTIDADES", sEntidades.ToString());
            dic.Add("SUBENTIDADES", sSubentidades.ToString());

            return dic;
        }

        private void SetInitialFilterValues()
        {
            ddlTipoEntidad.SetSelectedValue(-1);

            cbEntidad.SetSelectedIndexes(Entidades);
            cbSubEntidad.SetSelectedIndexes(SubEntidades);

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