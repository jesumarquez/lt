#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos.Partes;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Documentos.Partes;

#endregion

namespace Logictracker.Documentos
{
    public partial class Documentos_VerificarPartes : ApplicationSecuredPage
    {
        #region Constants

        private const string HorasTemplate = @"<span style=""color: #0000CC;"">{0}</span>-<span style=""color: #006600; font-weight: bold;"">{1}</span>(<span style=""color: #990000;"">{2}</span>) ";
        private const string ImporteTemplate = @"<span style=""color: #0000CC;"">${0:0.00}</span>-<span style=""color: #006600; font-weight: bold;"">${1:0.00}</span>(<span style=""color: #990000;"">${2:0.00}</span>) ";
        private const string KmTemplate = @"<span style=""color: #0000CC;"">{0}km</span>-<span style=""color: #006600; font-weight: bold;"">{1}km</span>(<span style=""color: #990000;"">{2}km</span>) ";
        private const string TotalesTemplate = @"{0} <br/>
                        <span style=""font-weight: normal; font-size: 9px;"">
                            Horas: {1} | 
                            Km: {2} | 
                            Importe:  {3}
                        </span>";

        #endregion

        #region Protected Properties

        protected int IdPeriodo
        {
            get { return Convert.ToInt32(ViewState["IdPeriodo"] ?? 0); }
            set { ViewState["IdPeriodo"] = value; }
        }
        protected int IdTransportista
        {
            get { return Convert.ToInt32(ViewState["IdTransportista"] ?? 0); }
            set { ViewState["IdTransportista"] = value; }
        }

        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        protected override string GetRefference() { return "VER_PARTE"; }

        #endregion

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { BindPeriodos();
                panelResultado.Visible = false; }
        }

        protected void BindPeriodos()
        {
            var transp = DAOFactory.TransportistaDAO.FindById(cbAseguradora.Selected);
            cbPeriodo.DataSource = DAOFactory.PeriodoDAO.GetByEstado(transp.Empresa != null ? transp.Empresa.Id:-1, Periodo.Cerrado);
            cbPeriodo.DataBind();
        }

