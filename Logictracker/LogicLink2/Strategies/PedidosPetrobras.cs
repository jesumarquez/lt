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
using Logictracker.Types.BusinessObjects.Ordenes;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Scheduler.Tasks.Logiclink2.Strategies
{
    public class PedidosPetrobras : Strategy
    {
        private const string Component = "Logiclink2";

        private Empresa Empresa { get; set; }
        private Linea Linea { get; set; }
        private LogicLinkFile Llfile { get; set; }
        private DAOFactory DaoFactory { get; set; }
        private readonly List<PuntoEntrega> _puntosBuffer = new List<PuntoEntrega>();
        private readonly List<Transportista> _transportistasBuffer = new List<Transportista>();
        private readonly List<Insumo> _insumosBuffer = new List<Insumo>();

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
            var rows = ParseExcelFile(Llfile.FilePath, false, 3);
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));
            te.Restart();
            PreBufferRows(rows);            
            STrace.Trace(Component, string.Format("PreBufferRows en {0} segundos", te.getTimeElapsed().TotalSeconds));
            PreBufferInsumos();

            var listOrdenes = new List<Order>(rows.Count);
            pedidos = 0;
            observaciones = string.Empty;

            STrace.Trace(Component, "Cantidad de filas: " + rows.Count);

            // ARRANCO EN LA FILA 5
            for (var i = 4; i < rows.Count; i++)
            {
                var row = rows[i];
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));

                var descripcionTransportista = row[Properties.PedidosPetrobras.DescripcionTransportista].ToString().Trim();
                var transportista = descripcionTransportista != string.Empty ? _transportistasBuffer.SingleOrDefault(t => t.Descripcion.Equals(descripcionTransportista)) : null;

                // BAJO UNA FILA
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                row = rows[i];
                var codigoPlanta = row[Properties.PedidosPetrobras.CodigoPlanta].ToString().Trim();
                var planta = DaoFactory.LineaDAO.FindByCodigo(Empresa.Id, codigoPlanta);

                // BAJO 2 FILAS
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                row = rows[i];

                while (row[Properties.PedidosPetrobras.CodigoCliente].ToString().Trim() != string.Empty)
                {
                    var condicion = row[Properties.PedidosPetrobras.Condicion].ToString().Trim();                   

                    if (condicion == "YIF")
                    {
                        var codigoCliente = row[Properties.PedidosPetrobras.CodigoCliente].ToString().Trim();
                        var puntoEntrega = _puntosBuffer.SingleOrDefault(p => p.Codigo == codigoCliente);
                        //var codigoEntrega = row[Properties.PedidosPetrobras.CodigoEntrega].ToString().Trim();
                        var codigoPedido = row[Properties.PedidosPetrobras.CodigoPedido].ToString().Trim();
                        
                        var fechaSolicitud = row[Properties.PedidosPetrobras.FechaSolicitud].ToString().Trim();
                        var arrayFechaSolicitud = fechaSolicitud.Split('.');
                        var diaSolicitud = Convert.ToInt32(arrayFechaSolicitud[0]);
                        var mesSolicitud = Convert.ToInt32(arrayFechaSolicitud[1]);
                        var anioSolicitud = Convert.ToInt32(arrayFechaSolicitud[2]);
                        var dtFechaSolicitud = new DateTime(anioSolicitud, mesSolicitud, diaSolicitud).AddHours(3);
                        //var fechaLiberacion = row[Properties.PedidosPetrobras.FechaLiberacion].ToString().Trim();

                        // ARMAR NUEVO PEDIDO
                        var orden = new Order();
                        orden.CodigoPedido = codigoPedido;
                        orden.Empresa = Empresa;
                        orden.FechaPedido = dtFechaSolicitud;
                        orden.FechaAlta = DateTime.UtcNow;
                        orden.Linea = planta;
                        orden.PuntoEntrega = puntoEntrega;
                        orden.Transportista = transportista;
                        orden.Programado = false;

                        var naftaSuper = Convert.ToDouble(row[Properties.PedidosPetrobras.NaftaSuper].ToString().Trim());
                        var naftaPremium = Convert.ToDouble(row[Properties.PedidosPetrobras.NaftaPremium].ToString().Trim());
                        var gasoilMenos500 = Convert.ToDouble(row[Properties.PedidosPetrobras.GasoilMenor500].ToString().Trim());
                        var gasoilMayor500 = Convert.ToDouble(row[Properties.PedidosPetrobras.GasoilMayor500].ToString().Trim());
                        var gasoilPremium = Convert.ToDouble(row[Properties.PedidosPetrobras.GasoilPremium].ToString().Trim());

                        Insumo insumo = null;
                        if (naftaSuper > 0)
                        {
                            var detalle = new OrderDetail();
                            detalle.Cantidad = (int)naftaSuper;
                            insumo = _insumosBuffer.FirstOrDefault(a => a.Codigo == "NS");
                            detalle.Insumo = insumo;
                            detalle.PrecioUnitario = (decimal)insumo.ValorReferencia;
                            detalle.Order = orden;

                            orden.OrderDetails.Add(detalle);
                        }
                        if (naftaPremium > 0)
                        {
                            var detalle = new OrderDetail();
                            detalle.Cantidad = (int)naftaPremium;
                            insumo = _insumosBuffer.FirstOrDefault(b => b.Codigo == "NP");
                            detalle.Insumo = insumo;
                            detalle.PrecioUnitario = (decimal)insumo.ValorReferencia;
                            detalle.Order = orden;

                            orden.OrderDetails.Add(detalle);
                        }
                        if (gasoilMenos500 > 0)
                        {
                            var detalle = new OrderDetail();
                            detalle.Cantidad = (int)gasoilMenos500;
                            insumo = _insumosBuffer.FirstOrDefault(b => b.Codigo == "GI500");
                            detalle.Insumo = insumo;
                            detalle.PrecioUnitario = (decimal)insumo.ValorReferencia;
                            detalle.Order = orden;

                            orden.OrderDetails.Add(detalle);
                        }
                        if (gasoilMayor500 > 0)
                        {
                            var detalle = new OrderDetail();
                            detalle.Cantidad = (int)gasoilMayor500;
                            insumo = _insumosBuffer.FirstOrDefault(b => b.Codigo == "GS500");
                            detalle.Insumo = insumo;
                            detalle.PrecioUnitario = (decimal)insumo.ValorReferencia;
                            detalle.Order = orden;

                            orden.OrderDetails.Add(detalle);
                        }
                        if (gasoilPremium > 0)
                        {
                            var detalle = new OrderDetail();
                            detalle.Cantidad = (int)gasoilPremium;
                            insumo = _insumosBuffer.FirstOrDefault(b => b.Codigo == "GP");
                            detalle.Insumo = insumo;
                            detalle.PrecioUnitario = (decimal)insumo.ValorReferencia;
                            detalle.Order = orden;

                            orden.OrderDetails.Add(detalle);
                        }
                        
                        listOrdenes.Add(orden);
                        pedidos++;
                    }

                    i++;
                    if (i < rows.Count)
                    {
                        STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                        row = rows[i];
                    }
                    else
                    {
                        break;
                    }
                }
            }

            STrace.Trace(Component, "Guardando pedidos: " + listOrdenes.Count);
            te.Restart();
            foreach (var orden in listOrdenes)
            {
                DaoFactory.OrderDAO.SaveOrUpdate(orden);
            }
            STrace.Trace(Component, string.Format("Pedidos guardadas en {0} segundos", te.getTimeElapsed().TotalSeconds));
        }

        private void PreBufferInsumos()
        {
            var insumos = DaoFactory.InsumoDAO.GetList(new[] { Empresa.Id }, new[] { -1 }, new[] { -1 });
            _insumosBuffer.AddRange(insumos);
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

                    if (lastCodPunto != codigoPuntoEntrega && codigoPuntoEntrega != string.Empty)
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
                    var puntos = DaoFactory.PuntoEntregaDAO.FindByEmpresaAndCodes(Empresa.Id, l);
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
