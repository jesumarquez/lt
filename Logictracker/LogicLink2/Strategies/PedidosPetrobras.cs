using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using LinqToExcel;
using Geocoder.Core.VO;

namespace Logictracker.Scheduler.Tasks.Logiclink2.Strategies
{
    public class PedidosPetrobras : Strategy
    {
        private static Dictionary<int, List<int>> EmpresasLineas = new Dictionary<int, List<int>>();
        private const string Component = "Logiclink2";

        private Empresa Empresa { get; set; }
        private Linea Linea { get; set; }
        private LogicLinkFile Llfile { get; set; }
        private DAOFactory DaoFactory { get; set; }
        private readonly List<PuntoEntrega> _puntosBuffer = new List<PuntoEntrega>();
        private readonly List<Transportista> _transportistasBuffer = new List<Transportista>();

        public static void Parse(LogicLinkFile file, out int pedidos, out string observaciones)
        {
            new PedidosPetrobras(file).Parse(out pedidos, out observaciones);            
        }

        public PedidosPetrobras(LogicLinkFile file)
        {
            Llfile = file;
            DaoFactory = new DAOFactory();
            Empresa = file.Empresa;
            Linea = file.Linea;
        }

        public void Parse(out int pedidos, out string observaciones)
        {
            var te = new TimeElapsed();
            var rows = ParseExcelFile(Llfile.FilePath, true);
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));
            te.Restart();
            PreBufferRows(rows);
            STrace.Trace(Component, string.Format("PreBufferRows en {0} segundos", te.getTimeElapsed().TotalSeconds));

            //var listPedidos = new List<Pedido>(rows.Count);
            
            pedidos = 0;
            observaciones = string.Empty;

            STrace.Trace(Component, "Cantidad de filas: " + rows.Count);
            var filas = 0;