        protected IList GetPartesAVerificar()
        {
            var periodo = DAOFactory.PeriodoDAO.FindById(Convert.ToInt32(cbPeriodo.SelectedValue));
            var transportista = DAOFactory.TransportistaDAO.FindById(cbAseguradora.Selected);
            IdPeriodo = periodo.Id;
            IdTransportista = transportista.Id;
            // Busco el detalle de periodo para el transportista seleccionado
            var detallePeriodo = (from DetallePeriodo detalle in periodo.Detalles
                                  where detalle.Transportista.Id == cbAseguradora.Selected
                                  select detalle).ToList();

            // Si existe el detalle y ya esta verificado, no puedo verificar nada
            if(detallePeriodo.Count > 0 && detallePeriodo[0].Estado == Periodo.Liquidado)
                throw new ApplicationException("La empresa transportista seleccionada ya ha sido verificada en este periodo");
            
            // Traigo los documentos y los convierto en PartePersonal
            var partes = from Documento d in DAOFactory.DocumentoDAO.FindPartes(cbAseguradora.Selected, periodo.FechaDesde, periodo.FechaHasta)
                         select new PartePersonal(d, DAOFactory);

            // Busco partes que no esten controlados
            var sinControlar = (from parte in partes
                                where parte.Estado == PartePersonal.SinControlar
                                select parte).Count();
            if(sinControlar > 0)
                throw new ApplicationException("Hay partes sin controlar en este periodo");

            // Agrupo los partes por equipo
            var byEquipo = new Dictionary<string, List<PartePersonal>>();

            foreach (var parte in partes)
            {
                if (!byEquipo.ContainsKey(parte.Equipo))
                    byEquipo.Add(parte.Equipo, new List<PartePersonal>());
                byEquipo[parte.Equipo].Add(parte);
            }

            // Calculo los importes segun la tarifa que corresponda
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
                    parte.Importe = parte.KmTotal * (kms < 14000 ? tarifaCorto : tarifaLargo);
                    parte.ImporteControlado = parte.KmTotalCalculado * (kmsc < 14000 ? tarifaCorto : tarifaLargo);
                }
            }

            // Calculo los totales por equipo 
            // e inserto en una lista los partes agrupados por equipo
            var sums = new Dictionary<string, string>();
            var partesOrdenados = new List<PartePersonal>();

            foreach (var l in byEquipo.Values)
            {
                partesOrdenados.AddRange(l);

                var horas = TimeSpan.Zero;
                var horasControladas = TimeSpan.Zero;
                var horasDiff = TimeSpan.Zero;
                var km = 0;
                double kmControlado = 0;
                double kmDiff = 0;
                double importe = 0;
                double importeControlado = 0;
                double importeDiff = 0;
                foreach (var parte in l)
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
                                            l[0].Equipo,
                                            string.Format(HorasTemplate, horas, horasControladas, horasDiff),
                                            string.Format(KmTemplate, km, kmControlado, kmDiff),
                                            string.Format(ImporteTemplate, importe, importeControlado, importeDiff)
                    );

                sums.Add(l[0].Equipo, totales);
            }

            // agrego a la descripcion del equipo los totales
            foreach (var ordenado in partesOrdenados)
                ordenado.Equipo = sums[ordenado.Equipo];

            return partesOrdenados;
        }

        protected void Buscar()
        {
            try
            {
                if (cbPeriodo.SelectedIndex < 0)
                    throw new ApplicationException("No hay periodos cerrados para verificar.");
                var partes = GetPartesAVerificar();
                gridResumen.DataSource = partes;
                gridResumen.DataBind();
                btGuardar.Visible = (partes.Count > 0);
                panelResultado.Visible = (partes.Count > 0);
            }
            catch (Exception ex)
            {
                infoLabel1.Text = ex.Message;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e) { Buscar(); }

        protected void btGuardar_Click(object sender, EventArgs e)
        {
            var transaction = SessionHelper.Current.BeginTransaction();

            var faltan = false;

            foreach (C1GridViewRow row in gridResumen.Rows)
            {
                // Si el item esta tildado pongo el estado del parte es Verificado
                var chkVerificar = row.FindControl("chkVerificar") as CheckBox;

                if (chkVerificar != null && chkVerificar.Checked)
                {
                    var id = Convert.ToInt32(gridResumen.DataKeys[row.RowIndex].Value);
                    var doc = DAOFactory.DocumentoDAO.FindById(id);

                    ParteSaveStrategy.SetValor(doc, ParteCampos.EstadoControl, 1, PartePersonal.Verificado.ToString());

                    DAOFactory.DocumentoDAO.SaveOrUpdate(doc);
                }
                else faltan = true;
            }

            // Si todos los partes de este periodo estan verificados...
            if(!faltan)
            {
                // Obtengo el DetallePeriodo para el Transportista o lo creo si no existe
                var periodo = DAOFactory.PeriodoDAO.FindById(IdPeriodo);

                var detalle = (from DetallePeriodo det in periodo.Detalles where det.Transportista.Id == IdTransportista select det).ToList();

                DetallePeriodo detPeriodo;

                if (detalle.Count == 0)
                {
                    detPeriodo = new DetallePeriodo
                                     {
                                         Periodo = periodo,
                                         Transportista = DAOFactory.TransportistaDAO.FindById(IdTransportista)
                                     };

                    periodo.Detalles.Add(detPeriodo);
                }
                else detPeriodo = detalle[0];

                // Seteo el estado del DetallePeriodo a Liquidado
                detPeriodo.Estado = Periodo.Liquidado;

                // Busco los partes que no esten verificados (excepto los de este Transportista)
                var controlados = DAOFactory.DocumentoDAO.FindPartesControlados(IdTransportista, periodo.FechaDesde, periodo.FechaHasta);

                var partes = from Documento doc in controlados where Convert.ToInt32(doc.Valores[ParteCampos.Empresa]) != IdTransportista select doc;
                
                if(!partes.Any())
                {
                    // Si no quedaron partes sin verificar cierro el periodo completo
                    periodo.Estado = Periodo.Liquidado;
                }
                
                DAOFactory.PeriodoDAO.SaveOrUpdate(periodo);
            }

            transaction.Commit();

            BindPeriodos();

            gridResumen.DataSource = null;

            gridResumen.DataBind();

            btGuardar.Visible = false;
            panelResultado.Visible = false;
        }

        protected void gridResumen_ItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if(e.Row.RowType != C1GridViewRowType.DataRow) return;
            var parte = e.Row.DataItem as PartePersonal;
            if (parte == null) return;

            e.Row.Cells[3].Text = string.Format(HorasTemplate, parte.Horas, parte.HorasControladas, parte.DiffHoras);
            e.Row.Cells[4].Text = string.Format(KmTemplate, parte.KmTotal, parte.KmTotalCalculado, parte.DiffKmTotal);
            e.Row.Cells[5].Text = string.Format(ImporteTemplate, parte.Importe, parte.ImporteControlado, parte.DiffImporte);

            var chkVerificado = e.Row.FindControl("chkVerificar") as CheckBox;
            if (chkVerificado != null)
            {
                chkVerificado.Checked = parte.Estado == PartePersonal.Verificado;
                chkVerificado.Enabled = parte.Estado != PartePersonal.Verificado;
            }
        }

        #endregion

    }
}
