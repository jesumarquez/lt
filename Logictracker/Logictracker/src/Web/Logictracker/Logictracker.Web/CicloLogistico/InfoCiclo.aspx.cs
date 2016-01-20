using System;
using System.Globalization;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using System.IO;

namespace Logictracker.CicloLogistico
{
    public partial class InfoCiclo : OnLineSecuredPage
    {
        protected override InfoLabel LblInfo { get { return null; } }
        protected override string GetRefference() { return "CLOG_MONITOR"; }
        protected override string PageTitle { get { return string.Format("{0} - InfoCiclo", ApplicationTitle); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int id;
                if (Request.QueryString["id"] == null || !int.TryParse(Request.QueryString["id"], out id))
                {
                    LblInfo.Text = CultureManager.GetError("NO_ENTREGA_INFO");
                    return;
                }
                
                var entrega = DAOFactory.EntregaDistribucionDAO.FindById(id);
                if (entrega != null)
                {
                    var icono = entrega.PuntoEntrega.ReferenciaGeografica.Icono != null ? Path.Combine(IconDir, entrega.PuntoEntrega.ReferenciaGeografica.Icono.PathIcono) : ResolveUrl("~/point.png");

                    imgIcono.Src = icono;
                    var direccion = entrega.PuntoEntrega.Nomenclado ? entrega.PuntoEntrega.DireccionNomenclada : GeocoderHelper.GetDescripcionEsquinaMasCercana(entrega.PuntoEntrega.ReferenciaGeografica.Latitude, entrega.PuntoEntrega.ReferenciaGeografica.Longitude);
                    lblCodigo.Text = string.Format("{0} - {1}<br>{2}", entrega.PuntoEntrega.Codigo, entrega.PuntoEntrega.Descripcion, direccion);
                    lblDescripcion.Text = entrega.Descripcion;
                    lblProgramado.Text = entrega.Programado.ToDisplayDateTime().ToString("HH:mm");
                    //lblGps.Text = entrega.KmGps.HasValue ? entrega.KmGps.Value.ToString(CultureInfo.InvariantCulture) : "N/D";
                    lblEntrada.Text = entrega.Entrada.HasValue ? entrega.Entrada.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty;
                    lblManual.Text = entrega.Manual.HasValue ? entrega.Manual.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty;
                    lblSalida.Text = entrega.Salida.HasValue ? entrega.Salida.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty;
                    //lblCalculado.Text = entrega.KmCalculado.HasValue ? entrega.KmCalculado.Value.ToString(CultureInfo.InvariantCulture) : "N/D";
                    //lblControlado.Text = entrega.Viaje.Controlado && entrega.KmControlado.HasValue ? entrega.KmControlado.Value.ToString(CultureInfo.InvariantCulture) : "N/D";
                    lblEstado.Text = CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(entrega.Estado));
                    lnkEditarPunto.OnClientClick = string.Format("window.open('../Parametrizacion/GeoRefAlta.aspx?id={0}')", entrega.PuntoEntrega.ReferenciaGeografica.Id);

                    lblBultos.Text = entrega.Bultos.ToString("#0");
                    lblPeso.Text = entrega.Peso.ToString("#0.00");
                    lblValor.Text = entrega.Valor.ToString("#0.00");
                    lblVolumen.Text = entrega.Volumen.ToString("#0.00");
                }
            }
        }

        protected void BtInfoClick(object sender, EventArgs e)
        {
            btInfo.Enabled = false;
            btPedidos.Enabled = true;
            MultiView1.ActiveViewIndex = 0;
        }

        protected void BtPedidosClick(object sender, EventArgs e)
        {
            btPedidos.Enabled = false;
            btInfo.Enabled = true;
            MultiView1.ActiveViewIndex = 1;
        }
    }
}