            // ARRANCO EN LA FILA 8
            for (var i = 7; i < rows.Count; i++)
            {
                var row = rows[i];
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));

                var descripcionTransportista = row[Properties.PedidosPetrobras.DescripcionTransportista].ToString().Trim();
                var transportista = descripcionTransportista != string.Empty ? _transportistasBuffer.SingleOrDefault(t => t.Descripcion.Equals(descripcionTransportista)) : null;

                // BAJO 2 FILAS
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                row = rows[i];
                var codigoPlanta = row[Properties.PedidosPetrobras.CodigoPlanta].ToString().Trim();
                var planta = DaoFactory.LineaDAO.FindByCodigo(Empresa.Id, codigoPlanta);

                // BAJO 4 FILAS
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                row = rows[i];

                while (row[Properties.PedidosPetrobras.CodigoCliente].ToString().Trim() != string.Empty)
                {
                    var condicion = row[Properties.PedidosPetrobras.Condicion].ToString().Trim();
                    var codigoCliente = row[Properties.PedidosPetrobras.CodigoCliente].ToString().Trim();
                    //var codigoEntrega = row[Properties.PedidosPetrobras.CodigoEntrega].ToString().Trim();
                    var codigoPedido = row[Properties.PedidosPetrobras.CodigoPedido].ToString().Trim();
                    var fechaSolicitud = row[Properties.PedidosPetrobras.FechaSolicitud].ToString().Trim();
                    //var fechaLiberacion = row[Properties.PedidosPetrobras.FechaLiberacion].ToString().Trim();
                    var naftaSuper = row[Properties.PedidosPetrobras.NaftaSuper].ToString().Trim();
                    var naftaPremium = row[Properties.PedidosPetrobras.NaftaPremium].ToString().Trim();
                    var gasoilMenos500 = row[Properties.PedidosPetrobras.GasoilMenor500].ToString().Trim();
                    var gasoilMayor500 = row[Properties.PedidosPetrobras.GasoilMayor500].ToString().Trim();
                    var gasoilPremium = row[Properties.PedidosPetrobras.GasoilPremium].ToString().Trim();

                    var puntoEntrega = _puntosBuffer.SingleOrDefault(p => p.Codigo == codigoCliente);

                    if (condicion == "YIF")
                    {
                        // ARMAR NUEVO PEDIDO
                        pedidos++;
                    }

                    i++;
                    STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                    row = rows[i];
                }

                // TERMINA EL LISTADO DE PEDIDOS PARA UN TRANSPORTISTA
                // BAJO 3 FILAS
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                // EL i++ DEL FOR BAJA LA 4TA FILA AUTOMATICAMENTE
            }

            //STrace.Trace(Component, "Guardando pedidos: " + listPedidos.Count);
            te.Restart();
            //foreach (var pedido in pedidos)
            {
                // GUARDAR PEDIDOS                
            }
            STrace.Trace(Component, string.Format("Pedidos guardadas en {0} segundos", te.getTimeElapsed().TotalSeconds));
        }

        private void PreBufferRows(IEnumerable<Row> rows)
        {
            var lastCodPunto = string.Empty;
            var lastCodTransportista = string.Empty;

            var codPuntoStrList = new List<string>();
            var codTransportistaStrList = new List<string>();

            foreach (var row in rows)
            {
                #region Buffer PuntoEntrega

                try
                {
                    var codigoPuntoEntrega = row[Properties.PedidosPetrobras.CodigoCliente].ToString().Trim();

                    if (lastCodPunto != codigoPuntoEntrega)
                    {
                        if (!codPuntoStrList.Contains(codigoPuntoEntrega))
                            codPuntoStrList.Add(codigoPuntoEntrega);

                        lastCodPunto = codigoPuntoEntrega;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex,
                        String.Format("Error Buffering Punto de Entrega ({0})", row[Properties.PedidosPetrobras.CodigoCliente]));
                }

                #endregion

                #region Buffer Transportistas

                try
                {
                    var descripcion = row[Properties.PedidosPetrobras.DescripcionTransportista].ToString().Trim();
                    
                    if (lastCodTransportista != descripcion)
                    {
                        if (!codTransportistaStrList.Contains(descripcion))
                            codTransportistaStrList.Add(descripcion);

                        lastCodTransportista = descripcion;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex,
                        String.Format("Error Buffering Transportista ({0})",
                                      row[Properties.PedidosPetrobras.DescripcionTransportista]));
                }

                #endregion
            }

            const int batchSize = 1000;

            if (codPuntoStrList.Any())
            {
                foreach (var l in codPuntoStrList.InSetsOf(batchSize))
                {
                    var puntos = DaoFactory.PuntoEntregaDAO.FindByCodes(new[] { Empresa.Id },
                                                                        new[] { -1 },
                                                                        new[] { -1 },
                                                                        l);
                    if (puntos != null && puntos.Any())
                    {
                        _puntosBuffer.AddRange(puntos);
                    }
                }
            }

            if (codTransportistaStrList.Any())
            {
                foreach (var l in codTransportistaStrList.InSetsOf(batchSize))
                {
                    var transportistas = DaoFactory.TransportistaDAO.FindByCodigos(Empresa.Id, -1, l);
                    if (transportistas != null && transportistas.Any())
                    {
                        _transportistasBuffer.AddRange(transportistas);
                    }
                }
            }
        }

        private void SendMail(string[] parametros)
        {
            var configFile = Config.Mailing.LogiclinkErrorMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);
            var destinatarios = new List<string> { "soporte@logictracker.com", "metzler.lucas@gmail.com" };

            if (WebSecurity.AuthenticatedUser.Name != string.Empty)
            {
                var usuario = DaoFactory.UsuarioDAO.GetByNombreUsuario(WebSecurity.AuthenticatedUser.Name);
                if (usuario != null && usuario.Email.Trim() != string.Empty)
                    destinatarios.Add(usuario.Email.Trim());
            }

            sender.Config.Subject = "Logiclink2: Error de importación";
            foreach (var destinatario in destinatarios)
            {
                sender.Config.ToAddress = destinatario;
                sender.SendMail(parametros);
                STrace.Trace(Component, "Email sent to: " + destinatario);
            }
        }
    }
}
