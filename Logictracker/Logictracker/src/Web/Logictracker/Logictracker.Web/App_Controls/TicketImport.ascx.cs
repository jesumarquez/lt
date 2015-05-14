using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Geocoder.Core.VO;
using LinqToExcel;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.ImportadorObjects;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces;
using Logictracker.Configuration;

namespace Logictracker.App_Controls
{
    public partial class App_Controls_TicketImport : BaseUserControl
    {

        #region Private Const Properties

        /// <summary>
        /// Excel Type
        /// </summary>
        private const string XlsType = "application/vnd.ms-excel";

        private const string Reporte = "Procesado: {0} de {1} <br/> Exitosos: {2} ({3}%)<br/>Fallidos: {4} ({5}%)<br/>Sin Procesar: {6} ({7}%)<br/>Tiempo: {8}<br/>";

        private const string Result = "{0}, Cliente: {1} - {2}, Obra: {3} - {4}, Ticket: {5}, Cantidad: {6} ({7}) {8}<br/>";

        private string ProcessJsFunction {get{return string.Concat(ClientID,"_process");}}

        #endregion

        #region ViewState Properties
        private string FileName
        {
            get { return (string)(ViewState["FileName"] ?? (ViewState["FileName"] = string.Empty)); }
            set { ViewState["FileName"] = value; }
        }

        private int Index
        {
            get { return (int)(ViewState["Index"] ?? 0); }
            set { ViewState["Index"] = value; }
        }
        private int TotalCount
        {
            get { return (int)(ViewState["TotalCount"] ?? 0); }
            set { ViewState["TotalCount"] = value; }
        }
        private int Erroneos
        {
            get { return (int)(ViewState["Erroneos"] ?? 0); }
            set { ViewState["Erroneos"] = value; }
        }
        private int Correctos
        {
            get { return (int)(ViewState["Correctos"] ?? 0); }
            set { ViewState["Correctos"] = value; }
        }

        private int SinProcesar
        {
            get { return (int)(ViewState["SinProcesar"] ?? 0); }
            set { ViewState["SinProcesar"] = value; }
        }
        private long Start
        {
            get { return (long)(ViewState["Start"] ?? Convert.ToInt64(0)); }
            set { ViewState["Start"] = value; }
        } 
        #endregion

        #region DropDownList Value Getter Properties
        private string CurrentWorkSheet { get { return cbWorksheets.SelectedIndex < 0 ? string.Empty : cbWorksheets.SelectedValue; } }

