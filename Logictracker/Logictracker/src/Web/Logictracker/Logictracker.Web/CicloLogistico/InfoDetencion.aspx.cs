using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Web.CicloLogistico
{
    public partial class InfoDetencion : OnLineSecuredPage
    {
        protected override InfoLabel LblInfo { get { return null; } }
        protected override string GetRefference() { return "CLOG_MONITOR"; }
        protected override string PageTitle { get { return string.Format("{0} - InfoDetencion", ApplicationTitle); } }

        public LogMensaje Evento { get; set; }
        public ViajeDistribucion Viaje { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (C1GridViewRow row in gridEntregas.Rows)
            {
                if (row.RowType == C1GridViewRowType.DataRow)
                {
                    var chk = row.Cells[5].Controls[0] as CheckBox;
                    chk.Enabled = true;
                    row.Cells[5].Controls.Clear();
                    row.Cells[5].Controls.Add(chk);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int idEvento, idViaje;
            if (Request.QueryString["idEvento"] == null || !int.TryParse(Request.QueryString["idEvento"], out idEvento)
             || Request.QueryString["idViaje"] == null || !int.TryParse(Request.QueryString["idViaje"], out idViaje))
            {
                LblInfo.Text = CultureManager.GetError("NO_ENTREGA_INFO");
                return;
            }

            var viaje = DAOFactory.ViajeDistribucionDAO.FindById(idViaje);
            var evento = DAOFactory.LogMensajeDAO.FindById(idEvento);
            Evento = evento;
            Viaje = viaje;

            if (!IsPostBack)
            {
                if (evento != null && viaje != null)
                {
                    lblFecha.Text = evento.Fecha.ToDisplayDateTime().ToString("dd-MM-yyyy HH:mm");
                    lblEvento.Text = evento.Texto;
                    lblLatLong.Text = "(" + evento.Latitud + " ; " + evento.Longitud + ")";
                    gridEntregas.DataSource = viaje.Detalles.Where(d => d.PuntoEntrega != null);
                    gridEntregas.DataBind();
                }
            }
        }

        protected void GridEntregasRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;
            var entrega = e.Row.DataItem as EntregaDistribucion;
            if (entrega == null) return;

            e.Row.Cells[0].Text = entrega.Orden.ToString("#0");
            e.Row.Cells[1].Text = entrega.Descripcion;
            e.Row.Cells[2].Text = entrega.PuntoEntrega.Codigo;
            e.Row.Cells[3].Text = entrega.PuntoEntrega.Descripcion;
            e.Row.Cells[4].Text = CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(entrega.Estado));
            e.Row.Attributes.Add("idEntrega", entrega.Id.ToString("#0"));
            
            switch (entrega.Estado)
            {
                case EntregaDistribucion.Estados.Completado: e.Row.BackColor = Color.GreenYellow; break;
                case EntregaDistribucion.Estados.NoCompletado: e.Row.BackColor = Color.Red; break;
                case EntregaDistribucion.Estados.Visitado: e.Row.BackColor = Color.Yellow; break;
                case EntregaDistribucion.Estados.EnSitio: e.Row.BackColor = Color.CornflowerBlue; break;
                case EntregaDistribucion.Estados.EnZona: e.Row.BackColor = Color.Gray; break;
                case EntregaDistribucion.Estados.SinVisitar:
                case EntregaDistribucion.Estados.Pendiente: e.Row.BackColor = Color.Orange; break;
            }
        }

        protected void BtnAsociarClick(object sender, EventArgs e)
        {
            var puntos = new List<PuntoEntrega>();

            foreach (C1GridViewRow row in gridEntregas.Rows)
            {
                if (row.RowType == C1GridViewRowType.DataRow)
                {
                    var chk = row.Cells[5].Controls[0] as CheckBox;
                    if (chk != null && chk.Checked)
                    {
                        int idEntrega;
                        var id = row.Attributes["idEntrega"];
                        if (int.TryParse(id, out idEntrega))
                        {
                            var entrega = DAOFactory.EntregaDistribucionDAO.FindById(idEntrega);
                            if (entrega != null && entrega.PuntoEntrega != null)
                            {
                                puntos.Add(entrega.PuntoEntrega);
                            }
                        }
                        chk.Checked = false;
                        row.Cells[5].Controls.Clear();
                        row.Cells[5].Controls.Add(chk);
                    }
                }
            }

            if (puntos.Any())
            {
                var primerEntrega = puntos.First();
                var nombre = txtNombre.Text.Trim();
                var codigo = nombre != string.Empty ? nombre : primerEntrega.Codigo;
                var descripcion = nombre != string.Empty ? nombre : primerEntrega.Descripcion;
                int radio;
                if (!int.TryParse(txtRadio.Text.Trim(), out radio)) radio = 50;
                var utcNow = DateTime.UtcNow;

                var nuevaReferencia = new ReferenciaGeografica
                {
                    Codigo = codigo,
                    Descripcion = descripcion,
                    Empresa = primerEntrega.Cliente.Empresa,
                    Linea = primerEntrega.Cliente.Linea,
                    Icono = primerEntrega.ReferenciaGeografica.Icono,
                    IgnoraLogiclink = true,
                    TipoReferenciaGeografica = primerEntrega.ReferenciaGeografica.TipoReferenciaGeografica,
                    Vigencia = new Vigencia { Inicio = utcNow },
                    Color = primerEntrega.ReferenciaGeografica.Color
                };
                
                var direccion = GetNewDireccion(Evento.Latitud, Evento.Longitud);
                var poligono = new Poligono { Radio = radio, Vigencia = new Vigencia { Inicio = utcNow } };
                poligono.AddPoints(new[] {new PointF((float) Evento.Longitud, (float) Evento.Latitud)});
                
                nuevaReferencia.AddHistoria(direccion, poligono, utcNow);

                DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(nuevaReferencia);
                STrace.Trace("QtreeReset", "InfoDetencion 1");

                foreach (var punto in puntos)
                {
                    punto.ReferenciaGeografica.Direccion.Vigencia.Fin = utcNow;
                    punto.ReferenciaGeografica.Poligono.Vigencia.Fin = utcNow;
                    punto.ReferenciaGeografica.Vigencia.Fin = utcNow;
                    DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(punto.ReferenciaGeografica);
                    STrace.Trace("QtreeReset", "InfoDetencion 2");
                    
                    punto.ReferenciaGeografica = nuevaReferencia;
                    DAOFactory.PuntoEntregaDAO.SaveOrUpdate(punto);
                }
            }

            lblAsociados.Visible = true;
        }

        //private void AsociarDetencion(EntregaDistribucion entrega)
        //{
        //    if (Evento != null && entrega != null && entrega.PuntoEntrega != null &&
        //        entrega.PuntoEntrega.ReferenciaGeografica != null)
        //    {
        //        entrega.PuntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
        //        entrega.PuntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;
        //        entrega.PuntoEntrega.ReferenciaGeografica.IgnoraLogiclink = true;

        //        var posicion = GetNewDireccion(Evento.Latitud, Evento.Longitud);
        //        var poligono = new Poligono {Radio = 50, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
        //        poligono.AddPoints(new[] {new PointF((float) Evento.Longitud, (float) Evento.Latitud)});

        //        entrega.PuntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);

        //        DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(entrega.PuntoEntrega.ReferenciaGeografica);
        //        DAOFactory.PuntoEntregaDAO.SaveOrUpdate(entrega.PuntoEntrega);
        //    }
        //}

        private static Direccion GetNewDireccion(double latitud, double longitud)
        {
            return new Direccion
            {
                Altura = -1,
                IdMapa = -1,
                Provincia = string.Empty,
                IdCalle = -1,
                IdEsquina = -1,
                IdEntrecalle = -1,
                Latitud = latitud,
                Longitud = longitud,
                Partido = string.Empty,
                Pais = string.Empty,
                Calle = string.Empty,
                Descripcion = string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture), longitud.ToString(CultureInfo.InvariantCulture)),
                Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
            };
        }
    }
}
