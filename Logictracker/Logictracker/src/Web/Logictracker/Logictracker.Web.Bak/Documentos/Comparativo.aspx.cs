using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos.Partes;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Documentos.Partes;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Documentos
{
    public partial class Comparativo : ApplicationSecuredPage
    {
        //    private const string HeaderHorasTemplate = @"<span style=""color: #619DFF;"">{0}</span>-<span style=""color: #1DE602; font-weight: bold;"">{1}</span>(<span style=""color: #E60202;"">{2}</span>) ";
        //    private const string HeaderImporteTemplate = @"<span style=""color: #619DFF;"">${0:0.00}</span>-<span style=""color: #1DE602; font-weight: bold;"">${1:0.00}</span>(<span style=""color: #E60202;"">${2:0.00}</span>) ";
        //    private const string HeaderKmTemplate = @"<span style=""color: #619DFF;"">{0}km</span>-<span style=""color: #1DE602; font-weight: bold;"">{1}km</span>(<span style=""color: #E60202;"">{2}km</span>) ";

        private const string HeaderHorasTemplate = @"{0}-{1}({2})";
        private const string HeaderImporteTemplate = @"${0:0.00}-${1:0.00}(${2:0.00}) ";
        private const string HeaderKmTemplate = @"{0}km-{1}km({2}km) ";


        private const string HorasTemplate = @"<span style=""color: #0000CC;"">{0}</span>-<span style=""color: #006600; font-weight: bold;"">{1}</span>(<span style=""color: #990000;"">{2}</span>) ";
        private const string ImporteTemplate = @"<span style=""color: #0000CC;"">${0:0.00}</span>-<span style=""color: #006600; font-weight: bold;"">${1:0.00}</span>(<span style=""color: #990000;"">${2:0.00}</span>) ";
        private const string KmTemplate = @"<span style=""color: #0000CC;"">{0}km</span>-<span style=""color: #006600; font-weight: bold;"">{1}km</span>(<span style=""color: #990000;"">{2}km</span>) ";
        //private const string TotalesTemplate = @"{0} <br/><span style=""font-weight: normal; font-size: 9px;"">Horas: {1} | Km: {2} | Importe:  {3}</span>";
        private const string TotalesTemplate = @"{0} | Horas: {1} | Km: {2} | Importe:  {3}";

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(typeof(string), "setModified",
                                                    @"function setModified(el, ask)
            {
                el.className = 'parte_txt_mod';
                if(ask) $get('" + hidChanged.ClientID + @"').value = 'true';
            }", true);

            btAceptarParte.Attributes.Add("onclick", "if($get('" + hidChanged.ClientID + "').value == 'true') { var re = confirm('Los datos del parte cambiaron. ¿Desea recalcular el kilometraje?'); $get('" + hidChanged.ClientID + "').value = re?'true':'false'; return re;} else return true;");
            btCancelarParte.Attributes.Add("onclick", "return confirm('Esto hara que se pierdan los cambio a este parte.\n ¿Desea continuar?');");
            btSiguienteParte.Attributes.Add("onclick", "return confirm('Esto hara que se pierdan los cambio a este parte.\n ¿Desea continuar?');");

            if (!IsPostBack)
            {
                BindPeriodos();
            }
        }
        #endregion

        #region DataBindings
        protected void BindPeriodos()
        {
            var transp = DAOFactory.TransportistaDAO.FindById(cbAseguradora.Selected);

            var list = DAOFactory.PeriodoDAO.GetByEstado(transp.Empresa != null ? transp.Empresa.Id:-1, Periodo.Abierto, Periodo.Cerrado);

            foreach (Periodo periodo in list)
                cbPeriodo.Items.Add(new ListItem(periodo.Descripcion + (periodo.Estado == Periodo.Abierto ? " (Abierto)" : " (Cerrado)"), periodo.Id.ToString()));
        }
        #endregion

        #region Search
        protected void Search(SearchData search)
        {
            var movStrategy = new ParteComparativoMovilReportStrategy(DAOFactory);
            var data = movStrategy.GetData(search.aseguradora, search.inicio.ToDataBaseDateTime(), search.fin.ToDataBaseDateTime());
            panelContent.Visible = true;// (data.Count > 0);
            SetViewIndex(0);
            IdPartes = null;
            IndexParte = 0;

            //if (data.Count == 0) return;
            search.Save(ViewState);

            grid.DataSource = data;
            grid.DataBind();
        }
        #endregion

        #region LoadParte
        protected bool LoadNextParte()
        {
            return LoadParte(IndexParte + 1);
        }

        protected bool LoadParte(int index)
        {
            var ids = IdPartes;
            if (index >= ids.Count) return false;

            IndexParte = index;

            lblCurrent.Text = (index + 1).ToString();
            LoadParte(DAOFactory.DocumentoDAO.FindById(ids[index]));
            btSiguienteParte.Visible = index < ids.Count - 1;
            hidChanged.Value = "false";
            return true;
        }
        protected void LoadParte(Documento parte)
        {
            DatosParte1.SetData(parte, DAOFactory);
        }
        #endregion

        #region GetPartes
        protected IList GetAllPartes()
        {
            var search = SearchData.Load(ViewState);
            var usuario = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            return DAOFactory.DocumentoDAO.FindList(search.aseguradora, -1, -1, search.inicio.ToDataBaseDateTime(), search.fin.ToDataBaseDateTime(), 0, -1, usuario);
        }

        protected IList GetPartes(List<int> selectedEquipos, List<int> selectedTipos)
        {
            //var search = SearchData.Load(ViewState);
            var allPartes = GetAllPartes();
            var result = new List<Documento>();
            for (int i = 0; i < selectedEquipos.Count; i++)
            {
                var equipo = selectedEquipos[i];
                var tipo = selectedTipos[i];
                if (tipo == 0)
                {
                    var match = allPartes.OfType<Documento>().Where(d =>
                            d.Valores.ContainsKey(ParteCampos.Equipo)
                            && Convert.ToInt32(d.Valores[ParteCampos.Equipo]) == equipo
                            && (!d.Valores.ContainsKey(ParteCampos.TipoServicio)
                                    || Convert.ToInt32(d.Valores[ParteCampos.TipoServicio]) == 0)
                        );
                    result.AddRange(match);
                }
                else
                {
                    var match = allPartes.OfType<Documento>().Where(d =>
                            d.Valores.ContainsKey(ParteCampos.TipoServicio)
                            && Convert.ToInt32(d.Valores[ParteCampos.TipoServicio]) == tipo
                        );
                    result.AddRange(match);
                }
            }

            return result
                .OrderBy(d => d.Valores.ContainsKey(ParteCampos.TipoServicio) ? Convert.ToInt32(d.Valores[ParteCampos.TipoServicio]) : 0)
                .ThenBy(d => d.Valores[ParteCampos.Equipo]).ToList();

            //var idEquipos = selectedEquipos.Select(t => t.ToString());
            //var idTipos = selectedTipos.Select(t => t.ToString());

            //var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            //var coches = DAOFactory.CocheDAO.GetAllByTransportista(user,new List<int>{search.aseguradora});

            //if (coches.Count == 0) return new ArrayList();

            //var idCoches = from Coche c in coches select c.Id.ToString();

            //var partes = DAOFactory.DocumentoDAO.FindParteReport(search.aseguradora, search.inicio.ToDataBaseDateTime(), search.fin.ToDataBaseDateTime(), idCoches.ToList(), 0, idEquipos.ToList(), idTipos.ToList());
            //return (from Documento d in partes orderby d.Valores[ParteCampos.Equipo] select d).ToList();
        }

        protected IList GetPartesAVerificar()
        {
            var search = SearchData.Load(ViewState);
            var documentos = DAOFactory.DocumentoDAO.FindPartesControladosOVerificados(search.aseguradora, search.inicio.ToDataBaseDateTime(), search.fin.ToDataBaseDateTime());

            var partes = from Documento d in documentos select new PartePersonal(d, DAOFactory);

            var byEquipo = new Dictionary<string, List<PartePersonal>>();

            foreach (var parte in partes)
            {
                if (!byEquipo.ContainsKey(parte.Grupo))
                    byEquipo.Add(parte.Grupo, new List<PartePersonal>());
                byEquipo[parte.Grupo].Add(parte);
            }

            var transportista = DAOFactory.TransportistaDAO.FindById(search.aseguradora);
            foreach (var equipo in byEquipo.Keys)
            {
                var oEquipo = DAOFactory.EquipoDAO.FindById(byEquipo[equipo][0].IdEquipo);
                var tarifa = DAOFactory.TarifaTransportistaDAO.GetTarifaParaCliente(transportista.Id,
                                                                                    oEquipo.Cliente.Id);

                var tarifaCorto = tarifa != null ? tarifa.TarifaTramoCorto : transportista.TarifaTramoCorto;
                var tarifaLargo = tarifa != null ? tarifa.TarifaTramoLargo : transportista.TarifaTramoLargo;

                double kms = (from p in byEquipo[equipo] select p.KmTotal).Sum();
                var kmsc = (from p in byEquipo[equipo] select p.KmTotalCalculado).Sum();
                foreach (var parte in byEquipo[equipo])
                {
                    parte.Importe = parte.KmTotal *
                                    (kms < 14000 ? tarifaCorto : tarifaLargo);
                    parte.ImporteControlado = parte.KmTotalCalculado *
                                              (kmsc < 14000 ? tarifaCorto : tarifaLargo);
                }
            }
            var sums = new Dictionary<string, string>();

            var partesOrdenados = new List<PartePersonal>();
            foreach (var list in byEquipo.Values)
            {
                partesOrdenados.AddRange(list);

                var horas = TimeSpan.Zero;
                var horasControladas = TimeSpan.Zero;
                var horasDiff = TimeSpan.Zero;
                var km = 0;
                double kmControlado = 0;
                double kmDiff = 0;
                double importe = 0;
                double importeControlado = 0;
                double importeDiff = 0;
                foreach (var parte in list)
                {
                    horas = horas.Add(parte.Horas);
                    horasControladas = horasControladas.Add(parte.HorasControladas);
                    horasDiff = horasDiff.Add(parte.DiffHoras);
                    km += parte.KmTotal;
                    kmControlado += parte.KmTotalCalculado;
                    kmDiff += parte.DiffKmTotal;
                    importe += parte.Importe;
                    importeControlado += parte.ImporteControlado;
                    importeDiff += parte.DiffImporte;
                }
                var totales = string.Format(TotalesTemplate,
                                            list[0].Grupo,
                                            string.Format(HeaderHorasTemplate, horas, horasControladas, horasDiff),
                                            string.Format(HeaderKmTemplate, km, kmControlado, kmDiff),
                                            string.Format(HeaderImporteTemplate, importe, importeControlado, importeDiff)
                    );

                sums.Add(list[0].Grupo, totales);
            }
            foreach (var ordenado in partesOrdenados)
            {
                if (ordenado.TipoServicio == ParteCampos.ListaTipoServicios[0])
                    ordenado.Equipo = sums[ordenado.Grupo];
                else ordenado.TipoServicio = sums[ordenado.Grupo];
            }


            return partesOrdenados;
        }
        #endregion

        #region Action Buttons

        #region Search
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var periodo = DAOFactory.PeriodoDAO.FindById(Convert.ToInt32(cbPeriodo.SelectedValue));
            var search = new SearchData(cbAseguradora.Selected, periodo.FechaDesde, periodo.FechaHasta.Date.AddDays(1).AddMilliseconds(-1));

            Search(search);
        }
        #endregion

        #region Lista Equipos
        protected void btControlar_Click(object sender, EventArgs e)
        {
            ControlarPartes();
        }
        #endregion

        #region Control
        protected void btCancelarParte_Click(object sender, EventArgs e)
        {
            CancelarEdicionDePartes();
        }


        protected void btSiguienteParte_Click(object sender, EventArgs e)
        {
            LoadNextParte();
        }
        protected void btAceptarParte_Click(object sender, EventArgs e)
        {
            try
            {
                var turnos = DatosParte1.Turnos;

                var parte = DAOFactory.DocumentoDAO.FindById(IdPartes[IndexParte]);

                var recalcularKm = hidChanged.Value == "true";

                if (recalcularKm)
                {
                    DatosParte1.RecalcularKm(DAOFactory);
                    hidChanged.Value = "false";
                    return;
                }

                for (var i = 0; i < turnos.Count; i++)
                {
                    var turno = turnos[i];
                    ParteSaveStrategy.SetValor(parte, ParteCampos.SalidaAlPozoControl, i,
                                               turno.AlPozoSalida.ToString(CultureInfo.InvariantCulture));
                    ParteSaveStrategy.SetValor(parte, ParteCampos.LlegadaAlPozoControl, i,
                                               turno.AlPozoLlegada.ToString(CultureInfo.InvariantCulture));
                    ParteSaveStrategy.SetValor(parte, ParteCampos.SalidaDelPozoControl, i,
                                               turno.DelPozoSalida.ToString(CultureInfo.InvariantCulture));
                    ParteSaveStrategy.SetValor(parte, ParteCampos.LlegadaDelPozoControl, i,
                                               turno.DelPozoLlegada.ToString(CultureInfo.InvariantCulture));
                    ParteSaveStrategy.SetValor(parte, ParteCampos.KilometrajeControl, i,
                                               turno.Km.ToString(CultureInfo.InvariantCulture));
                    ParteSaveStrategy.SetValor(parte, ParteCampos.KilometrajeGps, i, turno.KmGps.ToString());
                }
                ParteSaveStrategy.SetValor(parte, ParteCampos.EstadoControl, 1, "1");
                ParteSaveStrategy.SetValor(parte, ParteCampos.UsuarioControl, 1, Usuario.Id.ToString());
                DAOFactory.DocumentoDAO.SaveOrUpdate(parte);

                if (!LoadNextParte()) VerificarPartes();
            }
            catch (Exception ex)
            {
                infoLabel1.Text = ex.Message;
            }
        }
        #endregion

        #endregion

        #region gridResumen Events

        protected void gridResumen_ItemCommand(object sender, C1GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Modificar")
            {
                var id = Convert.ToInt32(gridResumen.DataKeys[e.Row.RowIndex].Value);

                var doc = DAOFactory.DocumentoDAO.FindById(id);
                ParteSaveStrategy.SetValor(doc, ParteCampos.EstadoControl, 1, "0");
                DAOFactory.DocumentoDAO.SaveOrUpdate(doc);

                var selectedEquipos = new List<int> { Convert.ToInt32(doc.Valores[ParteCampos.Equipo]) };
                var selectedTipos = new List<int>
                                    {
                                        doc.Valores.ContainsKey(ParteCampos.TipoServicio)
                                            ? Convert.ToInt32(doc.Valores[ParteCampos.TipoServicio])
                                            : 0
                                    };

                var listPartes = GetPartes(selectedEquipos, selectedTipos);

                IdPartes = listPartes.OfType<Documento>().Select(d => d.Id).ToList();

                SetViewIndex(1);
                lblCount.Text = listPartes.Count.ToString();
                LoadParte(0);
            }
        }

        protected void gridResumen_ItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;

            var parte = e.Row.DataItem as PartePersonal;
            if (parte == null) return;

            e.Row.Cells[3].Text = string.Format(HorasTemplate, parte.Horas, parte.HorasControladas, parte.DiffHoras);
            e.Row.Cells[4].Text = string.Format(KmTemplate, parte.KmTotal, parte.KmTotalCalculado, parte.DiffKmTotal);
            e.Row.Cells[5].Text = string.Format(ImporteTemplate, parte.Importe, parte.ImporteControlado, parte.DiffImporte);
            if (parte.Estado == PartePersonal.Verificado) e.Row.Cells[6].Text = "";
        }
        #endregion

        #region Actions
        private const int MainGrid_Id = 0;
        private const int MainGrid_CheckBox = 1;
        private const int MainGrid_Count = 5;
        private const int MainGrid_Service = 6;

        protected void ControlarPartes()
        {
            var selectedEquipos = new List<int>();
            var selectedTipos = new List<int>();
            var partes = 0;
            foreach (C1GridViewRow row in grid.Rows)
            {
                var chkSelected = row.Cells[MainGrid_CheckBox].FindControl("chkSelected") as CheckBox;
                if (chkSelected != null && chkSelected.Checked)
                {
                    selectedTipos.Add(Convert.ToInt32(row.Cells[MainGrid_Service].Text));
                    selectedEquipos.Add(Convert.ToInt32(row.Cells[MainGrid_Id].Text));
                    partes += Convert.ToInt32(row.Cells[MainGrid_Count].Text);
                }
            }

            if (!Check(selectedEquipos.Count > 0, "No hay ningun equipo seleccionado")) return;
            if (!Check(partes > 0, "No hay ningun parte para los equipos seleccionados")) return;

            var listPartes = GetPartes(selectedEquipos, selectedTipos);

            var idPartes = from Documento d in listPartes select d.Id;

            IdPartes = idPartes.ToList();

            SetViewIndex(1);
            lblCount.Text = listPartes.Count.ToString();
            LoadParte(0);
        }
        protected void CancelarEdicionDePartes()
        {
            Search(SearchData.Load(ViewState));
        }
        protected void VerificarPartes()
        {
            var listPartes = GetPartesAVerificar();

            gridResumen.GroupedColumns.Add((C1Field)gridResumen.Columns[0]);
            gridResumen.DataSource = listPartes;
            gridResumen.DataBind();



            SetViewIndex(2);
        }
        #endregion

        #region ViewState Properties
        protected List<int> IdPartes
        {
            get { return ViewState["IdPartes"] as List<int>; }
            set { ViewState["IdPartes"] = value; }
        }

        protected int IndexParte
        {
            get { return (int)(ViewState["IndexParte"] ?? 0); }
            set { ViewState["IndexParte"] = value; }
        }
        #endregion

        #region BasePage Overrides
        protected override InfoLabel LblInfo { get { return infoLabel1; } }
        #endregion

        #region Helpers
        private void SetViewIndex(int idx)
        {
            MultiView1.ActiveViewIndex = idx;
            liMoviles.Attributes.Add("class", idx == 0 ? "tab" : "tab2");
            liPartes.Attributes.Add("class", idx == 1 ? "tab" : "tab2");
            liResumen.Attributes.Add("class", idx == 2 ? "tab" : "tab2");
        }

        protected bool Check(bool condition, string errorMessage)
        {
            if (!condition)
                infoLabel1.Text = errorMessage;
            return condition;
        }
        #endregion

        #region Tab Events
        protected void btViewTab_Command(object sender, CommandEventArgs e)
        {
            var view = Convert.ToInt32(e.CommandArgument);

            switch (view)
            {
                case 0:
                    if (MultiView1.ActiveViewIndex != 0) CancelarEdicionDePartes();
                    break;
                case 1:
                    if (MultiView1.ActiveViewIndex != 1) ControlarPartes();
                    break;
                case 2:
                    if (MultiView1.ActiveViewIndex != 2) VerificarPartes();
                    break;
            }
        }
        #endregion

        #region SearchData
        [Serializable]
        protected class SearchData
        {
            public int aseguradora;
            public DateTime fin;
            public DateTime inicio;

            public SearchData(int aseguradora, DateTime inicio, DateTime fin)
            {
                this.aseguradora = aseguradora;
                this.inicio = inicio;
                this.fin = fin.Date.AddDays(1).AddMilliseconds(-1);
            }

            public void Save(StateBag viewstate)
            {
                viewstate["SearchData"] = this;
            }
            public static SearchData Load(StateBag viewstate)
            {
                return viewstate["SearchData"] as SearchData;
            }
        }
        #endregion

        protected override string GetRefference()
        {
            return "CONTROL_PARTE";
        }
    }
}