        private string ColumnaCodigoCliente { get { return cbCodigoCliente.SelectedIndex < 0 ? string.Empty : cbCodigoCliente.SelectedValue; } }
        private string ColumnaDescripcionCliente { get { return cbDescripcionCliente.SelectedIndex < 0 ? string.Empty : cbDescripcionCliente.SelectedValue; } }
        private string ColumnaDireccionCliente { get { return cbDireccionCliente.SelectedIndex < 0 ? string.Empty : cbDireccionCliente.SelectedValue; } }
        private string ColumnaLocalidadCliente { get { return cbLocalidadCliente.SelectedIndex < 0 ? string.Empty : cbLocalidadCliente.SelectedValue; } }
        private string ColumnaProvinciaCliente { get { return cbProvinciaCliente.SelectedIndex < 0 ? string.Empty : cbProvinciaCliente.SelectedValue; } }
        private string ColumnaTelefonoCliente { get { return cbTelefonoCliente.SelectedIndex < 0 ? string.Empty : cbTelefonoCliente.SelectedValue; } }
        private string ColumnaCodigoPtoDeEntrega { get { return cbCodigoPuntoEntrega.SelectedIndex < 0 ? string.Empty : cbCodigoPuntoEntrega.SelectedValue; } }
        private string ColumnaDescripcionPtoDeEntrega { get { return cbDescripcionPuntoEntrega.SelectedIndex < 0 ? string.Empty : cbDescripcionPuntoEntrega.SelectedValue; } }
        private string ColumnaDireccionPtoDeEntrega { get { return cbDireccionPuntoEntrega.SelectedIndex < 0 ? string.Empty : cbDireccionPuntoEntrega.SelectedValue; } }
        private string ColumnaLocalidadPtoDeEntrega { get { return cbLocalidadPuntoEntrega.SelectedIndex < 0 ? string.Empty : cbLocalidadPuntoEntrega.SelectedValue; } }
        private string ColumnaCodigoTicket { get { return cbCodigoTicket.SelectedIndex < 0 ? string.Empty : cbCodigoTicket.SelectedValue; } }
        private string ColumnaFecha { get { return cbFecha.SelectedIndex < 0 ? string.Empty : cbFecha.SelectedValue; } }
        private string ColumnaCantPedido { get { return cbCantidadPedido.SelectedIndex < 0 ? string.Empty : cbCantidadPedido.SelectedValue; } }
        private string ColumnaCantAcumulada { get { return cbCantidadAcumulada.SelectedIndex < 0 ? string.Empty : cbCantidadAcumulada.SelectedValue; } }
        private string ColumnaUnidad { get { return cbUnidad.SelectedIndex < 0 ? string.Empty : cbUnidad.SelectedValue; } }
        private string ColumnaCodigoProducto { get { return cbCodigoProducto.SelectedIndex < 0 ? string.Empty : cbCodigoProducto.SelectedValue; } }
        private string ColumnaComentario1 { get { return cbComentario1.SelectedIndex < 0 ? string.Empty : cbComentario1.SelectedValue; } }
        private string ColumnaComentario2 { get { return cbComentario2.SelectedIndex < 0 ? string.Empty : cbComentario2.SelectedValue; } }
        private string ColumnaComentario3 { get { return cbComentario3.SelectedIndex < 0 ? string.Empty : cbComentario3.SelectedValue; } }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(Page).AsyncPostBackTimeout = (int)TimeSpan.FromMinutes(30).TotalSeconds;
            Page.ClientScript.RegisterStartupScript(typeof(string), ProcessJsFunction,
                                                    "function "+ProcessJsFunction+"() { " +

                                                    Page.ClientScript.GetPostBackEventReference(btProcess, string.Empty) + "; }",
                                                    true);
        }

        protected void BtUploadClick(object sender, EventArgs e)
        {
            if (!filExcel.HasFile) return;
            if (filExcel.PostedFile.ContentType != XlsType) return;

            FileName = string.Concat(Server.MapPath(Config.Directory.TmpDir), 
                                     DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                     "_",
                                     filExcel.FileName,
                                     ".xls");

            filExcel.SaveAs(FileName);

            LoadWorkSheets();
            panelProcess.Visible = panelProgress.Visible = true;
            lblFileName.Text = filExcel.FileName;
            lblResult.Text = string.Empty;
        }

        protected void CbWorksheetsSelectedIndexChanged(object sender, EventArgs e)
        {
            LoadColumns();
        }

        protected void BtProcessClick(object sender, EventArgs e)
        {
            try
            {
                var excel = new ExcelQueryFactory(FileName);
                excel.AddMapping<ImportTicket>(dir => dir.CodigoCliente, ColumnaCodigoCliente);
                excel.AddMapping<ImportTicket>(dir => dir.DescripcionCliente, ColumnaDescripcionCliente);
                excel.AddMapping<ImportTicket>(dir => dir.DireccionCliente, ColumnaDireccionCliente);
                excel.AddMapping<ImportTicket>(dir => dir.LocalidadCliente, ColumnaLocalidadCliente);
                excel.AddMapping<ImportTicket>(dir => dir.ProvinciaCliente, ColumnaProvinciaCliente);
                excel.AddMapping<ImportTicket>(dir => dir.TelefonoCliente, ColumnaTelefonoCliente);
                excel.AddMapping<ImportTicket>(dir => dir.CodigoPtoDeEntrega, ColumnaCodigoPtoDeEntrega);
                excel.AddMapping<ImportTicket>(dir => dir.DescripcionPtoDeEntrega, ColumnaDescripcionPtoDeEntrega);
                excel.AddMapping<ImportTicket>(dir => dir.DireccionPtoDeEntrega, ColumnaDireccionPtoDeEntrega);
                excel.AddMapping<ImportTicket>(dir => dir.LocalidadPtoDeEntrega, ColumnaLocalidadPtoDeEntrega);
                if (!string.IsNullOrEmpty(ColumnaCodigoTicket)) excel.AddMapping<ImportTicket>(dir => dir.CodigoTicket, ColumnaCodigoTicket);
                excel.AddMapping<ImportTicket>(dir => dir.Fecha, ColumnaFecha);
                excel.AddMapping<ImportTicket>(dir => dir.CantPedido, ColumnaCantPedido);
                excel.AddMapping<ImportTicket>(dir => dir.CantAcumulada, ColumnaCantAcumulada);
                excel.AddMapping<ImportTicket>(dir => dir.Unidad, ColumnaUnidad);
                excel.AddMapping<ImportTicket>(dir => dir.CodigoProducto, ColumnaCodigoProducto);
                excel.AddMapping<ImportTicket>(dir => dir.Comentario1, ColumnaComentario1);
                excel.AddMapping<ImportTicket>(dir => dir.Comentario2, ColumnaComentario2);
                excel.AddMapping<ImportTicket>(dir => dir.Comentario3, ColumnaComentario3);


                var ws = excel.Worksheet<ImportTicket>(CurrentWorkSheet).ToList();
                lblDirs.Text = string.Empty;
                Procesar(ws);
            }
            catch (Exception ex)
            {
                lblResult.Text += "<hr/>" + ex;
            }
        }
    
        private void Procesar(List<ImportTicket> importData)
        {
            var step = 1;// Index < 10 || importData.Count < 20
            //   ? 1 : Index > importData.Count - 10 ? 1 : importData.Count <= 100 ? 5 : 10;
            var next = Index + step;
            if (Start == 0)
            {
                Start = DateTime.Now.Ticks;
                lblResult.Text = string.Empty;
            }
        
            var empresa = DAOFactory.EmpresaDAO.FindById(ddlEmpresaTickets.Selected);
            var linea = DAOFactory.LineaDAO.FindById(ddlBaseTickets.Selected);
            var ticketDetails = DAOFactory.EstadoDAO.GetByPlanta(linea.Id);

            if (ticketDetails.Count.Equals(0))
            {
                Log(@"No existen Estados Logisticos definidos para esta planta.");
                return;
            }

            for (var i = Index; i < importData.Count && i < next; i++)
            {
                var row = importData[i];
                try
                {
                    Index = i + 1;
                    TotalCount++;

                    var codigoTicket = GetCodigoTicket(row);
                    var ticket = DAOFactory.TicketDAO.FindByCode(new[]{empresa.Id}, new[]{linea.Id}, codigoTicket);
                    if(ticket != null)
                    {
                        Log(@"Código de Ticket Duplicado", row);
                        SinProcesar++;
                        continue;
                    }

            
                    ticket = CreateTicket(linea, row, empresa, ticketDetails);

                    if (IsValidTicket(ticket))
                    {
                        Correctos++;
                        DAOFactory.TicketDAO.SaveOrUpdate(ticket);
                        Log("Ok", row);
                    }
                    else
                    {
                        Erroneos++;
                        Log("Error", row);
                    }
                }
                catch (Exception e) 
                {
                    SinProcesar++;
                    Log(e.Message,row);
                }       
            }

            lblDirs.Text = string.Format(Reporte,
                                         TotalCount,
                                         importData.Count,
                                         Correctos,
                                         TotalCount > 0 ? (Correctos * 100.0 / TotalCount).ToString("0.00") : "0.00",
                                         Erroneos,
                                         TotalCount > 0 ? (Erroneos * 100.0 / TotalCount).ToString("0.00") : "0.00",
                                         SinProcesar,
                                         TotalCount > 0 ? (SinProcesar * 100.0 / TotalCount).ToString("0.00") : "0.00",
                                         TimeSpan.FromTicks(DateTime.Now.Ticks-Start));

            SetProgressBar(Index * 100 / importData.Count);
            if(Index < importData.Count)
            {
                ScriptManager.RegisterStartupScript(Page, typeof(string), "recall" + Index, " " +ProcessJsFunction+"(); ", true);
                if (panelProcess.Enabled)
                {
                    panelProcess.Enabled = false;
                    updProcess.Update();
                }
            }
            else
            {
                if (!panelProcess.Enabled)
                {
                    panelProcess.Enabled = true;
                    updProcess.Update();
                }
                Index = 0;
                Correctos = 0;
                Erroneos = 0;
                SinProcesar = 0;
                TotalCount = 0;
                Start = Convert.ToInt64(0);
            }
        }

        private void Log(string status)
        {
            lblResult.Text += status+"</br>";
        }
        private void Log(string status, ImportTicket row)
        {
            var codigo = GetCodigoTicket(row);
            lblResult.Text += string.Format(Result, 
                                            status, 
                                            row.CodigoCliente, 
                                            row.DescripcionCliente, 
                                            row.CodigoPtoDeEntrega,
                                            row.DescripcionPtoDeEntrega,
                                            codigo,
                                            row.CantPedido,
                                            row.CantAcumulada,
                                            row.Unidad);
        }

        private void SetProgressBar(int percent)
        {
            const int totalWidth = 200;
            litProgress.Text = string.Format(@"<div style='margin: auto; border: solid 1px #999999; background-color: #FFFFFF; width: {0}px; height: 10px;'>
                <div style='background-color: #0000AA; width: {1}px; height: 10px; font-size: 8px; color: #CCCCCC;'>{2}%</div>
                </div>", totalWidth, percent * totalWidth / 100, percent);
        }
        private void LoadWorkSheets()
        {
            var excel = new ExcelQueryFactory(FileName);
            cbWorksheets.DataSource = excel.GetWorksheetNames();
            cbWorksheets.DataBind();
        
            LoadColumns();
        }
        private void LoadColumns()
        {
            var excel = new ExcelQueryFactory(FileName);
            var columns = excel.GetColumnNames(CurrentWorkSheet).OrderBy(col => col);

            BindColumns(cbCodigoCliente, columns, true, "CodigoCliente");
            BindColumns(cbDescripcionCliente, columns, true, "DescripcionCliente");
            BindColumns(cbDireccionCliente, columns, true, "DireccionCliente");
            BindColumns(cbLocalidadCliente, columns, true, "LocalidadCliente");
            BindColumns(cbProvinciaCliente, columns, true, "ProvinciaCliente");
            BindColumns(cbTelefonoCliente, columns, true, "TelefonoCliente");
            BindColumns(cbCodigoPuntoEntrega, columns, true, "CodigoPtoDeEntrega");
            BindColumns(cbDescripcionPuntoEntrega, columns, true, "DescripcionPtoDeEntrega");
            BindColumns(cbDireccionPuntoEntrega, columns, true, "DireccionPtoDeEntrega");
            BindColumns(cbLocalidadPuntoEntrega, columns, true, "LocalidadPtoDeEntrega");
            BindColumns(cbCodigoTicket, columns, true, "CodigoTicket");
            BindColumns(cbFecha, columns, true, "Fecha");
            BindColumns(cbCantidadPedido, columns, true, "CantPedido");
            BindColumns(cbCantidadAcumulada, columns, true, "CantAcumulada");
            BindColumns(cbUnidad, columns, true, "Unidad");
            BindColumns(cbCodigoProducto, columns, true, "CodigoProducto");
            BindColumns(cbComentario1, columns, true, "Comentario1");
            BindColumns(cbComentario2, columns, true, "Comentario2");
            BindColumns(cbComentario3, columns, true, "Comentario3");

            lblDirs.Text = string.Format(Reporte, 0, excel.Worksheet(CurrentWorkSheet).Count(), 0, "0.00", 0, "0.00", 0, "0.00", 0);
            SetProgressBar(0);
        }
        private static void BindColumns(ListControl cb, IEnumerable<string> values, bool addEmpty, string trySelect)
        {
            cb.Items.Clear();
            if (addEmpty) cb.Items.Add(string.Empty);
            foreach (var val in values) cb.Items.Add(val);
            TrySelect(cb, trySelect);
        }

        private static void TrySelect(ListControl cb, string value)
        {
            var it = cb.Items.FindByValue(value);
            if (it != null) it.Selected = true;
        }

        #region Private Methods

        private Ticket CreateTicket(Linea linea, ImportTicket ticketInfo, Empresa empresa, IList ticketDetails)
        {
            var cod = Trunc(ticketInfo.CodigoCliente, 32);
            var idEmpresa = new[] {empresa != null ? empresa.Id : -1};
            var idLinea = new[] {linea != null ? linea.Id : -1};
            var cliente = DAOFactory.ClienteDAO.FindByCode(idEmpresa, idLinea, cod) ??
                          CreateCliente(ticketInfo, empresa, linea);

            if (cliente == null) return null;

            var ptoEntrega = DAOFactory.PuntoEntregaDAO.GetByCode(idEmpresa, idLinea, new[] { cliente.Id }, ticketInfo.CodigoPtoDeEntrega)
                             ?? CreatePtoDeEntrega(ticketInfo, cliente);
        
            if (ptoEntrega == null) return null;
        
            var ticket = new Ticket
                             {
                                 Linea = linea,
                                 BaseDestino = linea,
                                 Codigo = GetCodigoTicket(ticketInfo),
                                 FechaTicket = Convert.ToDateTime(ticketInfo.Fecha).ToDataBaseDateTime(),
                                 Cliente = cliente,
                                 PuntoEntrega = ptoEntrega,
                                 Unidad = ticketInfo.Unidad,
                                 CodigoProducto = ticketInfo.CodigoProducto,
                                 UserField1 = ticketInfo.Comentario1,
                                 UserField2 = ticketInfo.Comentario2,
                                 UserField3 = ticketInfo.Comentario3,
                                 CantidadCarga = "0",
                                 CantidadPedido = ticketInfo.CantPedido,
                                 CumulativeQty = string.IsNullOrEmpty(ticketInfo.CantAcumulada) ? "0" : ticketInfo.CantAcumulada,
                                 Estado = 0,
                                 Vehiculo = null,
                                 Empleado = null,
                                 EstadoLogistico = null,
                                 FechaDescarga = null,
                                 FechaFin = null,
                                 SourceFile = "LOGICTRACKER",
                                 SourceStation = "LOGICTRACKER"
                             };

            var fecha = ticket.FechaTicket;
            foreach (var e in (from Estado e in ticketDetails orderby e.Orden select e))
            {
                ticket.Detalles.Add(new DetalleTicket { EstadoLogistico = e, Ticket = ticket, Programado = fecha });
                fecha = fecha.Value.Add(TimeSpan.FromMinutes(e.Deltatime));
            }

            ticket.OrdenDiario = DAOFactory.TicketDAO.FindNextOrdenDiario(idEmpresa[0], idLinea[0], ticket.FechaTicket.Value);
            if (ticket.Vehiculo != null) ticket.Dispositivo = ticket.Vehiculo.Dispositivo;
            return ticket;
        }

        private string GetCodigoTicket(ImportTicket ticketInfo)
        {
            //opf
            var codigo = string.IsNullOrEmpty(ColumnaCodigoTicket) ? string.Concat(Convert.ToDateTime(ticketInfo.Fecha).ToString("yyMMdd"), ".",ticketInfo.CodigoCliente,".", ticketInfo.CodigoPtoDeEntrega, ".", ticketInfo.CodigoProducto ) : ticketInfo.CodigoTicket;

            return codigo;
        }

        private PuntoEntrega CreatePtoDeEntrega(ImportTicket ticketInfo, Cliente cliente)
        {
            var poi = ticketInfo.DireccionPtoDeEntrega + ", " + ticketInfo.LocalidadPtoDeEntrega;

            bool nomenclado;
            var puntoDeInteres = GetGeoRef(poi, ddlTipoGeoRefPtoInteres, cliente.Codigo + "_" + ticketInfo.CodigoPtoDeEntrega, ticketInfo.DescripcionPtoDeEntrega, cliente.Linea, out nomenclado);

            var pto = new PuntoEntrega
                          {
                              Cliente = cliente,           
                              Codigo = Trunc(ticketInfo.CodigoPtoDeEntrega, 32),
                              Descripcion = Trunc(ticketInfo.DescripcionPtoDeEntrega,40),
                              Telefono = "",
                              Baja = false,
                              ReferenciaGeografica = puntoDeInteres,
                              Nomenclado = nomenclado,
                              DireccionNomenclada = Trunc(poi, 255)
                          };
            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(pto);
            return pto;
        }

        private Cliente CreateCliente(ImportTicket ticketInfo, Empresa emp, Linea lin)
        {
            var clientPoi = ticketInfo.DireccionCliente + ", " + ticketInfo.LocalidadCliente + ", " + ticketInfo.ProvinciaCliente;

            bool nomenclado;
            var puntoDeInteres = GetGeoRef(clientPoi, ddlTipoGeoRefCli, ticketInfo.CodigoCliente, ticketInfo.DescripcionCliente, lin, out nomenclado);

            var cli = new Cliente
                          {
                              Codigo = Trunc(ticketInfo.CodigoCliente,32),
                              Descripcion = Trunc(ticketInfo.DescripcionCliente, 40),
                              DescripcionCorta = Trunc(ticketInfo.DescripcionCliente, 17),
                              Empresa = emp,
                              Linea = lin,
                              Telefono = Trunc(ticketInfo.TelefonoCliente, 32),
                              Baja = false,
                              ReferenciaGeografica = puntoDeInteres,
                              Nomenclado = nomenclado,
                              DireccionNomenclada = Trunc(clientPoi, 255)
                          };
            DAOFactory.ClienteDAO.SaveOrUpdate(cli);
            return cli;
        }

        public ReferenciaGeografica GetGeoRef(string direccion, IAutoBindeable cbTipo, string codigo, string descripcion, Linea linea, out bool nomenclado)
        {
            var dir = GeocoderHelper.Cleaning.GetSmartSearch(direccion).FirstOrDefault();

            if (dir == null)
            {
                nomenclado = false;
                Log(CultureManager.GetError("NOMENCLATE_POI") + direccion);
                return  linea.ReferenciaGeografica;
            }
            nomenclado = true;
            var tipoGeoRef = DAOFactory.TipoReferenciaGeograficaDAO.FindById(cbTipo.Selected);
            return CreateGeoRef(codigo, descripcion, dir, linea, tipoGeoRef);
        }

        private ReferenciaGeografica CreateGeoRef(String codigo, String descripcion, DireccionVO dir, Linea lin, TipoReferenciaGeografica tipoGeoRef)
        {
            var posicion = new Direccion
                               {
                                   Altura = dir.Altura,
                                   IdMapa = (short) dir.IdMapaUrbano,
                                   Provincia = dir.Provincia,
                                   IdCalle = -1,
                                   IdEsquina = dir.IdEsquina,
                                   IdEntrecalle = dir.IdEsquina,
                                   Latitud = dir.Latitud,
                                   Longitud = dir.Longitud,
                                   Partido = dir.Partido,
                                   Pais = string.Empty,
                                   Calle = dir.Calle,
                                   Descripcion = dir.Direccion,
                                   Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                               };

            var poligono = new Poligono
                               {
                                   Radio = 50,
                                   Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                               };
            poligono.AddPoints(new [] { new PointF((float)posicion.Longitud, (float)posicion.Latitud) });


            //Constructs the point.
            var puntoDeInteres = new ReferenciaGeografica
                                     {
                                         Codigo = codigo,
                                         Descripcion = Trunc(descripcion,128),
                                         Empresa = lin.Empresa, 
                                         Linea = lin ,
                                         EsFin = tipoGeoRef.EsFin,
                                         EsInicio = tipoGeoRef.EsInicio,
                                         EsIntermedio = tipoGeoRef.EsIntermedio,
                                         InhibeAlarma = tipoGeoRef.InhibeAlarma,
                                         TipoReferenciaGeografica = tipoGeoRef,
                                         Vigencia = new Vigencia { Inicio = DateTime.UtcNow },
                                         Icono = tipoGeoRef.Icono
                                     };

            puntoDeInteres.Historia.Add(new HistoriaGeoRef
                                            {
                                                ReferenciaGeografica = puntoDeInteres,
                                                Direccion = posicion,
                                                Poligono = poligono,
                                                Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                                            });

            DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(puntoDeInteres);
            return puntoDeInteres;
        }

        private static bool IsValidTicket(Ticket ticket)
        {
            return ticket != null && ticket.Cliente != null && ticket.PuntoEntrega != null && ticket.FechaTicket.HasValue;
        }

        private string Trunc(string text, int size)
        {
            if(text == null) return string.Empty;
            return text.Length > size
                       ? text.Substring(0, size)
                       : text;
        }
        #endregion

    }
}